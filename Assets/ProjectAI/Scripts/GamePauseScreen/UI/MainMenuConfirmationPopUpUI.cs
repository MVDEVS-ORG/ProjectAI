using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MainMenuConfirmationPopUpUI : MonoBehaviour
{
    [Inject] ISceneManager _sceneManager;

    [SerializeField] private Button Accept;
    [SerializeField] private Button Cancel;

    [Header("Transitions")]
    [SerializeField] GamePauseScreenUI _gamePauseScreenUI;

    private void OnEnable()
    {
        UIController.LookAtUI(true, gameObject);
    }

    private void Start()
    {
        Accept.onClick.AddListener(BackToMainMenu);
        Cancel.onClick.AddListener(BackToPauseScreen);
    }

    private void BackToMainMenu()
    {
        _sceneManager.LoadSceneAsync("MainMenu");
        Time.timeScale = 1.0f;
        gameObject.SetActive(false);
    }

    private void BackToPauseScreen()
    {
        gameObject.SetActive(false);
        _gamePauseScreenUI.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        UIController.LookAtUI(false, gameObject);
    }
}
