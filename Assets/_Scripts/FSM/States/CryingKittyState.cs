using _Scripts.CollectibleController;
using _Scripts.Node;
using UnityEngine;

public class CryingKittyState : StateBase
{

    private CollectManager _collectManager;

    public CryingKittyState(NodeFSM fsm) : base(fsm)
    {
        _collectManager = CollectManager.Instance;
    }

    public override void OnStateEnter()
    {
        FSM.RemainingCryingRoundAmount = FSM.CryingRoundAmount;
    }

    public override void Initialize()
    {
        CheckForObstacles();
        SpawnAnimalModel();
    }

    public override void CheckForObstacles()
    {
    }

    public override void SpawnAnimalModel()
    {
        if (FSM.AnimalType == AnimalType.NONE)
        {
            Debug.LogError("Obstacle is crying kitty but kitty type is not selected!");
            return;
        }

        var spawnModel = GameManager.Instance.GetBaseGameObject();
        FSM.NodeObject = FSM.InstantiateObject(spawnModel, FSM.transform.position, Quaternion.identity).GetComponent<NodeObject>();

        FSM.NodeObject.InitializeNodeObject(FSM);
        FSM.IsEmpty = false;
    }

    public override void HandleClick()
    {
        if (FSM.IsEmpty) return;

        FSM.MovingNodeObject = FSM.NodeObject;
        FSM.HandlePath(BeforeStartMove, OnMoveStart, OnMoveCompleted);
    }

    public override void HandleKittyJump()
    {
        if (FSM.IsEmpty) return;

        FSM.MovingNodeObject = FSM.NodeObject;
        FSM.HandleKittyJumpPath();
    }

    public override void BeforeStartMove()
    {
        FSM.CryingKittyTransform.gameObject.SetActive(false);
        FSM.CryingKittenMoving = true;
        FSM.OpenDirtKittyNeighbours();
        FSM.SetNodeAvailable();
        EventManager.KittyStartTurnExecute?.Invoke(FSM.MovingNodeObject);
    }

    public override void OnMoveStart()
    {
    }


    public override void OnMoveCompleted()
    {
        _collectManager.CollectCat(FSM.MovingNodeObject);
    }

    public void CheckCryingKittyState()
    {
        if (FSM.ObstacleType == ObstacleType.CryingKitty && !FSM.CryingKittenMoving)
        {

            FSM.RemainingCryingRoundAmount -= 1;

            if (FSM.RemainingCryingRoundAmount <= 0)
            {
                Debug.Log("Crying kitty FAIL.");
                EventManager.GameLoseExecute?.Invoke();
                return;
            }

            FSM.CryingKittyTmpText.text = FSM.RemainingCryingRoundAmount.ToString();
        }
    }


    public override void OnStateExit()
    {
    }
}

