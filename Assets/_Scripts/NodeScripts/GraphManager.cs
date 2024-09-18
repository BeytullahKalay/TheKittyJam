using DG.Tweening;
using Pandoras.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Scripts.Node
{
    public class GraphManager : MonoSingleton<GraphManager>
    {
        [SerializeField] private NodeFSM hiddenRootNode;
        [SerializeField] private Transform nodesParent;
        [SerializeField] private GameObject lineGameObject;
        [SerializeField] private GameObject pointerArrow;

        public List<NodeFSM> AllNodes { get; private set; } = new();

        private GameObject _lineRendererParent;

        private GameManager _gameManager;

        private void Awake()
        {
            _gameManager = GameManager.Instance;
        }

        private void OnEnable()
        {
            EventManager.OnLevelLoadPrepare += ClearManagerData;
        }

        private void OnDisable()
        {
            EventManager.OnLevelLoadPrepare -= ClearManagerData;
        }


        public void Initialize(NodeFSM hiddenNode, Transform nodesParent)
        {
            this.hiddenRootNode = hiddenNode;
            this.nodesParent = nodesParent;
            GetAllNodes();
            AssignAllNeighbours();
            CreatePathLines();
            HideRootNode();
            CreateExitPointersOnEndNodes();
        }

        public NodeFSM HiddenRootNode => hiddenRootNode;

        public List<NodeFSM> FindShortestPathToRootNode(NodeFSM startNode)
        {
            return FindPathToNode(startNode, hiddenRootNode);
        }

        public void TryStartMovingOnPath(NodeFSM startNode, NodeObject movingObject, Action beforeMove, Action onMoveStart, Action onMoveCompleted)
        {
            Debug.Log("start moving");

            var _pathNodes = FindShortestPathToRootNode(startNode);


            // path might not found
            if (_pathNodes != null)
            {
                beforeMove?.Invoke();

                // The level may be over. On Level over im resetting the path.
                if (_pathNodes == null) return;

                var pathTween = GetPathTween(_pathNodes[0], movingObject, _pathNodes, onMoveStart, onMoveCompleted);


                movingObject.PathDataQueue.Enqueue(new PathData(pathTween, _pathNodes.Last(), AnimationType.Walk));
            }
            else
                Debug.Log("Path not found!");
        }

        public bool IsHasPathToRootNode(NodeFSM startNode)
        {
            var _pathNodes = FindShortestPathToRootNode(startNode);

            return _pathNodes != null;

        }

        public void StartMovingOneNodeTowardsRootNode(NodeFSM startNode, NodeObject movingObject, Action beforeStartMove, Action onMoveStart, Action onMoveCompleted)
        {
            var _pathNodes = FindShortestPathToRootNode(startNode);

            // path might not found
            if (_pathNodes != null)
            {
                beforeStartMove?.Invoke();

                // The level might be over. On Level over im resetting the path.
                if (_pathNodes == null) return;

                while (_pathNodes.Count > 2)
                    _pathNodes.RemoveAt(_pathNodes.Count - 1);

                var pathTween = GetPathTween(_pathNodes[0], movingObject, _pathNodes, onMoveStart, onMoveCompleted);



                movingObject.PathDataQueue.Enqueue(new PathData(pathTween, _pathNodes.Last(), AnimationType.Walk));


                //THIS IS HACK!!
                if (_pathNodes.Last().IsEndNode() && startNode.ObstacleType == ObstacleType.Fox)
                    EventManager.GameLoseExecute?.Invoke();
            }
            else
                Debug.Log("Path not found!");
        }

        public void FindMovableNodes(NodeFSM jumperNode, List<NodeFSM> movableNodes, List<NodeFSM> visitedNodes, List<List<NodeFSM>> neighboursPath)
        {
            while (movableNodes.Count > 0)
            {
                var searchNode = movableNodes[0];
                foreach (var neighbour in searchNode.Neighbours)
                {
                    if (visitedNodes.Contains(neighbour)) continue;

                    visitedNodes.Add(neighbour);

                    if (neighbour.IsEmpty)
                        movableNodes.Add(neighbour);
                    else
                        neighboursPath.Add(FindShortestPathToRootNode(neighbour));
                }

                movableNodes.RemoveAt(0);
            }
        }

        public List<NodeFSM> GetShortestPath(List<List<NodeFSM>> neighboursPath)
        {
            var minNodeAmount = int.MaxValue;
            var selectedPath = new List<NodeFSM>();

            foreach (var path in neighboursPath)
            {
                if (path?.Count < minNodeAmount)
                {
                    selectedPath = path;
                    minNodeAmount = path.Count;
                }
            }

            return selectedPath;
        }

        public Tween CreatePathToJumpNodeTween(NodeFSM jumperNode, NodeObject moveNode, List<NodeFSM> selectedPath, Action beforeStartMove, Action onStartMove)
        {
            Tween pathToJumpNodeTween = null;
            var pathToJumpAboveNode = FindPathToNode(jumperNode, selectedPath[0], true);
            if (pathToJumpAboveNode?.Count > 1)
            {
                pathToJumpAboveNode.RemoveAt(pathToJumpAboveNode.Count - 1);
                var pathToJumpNodePositions = GetPathPositions(pathToJumpAboveNode);


                beforeStartMove?.Invoke();
                pathToJumpNodeTween = moveNode.transform.
                    DOPath(pathToJumpNodePositions.ToArray(), _gameManager.GameSettingsDataSO.CatPathSpeed, PathType.Linear, PathMode.Full3D)
                    .SetSpeedBased(true).SetLookAt(0.01f).OnStart(() => onStartMove?.Invoke()).SetEase(Ease.Linear);

                moveNode.PathDataQueue.Enqueue(new PathData(pathToJumpNodeTween, pathToJumpAboveNode.Last(), AnimationType.Walk));

            }
            pathToJumpNodeTween.Pause();
            return pathToJumpNodeTween;
        }

        public Tween CreatePathToRootTween(NodeObject moveNodeObject, List<NodeFSM> selectedPath, Action onMoveCompleted)
        {

            Debug.Log("try jump");
            var pathTheCollectNodePositionsList = GetPathPositions(selectedPath);
            pathTheCollectNodePositionsList.RemoveAt(0); // start path position from list

            Tween pathToRootTween = null;
            var tempAnimationType = AnimationType.Walk;

            // if pathTheCollectNodePositionsList has path then we are not on the collect node. We can continue with jump and doing path
            if (pathTheCollectNodePositionsList.Count > 0)
            {

                tempAnimationType = AnimationType.Jump;
                // jump above node
                pathToRootTween = moveNodeObject.transform.DOJump(selectedPath[1].transform.position, _gameManager.GameSettingsDataSO.CatJumpPower, 1, _gameManager.GameSettingsDataSO.CatJumpDuration).OnStart(() =>
                {
                    var dir = selectedPath[1].transform.position - moveNodeObject.transform.position;
                    var angle = Helpers.GetAngleFromVectorFloatForRotY(dir);
                    moveNodeObject.transform.rotation = Quaternion.Euler(Vector3.up * angle);
                }).OnComplete(() =>
                   {
                       if (selectedPath[1] == hiddenRootNode)
                       {

                           onMoveCompleted?.Invoke();
                           Debug.Log("jump complete");
                       }
                       else
                       {
                           Tween tweenHolder = null;
                           tweenHolder = moveNodeObject.transform.DOPath(pathTheCollectNodePositionsList.ToArray(), _gameManager.GameSettingsDataSO.CatPathSpeed, PathType.Linear, PathMode.Full3D)
                                .SetSpeedBased(true).SetLookAt(0.01f).SetEase(Ease.Linear).OnComplete(() =>
                                {
                                    onMoveCompleted?.Invoke();
                                    Debug.Log("path complete");
                                });

                           moveNodeObject.PathDataQueue.Enqueue(new PathData(tweenHolder, selectedPath.Last(), AnimationType.Walk));

                       }
                   });


            }
            else // we are just before to collect node to trying jump but jump action is not possible. Because of that we are collecting the object directly
            {

                pathToRootTween = DOTween.Sequence().OnComplete(() =>
                {
                    onMoveCompleted?.Invoke();
                    Debug.Log("sequence complete");
                });
            }


            moveNodeObject.PathDataQueue.Enqueue(new PathData(pathToRootTween, selectedPath.Last(), tempAnimationType));

            pathToRootTween?.Pause();
            return pathToRootTween;
        }

        private List<NodeFSM> FindPathToNode(NodeFSM startNode, NodeFSM endNode, bool lookingForAJumpingNode = false)
        {
            var cameFrom = new Dictionary<NodeFSM, NodeFSM>();
            var frontier = new Queue<NodeFSM>();
            frontier.Enqueue(startNode);

            while (frontier.Count > 0)
            {
                NodeFSM current = frontier.Dequeue();

                if (current == endNode)
                    break;

                foreach (NodeFSM next in current.Neighbours)
                {
                    if (!cameFrom.ContainsKey(next))
                    {
                        if (lookingForAJumpingNode && next == endNode)
                        {
                            frontier.Enqueue(next);
                            cameFrom[next] = current;
                            continue;
                        }

                        if (next.IsEmpty)
                        {
                            frontier.Enqueue(next);
                            cameFrom[next] = current;
                        }
                    }
                }
            }

            return ReconstructPath(startNode, endNode, cameFrom, lookingForAJumpingNode);
        }


        private void GetAllNodes()
        {
            var children = Helpers.GetAllChildren(nodesParent);
            foreach (var child in children)
            {
                AllNodes.Add(child.GetComponent<NodeFSM>());
            }
        }

        private void AssignAllNeighbours()
        {
            foreach (var node in AllNodes)
            {
                foreach (var searchNode in AllNodes)
                {
                    if (node == searchNode) continue;
                    if (node.Neighbours.Contains(searchNode)) continue;

                    if (searchNode.Neighbours.Contains(node))
                        node.Neighbours.Add(searchNode);
                }
            }
        }

        private Tween GetPathTween(NodeFSM startPathNode, NodeObject movingObject, List<NodeFSM> pathNodes, Action onMoveStart, Action onMoveCompleted)
        {
            var pathPositionList = GetPathPositions(pathNodes);

            var path = CreatePathTween(startPathNode, pathPositionList, movingObject, onMoveStart, onMoveCompleted);

            path.Pause();
            return path;
        }

        private Tween CreatePathTween(NodeFSM StartPathNode, List<Vector3> pathPositions, NodeObject movingNodeObject, Action onMoveStart, Action onComplete)
        {
            return movingNodeObject.transform.DOPath(pathPositions.ToArray(), _gameManager.GameSettingsDataSO.CatPathSpeed, PathType.Linear, PathMode.Full3D)
                .SetSpeedBased(true).SetLookAt(0.01f).SetEase(Ease.Linear).OnStart(() => { onMoveStart?.Invoke(); }).OnComplete(() => { onComplete?.Invoke(); });
        }

        private List<NodeFSM> ReconstructPath(NodeFSM startNode, NodeFSM endNode, Dictionary<NodeFSM, NodeFSM> cameFrom, bool lookingForAJumpingNode)
        {
            if (cameFrom.Count <= 0 || !cameFrom.ContainsKey(endNode))
            {
                return null;
            }

            var path = new List<NodeFSM>();
            var currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = cameFrom[currentNode];
            }

            path.Add(startNode);
            path.Reverse();

            if (!lookingForAJumpingNode)
            {
                //Debug.Log("Remove last root path to hide hidden root");
                path.RemoveAt(path.Count - 1);
            }
            return path;
        }

        private void CreatePathLines()
        {
            var linedFromTo = new HashSet<ValueTuple<NodeFSM, NodeFSM>>();
            var lineParent = new GameObject("LineParent");
            _lineRendererParent = lineParent;
            lineParent.transform.SetParent(gameObject.transform);

            foreach (var node in AllNodes)
            {
                foreach (var searchNode in AllNodes)
                {
                    if (node == hiddenRootNode || searchNode == hiddenRootNode) continue;
                    if (node == searchNode) continue;
                    if (!node.Neighbours.Contains(searchNode)) continue;

                    var nodePair = (node, searchNode);
                    if (!linedFromTo.Add(nodePair) && !linedFromTo.Add((searchNode, node))) continue;

                    var lineObject = Instantiate(lineGameObject, lineParent.transform);
                    var lr = lineObject.GetComponent<LineRenderer>();
                    lr.positionCount = 2;

                    var startPos = node.transform.position;
                    var endPos = searchNode.transform.position;

                    startPos.y = 0;
                    endPos.y = 0;

                    lr.SetPosition(0, startPos);
                    lr.SetPosition(1, endPos);
                }
            }
        }

        private bool AreTheyNeighbour(NodeFSM node1, NodeFSM node2)
        {
            return node1.Neighbours.Contains(node2) || node2.Neighbours.Contains(node1);
        }

        private void HideRootNode()
        {
            hiddenRootNode.gameObject.SetActive(false);
        }

        private List<Vector3> GetPathPositions(List<NodeFSM> pathNodes)
        {
            var pathPositions = new List<Vector3>();
            foreach (var node in pathNodes)
            {
                pathPositions.Add(node.transform.position);
            }
            return pathPositions;
        }


        private void CreateExitPointersOnEndNodes()
        {
            foreach (var item in hiddenRootNode.Neighbours)
            {
                var dir = (item.transform.position + Vector3.forward) - item.transform.position;

                var angle = Helpers.GetAngleFromVectorFloatForRotY(dir);

                var spawnPosition = new Vector3(0, .1f, 1);

                var pointerArrowObj = Instantiate(pointerArrow, item.transform.position + spawnPosition, Quaternion.Euler(Vector3.up * angle));

                pointerArrowObj.transform.SetParent(item.transform);
            }
        }

        private void ClearManagerData()
        {
            Destroy(_lineRendererParent);
            hiddenRootNode = null;
            nodesParent = null;
            AllNodes?.Clear();
        }
    }
}
