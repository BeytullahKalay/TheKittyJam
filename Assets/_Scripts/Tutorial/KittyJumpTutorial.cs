using _Scripts.CollectibleController;
using _Scripts.Node;
using UnityEngine;
using UnityEngine.Events;

namespace Tutorials
{
    public class KittyJumpTutorial : TutorialBase
    {
        [SerializeField, Range(0, 1)] private float pointerHoleRadius = .3f;

        private BusManager _busManager;
        private CanvasButtonsController _canvasButtonController;
        private bool _isOpened;


        private void Awake()
        {
            _busManager = BusManager.Instance;
            _canvasButtonController = CanvasButtonsController.Instance;
        }

        public override void OnEnable()
        {
            base.OnEnable();
            EventManager.KittyStartTurnExecute += CheckBusCollectAmount;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            EventManager.KittyStartTurnExecute -= CheckBusCollectAmount;

        }


        private void CheckBusCollectAmount(NodeObject movingCatObject)
        {

            if (_isOpened) return;
            if (!_busManager.HasBus()) return;

            if (movingCatObject.AnimalType != AnimalType.Yellow) return;


            if (_busManager.ActiveBus.CurrentCollectIndex >= 1)
            {
                _isOpened = true;

                var t = new UnityEvent();
                t.AddListener(_canvasButtonController.KittyJumpButtonAction);
                t.AddListener(_canvasButtonController.DecreaseKittyJumpAmount);

                TutorialData.Insert(0, new TutorialData(_canvasButtonController.KittyJumpButton.transform, pointerHoleRadius, t));

                TutorialCanvas.SetActive(true);
                _canvasButtonController.KittyJumpButton.gameObject.SetActive(true);
                InitializeTutorailPanelAndButton();

                // set jump button
                PlayerPrefs.SetInt(PlayerPrefNames.KittyJumpButton, 1);
            }
        }


    }
}
