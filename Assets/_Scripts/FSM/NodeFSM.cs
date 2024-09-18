using _Scripts.Node;
using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NodeFSM : MonoBehaviour
{
    // animal type and neighbours
    public AnimalType AnimalType;
    public List<NodeFSM> Neighbours = new();

    // kitty house
    public List<AnimalType> KittyHouseOptions;
    public Transform KittyHouseTransform;
    public TMP_Text KittyHouseTmpText;
    public MeshRenderer KittyHouseMeshRenderer;

    // Mole block
    public Transform MoleParentTransform;
    public Transform MoleTransform;
    public bool MoleBlock;

    //// crying kitty
    public Transform CryingKittyTransform;
    public TMP_Text CryingKittyTmpText;
    public int CryingRoundAmount;
    public int RemainingCryingRoundAmount;
    public bool CryingKittenMoving;

    public bool IsEmpty { get; set; } = true;
    public NodeObject NodeObject { get; set; }

    internal NodeObject MovingNodeObject;


    public Action<ObstacleType> OnObstacleStateChange;
    [SerializeField] private ObstacleType _obstacle;
    public ObstacleType ObstacleType
    {
        get { return _obstacle; }
        set
        {
            if (_obstacle == value) return;
            else
            {
                _obstacle = value;
                OnObstacleStateChange?.Invoke(value);
            }
        }
    }


    private NoneState _noneState;
    private MoleState _moleState;
    private DirtyKittyState _dirtyKittyState;
    private DogState _dogState;
    private FoxState _foxState;
    private KittyHouseState _kittyHouseState;
    private CryingKittyState _cryingKittyState;
    private DirtyCryingKittyState _dirtyCryingKittyState;




    private StateBase _currentState;
    private GraphManager _graphManager;



    private void Awake()
    {
        InitializeStates();
        SetCurrentState();
        _graphManager = GraphManager.Instance;
    }

    private void InitializeStates()
    {
        _noneState = new NoneState(this);
        _moleState = new MoleState(this);
        _dirtyKittyState = new DirtyKittyState(this);
        _dogState = new DogState(this);
        _foxState = new FoxState(this);
        _kittyHouseState = new KittyHouseState(this);
        _cryingKittyState = new CryingKittyState(this);
        _dirtyCryingKittyState = new DirtyCryingKittyState(this);
    }

    private void OnEnable()
    {
        OnObstacleStateChange += OnDirtyStateChange;
        EventManager.OnKittyStartTurn += _moleState.ReverseBeaverState;
        EventManager.OnKittyStartTurn += _cryingKittyState.CheckCryingKittyState;
        EventManager.OnKittyStartTurn += _foxState.PathJustOneNodeTowardsRootNode;
        EventManager.OnKittyStartTurn += _dogState.CheckDogWillChaseACat;
        EventManager.OnKittyStartTurn += _moleState.CheckMoleIsOnlyNodeInGame;
    }

    private void OnDisable()
    {
        OnObstacleStateChange -= OnDirtyStateChange;
        EventManager.OnKittyStartTurn -= _moleState.ReverseBeaverState;
        EventManager.OnKittyStartTurn -= _cryingKittyState.CheckCryingKittyState;
        EventManager.OnKittyStartTurn -= _foxState.PathJustOneNodeTowardsRootNode;
        EventManager.OnKittyStartTurn -= _dogState.CheckDogWillChaseACat;
        EventManager.OnKittyStartTurn -= _moleState.CheckMoleIsOnlyNodeInGame;
    }

    private void Start()
    {
        _currentState.Initialize();
    }

    public void HandleNodeClick()
    {
        if (IsEmpty) return;

        _currentState.HandleClick();

    }

    internal void HandlePath(Action beforeMove, Action onMoveStart, Action onMoveCompleted)
    {
        _graphManager.TryStartMovingOnPath(this, MovingNodeObject, beforeMove, onMoveStart, onMoveCompleted);
    }

    internal void HandleOneNodePath(Action beforeStartMove, Action onMoveStart, Action onMoveCompleted)
    {
        _graphManager.StartMovingOneNodeTowardsRootNode(this, MovingNodeObject, beforeStartMove, onMoveStart, onMoveCompleted);
    }

    public void HandleKittyJump()
    {
        if (IsEmpty) return;

        _currentState.HandleKittyJump();
    }

    public void HandleKittyJumpPath()
    {
        var jumperKittyPathToRootNode = _graphManager.FindShortestPathToRootNode(this);
        if (jumperKittyPathToRootNode != null)
        {
            Debug.Log("Jumpler kitty has path to root node!");
            return;
        }

        var neighboursPath = new List<List<NodeFSM>>();
        var movableNodes = new List<NodeFSM>();
        var visitedNodes = new List<NodeFSM>();

        visitedNodes.Add(this);
        movableNodes.Add(this);

        _graphManager.FindMovableNodes(this, movableNodes, visitedNodes, neighboursPath);

        var selectedPath = _graphManager.GetShortestPath(neighboursPath);

        if (selectedPath.Count <= 0)
        {
            Debug.Log("This no place to jump and move to node");
            return;
        }

        var pathToJumpNodeTween = _graphManager.CreatePathToJumpNodeTween(this, MovingNodeObject, selectedPath, _currentState.BeforeStartMove, _currentState.OnMoveStart);

        var pathToRootTween = _graphManager.CreatePathToRootTween(MovingNodeObject, selectedPath, _currentState.OnMoveCompleted);

        if (pathToJumpNodeTween != null)
        {
            pathToJumpNodeTween.OnComplete(() => pathToRootTween?.Play());
        }
        else
        {
            pathToRootTween?.Play();
        }
    }

    public GameObject InstantiateObject(GameObject obj, Vector3 spawnPos, Quaternion quaternion)
    {
        return Instantiate(obj, spawnPos, quaternion);
    }

    // ObstacleType setter method using by editor script
    public void SetObstacleType(ObstacleType obstacleType)
    {
        ObstacleType = obstacleType;
    }

    internal void SetNodeAvailable()
    {
        AnimalType = AnimalType.NONE;
        IsEmpty = true;
        NodeObject = null;
    }

    internal void OpenDirtKittyNeighbours()
    {
        foreach (var neighbourNode in Neighbours)
        {
            if (neighbourNode.ObstacleType == ObstacleType.DirtyKitty)
            {
                neighbourNode.ObstacleType = ObstacleType.NONE;
            }
            else if (neighbourNode.ObstacleType == ObstacleType.DirtyCryingKitty)
            {
                neighbourNode.ObstacleType = ObstacleType.CryingKitty;

                // this is hack
                neighbourNode.RemainingCryingRoundAmount += 1;
            }
        }
    }

    internal bool IsEndNode()
    {
        var hiddenRootNode = _graphManager.HiddenRootNode;
        foreach (var node in Neighbours)
        {
            if (node == hiddenRootNode)
                return true;
        }
        return false;
    }

    private void SetCurrentState()
    {
        switch (ObstacleType)
        {
            case ObstacleType.NONE:
                ChangeState(_noneState);
                break;
            case ObstacleType.Fox:
                ChangeState(_foxState);
                break;
            case ObstacleType.DirtyKitty:
                ChangeState(_dirtyKittyState);
                break;
            case ObstacleType.KittyHouse:
                ChangeState(_kittyHouseState);
                break;
            case ObstacleType.BondedKities:
                Debug.Log("bonded kities state not setted yet");
                break;
            case ObstacleType.Dog:
                ChangeState(_dogState);
                break;
            case ObstacleType.Mole:
                ChangeState(_moleState);
                break;
            case ObstacleType.CryingKitty:
                ChangeState(_cryingKittyState);
                break;
            case ObstacleType.DirtyCryingKitty:
                ChangeState(_dirtyCryingKittyState);
                break;
        }
    }

    private void ChangeState(StateBase newState)
    {
        _currentState?.OnStateExit();
        _currentState = newState;
        _currentState.OnStateEnter();
    }

    private void OnDirtyStateChange(ObstacleType newObstacleType)
    {
        if (IsEmpty) return;
        NodeObject.ChangeMaterial(newObstacleType);
        SetCurrentState();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        DrawLines();
    }

    private void DrawLines()
    {
        if (Neighbours.Count <= 0) return;

        foreach (var parent in Neighbours)
        {

            if (!parent.IsEmpty) continue;

            var p1 = transform.position;
            var p2 = parent.transform.position;
            var thickness = 6;
            //Handles.DrawBezier(p1, p2, p1, p2, Color.white, null, thickness);
        }
    }
#endif
}
