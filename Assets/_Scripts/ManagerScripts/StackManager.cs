using _Scripts.Node;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.CollectibleController
{

    public class StackManager : MonoSingleton<StackManager>
    {
        [SerializeField] private GameObject stackGroundObj;
        [SerializeField] private Transform stackGroundSpawnTransform;
        [SerializeField] private int stackAmount = 6;
        [SerializeField] private float offset = 1f;

        private List<StackDataHolder> _stackDataHolderList = new List<StackDataHolder>();


        private void Start()
        {
            InitializeStackArea();
        }

        public void AddObjectToStack(NodeObject nodeObject)
        {
            var getEmptyStackIndex = FindEmptyStackIndex();
            CheckStackIsFulled(getEmptyStackIndex);
            _stackDataHolderList[getEmptyStackIndex].StackGameObject = nodeObject;
            nodeObject.gameObject.transform.position = _stackDataHolderList[getEmptyStackIndex].StackTransform.position;
            nodeObject.gameObject.transform.rotation = Quaternion.identity;
            nodeObject.gameObject.transform.SetParent(_stackDataHolderList[getEmptyStackIndex].StackTransform);
        }

        public List<StackDataHolder> GetStackDataHolderArray()
        {
            return _stackDataHolderList;
        }

        public void AddExtraStack()
        {
            stackAmount += 1;
            InitializeStackArea();
        }

        private int FindEmptyStackIndex()
        {
            for (int i = 0; i < stackAmount; i++)
            {
                if (_stackDataHolderList[i].StackGameObject == null) return i;
            }
            Debug.LogError("No empty space in stack");
            return -1;
        }

        private void CheckStackIsFulled(int foundIndex)
        {
            if (foundIndex >= _stackDataHolderList.Count - 1)
            {
                Debug.Log("STACK IS FULL. FAIL");
                EventManager.GameLoseExecute?.Invoke();
            }
        }

        private void InitializeStackArea()
        {

            // create stack ground object and set positions
            for (int i = _stackDataHolderList.Count; i < stackAmount; i++)
            {
                Debug.Log("stack initialized");
                // find spawn position for stack ground object
                var spawnPostion = stackGroundSpawnTransform.localPosition;
                spawnPostion.x += i * offset;

                // spawn stack ground object
                var obj = Instantiate(stackGroundObj, spawnPostion, Quaternion.identity);

                // set parent
                obj.transform.SetParent(stackGroundSpawnTransform);

                // add object to array
                _stackDataHolderList.Add(new StackDataHolder(null, obj.transform));
            }

            var pos = stackGroundSpawnTransform.position;
            var xPos = ((float)stackAmount / 2) + (offset / 2);
            pos.x = -xPos;
            stackGroundSpawnTransform.position = pos;
        }
        [System.Serializable]
        public class StackDataHolder
        {
            public StackDataHolder(NodeObject stackGameObject, Transform stackTransform)
            {
                StackGameObject = stackGameObject;
                StackTransform = stackTransform;
            }

            public NodeObject StackGameObject;
            public Transform StackTransform;
        }
    }
}
