using _Scripts.Node;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.CollectibleController
{
    public class CollectManager : MonoSingleton<CollectManager>
    {
        [SerializeField] private BusManager _busManager;
        [SerializeField] private StackManager _stackManager;

        public Action OnBusLeavingTheCollectPos;
        public Action OnBusStopOnCollectPos;


        private Queue<NodeObject> _collecObjectQue = new();

        private bool _lock = false;

        private void OnEnable()
        {
            OnBusLeavingTheCollectPos += LockCollectAction;
            OnBusStopOnCollectPos += UnlockCollectAction;
            OnBusStopOnCollectPos += CheckStackObjects;
        }

        private void OnDisable()
        {
            OnBusLeavingTheCollectPos -= LockCollectAction;
            OnBusStopOnCollectPos -= UnlockCollectAction;
            OnBusStopOnCollectPos -= CheckStackObjects;
        }

        public void CollectCat(NodeObject nodeObject)
        {
            _collecObjectQue.Enqueue(nodeObject);
        }



        private void Update()
        {
            if (!_lock && _collecObjectQue.Count > 0)
            {
                var nodeObject = _collecObjectQue.Dequeue();

                if (nodeObject.AnimalType == _busManager.ActiveBusAnimaltype)
                    _busManager.CollectCat(nodeObject);
                else
                    _stackManager.AddObjectToStack(nodeObject);
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

        private void CheckStackObjects()
        {
            var arr = _stackManager.GetStackDataHolderArray();

            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i].StackGameObject == null) continue;

                if (arr[i].StackGameObject.AnimalType == _busManager.ActiveBusAnimaltype)
                {
                    _busManager.CollectCat(arr[i].StackGameObject);
                    arr[i].StackGameObject = null;
                }
            }
        }
    }
}
