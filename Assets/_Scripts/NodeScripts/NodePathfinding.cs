using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Scripts.Node
{

    public class NodePathfinding : MonoBehaviour
    {
        [SerializeField] private Node rootNode;

        //private Node _selectedNode;
        private List<Node> _pathNodes = new List<Node>();
        private StackManager _stackManager;

        private void Awake()
        {
            _stackManager = StackManager.Instance;
        }

        public void StartMovingOnPath(Node startNode)
        {

            _pathNodes.Clear();

            _pathNodes.Add(startNode);
            FindPathToRoot(startNode, ref _pathNodes);
            if (_pathNodes.Count > 1)
            {
                startNode.SetNodeWalkable();

                var spawnPositionList = new List<Vector3>();
                foreach (var node in _pathNodes)
                {
                    spawnPositionList.Add(node.transform.position);
                }


                startNode.SpawnedCatModel.transform.DOPath(spawnPositionList.ToArray(), 10, PathType.Linear, PathMode.Full3D)
                    .SetSpeedBased(true).SetLookAt(0.01f).OnComplete(() =>
                    {
                        _stackManager.AddObjectToStack(startNode);
                    });
            }
            else
            {
                Debug.LogWarning("Path not found for this node: " + startNode.gameObject.name);
            }
        }

        private void FindPathToRoot(Node node, ref List<Node> pathList)
        {
            foreach (Node parent in node.ParentNodes)
            {
                if (parent.IsEmpty)
                {
                    pathList.Add(parent);
                    if (parent == rootNode) break;


                    FindPathToRoot(parent, ref pathList);

                    if (pathList.Last() != rootNode)
                        pathList.Remove(pathList[^1]);
                    else
                        break;
                }
            }
        }
    }
}