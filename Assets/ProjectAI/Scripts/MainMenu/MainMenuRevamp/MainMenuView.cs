using Assets.ProjectAI.Scripts.MainMenu;
using Assets.Services;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MainMenuView : MonoBehaviour
{
    [SerializeField] private Button NewGame;
    [SerializeField] private Button Settings;
    [SerializeField] private Button Credits;
    [SerializeField] private Button Exit;

    [Header("Transitions to")]
    [SerializeField] private CharacterSelectionView CharacterSelection;

    private void Start()
    {
        NewGame.onClick.AddListener(StartNewGame);
        Settings.onClick.AddListener(OpenSettings);
        Credits.onClick.AddListener(OpenCredits);
        Exit.onClick.AddListener(ExitGame);
    }

    private void StartNewGame()
    {
        CharacterSelection.gameObject.SetActive(true);
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
