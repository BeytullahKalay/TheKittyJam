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

        private StackDataHolder[] stackDataHolderArray;

        private void Awake()
        {
            stackDataHolderArray = new StackDataHolder[stackAmount];
        }

        private void Start()
        {
            InitializeStackArea();
        }

        public void AddObjectToStack(NodeObject nodeObject)
        {
            Debug.Log("Add object to stack");
            var getEmptyStackIndex = FindEmptyStackIndex();
            stackDataHolderArray[getEmptyStackIndex].StackGameObject = nodeObject.gameObject;
            nodeObject.gameObject.transform.position = stackDataHolderArray[getEmptyStackIndex].StackTransform.position;
            nodeObject.gameObject.transform.rotation = Quaternion.identity;
        }

        private int FindEmptyStackIndex()
        {
            for (int i = 0; i < stackAmount - 1; i++)
            {
                if (stackDataHolderArray[i].StackGameObject == null) return i;
            }

            Debug.LogError("STACK IS FULL. FAIL");

            return -1;
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
                stackDataHolderArray[i] = new StackDataHolder(null, obj.transform);
            }

            var pos = stackGroundSpawnTransform.position;
            pos.x -= ((float)stackAmount / 2) + (offset / 2);
            stackGroundSpawnTransform.position = pos;
        }

        private struct StackDataHolder
        {
            public StackDataHolder(GameObject stackGameObject, Transform stackTransform)
            {
                StackGameObject = stackGameObject;
                StackTransform = stackTransform;
            }

            public GameObject StackGameObject;
            public Transform StackTransform;
        }
    }
}
