using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Node
{
    public class NodeObject : MonoBehaviour
    {
        [field: SerializeField] public NodeObjectAnimatorController NodeObjectAnimatorController { get; private set; }
        [SerializeField] private SkinnedMeshRenderer _catmeshRenderer;
        [SerializeField] private GameObject catModel;
        [SerializeField] private GameObject dogModel;
        [SerializeField] private GameObject foxModel;





        public AnimalType AnimalType { get; private set; }
        public Queue<PathData> PathDataQueue = new Queue<PathData>();



        public bool IsOnPathTween { get; private set; } = false;
        public bool IsCollected { get; private set; } = false;
        public PathData CurrentPathData { get; private set; }


        private Material _material;
        private Material _dirtMaterial;
        private PathData _pathData;


        public Action OnPathStepCompleted;
        public Action<AnimationType> OnCurrentPathChanged;
        public Action OnCollected;


        public void InitializeNodeObject(NodeFSM node)
        {
            AnimalType = node.AnimalType;
            transform.SetParent(node.transform);
            HandleMeshMaterial(node);
        }

        public void ChangeMaterial(ObstacleType obstacleType)
        {
            if (obstacleType != ObstacleType.DirtyKitty)
                _catmeshRenderer.material = _material;
            else
                _catmeshRenderer.material = _dirtMaterial;
        }

        public void SetIsCollectedToTrue()
        {
            if (IsCollected) return;
            IsCollected = true;
            OnCollected?.Invoke();
        }

        private void HandleMeshMaterial(NodeFSM node)
        {
            _material = GameManager.Instance.GetCatTypeMaterial(AnimalType);
            _dirtMaterial = GameManager.Instance.GetDirtMaterial();


            if (node.ObstacleType == ObstacleType.DirtyKitty || (node.ObstacleType == ObstacleType.DirtyCryingKitty))
            {
                _catmeshRenderer.material = _dirtMaterial;
                catModel.SetActive(true);
            }
            else if (node.ObstacleType == ObstacleType.Fox)
            {
                foxModel.SetActive(true);
            }
            else if (node.ObstacleType == ObstacleType.Dog)
            {
                dogModel.SetActive(true);
            }
            else
            {
                _catmeshRenderer.material = _material;
                catModel.SetActive(true);
            }

        }

        private void Update()
        {
            if (IsOnPathTween) return;
            if (PathDataQueue.Count <= 0) return;


            CurrentPathData = PathDataQueue.Dequeue();

            CurrentPathData.PathTween.Play();
            IsOnPathTween = true;

            OnCurrentPathChanged?.Invoke(CurrentPathData.AnimationType);

            CurrentPathData.PathTween.OnKill(() =>
            {
                IsOnPathTween = false;
                OnPathStepCompleted?.Invoke();
            });
        }
    }
}