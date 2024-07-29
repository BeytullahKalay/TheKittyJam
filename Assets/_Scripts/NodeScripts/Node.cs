using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace _Scripts.Node
{
    public class Node : MonoBehaviour
    {
        [field: SerializeField] public AnimalType AnimalType { get; private set; }
        [field: SerializeField] public List<Node> Neighbours { get; set; } = new();


        public bool IsEmpty { get; private set; } = true;
        public NodeObject NodeObject { get; private set; }


        public Action<bool> IsDirtyStateChangedTo;
        [SerializeField] private bool isDirty;
        public bool IsDirty
        {
            get { return isDirty; }
            set
            {
                if (isDirty == value) return;
                else
                {
                    isDirty = value;
                    IsDirtyStateChangedTo?.Invoke(value);
                }
            }
        }



        private GameObject _catModelToSpawn;

        private void OnEnable()
        {
            IsDirtyStateChangedTo += OnDirtyStateChange;
        }

        private void OnDisable()
        {
            IsDirtyStateChangedTo -= OnDirtyStateChange;
        }

        private void OnDirtyStateChange(bool newDirtyState)
        {
            if (IsEmpty) return;
            NodeObject.ChangeMaterial(newDirtyState);
        }

        private void Start()
        {
            CheckDirtyState();
            SpawnAnimalModel();
        }

        private void CheckDirtyState()
        {
            if (AnimalType == AnimalType.NONE) isDirty = false;

            if (IsDirty)
            {
                foreach (var neigbour in Neighbours)
                {
                    if (neigbour.IsEmpty)
                    {
                        Debug.LogWarning(this.gameObject.name + " object is dirty but " + neigbour + " is empty!");
                    }
                }
            }
        }

        public void SetNodeAvailable()
        {
            AnimalType = AnimalType.NONE;
            IsEmpty = true;
        }

        private void SpawnAnimalModel()
        {
            if (AnimalType == AnimalType.NONE) return;


            _catModelToSpawn = GameManager.Instance.GetBaseGameObject();

            NodeObject = Instantiate(_catModelToSpawn, transform.position, Quaternion.identity).GetComponent<NodeObject>();
            NodeObject.InitializeNodeObject(this);
            IsEmpty = false;
        }

        private void OnDrawGizmos()
        {
            DrawLines();
        }

        private void DrawLines()
        {
            if (Neighbours.Count <= 0) return;

            foreach (var parent in Neighbours)
            {

                if (!parent.IsEmpty) continue;

                var p1 = transform.position;
                var p2 = parent.transform.position;
                var thickness = 6;
                Handles.DrawBezier(p1, p2, p1, p2, Color.white, null, thickness);
            }
        }
    }
}