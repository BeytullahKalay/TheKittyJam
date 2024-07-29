using _Scripts.CollectibleController;
using UnityEngine;

public class CanvasButtonsController : MonoBehaviour
{
    // using by unity event
    public void AddExtraBasketButton()
    {
        StackManager.Instance.AddExtraStack();
    }

    // using by unity event
    public void KittyJumpButton()
    {
        MouseInputController.Instance.ToggleKittyJumpState();
    }
}
