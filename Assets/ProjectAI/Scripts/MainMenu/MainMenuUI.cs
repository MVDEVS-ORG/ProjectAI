using Assets.ProjectAI.Scripts.MainMenu;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("Buttons")]
    public Button NewGameButton;
    public Button LoadGameButton;
    public Button CharacterSelectionButton;
    public Button SettingsButton;
    public Button creditsButton;
    public Button QuitButton;

    [Header("Panels")]
    public CharacterSelectionView characterSelectionView;
    public GameObject mainMenuPanel;
}
