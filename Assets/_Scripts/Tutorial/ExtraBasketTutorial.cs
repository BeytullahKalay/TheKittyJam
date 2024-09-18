using _Scripts.CollectibleController;
using _Scripts.Node;
using UnityEngine;
using UnityEngine.Events;

namespace Tutorials
{
    public class ExtraBasketTutorial : TutorialBase
    {
        [SerializeField, Range(0, 1)] private float pointerHoleRadius = .035f;

        private CanvasButtonsController _canvasButtonController;
        private StackManager _stackManager;
        private bool _isOpened;
        private Transform _lastStackTransform;


        public override void OnEnable()
        {
            base.OnEnable();
            EventManager.KittyStartTurnExecute += OnKittyTurnExecute;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            EventManager.KittyStartTurnExecute -= OnKittyTurnExecute;

        }

        private void Awake()
        {
            _canvasButtonController = CanvasButtonsController.Instance;
            _stackManager = StackManager.Instance;
        }


        private void OnKittyTurnExecute(NodeObject movingNodeObject)
        {
            if (_isOpened) return;


            _isOpened = true;

            var t = new UnityEvent();
            t.AddListener(_canvasButtonController.AddExtraBasketButton);

            var emptyUnityEvet = new UnityEvent();

            TutorialData.Add(new TutorialData(_canvasButtonController.ExtraStackButton.transform, pointerHoleRadius, t));

            TutorialCanvas.SetActive(true);

            _canvasButtonController.ExtraStackButton.gameObject.SetActive(true);
            InitializeTutorailPanelAndButton();

            // set extra button
            PlayerPrefs.SetInt(PlayerPrefNames.ExtraStackButton, 1);
        }
    }
}
