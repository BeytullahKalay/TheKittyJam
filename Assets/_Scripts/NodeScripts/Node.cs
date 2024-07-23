using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace _Scripts.Node
{
    public class Node : MonoBehaviour
    {
        [field: SerializeField] public CatType CatType { get; private set; }
        [field: SerializeField] public List<Node> ParentNodes { get; private set; } = new();
        [SerializeField] private Color drawLineColor = Color.white;

        public bool IsEmpty { get; private set; } = true;
        public GameObject SpawnedCatModel { get; private set; }

        private GameObject _catModelToSpawn;

        private void Start()
        {
            SpawnCatModel();
        }

        public void SetNodeWalkable()
        {
            CatType = CatType.NONE;
            IsEmpty = true;
        }

        public void DestroyModel()
        {
            Destroy(_catModelToSpawn);
        }

        private void SpawnCatModel()
        {
            if (CatType != CatType.NONE)
            {
                _catModelToSpawn = GameManager.Instance.GetCatTypeModel(CatType);
                SpawnedCatModel = Instantiate(_catModelToSpawn, transform.position, Quaternion.identity);
                IsEmpty = false;
            }
        }

        private void OnDrawGizmos()
        {
            DrawLines();
        }

        private void DrawLines()
        {
            if (ParentNodes.Count <= 0) return;

            foreach (var parent in ParentNodes)
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