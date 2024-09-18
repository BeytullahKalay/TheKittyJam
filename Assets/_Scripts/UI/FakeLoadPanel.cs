using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FakeLoadPanel : MonoSingleton<FakeLoadPanel>
{
    [SerializeField] GameObject canvasTransform;
    [SerializeField] private Slider fakeLoadSlider;
    [SerializeField] Transform sliderHandle;
    [SerializeField] private float durationOfFakeLoad = 1;
    [SerializeField] private Transform catFlyImageTransform;
    [SerializeField] private Transform logoImageTransform;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void LoadScene(string sceneName)
    {
        canvasTransform.SetActive(true);
        //sliderHandle.DORotate(Vector3.forward * 360 * -1, durationOfFakeLoad, RotateMode.FastBeyond360);

        //DOVirtual.Float(0, 1, durationOfFakeLoad, (t) =>
        //{
        //    fakeLoadSlider.value = t;
        //}).OnComplete(() =>
        //{
        //    StartCoroutine(LoadSceneAsync(sceneName));
        //});

        StartCoroutine(LoadSceneAsync(sceneName));

    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            // normally this is the progress
            sliderHandle.transform.Rotate(Vector3.forward * -100 * Time.deltaTime);
            var progressValue = Mathf.Clamp01(operation.progress / 0.9f);
            fakeLoadSlider.value = progressValue;
            yield return null;
        }
        canvasTransform?.SetActive(false);
        fakeLoadSlider.value = 0;
    }

    public void OpenCatFly()
    {
        logoImageTransform.gameObject.SetActive(false);
        catFlyImageTransform.gameObject.SetActive(true);
    }

    public void OpenLogo()
    {
        logoImageTransform.gameObject.SetActive(true);
        catFlyImageTransform.gameObject.SetActive(false);
    }
}
