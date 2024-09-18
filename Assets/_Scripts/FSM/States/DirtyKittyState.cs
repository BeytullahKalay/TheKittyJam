using _Scripts.CollectibleController;
using _Scripts.Node;
using UnityEngine;

public class DirtyKittyState : StateBase
{
    private CollectManager _collectManager;
    public DirtyKittyState(NodeFSM fsm) : base(fsm)
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
            Debug.LogError("Animal type is null but dirty cat obstacle selected!", FSM.gameObject);
            return;
        }

        var spawnModel = GameManager.Instance.GetBaseGameObject();
        FSM.NodeObject = FSM.InstantiateObject(spawnModel, FSM.transform.position, Quaternion.identity).GetComponent<NodeObject>();

        FSM.NodeObject.InitializeNodeObject(FSM);
        FSM.MovingNodeObject = FSM.NodeObject;
        FSM.IsEmpty = false;
    }

    public override void HandleClick()
    {
        Debug.Log("Dirty cat can not move");
    }

    public override void HandleKittyJump()
    {
        Debug.Log("Dirt kitty can not jump!");
    }


    public override void BeforeStartMove()
    {
        //FSM.OpenDirtKittyNeighbours();
        //FSM.SetNodeAvailable();
        //EventManager.KittyStartTurnExecute?.Invoke();
    }

    public override void OnMoveStart()
    {
    }

    public override void OnMoveCompleted()
    {
        //Debug.Log("on path completed");
        //_collectManager.CollectCat(FSM.MovingNodeObject);
    }



    public override void OnStateExit()
    {
    }
}
