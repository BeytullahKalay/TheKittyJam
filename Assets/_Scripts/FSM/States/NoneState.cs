using _Scripts.CollectibleController;
using _Scripts.Node;
using UnityEngine;

public class NoneState : StateBase
{
    private CollectManager _collectManager;
    private NodeObject _movedNodeObject;
    public NoneState(NodeFSM fsm) : base(fsm)
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
        // empty
    }

    public override void SpawnAnimalModel()
    {
        if (FSM.AnimalType == AnimalType.NONE) return;

        var spawnModel = GameManager.Instance.GetBaseGameObject();
        FSM.NodeObject = FSM.InstantiateObject(spawnModel, FSM.transform.position, Quaternion.identity).GetComponent<NodeObject>();

        FSM.NodeObject.InitializeNodeObject(FSM);
        FSM.IsEmpty = false;

        FSM.MovingNodeObject = FSM.NodeObject;
    }

    public override void HandleClick()
    {
        if (FSM.IsEmpty) return;

        FSM.HandlePath(BeforeStartMove, OnMoveStart, OnMoveCompleted);
    }

    public override void HandleKittyJump()
    {
        if (FSM.IsEmpty) return;

        FSM.HandleKittyJumpPath();
    }

    public override void BeforeStartMove()
    {
        FSM.OpenDirtKittyNeighbours();
        FSM.SetNodeAvailable();
        EventManager.KittyStartTurnExecute?.Invoke(FSM.MovingNodeObject);
        _movedNodeObject = FSM.MovingNodeObject;
    }

    public override void OnMoveStart()
    {
    }

    public override void OnMoveCompleted()
    {
        Debug.Log("on path completed");
        _collectManager.CollectCat(_movedNodeObject);
    }



    public override void OnStateExit()
    {
    }
}
