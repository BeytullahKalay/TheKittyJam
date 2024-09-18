using _Scripts.CollectibleController;
using _Scripts.Node;
using System.Collections.Generic;
using UnityEngine;

public class KittyHouseState : StateBase
{
    private CollectManager _collectManager;
    private Queue<NodeObject> _kittyHouseCollectQueue;
    private GameManager _gameManager;

    public KittyHouseState(NodeFSM fsm) : base(fsm)
    {
        _collectManager = CollectManager.Instance;

        _kittyHouseCollectQueue = new Queue<NodeObject>();

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
        FSM.AnimalType = FSM.KittyHouseOptions[0];

        SetHouseMaterial();

        FSM.KittyHouseTmpText.text = (FSM.KittyHouseOptions.Count - 1).ToString();
    }

    public override void SpawnAnimalModel()
    {
        if (FSM.AnimalType == AnimalType.NONE)
        {
            Debug.LogError("Animal type is null but Kitty House obstacle selected!", FSM.gameObject);
            return;
        }

        var spawnModel = GameManager.Instance.GetBaseGameObject();
        FSM.NodeObject = FSM.InstantiateObject(spawnModel, FSM.transform.position, Quaternion.identity).GetComponent<NodeObject>();

        FSM.NodeObject.InitializeNodeObject(FSM);
        FSM.IsEmpty = false;

    }

    public override void HandleClick()
    {
        FSM.MovingNodeObject = FSM.NodeObject;
        FSM.HandlePath(BeforeStartMove, OnMoveStart, OnMoveCompleted);
    }

    public override void HandleKittyJump()
    {
        FSM.MovingNodeObject = FSM.NodeObject;
        FSM.HandleKittyJumpPath();
    }

    public override void BeforeStartMove()
    {


        FSM.KittyHouseOptions.RemoveAt(0);

        if (FSM.KittyHouseOptions.Count > 0)
        {
            FSM.KittyHouseTmpText.text = (FSM.KittyHouseOptions.Count - 1).ToString();
            EventManager.KittyStartTurnExecute?.Invoke(FSM.MovingNodeObject);
            FSM.AnimalType = FSM.KittyHouseOptions[0];
            SetHouseMaterial();
            SpawnAnimalModel();
            return;
        }
        else
        {
            FSM.KittyHouseMeshRenderer.material = _gameManager.KittyHouseColorDataSO.GetTypeMaterial(AnimalType.NONE);
        }

        FSM.OpenDirtKittyNeighbours();
        FSM.SetNodeAvailable();
        EventManager.KittyStartTurnExecute?.Invoke(FSM.MovingNodeObject);
    }

    public override void OnMoveStart()
    {
        _kittyHouseCollectQueue.Enqueue(FSM.MovingNodeObject);
    }

    public override void OnMoveCompleted()
    {
        _collectManager.CollectCat(_kittyHouseCollectQueue.Dequeue());
    }

    public override void OnStateExit()
    {
    }

    private void SetHouseMaterial()
    {
        if (FSM.KittyHouseOptions.Count > 1)
            FSM.KittyHouseMeshRenderer.material = _gameManager.KittyHouseColorDataSO.GetTypeMaterial(FSM.KittyHouseOptions[1]);
        else
        {
            FSM.KittyHouseMeshRenderer.material = _gameManager.KittyHouseColorDataSO.GetTypeMaterial(AnimalType.NONE);
        }
    }
}
