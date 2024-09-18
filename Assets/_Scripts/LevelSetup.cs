using _Scripts.CollectibleController;
using _Scripts.Node;
using System.Collections.Generic;
using UnityEngine;

public class LevelSetup : MonoBehaviour
{
    [Header("Stack Manager Values")]
    [SerializeField] private int stackAmount = 4;

    [Header("Graph Manager Values")]
    [SerializeField] private Transform nodesParent;
    [SerializeField] private NodeFSM hiddenRootNode;

    [Header("Bus Manager Values")]
    [SerializeField] private List<AnimalType> busCollectibleType = new List<AnimalType>();

    [Header("On Level Complete Earnings")]
    [SerializeField] private int gold = 10;
    [SerializeField] private int paw = 10;
    [SerializeField] private int pawAndGoldMultiplierOnAddWatch = 2;


    private GraphManager _graphManager;
    private StackManager _stackManager;
    private BusManager _busManager;
    private CollectManager _collectManager;
    private EarnablesManager _earnableManager;
    private CanvasButtonsController _canvasButtonsController;


    private void Awake()
    {
        _graphManager = GraphManager.Instance;
        _stackManager = StackManager.Instance;
        _busManager = BusManager.Instance;
        _collectManager = CollectManager.Instance;
        _earnableManager = EarnablesManager.Instance;
        _canvasButtonsController = CanvasButtonsController.Instance;
    }

    private void Start()
    {
        _graphManager.Initialize(hiddenRootNode, nodesParent);

        _stackManager.Initialize(stackAmount);

        _busManager.Initialize(busCollectibleType);

        _earnableManager.Initialize(gold, paw);

        _canvasButtonsController.Initialize(gold, paw, pawAndGoldMultiplierOnAddWatch);
    }
}
