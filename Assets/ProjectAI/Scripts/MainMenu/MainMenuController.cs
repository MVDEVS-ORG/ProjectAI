using Assets.Services;
using UnityEngine;
using Zenject;

public class MainMenuController
{
    [Inject] private IAssetService _assetService;
    [Inject] private ISceneManager _sceneManager;
    private MainMenuUI _mainMenuUI;

    [Inject]
    private void Initialize()
    {
        _ = StartMainMenu();
    }

    private async Awaitable StartMainMenu()
    {
        GameObject mainMenu = await _assetService.InstantiateAsync(AddressableIds.MainMenuUI);
        _mainMenuUI = mainMenu.GetComponent<MainMenuUI>();
        _mainMenuUI.PlayButton.onClick.AddListener(PlayGameScene);
    }

    private async void PlayGameScene()
    {
        await _sceneManager.LoadSceneAsync("GameScene");
    }
}
