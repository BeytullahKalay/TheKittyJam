using _Scripts.CollectibleController;
using UnityEngine;

public class CanvasButtonsController : MonoBehaviour
{
    // using by unity event
    public void AddExtraBasketButton()
    {
        StackManager.Instance.AddExtraStack();
    }
}
