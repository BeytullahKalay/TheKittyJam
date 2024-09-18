using _Scripts.Node;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace _Scripts.CollectibleController
{

    public class BusManager : MonoSingleton<BusManager>
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private CollectManager collectManager;
        [SerializeField] private Transform busGetCatPosition;
        [SerializeField] private Transform busMovePosition;
        [SerializeField] private GameObject busPrefab;
        [SerializeField] private float busSpawnOffset;
        [SerializeField] private TypeAndMaterialDataSO busTypeAndMaterialDataSO;
        [SerializeField] private List<AnimalType> busCollectibleType = new();


        public Bus ActiveBus { get; private set; }
        public bool BusOnAnimation { get; set; }


        private float _moveOnXDistance;
        private GameObject _busParentHolder;
        private Action OnBusFulledAction;
        private List<Bus> _spawnedBusses = new();
        private List<GameObject> _movedBusses = new List<GameObject>();

        private void OnEnable()
        {
            EventManager.OnLevelLoadPrepare += ClearLevelData;
            OnBusFulledAction += OnBusFulled;
        }

        private void OnDisable()
        {
            EventManager.OnLevelLoadPrepare -= ClearLevelData;
            OnBusFulledAction += OnBusFulled;
        }

        private void ClearLevelData()
        {
            BusOnAnimation = false;
            busCollectibleType.Clear();
            ActiveBus = null;
            Destroy(_busParentHolder);
            _moveOnXDistance = 0;
            _busParentHolder = null;
            _spawnedBusses.Clear();

            foreach (var bus in _movedBusses)
                Destroy(bus);
            _movedBusses.Clear();

        }

        public void Initialize(List<AnimalType> busCollectibleType)
        {
            this.busCollectibleType = busCollectibleType;

            _busParentHolder = new GameObject("busParentHolder");
            _busParentHolder.transform.position = Vector3.zero;
            _busParentHolder.transform.SetParent(transform);

            SpawnBuses();

            ActiveBus = _spawnedBusses[0];

            if (_spawnedBusses.Count > 1)
                _moveOnXDistance = Mathf.Abs(_spawnedBusses[1].transform.position.x - busGetCatPosition.transform.position.x);
        }

        public void CollectAnimal(NodeObject nodeObject)
        {
            ActiveBus.CollectAnimal(nodeObject, OnBusFulled);
        }

        public bool HasBus()
        {
            return _spawnedBusses.Count > 0;
        }

        private void SpawnBuses()
        {
            // spawn busses
            for (int i = 0; i < busCollectibleType.Count; i++)
            {
                // find spawn position
                var spawnPos = busGetCatPosition.position;
                spawnPos.y += busPrefab.transform.localScale.y;
                spawnPos.x -= (busPrefab.transform.localScale.x + busSpawnOffset) * i;

                // spawn bus
                var busObj = Instantiate(busPrefab, spawnPos, Quaternion.identity);

                // set parent
                busObj.transform.SetParent(_busParentHolder.transform);

                // set bus script animal type
                var busScript = busObj.GetComponent<Bus>();


                busScript.InitilizeBuss(busCollectibleType[i], busTypeAndMaterialDataSO.GetTypeMaterial(busCollectibleType[i]));

                // add to list
                _spawnedBusses.Add(busScript);

            }
        }

        private void OnBusFulled()
        {
            var oldBus = ActiveBus;
            BusOnAnimation = true;

            oldBus.transform.SetParent(null);
            _movedBusses.Add(oldBus.gameObject);
            oldBus.TriggerMoveAnimation();

            var moveTweenDuration = gameManager.GameSettingsDataSO.MotherCatMoveDuration;

            oldBus.transform.DOMoveX(busMovePosition.position.x, moveTweenDuration).OnStart(() =>
            {
                if (collectManager == null) Debug.LogError("collect manager is null.");
                collectManager.OnBusLeavingTheCollectPos?.Invoke();
            }).OnComplete(() =>
            {
                if (_spawnedBusses.Count <= 0)
                    EventManager.GameWinExecute?.Invoke();

                oldBus.TriggerIdleAnimation();
            });

            // remove first object from bus and bus collectible type from lists
            _spawnedBusses.RemoveAt(0);
            busCollectibleType.RemoveAt(0);

            // check is we still have bus in list
            if (_spawnedBusses.Count > 0)
            {
                // assign new values to active bus and active bus animal type
                ActiveBus = _spawnedBusses[0];

                // move new bus to stop position
                _busParentHolder.transform.DOMoveX(_busParentHolder.transform.position.x + _moveOnXDistance, moveTweenDuration).OnStart(() =>
                {
                    ActiveBus.TriggerMoveAnimation();

                    if (_spawnedBusses.Count > 1)
                    {
                        for (int i = 1; i < _spawnedBusses.Count; i++)
                        {
                            _spawnedBusses[i].TriggerMoveAnimation();
                        }
                    }

                }).OnComplete(() =>
                {
                    BusOnAnimation = false;
                    if (collectManager == null) Debug.LogError("collect manager is null.");
                    collectManager.OnBusStopOnCollectPos?.Invoke();
                    ActiveBus.TriggerIdleAnimation();


                    if (_spawnedBusses.Count > 1)
                    {
                        for (int i = 1; i < _spawnedBusses.Count; i++)
                        {
                            _spawnedBusses[i].TriggerIdleAnimation();
                        }
                    }

                });





            }
            else
            {
                GameManager.Instance.SetGameStateToWin();
            }

        }

    }
}
