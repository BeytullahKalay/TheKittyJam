using System;
using UnityEngine;

public class Bus : MonoBehaviour
{
    [field: SerializeField] public AnimalType CollectibleAnimalType { get; private set; }
    [field: SerializeField] public Transform[] Slots { get; private set; } = new Transform[MAX_COLLECT_AMOUNT];

    private int _currentCollectIndex = 0;
    private MeshRenderer _meshRenderer;

    private const int MAX_COLLECT_AMOUNT = 3;


    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }


    public void InitilizeBuss(AnimalType animalType, Material busMaterial)
    {
        CollectibleAnimalType = animalType;
        _meshRenderer.material = busMaterial;
    }

    public void CollectAnimal(GameObject collectObject, Action onBussFulled)
    {

        collectObject.transform.position = Slots[_currentCollectIndex].position + Vector3.up;
        collectObject.transform.rotation = Quaternion.identity;
        collectObject.transform.SetParent(transform);
        _currentCollectIndex += 1;

        if (_currentCollectIndex >= MAX_COLLECT_AMOUNT)
        {
            onBussFulled?.Invoke();
        }
    }
}
