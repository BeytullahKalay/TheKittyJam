using _Scripts.Node;
using UnityEngine;

public class DogTutorialPart2 : MonoBehaviour
{
    [SerializeField] private GameObject tutorialCanvas;
    [SerializeField] private GameObject tipTextGameObj;
    [SerializeField] private GameObject descTextGameObj;
    [SerializeField] private Material customMat;

    private int _turn = 0;
    private bool _isOpened;


    private void OnEnable()
    {
        EventManager.KittyStartTurnExecute += CheckTipText;
    }


    private void OnDisable()
    {
        EventManager.KittyStartTurnExecute -= CheckTipText;

    }

    private void CheckTipText(NodeObject movingKittyObject)
    {
        Debug.Log("test");
        _turn++;

        if (_turn < 2) return;
        if (_isOpened) return;
        _isOpened = true;

        tutorialCanvas.SetActive(true);
        customMat.SetFloat("_HoleRadius", 0);
        tipTextGameObj.SetActive(true);
        descTextGameObj.SetActive(false);
    }


}
