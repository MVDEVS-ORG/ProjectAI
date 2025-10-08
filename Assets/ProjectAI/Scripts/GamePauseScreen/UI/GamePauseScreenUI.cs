using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GamePauseScreenUI : MonoBehaviour
{
    [Inject] IGamePauseController _gamePauseController;

    [SerializeField] private Button Resume;
    [SerializeField] private Button Settings;
    [SerializeField] private Button MainMenu;

    [Header("Transitions")]
    [SerializeField] private MainMenuConfirmationPopUpUI _mainMenuConfirmationPopUpUI;
    [SerializeField] private SoundSettingsUI _soundSettingsUI;

    private void OnEnable()
    {
        UIController.LookAtUI(true, gameObject);
    }

    void Start()
    {
        Resume.onClick.AddListener(ResumeGame);
        Settings.onClick.AddListener(Setting);
        MainMenu.onClick.AddListener(ToMainMenu);
    }

    private void ResumeGame()
    {
        _gamePauseController.ResumeGame();
    }

    private void Setting()
    {
        _soundSettingsUI.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    private void ToMainMenu()
    {
        _mainMenuConfirmationPopUpUI.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        UIController.LookAtUI(false, gameObject);
    }
}
