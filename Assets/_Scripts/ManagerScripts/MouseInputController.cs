using _Scripts.Node;
using UnityEngine;

public class MouseInputController : MonoSingleton<MouseInputController>
{
    [SerializeField] private GraphManager _nodePathfinding;

    private InputState _inputState;

    private Node _selectedNode;

    public void ToggleKittyJumpState()
    {
        //_inputState = InputState.KittyJump;

        if (_inputState == InputState.KittyJump)
        {
            _inputState = InputState.Normal;
            Debug.Log("kitty jump deactivated");
        }
        else if (_inputState == InputState.Normal)
        {
            _inputState = InputState.KittyJump;
            Debug.Log("kitty jump activated");
        }
    }

    private void SetInputStateToNormal()
    {
        _inputState = InputState.Normal;
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            var selection = hit.transform;

            _selectedNode = selection.parent.GetComponent<Node>();
            if (_selectedNode.TryGetComponent<Node>(out var node) && !node.IsEmpty)
            {
                //if (node.AnimalType == AnimalType.Fox)
                //{
                //    Debug.Log("CLICKED TO FOX. FAIL");
                //    EventManager.GameLoseExecute?.Invoke();
                //    return;
                //}

                if (_inputState == InputState.Normal)
                    _nodePathfinding.HandleNodeClick(_selectedNode);
                else if (_inputState == InputState.KittyJump)
                {
                    if (GraphManager.Instance.HandleKittyJump(_selectedNode))
                        SetInputStateToNormal();
                }
            }

        }
    }
}
