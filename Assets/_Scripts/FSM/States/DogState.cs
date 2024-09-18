using _Scripts.Node;
using DG.Tweening;
using Pandoras.Helper;
using UnityEngine;

public class DogState : StateBase
{
    private GameManager _gameManager;
    public DogState(NodeFSM fsm) : base(fsm)
    {
        _gameManager = GameManager.Instance;
    }

    public override void OnStateEnter()
    {

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
        var spawnModel = GameManager.Instance.GetBaseGameObject();
        FSM.NodeObject = FSM.InstantiateObject(spawnModel, FSM.transform.position, Quaternion.identity).GetComponent<NodeObject>();

        FSM.NodeObject.InitializeNodeObject(FSM);
        FSM.IsEmpty = false;
        FSM.AnimalType = AnimalType.NONE;
    }


    public override void HandleClick()
    {
        Debug.Log("CLICKED TO DOG.");
    }

    public override void HandleKittyJump()
    {
        Debug.Log("CLICKED TO DOG.");
    }

    public override void BeforeStartMove()
    {
        Debug.LogError("DOG DOES NOT MOVE!");
    }

    public override void OnMoveStart()
    {

    }

    public override void OnMoveCompleted()
    {
        Debug.Log("on path completed");
    }

    public void CheckDogWillChaseACat()
    {
        if (FSM.ObstacleType != ObstacleType.Dog) return;

        foreach (var neighbour in FSM.Neighbours)
        {
            if (neighbour.IsEmpty) continue;
            if (neighbour.ObstacleType != ObstacleType.NONE
                && neighbour.ObstacleType != ObstacleType.CryingKitty
                && neighbour.ObstacleType != ObstacleType.Mole) continue;

            NodeFSM runningKitty = neighbour;

            var hasNeighbours = false;
            foreach (var item in neighbour.Neighbours)
            {
                if (item.IsEmpty) continue;
                if (item.ObstacleType == ObstacleType.Dog) continue;
                if (item.ObstacleType == ObstacleType.Fox) continue;
                hasNeighbours = true;
                break;
            }

            if (!hasNeighbours)
            {
                Debug.Log("Dog chasing the cat level fail.");

                _gameManager.SetGameStateToLose();

                FSM.NodeObject.NodeObjectAnimatorController.TriggerDogWalk();
                runningKitty.NodeObject.NodeObjectAnimatorController.TriggerCatWalkAnimation();


                var dir = runningKitty.NodeObject.transform.position - FSM.NodeObject.transform.position;

                var angle = Helpers.GetAngleFromVectorFloatForRotY(dir);


                var dirNormalized = dir.normalized;

                runningKitty.NodeObject.transform.rotation = Quaternion.Euler(Vector3.up * angle);
                FSM.NodeObject.transform.rotation = Quaternion.Euler(Vector3.up * angle);

                Sequence seq = DOTween.Sequence();

                var kittRun = runningKitty.NodeObject.transform.DOMove(runningKitty.NodeObject.transform.position + (dirNormalized * 200), 6).SetSpeedBased(true);

                var dogRun = FSM.NodeObject.transform.DOMove(FSM.NodeObject.transform.position + (dirNormalized * 200), 6).SetSpeedBased(true);

                seq.AppendInterval(3f).OnStepComplete(() =>
                {
                    EventManager.GameLoseExecute?.Invoke();

                });
            }

        }
    }



    public override void OnStateExit()
    {
    }
}
