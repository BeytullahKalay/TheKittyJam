using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MenuScripts
{
    public class LevelButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private HealthManager healthManager;
        [SerializeField] private Transform bottomLabelAndText;
        [SerializeField] private TMP_Text _bottomLabelText;
        [SerializeField] private Image pointImage;
        [SerializeField] private Transform catFace;
        [SerializeField] private TMP_Text completedLevelText;
        [SerializeField] private Sprite playerPointImage;

        private bool _clickable = false;
        private const string LEVEL_TEXT = "Level ";

        public void InitializeButton(int levelIndex)
        {
            _bottomLabelText.text = LEVEL_TEXT + (levelIndex + 1).ToString();
            completedLevelText.text = (levelIndex + 1).ToString();
        }

        public void SetButtonPlayed()
        {
            bottomLabelAndText.gameObject.SetActive(false);
            catFace.gameObject.SetActive(false);
            pointImage.sprite = playerPointImage;
        }

        public void SetButtonAvailable()
        {
            completedLevelText.gameObject.SetActive(false);
            _clickable = true;
            transform.localScale += Vector3.one;
        }

        public void SetButtonLocked()
        {
            bottomLabelAndText.gameObject.SetActive(false);
            catFace.gameObject.SetActive(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_clickable) return;
            if (!healthManager.IsHaveLive())
            {
                Debug.Log("No Health");
                return;
            }

            //healthManager.DecreaseLife();
            FakeLoadPanel.Instance.LoadScene(SceneNames.GameScene);
        }
    }
}