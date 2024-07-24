using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    [field: SerializeField] public CatAndTypeDataSO CatAndTypeDataSO { get; private set; }

    public GameObject GetCatTypeModel(AnimalType catType)
    {
        foreach (var catTypeData in CatAndTypeDataSO.catAndTypes)
        {
            if (catType == catTypeData.CatType)
                return catTypeData.CatModel;
        }

        Debug.LogError("Cannot find cat type in Data SO");
        return null;
    }

}
