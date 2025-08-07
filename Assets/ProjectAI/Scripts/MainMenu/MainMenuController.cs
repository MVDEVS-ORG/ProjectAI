using Assets.ProjectAI.Scripts.MainMenu;
using Assets.Services;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class MainMenuController
{
    [Inject] private IAssetService _assetService;
    [Inject] private ISceneManager _sceneManager;
    [Inject] private PlayerPicker _playerPicker;
    [Inject] private CharacterSelectionController _characterSelectionController;

    private MainMenuUI _mainMenuUI;
    private GameObject _currentOpenPanel;

    [Inject]
    private void Initialize()
    {
        _ = StartMainMenu();
    }

    private async Awaitable StartMainMenu()
    {
        GameObject mainMenu = await _assetService.InstantiateAsync(AddressableIds.Main_Menu_UI);
        _mainMenuUI = mainMenu.GetComponent<MainMenuUI>();
        _currentOpenPanel = _mainMenuUI.mainMenuPanel;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_mainMenuUI.NewGameButton.gameObject);
        _mainMenuUI.NewGameButton.onClick.AddListener(NewGame);
        _mainMenuUI.CharacterSelectionButton.onClick.AddListener(OpenCharacterSelection);
    }

    private async void NewGame()
    {
        await _sceneManager.LoadSceneAsync("GameScene");
    }
    private async void LoadGame()
    {
        await _sceneManager.LoadSceneAsync("GameScene");
    }
    private void QuitToDesktop()
    {
        Application.Quit();
    }

    private void OpenCharacterSelection()
    {
        _currentOpenPanel.SetActive(false);
        _currentOpenPanel = _mainMenuUI.characterSelectionView.gameObject;
        _currentOpenPanel.SetActive(true);
        _characterSelectionController.Initialize(_mainMenuUI.characterSelectionView);
    }

    private void OpenSettingsMenu()
    {

    }

    private void OpenCredits()
    {

    }
}
