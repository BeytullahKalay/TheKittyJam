using _Scripts.CollectibleController;
using _Scripts.Node;
using UnityEngine;

public class DirtyCryingKittyState : StateBase
{
    private CollectManager _collectManager;

    public DirtyCryingKittyState(NodeFSM fsm) : base(fsm)
    {
        _collectManager = CollectManager.Instance;
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
        if (FSM.AnimalType == AnimalType.NONE)
        {
            Debug.LogError("Obstacle is dirty crying kitty but kitty type is not selected!");
            return;
        }

        var spawnModel = GameManager.Instance.GetBaseGameObject();
        FSM.NodeObject = FSM.InstantiateObject(spawnModel, FSM.transform.position, Quaternion.identity).GetComponent<NodeObject>();

        FSM.NodeObject.InitializeNodeObject(FSM);
        FSM.IsEmpty = false;
    }

    public override void HandleClick()
    {
        Debug.Log("Dirty crying cat can not move");
    }

    public override void HandleKittyJump()
    {
        Debug.Log("Dirty crying cat can not jump");
    }


    public override void BeforeStartMove()
    {
    }

    public override void OnMoveStart()
    {
    }

    public override void OnMoveCompleted()
    {
    }

    public override void OnStateExit()
    {
    }
}
