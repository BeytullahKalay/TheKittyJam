using _Scripts.CollectibleController;
using _Scripts.Node;
using DG.Tweening;
using Pandoras.Helper;
using UnityEngine;

public class MoleState : StateBase
{
    private CollectManager _collectManager;
    private float _underGroundY = -1f;
    private float _overGround = -.1f;

    private Tween _moleTween;
    private Tween _catTween;

    private bool _moleObjectClicked;

    private GameManager _gameManager;
    private GraphManager _graphManager;

    public MoleState(NodeFSM fsm) : base(fsm)
    {
        _collectManager = CollectManager.Instance;
        _gameManager = GameManager.Instance;
        _graphManager = GraphManager.Instance;
    }

    public override void OnStateEnter()
    {
    }

    public override void Initialize()
    {
        CheckForObstacles();
        SpawnAnimalModel();

        var t = FSM.MoleTransform.position;
        if (FSM.MoleBlock)
            t.y = _overGround;
        else
            t.y = _underGroundY;

        FSM.MoleTransform.position = t;
    }

    public override void CheckForObstacles()
    {
    }

    public override void SpawnAnimalModel()
    {
        if (FSM.AnimalType == AnimalType.NONE) return;

        var spawnModel = GameManager.Instance.GetBaseGameObject();
        FSM.NodeObject = FSM.InstantiateObject(spawnModel, FSM.transform.position, Quaternion.identity).GetComponent<NodeObject>();

        FSM.NodeObject.InitializeNodeObject(FSM);
        FSM.IsEmpty = false;
    }

    public override void HandleClick()
    {
        if (FSM.IsEmpty) return;
        if (FSM.MoleBlock) return;

        FSM.MovingNodeObject = FSM.NodeObject;

        FSM.HandlePath(BeforeStartMove, OnMoveStart, OnMoveCompleted);
    }

    public override void HandleKittyJump()
    {
        if (FSM.IsEmpty) return;
        //if (FSM.BeaverBlock) return;


        FSM.MovingNodeObject = FSM.NodeObject;
        FSM.HandleKittyJumpPath();
    }

    public override void BeforeStartMove()
    {
        FSM.OpenDirtKittyNeighbours();
        FSM.SetNodeAvailable();
        _moleObjectClicked = true;
        EventManager.KittyStartTurnExecute?.Invoke(FSM.MovingNodeObject);
    }

    public override void OnMoveStart()
    {
    }


    public override void OnMoveCompleted()
    {
        Debug.Log("on path completed");
        _collectManager.CollectCat(FSM.MovingNodeObject);
    }

    public void ReverseBeaverState()
    {
        if (FSM.ObstacleType == ObstacleType.Mole)
        {
            FSM.MoleBlock = !FSM.MoleBlock;

            float moveYPos = 0;

            if (!_moleObjectClicked)
            {

                if (FSM.MoleBlock)
                {
                    moveYPos = _overGround;

                    var dir = (FSM.MoleTransform.position - FSM.transform.position).normalized;
                    var angle = Helpers.GetAngleFromVectorFloatForRotY(dir);


                    _catTween?.Kill();
                    _catTween = FSM.NodeObject.transform.DORotate(new Vector3(0, angle, 0), _gameManager.GameSettingsDataSO.CatRotateToMoleDuration, RotateMode.Fast);
                    FSM.NodeObject.NodeObjectAnimatorController.TriggerCatPunchAnimation();
                }
                else
                {
                    moveYPos = _underGroundY;
                    _catTween?.Kill();
                    _catTween = FSM.NodeObject.transform.DORotate(new Vector3(0, 0, 0), _gameManager.GameSettingsDataSO.CatRotateToMoleDuration);
                    FSM.NodeObject.NodeObjectAnimatorController.TriggerCatIdleAnimation();
                }

            }

            _moleTween?.Kill();
            _moleTween = FSM.MoleTransform.DOLocalMoveY(moveYPos, .5f);
        }
    }

    public void CheckMoleIsOnlyNodeInGame()
    {
        if (FSM.ObstacleType != ObstacleType.Mole) return;
        if (FSM.IsEmpty) return;

        int foundNodeAmount = 0;

        NodeFSM _holdedFsm = null;
        foreach (var node in _graphManager.AllNodes)
        {
            if (node.IsEmpty) continue;
            if (FSM.ObstacleType == ObstacleType.Fox) continue;
            if (FSM.ObstacleType == ObstacleType.Dog) continue;

            _holdedFsm = node;
            foundNodeAmount++;
        }

        if (foundNodeAmount == 1)
        {
            if (_holdedFsm.ObstacleType == ObstacleType.Mole && _holdedFsm.MoleBlock)
            {
                var dir = (FSM.MoleTransform.position - FSM.transform.position).normalized;
                var angle = Helpers.GetAngleFromVectorFloatForRotY(dir);

                Sequence endLevelSeq = DOTween.Sequence();


                _catTween?.Kill();
                var catRotate = _catTween = FSM.NodeObject.transform.DORotate(new Vector3(0, angle, 0), _gameManager.GameSettingsDataSO.CatRotateToMoleDuration, RotateMode.Fast);
                FSM.NodeObject.NodeObjectAnimatorController.TriggerCatPunchAnimation();


                _moleTween?.Kill();
                var moleMoveY = _moleTween = FSM.MoleTransform.DOLocalMoveY(_overGround, .5f);

                endLevelSeq.Insert(0, catRotate);
                endLevelSeq.Insert(0, moleMoveY);

                endLevelSeq.AppendInterval(2f).OnComplete(() => EventManager.GameLoseExecute?.Invoke());

                //EventManager.GameLoseExecute?.Invoke();
            }
        }
    }



    public override void OnStateExit()
    {
    }
}

