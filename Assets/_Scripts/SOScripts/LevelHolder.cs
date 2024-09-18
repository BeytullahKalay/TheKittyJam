using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Scriptable Objects/Level Object Holder")]

public class LevelHolder : ScriptableObject
{
    [field: SerializeField] public List<GameObject> Levels { get; private set; } = new List<GameObject>();
}
