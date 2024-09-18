using UnityEngine;


[CreateAssetMenu(menuName = "Scriptable Objects/Obstacle Data")]
public class ObstacleDataSO : ScriptableObject
{
    [field: SerializeField] public Material DirtMaterial { get; private set; }
    [field: SerializeField] public Material FoxMaterial { get; private set; }
    [field: SerializeField] public Material DogMarial { get; private set; }

}
