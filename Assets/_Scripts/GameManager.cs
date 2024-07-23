using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    [field: Header("Node Debug Values")]
    [field: SerializeField] public bool DrawAllNodeLines { get; private set; }
    [field: SerializeField] public float LineThickness { get; private set; } = 1;

}
