using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace _Scripts.Node
{
    public class Node : MonoBehaviour
    {
        [field: SerializeField] public AnimalType AnimalType { get; private set; }
        [field: SerializeField] public List<Node> Neighbours { get; set; } = new();
        [SerializeField] private Color drawLineColor = Color.white;

        public bool IsEmpty { get; private set; } = true;
        //public GameObject SpawnedCatModel { get; private set; }
        public NodeObject NodeObject { get; private set; }

        private GameObject _catModelToSpawn;

        private void Start()
        {
            SpawnAnimalModel();
        }

        public void SetNodeAvailable()
        {
            AnimalType = AnimalType.NONE;
            IsEmpty = true;
        }

        private void SpawnAnimalModel()
        {
            if (AnimalType == AnimalType.NONE) return;


            _catModelToSpawn = GameManager.Instance.GetCatTypeModel(AnimalType);
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
                Handles.DrawBezier(p1, p2, p1, p2, drawLineColor, null, thickness);
            }
        }
    }
}