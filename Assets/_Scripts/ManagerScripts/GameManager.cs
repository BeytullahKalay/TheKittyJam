using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    [field: SerializeField] public CatAndTypeDataSO CatAndTypeDataSO { get; private set; }

    public Material GetCatTypeMaterial(AnimalType catType)
    {
        foreach (var catTypeData in CatAndTypeDataSO.CatAndTypes)
        {
            if (catType == catTypeData.CatType)
                return catTypeData.CatMaterial;
        }

        Debug.LogError("Cannot find cat type in Data SO");
        return null;
    }

    public GameObject GetBaseGameObject()
    {
        return CatAndTypeDataSO.BaseCatModel;
    }

    public Material GetDirtMaterial()
    {
        return CatAndTypeDataSO.DirtMaterial;
    }

}
