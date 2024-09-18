using _Scripts.CollectibleController;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasButtonsController : MonoSingleton<CanvasButtonsController>
{
    [SerializeField] private HealthManager healthManager;
    [SerializeField] private EarnablesManager earnableManager;
    [field: SerializeField] public GameObject KittyJumpButton { get; private set; }
    [field: SerializeField] public GameObject ExtraStackButton { get; private set; }

    [SerializeField] private GameObject losePanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject retryButton;
    [SerializeField] private TMP_Text defaultGoldShowerText;
    [SerializeField] private TMP_Text addGoldShowerText;
    [SerializeField] private TMP_Text kittyJumpAmountText;
    [SerializeField] private TMP_Text stackAmountText;

    [Header("jump and stack amount values")]
    [SerializeField] private int stackAmount = 5;
    [SerializeField] private int jumpAmount = 5;


    private int _addWatchmultiplierForGoldAndPaw;
    private int _kittyJumpAmount;
    private int _stackAmount;

    private void OnEnable()
    {
        EventManager.OnGameWin += OpenWinPanel;
        EventManager.OnGameLose += OpenLosePanel;
        EventManager.OnLevelLoad += ClosePanels;
    }

    private void OnDisable()
    {
        EventManager.OnGameWin -= OpenWinPanel;
        EventManager.OnGameLose -= OpenLosePanel;
        EventManager.OnLevelLoad -= ClosePanels;
    }

    private void Awake()
    {
        HandleButtonPlayerPrefs();
    }

    private void Start()
    {
        InitializeButtonStates();
    }

    public void Initialize(int gold, int paw, int multiplier)
    {
        _addWatchmultiplierForGoldAndPaw = multiplier;
        defaultGoldShowerText.text = "+" + gold.ToString();
        addGoldShowerText.text = "WATCH AND ADD FOR +" + (gold * multiplier).ToString();
        retryButton.SetActive(healthManager.IsHaveLive());
    }

    // using by unity event
    public void AddExtraBasketButton()
    {
        if (_stackAmount <= 0) return;

        StackManager.Instance.AddExtraStack();
        _stackAmount--;
        stackAmountText.text = _stackAmount.ToString();
        PlayerPrefs.SetInt(PlayerPrefNames.ExtraStackAmount, _stackAmount);
    }

    // using by unity event
    public void KittyJumpButtonAction()
    {
        if (_kittyJumpAmount <= 0) return;
        MouseInputController.Instance.ToggleKittyJumpState();
    }

    public void DecreaseKittyJumpAmount()
    {
        _kittyJumpAmount -= 1;
        kittyJumpAmountText.text = _kittyJumpAmount.ToString();
        PlayerPrefs.SetInt(PlayerPrefNames.KittyJumpAmount, _kittyJumpAmount);
    }

    // using by unity event
    public void MainMenuButton()
    {
        Debug.Log("Load main menu here!");
        healthManager.DecreaseLife();
        FakeLoadPanel.Instance.LoadScene(SceneNames.MenuScene);
    }

    // using by unity event
    public void RetryButton()
    {
        EventManager.LevelLoadExecute?.Invoke();
        healthManager.DecreaseLife();
    }

    // using by unity event
    public void DefaultNextButton()
    {
        earnableManager.CollectDefaultEarnables();
        EventManager.LevelLoadExecute?.Invoke();
    }

    // using by unity event
    public void RetryWatchAddButton()
    {
        Debug.Log("Watch Add Button Clicked");
        EventManager.LevelLoadExecute?.Invoke();
    }

    // using by unity event
    public void EarnAddGoldNextButton()
    {
        earnableManager.CollectAddEarnables(earnableManager.CollectPawAmount * _addWatchmultiplierForGoldAndPaw, earnableManager.CollectGoldAmount * _addWatchmultiplierForGoldAndPaw);

        EventManager.LevelLoadExecute?.Invoke();
    }


    private void OpenLosePanel()
    {
        losePanel.SetActive(true);
    }

    private void OpenWinPanel()
    {
        winPanel.SetActive(true);
    }

    private void ClosePanels()
    {
        winPanel.SetActive(false);
        losePanel.SetActive(false);
    }

    private void InitializeButtonStates()
    {
        KittyJumpButton.SetActive(false);
        ExtraStackButton.SetActive(false);

        var kittyJumpButtonState = PlayerPrefs.GetInt(PlayerPrefNames.KittyJumpButton, 0);
        var extraStackButtonState = PlayerPrefs.GetInt(PlayerPrefNames.ExtraStackButton, 0);

        if (kittyJumpButtonState == 1)
            KittyJumpButton.SetActive(true);
        if (extraStackButtonState == 1)
            ExtraStackButton.SetActive(true);

        kittyJumpAmountText.text = _kittyJumpAmount.ToString();
        stackAmountText.text = _stackAmount.ToString();
    }

    private void OnApplicationQuit()
    {
        if (SceneManager.GetActiveScene().name == SceneNames.GameScene)
            healthManager.DecreaseLife();
    }

    private void HandleButtonPlayerPrefs()
    {
        _kittyJumpAmount = PlayerPrefs.GetInt(PlayerPrefNames.KittyJumpAmount, jumpAmount);
        _stackAmount = PlayerPrefs.GetInt(PlayerPrefNames.ExtraStackAmount, stackAmount);
    }
}
