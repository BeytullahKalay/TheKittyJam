using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Scriptable Objects/Bus Type And Material Data")]
public class BusTypeAndMaterialDataSO : ScriptableObject
{

    [SerializeField] private List<AnimalTypeAndMaterialData> busTypeAndMaterialData;

    public Material GetBusMaterial(AnimalType animalType)
    {
        if (animalType == AnimalType.NONE || animalType == AnimalType.Fox)
            Debug.LogError("Send NONE or FOX type to bus material scriptale object");


        foreach (var item in busTypeAndMaterialData)
        {
            if (item.AnimalType == animalType)
                return item.Material;
        }

        Debug.LogError("Bus material not found on scriptable object!");

        return null;
    }

    [Serializable]
    public struct AnimalTypeAndMaterialData
    {
        public AnimalType AnimalType;
        public Material Material;
    }
}
