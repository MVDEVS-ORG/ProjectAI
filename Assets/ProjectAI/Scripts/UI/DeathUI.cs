using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class DeathUI : MonoBehaviour
{
    [SerializeField] private Button _toMainMenu;
    [SerializeField] private Button _tryAgain;

    [Inject] private ISceneManager _sceneManager;

    private ResetData _resetData =  new ResetData();

    private void OnEnable()
    {
        Time.timeScale = 0f;
        _toMainMenu.onClick.AddListener(HeadToMainMenu);
        _tryAgain.onClick.AddListener(Retry);
        UIController.LookAtUI(true, gameObject);
    }

    private void HeadToMainMenu()
    {
        _resetData.ClearData();
        Time.timeScale = 1f;
        _sceneManager.FadeToBlack();
        _sceneManager.LoadSceneAsync("MainMenu");
    }

    private void Retry()
    {
        _resetData.ClearData();
        Time.timeScale = 1f;
        _sceneManager.FadeToBlack();
        _sceneManager.LoadSceneAsync("GameScene");
    }

    private void OnDisable()
    {
        _toMainMenu.onClick.RemoveListener(HeadToMainMenu);
        _tryAgain.onClick.RemoveListener(Retry);
        UIController.LookAtUI(false, gameObject);
    }
}
