using TMPro;
using UnityEngine;


public class EarnablesManager : MonoSingleton<EarnablesManager>
{
    [SerializeField] private int gold;
    [SerializeField] private int paw;
    [SerializeField] private TMP_Text cointTMP;
    [SerializeField] private TMP_Text pawTMP;


    public int CollectGoldAmount { get; private set; }
    public int CollectPawAmount { get; private set; }

    private void Awake()
    {
        CheckEarnablePlayerPrefs();
    }

    public void Initialize(int collectGold, int collectPaw)
    {
        CollectGoldAmount = collectGold;
        CollectPawAmount = collectPaw;
    }

    private void CheckEarnablePlayerPrefs()
    {
        if (!PlayerPrefs.HasKey(PlayerPrefNames.Gold))
            PlayerPrefs.SetInt(PlayerPrefNames.Gold, 0);
        else
            gold = PlayerPrefs.GetInt(PlayerPrefNames.Gold);


        if (!PlayerPrefs.HasKey(PlayerPrefNames.Paw))
            PlayerPrefs.SetInt(PlayerPrefNames.Paw, 0);
        else
            paw = PlayerPrefs.GetInt(PlayerPrefNames.Paw);

        UpdateEarningsUI();
    }

    private void UpdateEarningsUI()
    {
        cointTMP.text = gold.ToString();
        pawTMP.text = paw.ToString();
    }

    public void CollectDefaultEarnables()
    {
        gold += CollectGoldAmount;
        paw += CollectPawAmount;

        PlayerPrefs.SetInt(PlayerPrefNames.Gold, gold);
        PlayerPrefs.SetInt(PlayerPrefNames.Paw, paw);

        UpdateEarningsUI();
    }

    public void CollectAddEarnables(int newGoldCollectAmount, int newPawCollectAmount)
    {
        gold += newGoldCollectAmount;
        paw += newPawCollectAmount;

        PlayerPrefs.SetInt(PlayerPrefNames.Gold, gold);
        PlayerPrefs.SetInt(PlayerPrefNames.Paw, paw);

        UpdateEarningsUI();
    }


}
