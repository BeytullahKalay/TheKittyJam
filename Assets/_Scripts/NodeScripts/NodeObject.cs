using UnityEngine;

namespace _Scripts.Node
{
    public class NodeObject : MonoBehaviour
    {
        private MeshRenderer _meshRenderer;
        private Material _material;
        private Material _dirtMaterial;

        public AnimalType AnimalType { get; private set; }
        public void InitializeNodeObject(Node node)
        {
            AnimalType = node.AnimalType;
            transform.SetParent(node.transform);

            HandleMeshMaterial(node);

        }

        public void ChangeMaterial(bool isDirty)
        {
            if (isDirty)
                _meshRenderer.material = _dirtMaterial;
            else
                _meshRenderer.material = _material;
        }

        private void HandleMeshMaterial(Node node)
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _material = GameManager.Instance.GetCatTypeMaterial(AnimalType);
            _dirtMaterial = GameManager.Instance.GetDirtMaterial();

            ChangeMaterial(node.IsDirty);
        }


    }
}
