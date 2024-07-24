using _Scripts.Node;
using UnityEngine;

public class MouseInputController : MonoBehaviour
{
    [SerializeField] private NodePathfinding _nodePathfinding;

    private Node _selectedNode;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var selection = hit.transform;
                _selectedNode = selection.parent.GetComponent<Node>();

                if (_selectedNode.TryGetComponent<Node>(out var node) && !node.IsEmpty)
                    _nodePathfinding.StartMovingOnPath(_selectedNode);

            }
        }

    }
}