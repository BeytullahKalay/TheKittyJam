using Facebook.Unity;
using GameAnalyticsSDK;
using UnityEngine;

public class LoadSceneAction : MonoBehaviour
{
    private FakeLoadPanel _fakeLoadPanel;

    private void Awake()
    {
        _fakeLoadPanel = FakeLoadPanel.Instance;
    }

    private void OnDisable()
    {
        Application.targetFrameRate = 60;
        _fakeLoadPanel.OpenCatFly();
    }

    private void Start()
    {
        FB.Init();
        GameAnalytics.Initialize();

        _fakeLoadPanel.OpenLogo();
        _fakeLoadPanel.LoadScene(SceneNames.MenuScene);
    }
}
