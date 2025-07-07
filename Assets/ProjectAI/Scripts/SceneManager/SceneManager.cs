using Assets.Services;
using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class SceneManager : ISceneManager
{
    [Inject] private IAssetService _assetService;

    private bool _initialized;

    public event Action BeforeChangeScene;
    public event Action AfterChangeScene;

    private Image _fadeScreen;
    private TMP_Text _loadingText;

    [Inject]
    public void Initialize()
    {
        _ = CreateFadeScreen();
    }

    async Awaitable ISceneManager.LoadSceneAsync(string sceneName)
    {
        try
        {
            while (!_initialized)
            {
                await Awaitable.EndOfFrameAsync();
            }
            await (this as ISceneManager).FadeToBlack();
            if (BeforeChangeScene != null)
            {
                BeforeChangeScene.Invoke();
            }
            await UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
            if (AfterChangeScene != null)
            {
                AfterChangeScene.Invoke();
            }
        }
        catch (Exception exception)
        {
            Debug.LogError("Exception in scene manager during transition");
            Debug.LogError(exception);
        }
    }

    private async Awaitable CreateFadeScreen()
    {
        GameObject temp = await _assetService.InstantiateAsync(AddressableIds.FadeScreen);
        _fadeScreen = temp.transform.GetChild(0).GetComponent<Image>();
        _loadingText = temp.transform.GetChild(1).GetComponent<TMP_Text>();
        GameObject.DontDestroyOnLoad(temp);
        _initialized = true;
    }

    async Awaitable ISceneManager.FadeToBlack()
    {
        while(_fadeScreen.color.a<1)
        {
            Color color = _fadeScreen.color;
            color.a = Mathf.Clamp(color.a + Time.deltaTime,0,1);
            _fadeScreen.color = color;
            await Awaitable.EndOfFrameAsync();
        }
        _loadingText.gameObject.SetActive(true);
    }

    async Awaitable ISceneManager.FadeBack()
    {
        _loadingText.gameObject.SetActive(false);
        while (_fadeScreen.color.a <= 1)
        {
            Color color = _fadeScreen.color;
            color.a = Mathf.Clamp(color.a -  Time.deltaTime,0,1);
            _fadeScreen.color = color;
            await Awaitable.EndOfFrameAsync();
        }
    }
}
