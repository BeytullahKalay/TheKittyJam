using UnityEngine;


[CreateAssetMenu(menuName = "Scriptable Objects/Game Settings Data")]

public class GameSettingsDataSO : ScriptableObject
{
    [field: Header("Tween Values")]
    [field: SerializeField] public float CatPathSpeed { get; private set; } = 4f;
    [field: SerializeField] public float CatJumpPower { get; private set; } = 2f;
    [field: SerializeField] public float CatJumpDuration { get; private set; } = .5f;

    [field: Header("Animation Tween Values")]
    [field: SerializeField] public float CatRotateDuration { get; private set; } = .15f;
    [field: SerializeField] public float CatRotateToMoleDuration { get; private set; } = .35f;
    [field: SerializeField] public float MotherCatMoveDuration { get; private set; } = 1.5f;

    [field: Header("Live Values")]
    [field: SerializeField] public int MaxLiveAmount { get; private set; } = 5;
    [field: SerializeField] public int LiveRegenarationTimeMinute { get; private set; } = 15;
}
