using _Scripts.Node;
using DG.Tweening;
using Pandoras.Helper;
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

        private GameManager _gameManager;


        private void OnEnable()
        {
            EventManager.OnLevelLoadPrepare += ClearManagerData;
        }

        private void OnDisable()
        {
            EventManager.OnLevelLoadPrepare -= ClearManagerData;
        }


        private void Start()
        {
            _gameManager = GameManager.Instance;
        }

        public void Initialize(int stackAmount)
        {

            this.stackAmount = stackAmount;
            var objLocalScale = Vector3.zero;
            var spawnAmount = stackAmount - _stackDataHolderList.Count;

            // create stack ground object and set positions
            for (int i = _stackDataHolderList.Count; i < stackAmount; i++)
            {
                // find spawn position for stack ground object
                var spawnPostion = stackGroundSpawnTransform.localPosition;
                spawnPostion.x += i * offset;

                // spawn stack ground object
                var obj = Instantiate(stackGroundObj, spawnPostion, Quaternion.identity);

                // set parent
                obj.transform.SetParent(stackGroundSpawnTransform);
                objLocalScale = obj.transform.localScale;

                // add object to array
                _stackDataHolderList.Add(new StackDataHolder(null, obj.transform));
            }

            var pos = stackGroundSpawnTransform.position;
            var xPos = ((float)stackAmount * .5f) * objLocalScale.x + (offset * .5f) - 1f;
            pos.x = -xPos;
            stackGroundSpawnTransform.position = pos;
        }

        public void AddObjectToStack(NodeObject nodeObject)
        {
            var getEmptyStackIndex = FindEmptyStackIndex();
            if (CheckStackIsFulled(getEmptyStackIndex)) return;
            _stackDataHolderList[getEmptyStackIndex].StackGameObject = nodeObject;

            nodeObject.gameObject.transform.SetParent(_stackDataHolderList[getEmptyStackIndex].StackTransform);


            var jumpPos = _stackDataHolderList[getEmptyStackIndex].StackTransform.position + Vector3.up * .25f;
            var jumpRot = new Vector3(0, 180, 0);


            nodeObject.transform.DOJump(jumpPos, _gameManager.GameSettingsDataSO.CatJumpPower, 1, _gameManager.GameSettingsDataSO.CatJumpDuration).OnStart(() =>
            {
                nodeObject.NodeObjectAnimatorController.TriggerCatJumpAnimation();
                var dir = _stackDataHolderList[getEmptyStackIndex].StackTransform.position - nodeObject.transform.position;

                var angle = Helpers.GetAngleFromVectorFloatForRotY(dir);

                nodeObject.transform.rotation = Quaternion.Euler(Vector3.up * angle);


            }).OnComplete(() =>
            {
                nodeObject.NodeObjectAnimatorController.TriggerCatIdleAnimation();

                nodeObject.transform.DORotate(jumpRot, _gameManager.GameSettingsDataSO.CatRotateDuration, RotateMode.Fast);
            });
        }

        public List<StackDataHolder> GetStackDataHolderArray()
        {
            return _stackDataHolderList;
        }

        public void AddExtraStack()
        {
            stackAmount += 1;
            Initialize(stackAmount);
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

        private bool CheckStackIsFulled(int foundIndex)
        {
            if (foundIndex >= _stackDataHolderList.Count - 1)
            {
                Debug.Log("STACK IS FULL. FAIL");
                EventManager.GameLoseExecute?.Invoke();
                return true;
            }
            return false;
        }

        private void ClearManagerData()
        {
            _stackDataHolderList.Clear();

            var t = Helpers.GetAllChildren(stackGroundSpawnTransform);

            for (int i = 0; i < t.Count; i++)
            {
                Destroy(t[i].gameObject);
            }
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
