using _Scripts.Node;
using UnityEngine;

public class MouseInputController : MonoSingleton<MouseInputController>
{
    [SerializeField] private GraphManager nodePathfinding;
    [SerializeField] private GameManager gameManager;

    private InputState _inputState;

    private CanvasButtonsController _canvasButtonController;

    private void Awake()
    {
        _canvasButtonController = CanvasButtonsController.Instance;
    }


    public void ToggleKittyJumpState()
    {
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
        if (gameManager.GameState != GameState.Play) return;
        if (!Input.GetMouseButtonDown(0)) return;

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            var selection = hit.transform;


            if (selection.parent.TryGetComponent<NodeFSM>(out var node))
            {
                if (_inputState == InputState.Normal)
                {
                    node.HandleNodeClick();
                }
                else if (_inputState == InputState.KittyJump)
                {
                    node.HandleKittyJump();
                    _canvasButtonController.DecreaseKittyJumpAmount();
                    SetInputStateToNormal();
                }

            }

        }
    }
}
