using _Scripts.Node;
using UnityEngine;

public class FoxState : StateBase
{
    private GraphManager _graphManager;
    private bool _foxBlockedThePath;

    public FoxState(NodeFSM fsm) : base(fsm)
    {
        _graphManager = GraphManager.Instance;
        _foxBlockedThePath = false;
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
        if (FSM.ObstacleType == ObstacleType.Fox)
            FSM.AnimalType = AnimalType.NONE;
    }

    public override void SpawnAnimalModel()
    {
        var spawnModel = GameManager.Instance.GetBaseGameObject();

        FSM.NodeObject = FSM.InstantiateObject(spawnModel, FSM.transform.position, Quaternion.identity).GetComponent<NodeObject>();

        FSM.NodeObject.InitializeNodeObject(FSM);
        FSM.MovingNodeObject = FSM.NodeObject;

        FSM.IsEmpty = false;
        FSM.AnimalType = AnimalType.NONE;
    }

    public override void HandleClick()
    {
        Debug.Log("CLICKED TO FOX.");
    }

    public override void HandleKittyJump()
    {
        Debug.Log("FOX isn't a kitty!");
    }


    public override void BeforeStartMove()
    {
        FSM.OpenDirtKittyNeighbours();
        FSM.SetNodeAvailable();
        Debug.Log("FOX: before start move");
        FSM.ObstacleType = ObstacleType.NONE;
    }


    public override void OnMoveStart()
    {
        var destNode = FSM.MovingNodeObject.CurrentPathData.DestinationNodeFSM;

        if (destNode.IsEndNode()) return;


        destNode.ObstacleType = ObstacleType.Fox;
        destNode.NodeObject = FSM.MovingNodeObject;
        destNode.MovingNodeObject = FSM.MovingNodeObject;
        destNode.IsEmpty = false;


        foreach (var node in _graphManager.AllNodes)
        {
            if (node.IsEmpty) continue;

            if (node.ObstacleType == ObstacleType.Fox) continue;
            if (node.ObstacleType == ObstacleType.Dog) continue;
            if (node.ObstacleType == ObstacleType.DirtyKitty) continue;
            if (node.ObstacleType == ObstacleType.DirtyCryingKitty) continue;
            if (node.ObstacleType == ObstacleType.Mole && node.MoleBlock) continue;

            _foxBlockedThePath = true;

            if (_graphManager.IsHasPathToRootNode(node))
            {
                _foxBlockedThePath = false;
                break;
            }
        }

        if (_foxBlockedThePath)
        {
            GameManager.Instance.SetGameStateToLose();
            Debug.Log("Fox blocked the path!");
        }
    }


    public override void OnMoveCompleted()
    {
        var destNode = FSM.MovingNodeObject.CurrentPathData.DestinationNodeFSM;
        if (destNode.IsEndNode() || _foxBlockedThePath)
            EventManager.GameLoseExecute?.Invoke();
    }

    public void PathJustOneNodeTowardsRootNode()
    {
        if (FSM.ObstacleType != ObstacleType.Fox) return;

        Debug.Log("FOX:call handle path");

        FSM.HandleOneNodePath(BeforeStartMove, OnMoveStart, OnMoveCompleted);
    }


    public override void OnStateExit()
    {
    }
}
