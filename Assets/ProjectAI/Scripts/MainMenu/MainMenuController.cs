using Assets.ProjectAI.Scripts.MainMenu;
using Assets.Services;
using FMODUnity;
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
        _mainMenuUI.NewGameButton.onClick.AddListener(OpenCharacterSelection);
        _mainMenuUI.SettingsButton.onClick.AddListener(OpenSettingsMenu);
        _mainMenuUI.creditsButton.onClick.AddListener(OpenCredits);
        _mainMenuUI.QuitButton.onClick.AddListener(QuitToDesktop);
    }

    private void QuitToDesktop()
    {
        Debug.LogError("Quitting Application");
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
        Debug.LogError("Settings Menu is not yet Implemented");
    }

    private void OpenCredits()
    {
        Debug.LogError("Credits Scene is not yet Implemented");
    }
}
