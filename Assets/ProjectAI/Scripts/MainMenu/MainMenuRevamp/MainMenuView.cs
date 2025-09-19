using Assets.ProjectAI.Scripts.GameController;
using Assets.ProjectAI.Scripts.MainMenu;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MainMenuView : MonoBehaviour
{
    [Inject] private ISceneManager _sceneManager;
    [Inject] private LevelManager _levelManager;

    [SerializeField] private Button _newGame;
    [SerializeField] private Button _settings;
    [SerializeField] private Button _credits;
    [SerializeField] private Button _exit;

    [Header("Transitions to")]
    [SerializeField] private CharacterSelectionView _characterSelection;

    private void OnEnable()
    {
        UIController.LookAtUI(true, gameObject);
    }

    private void OnDisable()
    {
        UIController.LookAtUI(false, gameObject);
    }

    private void Start()
    {
        _newGame.onClick.AddListener(StartNewGame);
        _settings.onClick.AddListener(OpenSettings);
        _credits.onClick.AddListener(OpenCredits);
        _exit.onClick.AddListener(ExitGame);
        _sceneManager.FadeBack();
        _levelManager.ResetLevel();
    }

    private void StartNewGame()
    {
        _characterSelection.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    private void OpenSettings()
    {
        Debug.Log("Open Settings");
    }

    private void OpenCredits()
    {
        Debug.Log("Credits");
    }

    private void ExitGame()
    {
        Application.Quit();
    }

}
