using _Scripts.CollectibleController;
using DG.Tweening;
using Pandoras.Helper;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Node
{
    public class GraphManager : MonoSingleton<GraphManager>
    {
        [SerializeField] private CollectManager collectManager;
        [SerializeField] private Node rootNode;
        [SerializeField] private Transform nodesParent;
        [SerializeField] private GameObject lineGameObject;

        private List<Node> _pathNodes = new();
        private List<Node> _allNodes = new();

        private Tween _pathTween;
        private Node _movingNode;


        private void Start()
        {
            InitializeGraph();
        }

        public void HandleNodeClick(Node node)
        {
            if (node.AnimalType == AnimalType.Fox)
            {
                Debug.Log("CLICKED TO FOX. FAIL");
                EventManager.GameLoseExecute?.Invoke();
                return;
            }

            if (node.IsDirty)
            {
                Debug.Log("Dirty cat can not move");
                return;
            }

            StartMovingOnPath(node);
        }

        public bool HandleKittyJump(Node jumperNode)
        {
            if (jumperNode.AnimalType == AnimalType.Fox)
            {
                Debug.Log("FOX isn't a kitty!");
                return false;
            }

            if (jumperNode.IsDirty)
            {
                Debug.Log("Dirt kitty can not jump!");
                return false;
            }

            var jumperKittyPathToRootNode = FindShortestPathToRootNode(jumperNode);
            if (jumperKittyPathToRootNode != null)
            {
                Debug.Log("Jumpler kitty has path to root node!");
                return false;
            }


            var neighboursPath = new List<List<Node>>();
            var movableNodes = new List<Node>();
            var visitedNodes = new List<Node>();

            visitedNodes.Add(jumperNode);
            movableNodes.Add(jumperNode);

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



            // find shortest path
            var minNodeAmount = int.MaxValue;
            var selectedPath = new List<Node>();

            foreach (var path in neighboursPath)
            {
                if (path?.Count < minNodeAmount)
                {
                    selectedPath = path;
                    minNodeAmount = path.Count;
                }
            }

            // check is there a path
            if (selectedPath.Count <= 0)
            {
                Debug.Log("This no place to jump and move to nood");
                return false;
            }


            Tween pathToJumpNodeTween = null;
            var pathToJumpAboveNode = FindPathToNode(jumperNode, selectedPath[0], false);
            if (pathToJumpAboveNode?.Count > 2)
            {
                // remove jumping node from path list
                pathToJumpAboveNode.RemoveAt(pathToJumpAboveNode.Count - 1);


                // add path nodes for tween
                var pathToJumpNodePositions = new List<Vector3>();
                foreach (var node in pathToJumpAboveNode)
                    pathToJumpNodePositions.Add(node.transform.position);


                pathToJumpNodeTween = jumperNode.NodeObject.transform.
                    DOPath(pathToJumpNodePositions.ToArray(), 10, PathType.Linear, PathMode.Full3D)
                    .SetSpeedBased(true).SetLookAt(0.01f);
            }



            // add path nodes for tween
            var pathPositionList = new List<Vector3>();
            foreach (var node in selectedPath)
                pathPositionList.Add(node.transform.position);

            // removing jumped node
            pathPositionList.RemoveAt(0);

            // set node state to available
            SetNodeStateToAvailable(jumperNode);


            var pathToRootTween = // define kitty tweens
                    jumperNode.NodeObject.transform.DOJump(selectedPath[1].transform.position, 2, 1, .5f)
                        .OnComplete(() =>
                        {
                            // check if landed node is root node
                            if (selectedPath[1] == rootNode)
                            {
                                collectManager.CollectCat(jumperNode.NodeObject);
                            }
                            else
                            {
                                // play path tween
                                _pathTween = jumperNode.NodeObject.transform.DOPath(pathPositionList.ToArray(), 10, PathType.Linear, PathMode.Full3D)
                                    .SetSpeedBased(true).OnStart(() => CheckWillTheyFight()).SetLookAt(0.01f).OnComplete(() =>
                                    {
                                        collectManager.CollectCat(jumperNode.NodeObject);
                                    });
                            }
                        });
            pathToRootTween.Pause();

            if (pathToJumpNodeTween != null)
                pathToJumpNodeTween.OnComplete(() => pathToRootTween.Play());
            else
                pathToRootTween.Play();

            return true;
        }

        public List<Node> FindShortestPathToRootNode(Node startNode)
        {
            return FindPathToNode(startNode, rootNode);
        }

        private void StartMovingOnPath(Node startNode)
        {
            _pathNodes?.Clear();
            _pathNodes = FindShortestPathToRootNode(startNode);

            if (_pathNodes != null)
                StartPath(_pathNodes[0]);
            else
                Debug.Log("Path not found!");
        }

        private List<Node> FindPathToNode(Node startNode, Node endNode, bool LookingForAEmptyNode = true)
        {
            Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
            Queue<Node> frontier = new Queue<Node>();
            frontier.Enqueue(startNode);

            while (frontier.Count > 0)
            {
                Node current = frontier.Dequeue();

                if (current == endNode)
                    break;


                foreach (Node next in current.Neighbours)
                {
                    if (!cameFrom.ContainsKey(next))
                    {
                        if (LookingForAEmptyNode)
                        {
                            if (next.IsEmpty)
                            {
                                frontier.Enqueue(next);
                                cameFrom[next] = current;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            frontier.Enqueue(next);
                            cameFrom[next] = current;
                        }
                    }
                }
            }

            // if there is no path return a null
            if (cameFrom.Count <= 0 || !cameFrom.ContainsKey(endNode))
            {
                return null;
            }

            List<Node> path = new List<Node>();
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = cameFrom[currentNode];
            }

            path.Add(startNode);
            path.Reverse();

            return path;
        }

        private void InitializeGraph()
        {
            GetAllNodes();
            AssignAllNeighbours();
            CreatePathLines();
        }

        private void GetAllNodes()
        {
            var children = Helpers.GetAllChildren(nodesParent);
            foreach (var child in children)
            {
                _allNodes.Add(child.GetComponent<Node>());
            }
        }

        private void AssignAllNeighbours()
        {
            foreach (var node in _allNodes)
            {
                foreach (var searchNode in _allNodes)
                {
                    if (node == searchNode) continue;
                    if (node.Neighbours.Contains(searchNode)) continue;

                    if (searchNode.Neighbours.Contains(node))
                        node.Neighbours.Add(searchNode);
                }
            }
        }

        private void StartPath(Node movingNode)
        {
            // set node state to available
            SetNodeStateToAvailable(movingNode);


            // add path nodes for tween
            var pathPositionList = new List<Vector3>();
            foreach (var node in _pathNodes)
                pathPositionList.Add(node.transform.position);

            // play path tween
            _pathTween = movingNode.NodeObject.transform.DOPath(pathPositionList.ToArray(), 10, PathType.Linear, PathMode.Full3D)
                .SetSpeedBased(true).OnStart(() => CheckWillTheyFight()).SetLookAt(0.01f).OnComplete(() =>
                {
                    collectManager.CollectCat(movingNode.NodeObject);
                });
        }

        private void SetNodeStateToAvailable(Node movingNode)
        {
            _movingNode = movingNode;
            movingNode.SetNodeAvailable();

            // check neighbour dirt state
            foreach (var neighbourNode in _movingNode.Neighbours)
            {
                if (neighbourNode.IsDirty)
                    neighbourNode.IsDirty = false;
            }
        }

        private void CreatePathLines()
        {

            List<ValueTuple<Node, Node>> linedFromTo = new();

            var lineParent = new GameObject("LineParent");
            lineParent.transform.SetParent(gameObject.transform);

            foreach (var node in _allNodes)
            {
                foreach (var searchNode in _allNodes)
                {
                    if (node == searchNode) continue;
                    if (!node.Neighbours.Contains(searchNode)) continue;
                    if (linedFromTo.Contains((node, searchNode))) continue;
                    if (linedFromTo.Contains((searchNode, node))) continue;


                    linedFromTo.Add((node, searchNode));

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

        private void CheckWillTheyFight()
        {
            var remainigAnimalsList = new List<Node>();

            foreach (var node in _allNodes)
            {
                if (!node.IsEmpty)
                    remainigAnimalsList.Add(node);
            }

            // remaining animal amount less or equal to 1 or more than 2, there will be no fight
            if (remainigAnimalsList.Count > 2 || remainigAnimalsList.Count <= 1) return;

            var animal0 = remainigAnimalsList[0];
            var animal1 = remainigAnimalsList[1];

            // animal types are different, there will be no fight
            if (animal0.AnimalType == animal1.AnimalType) return;


            // they are not neigbours, there will be no fight
            if (!AreTheyNeighbour(animal0, animal1)) return;

            // override path tween on start action
            _pathTween.OnStart(() => collectManager.LockCollectAction()).OnComplete(() =>
            {
                collectManager.CollectCat(_movingNode.NodeObject);

                // there will be fight. FAIL
                Debug.Log("They are fighting. FAIL");
                EventManager.GameLoseExecute?.Invoke();
            });



        }

        private bool AreTheyNeighbour(Node node1, Node node2)
        {
            return node1.Neighbours.Contains(node2) || node2.Neighbours.Contains(node1);
        }
    }
}