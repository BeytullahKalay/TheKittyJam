using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Scriptable Objects/Bus Type And Material Data")]
public class TypeAndMaterialDataSO : ScriptableObject
{

    [SerializeField] private List<TypeAndMaterialData> busTypeAndMaterialData;

    public Material GetTypeMaterial(AnimalType animalType)
    {
        foreach (var item in busTypeAndMaterialData)
        {
            if (item.AnimalType == animalType)
                return item.Material;
        }

        Debug.LogError("Bus material not found on scriptable object!");

        return null;
    }

    [Serializable]
    public struct TypeAndMaterialData
    {
        public AnimalType AnimalType;
        public Material Material;
    }
}
