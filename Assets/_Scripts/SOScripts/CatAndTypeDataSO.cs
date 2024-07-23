using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Cat And Type Data")]
public class CatAndTypeDataSO : ScriptableObject
{
    [field: SerializeField] public List<CatAndType> catAndTypes = new();
}

[System.Serializable]
public struct CatAndType
{
    [field: SerializeField] public GameObject CatModel { get; private set; }
    [field: SerializeField] public CatType CatType { get; private set; }
}
