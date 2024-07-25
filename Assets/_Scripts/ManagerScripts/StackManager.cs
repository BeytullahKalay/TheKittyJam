using _Scripts.Node;
using UnityEngine;

namespace _Scripts.CollectibleController
{

    public class StackManager : MonoBehaviour
    {
        [SerializeField] private GameObject stackGroundObj;
        [SerializeField] private Transform stackGroundSpawnTransform;
        [SerializeField] private int stackAmount = 6;
        [SerializeField] private float offset = 1f;

        private StackDataHolder[] _stackDataHolderArray;

        private void Awake()
        {
            _stackDataHolderArray = new StackDataHolder[stackAmount];
        }

        private void Start()
        {
            InitializeStackArea();
        }

        public void AddObjectToStack(NodeObject nodeObject)
        {
            var getEmptyStackIndex = FindEmptyStackIndex();
            CheckStackIsFulled(getEmptyStackIndex);
            _stackDataHolderArray[getEmptyStackIndex].StackGameObject = nodeObject;
            nodeObject.gameObject.transform.position = _stackDataHolderArray[getEmptyStackIndex].StackTransform.position;
            nodeObject.gameObject.transform.rotation = Quaternion.identity;
        }

        public StackDataHolder[] GetStackDataHolderArray()
        {
            return _stackDataHolderArray;
        }

        private int FindEmptyStackIndex()
        {
            for (int i = 0; i < stackAmount; i++)
            {
                if (_stackDataHolderArray[i].StackGameObject == null) return i;
            }
            Debug.LogError("No empty space in stack");
            return -1;
        }

        private void CheckStackIsFulled(int foundIndex)
        {
            if (foundIndex >= _stackDataHolderArray.Length - 1)
            {
                Debug.Log("STACK IS FULL. FAIL");
                EventManager.GameLoseExecute?.Invoke();
            }
        }

        private void InitializeStackArea()
        {
            for (int i = 0; i < stackAmount; i++)
            {
                // find spawn position for stack ground object
                var spawnPostion = stackGroundSpawnTransform.position;
                spawnPostion.x = i * offset;

                // spawn stack ground object
                var obj = Instantiate(stackGroundObj, spawnPostion, Quaternion.identity);

                // set parent
                obj.transform.SetParent(stackGroundSpawnTransform);


                // add object to array
                _stackDataHolderArray[i] = new StackDataHolder(null, obj.transform);
            }

            var pos = stackGroundSpawnTransform.position;
            pos.x -= ((float)stackAmount / 2) + (offset / 2);
            stackGroundSpawnTransform.position = pos;
        }

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
