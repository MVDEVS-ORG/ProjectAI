using Assets.Services;
using UnityEngine;
using Zenject;

public class GamePauseController : IGamePauseController
{
    [Inject] IAssetService _assetService;

    private bool _paused = false;
    bool IGamePauseController.IsPaused => _paused;
    private GameObject _pauseScreen;

    [Inject]
    public void Initialize()
    {
        _ = CreatePauseScreen();
    }

    void IGamePauseController.PauseGame()
    {
        if (!_paused)
        {
            Time.timeScale = 0f;
            _pauseScreen.SetActive(true);
            _paused = true;
        }
    }

    void IGamePauseController.ResumeGame()
    {
        Time.timeScale = 1f;
        _paused = false;
    }

    private async Awaitable CreatePauseScreen()
    {
        _pauseScreen = await _assetService.InstantiateAsync(AddressableIds.Pause_Screen);
        _pauseScreen.SetActive(false);
    }
}
