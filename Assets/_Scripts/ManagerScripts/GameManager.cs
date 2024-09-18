using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{

    [field: SerializeField] public GameSettingsDataSO GameSettingsDataSO { get; private set; }
    [field: SerializeField] public CatAndTypeDataSO CatAndTypeDataSO { get; private set; }
    [field: SerializeField] public ObstacleDataSO ObstacleDataSO { get; private set; }
    [field: SerializeField] public TypeAndMaterialDataSO KittyHouseColorDataSO { get; private set; }
    [field: SerializeField] public GameState GameState { get; private set; }


    private void OnEnable()
    {
        EventManager.OnLevelLoad += SetGameStateToPlay;
        EventManager.OnGameWin += SetGameStateToWin;
        EventManager.OnGameLose += SetGameStateToLose;
    }

    private void OnDisable()
    {
        EventManager.OnLevelLoad -= SetGameStateToPlay;
        EventManager.OnGameWin -= SetGameStateToWin;
        EventManager.OnGameLose -= SetGameStateToLose;
    }

    public Material GetCatTypeMaterial(AnimalType catType)
    {

        foreach (var catTypeData in CatAndTypeDataSO.CatAndTypes)
        {
            if (catType == catTypeData.CatType)
                return catTypeData.CatMaterial;
        }

        Debug.LogError(catType);
        Debug.LogError("Cannot find cat type in Data SO");
        return null;
    }

    public GameObject GetBaseGameObject()
    {
        return CatAndTypeDataSO.BaseCatModel;
    }

    public Material GetDirtMaterial()
    {
        return ObstacleDataSO.DirtMaterial;
    }

    public void SetGameStateToPlay()
    {
        GameState = GameState.Play;
    }

    public void SetGameStateToWin()
    {
        GameState = GameState.Win;
    }

    public void SetGameStateToLose()
    {
        GameState = GameState.Lose;
    }

    public void SetGameStateToTutorial()
    {
        GameState = GameState.Tutorial;
    }
}
