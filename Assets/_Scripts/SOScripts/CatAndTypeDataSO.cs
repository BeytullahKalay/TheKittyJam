using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Cat And Type Data")]
public class CatAndTypeDataSO : ScriptableObject
{
    [field: SerializeField] public GameObject BaseCatModel { get; private set; }
    [field: SerializeField] public List<CatMaterialAndType> CatAndTypes = new();
}

[System.Serializable]
public struct CatMaterialAndType
{
    [field: SerializeField] public Material CatMaterial { get; private set; }
    [field: SerializeField] public AnimalType CatType { get; private set; }
}
