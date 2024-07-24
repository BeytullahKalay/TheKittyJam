using UnityEngine;

namespace _Scripts.Node
{
    public class NodeObject : MonoBehaviour
    {
        public AnimalType AnimalType { get; private set; }
        public void InitializeNodeObject(Node node)
        {
            AnimalType = node.AnimalType;
        }
    }
}
