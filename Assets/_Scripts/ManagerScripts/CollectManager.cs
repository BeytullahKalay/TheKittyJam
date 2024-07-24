using _Scripts.Node;
using UnityEngine;

namespace _Scripts.CollectibleController
{
    public class CollectManager : MonoSingleton<CollectManager>
    {
        [SerializeField] private BusManager _busManager;
        [SerializeField] private StackManager _stackManager;


        public void CollectCat(NodeObject nodeObject)
        {
            if (nodeObject.AnimalType == _busManager.ActiveBusAnimaltype)
            {
                _busManager.CollectCat(nodeObject);
            }
            else
            {
                _stackManager.AddObjectToStack(nodeObject);
            }
        }
    }
}
