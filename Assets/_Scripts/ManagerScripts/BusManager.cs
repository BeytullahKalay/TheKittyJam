using _Scripts.Node;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace _Scripts.CollectibleController
{

    public class BusManager : MonoBehaviour
    {
        [SerializeField] private CollectManager _collectManager;
        [SerializeField] private Transform busGetCatPosition;
        [SerializeField] private Transform busMovePosition;
        [SerializeField] private GameObject busPrefab;
        [SerializeField] private float busSpawnOffset;
        [SerializeField] private BusTypeAndMaterialDataSO busTypeAndMaterialDataSO;
        [SerializeField] private List<AnimalType> busCollectibleType = new();


        public AnimalType ActiveBusAnimaltype { get; private set; }


        private Bus _activeBus;
        private GameObject _busParentHolder;
        private Action OnBusFulledAction;
        private List<Bus> _spawnedBusses = new();

        private void OnEnable()
        {
            OnBusFulledAction += OnBusFulled;
        }

        private void OnDisable()
        {
            OnBusFulledAction += OnBusFulled;
        }


        private void Start()
        {
            InitializeBussManager();
        }

        public void CollectCat(NodeObject nodeObject)
        {
            _activeBus.CollectAnimal(nodeObject.gameObject, OnBusFulled);
        }

        private void InitializeBussManager()
        {
            _busParentHolder = new GameObject("busParentHolder");
            _busParentHolder.transform.SetParent(transform);

            SpawnBuses();

            _activeBus = _spawnedBusses[0];

            // set active bus animal type
            ActiveBusAnimaltype = busCollectibleType.First();
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


                busScript.InitilizeBuss(busCollectibleType[i], busTypeAndMaterialDataSO.GetBusMaterial(busCollectibleType[i]));

                // add to list
                _spawnedBusses.Add(busScript);

            }
        }

        private void OnBusFulled()
        {
            var oldBus = _activeBus;
            oldBus.transform.DOMoveX(busMovePosition.position.x, 1f).OnStart(() =>
            {
                if (_collectManager == null) Debug.LogError("collect manager is null.");
                _collectManager.OnBusLeavingTheCollectPos?.Invoke();
            });

            // remove first object from bus and bus collectible type from lists
            _spawnedBusses.RemoveAt(0);
            busCollectibleType.RemoveAt(0);

            // check is we still have bus in list
            if (_spawnedBusses.Count > 0)
            {
                // assign new values to active bus and active bus animal type
                _activeBus = _spawnedBusses[0];
                ActiveBusAnimaltype = busCollectibleType[0];

                // move new bus to stop position
                _activeBus.transform.DOMoveX(busGetCatPosition.position.x, 1f).OnComplete(() =>
                {
                    if (_collectManager == null) Debug.LogError("collect manager is null.");
                    _collectManager.OnBusStopOnCollectPos?.Invoke();
                });
            }
            else
            {
                Debug.Log("There is no more bus on queue. YOU WON!");
                EventManager.GameWinExecute?.Invoke();
            }

        }

    }
}
