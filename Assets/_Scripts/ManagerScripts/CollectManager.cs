using _Scripts.Node;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.CollectibleController
{
    public class CollectManager : MonoSingleton<CollectManager>
    {
        [SerializeField] private BusManager _busManager;
        [SerializeField] private StackManager stackManager;

        public Action OnBusLeavingTheCollectPos;
        public Action OnBusStopOnCollectPos;


        private Queue<NodeObject> _collecObjectQue = new();

        private bool _lock = false;



        private void OnEnable()
        {
            EventManager.OnLevelLoadPrepare += ClearManagerData;
            OnBusLeavingTheCollectPos += LockCollectAction;
            OnBusStopOnCollectPos += UnlockCollectAction;
        }

        private void OnDisable()
        {
            EventManager.OnLevelLoadPrepare -= ClearManagerData;
            OnBusLeavingTheCollectPos -= LockCollectAction;
            OnBusStopOnCollectPos -= UnlockCollectAction;
        }

        public void CollectCat(NodeObject nodeObject)
        {
            _collecObjectQue.Enqueue(nodeObject);
            nodeObject.SetIsCollectedToTrue();
        }



        private void Update()
        {
            if (!_busManager.HasBus()) return;

            if (_busManager.ActiveBus.IsFull()) return;

            if (!_busManager.BusOnAnimation)
            {
                var arr = stackManager.GetStackDataHolderArray();

                for (int i = 0; i < arr.Count; i++)
                {

                    if (arr[i].StackGameObject == null) continue;
                    if (arr[i].StackGameObject.AnimalType != _busManager.ActiveBus.CollectibleAnimalType) continue;
                    if (_busManager.ActiveBus.IsFull()) continue;


                    _busManager.CollectAnimal(arr[i].StackGameObject);


                    if (arr.Count > 0)
                        arr[i].StackGameObject = null;


                    // on CollectAnimal we have a action parameter which is start changing bus move animation state
                    // because of this we should check bus animation here as well
                    if (_busManager.BusOnAnimation) return;


                }
            }

            if (!_lock && !_busManager.BusOnAnimation && _collecObjectQue.Count > 0)
            {
                //stackManager.AddObjectToStack(_collecObjectQue.Dequeue());

                var nodeObject = _collecObjectQue.Dequeue();

                if (nodeObject.AnimalType == _busManager.ActiveBus.CollectibleAnimalType)
                    _busManager.CollectAnimal(nodeObject);
                else
                    stackManager.AddObjectToStack(nodeObject);
            }
        }

        public void LockCollectAction()
        {
            _lock = true;
        }

        private void UnlockCollectAction()
        {
            _lock = false;
        }

        private void ClearManagerData()
        {
            _collecObjectQue.Clear();
            _lock = false;
        }
    }
}
