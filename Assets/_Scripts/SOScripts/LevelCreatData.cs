using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Level Create Data")]

public class LevelCreatData : MonoBehaviour
{
    [field: SerializeField] public LevelCreatData LevelCreateData { get; private set; }

}

[System.Serializable]
public struct LevelCreateData
{
    public int StackAmount;
    public List<AnimalType> BusCollectType;
}