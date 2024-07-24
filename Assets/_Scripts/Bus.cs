using System;
using UnityEngine;

public class Bus : MonoBehaviour
{
    [field: SerializeField] public AnimalType CollectibleAnimalType { get; private set; }
    [field: SerializeField] public Transform[] Slots { get; private set; } = new Transform[MAX_COLLECT_AMOUNT];

    private int _currentCollectIndex = 0;

    private const int MAX_COLLECT_AMOUNT = 3;

    public void SetCollectibleAnimalType(AnimalType animalType)
    {
        CollectibleAnimalType = animalType;
    }

    public void CollectAnimal(GameObject collectObject, Action onBussFulled)
    {

        collectObject.transform.position = Slots[_currentCollectIndex].position + Vector3.up;
        collectObject.transform.rotation = Quaternion.identity;
        collectObject.transform.SetParent(transform);
        _currentCollectIndex += 1;

        if (_currentCollectIndex >= MAX_COLLECT_AMOUNT)
        {
            Debug.Log("Bus full actions!");
            onBussFulled?.Invoke();
        }
    }
}
