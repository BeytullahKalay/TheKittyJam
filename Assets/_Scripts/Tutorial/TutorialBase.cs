using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Tutorials
{

    public abstract class TutorialBase : MonoBehaviour
    {
        [SerializeField] protected GameObject TutorialCanvas;
        [SerializeField] protected Button ActionButton;
        [SerializeField] private Material panelMaterial;
        [SerializeField] protected List<TutorialData> TutorialData = new List<TutorialData>();


        private RectTransform _actionButtonRectTransform;

        public int CurrentTutorialIndex { get; private set; } = 0;

        public virtual void OnEnable()
        {
            ActionButton.onClick.AddListener(() => GetNextPointPosition());
        }

        public virtual void OnDisable()
        {
            ActionButton.onClick.RemoveListener(() => GetNextPointPosition());
        }

        private void Awake()
        {
        }



        public void InitializeTutorailPanelAndButton()
        {
            GameManager.Instance.SetGameStateToTutorial();
            CurrentTutorialIndex = 0;

            GetNextPointPosition();
        }

        private void GetNextPointPosition()
        {

            if (CurrentTutorialIndex >= TutorialData.Count)
            {
                GameManager.Instance.SetGameStateToPlay();
                TutorialCanvas.SetActive(false);
                gameObject.SetActive(false);
                return;
            }

            if (CurrentTutorialIndex > 0)
            {
                var previousAction = TutorialData[CurrentTutorialIndex - 1].Action;
                ActionButton.onClick.RemoveListener(previousAction.Invoke);
            }


            _actionButtonRectTransform = ActionButton.GetComponent<RectTransform>();


            Vector3 objViewPos = Vector3.zero;
            if (TutorialData[CurrentTutorialIndex].ObjToPoint.transform is RectTransform)
            {
                var rectTransform = TutorialData[CurrentTutorialIndex].ObjToPoint.GetComponent<RectTransform>();
                _actionButtonRectTransform.position = rectTransform.position;
                objViewPos = Camera.main.ScreenToViewportPoint(rectTransform.position);
            }
            else
            {
                var objToScreenPos = Camera.main.WorldToScreenPoint(TutorialData[CurrentTutorialIndex].ObjToPoint.position);
                _actionButtonRectTransform.position = objToScreenPos;
                objViewPos = Camera.main.WorldToViewportPoint(TutorialData[CurrentTutorialIndex].ObjToPoint.position);
            }

            panelMaterial.SetVector("_HoleCenter", new Vector4(objViewPos.x, objViewPos.y, 0, 0));
            panelMaterial.SetFloat("_HoleRadius", TutorialData[CurrentTutorialIndex].HolerRadius);

            var curentAction = TutorialData[CurrentTutorialIndex].Action;
            ActionButton.onClick.AddListener(curentAction.Invoke);

            CurrentTutorialIndex++;
        }
    }

    [System.Serializable]
    public struct TutorialData
    {
        public Transform ObjToPoint;
        [Range(0, 1)] public float HolerRadius;
        public UnityEvent Action;

        public TutorialData(Transform ObjToPoint, float HolerRadius, UnityEvent Action)
        {
            this.ObjToPoint = ObjToPoint;
            this.HolerRadius = HolerRadius;
            this.Action = Action;
        }
    }
}
