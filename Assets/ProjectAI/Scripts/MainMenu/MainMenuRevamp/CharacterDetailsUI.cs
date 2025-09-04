using Assets.ProjectAI.Scripts.Player;
using Assets.ProjectAI.Scripts.Player.Characters;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Assets.ProjectAI.Scripts.MainMenu
{
    public class CharacterDetailsUI : MonoBehaviour
    {
        [Inject] PlayerPicker _playerPicker;
        [Inject] ISceneManager _sceneManager;
        [Inject] PlayerSelectionService _playerSelectionService;

        [SerializeField] private Image _characterSprite;
        [SerializeField] private TextMeshProUGUI _characterName;
        [SerializeField] private TextMeshProUGUI _description;
        [SerializeField] private Image _passiveAbilitySprite;
        [SerializeField] private TextMeshProUGUI _passiveDesc;
        [SerializeField] private Image _activeAbilitySprite;
        [SerializeField] private TextMeshProUGUI _activeDesc;

        [SerializeField] private Button _changeAvatarButton;
        [SerializeField] private Button _enterDungeonButton;

        [Header("Transitions to")]
        [SerializeField] private CharacterSelectionView _characterSelectionView;

        private Character _character;

        private IUniversalDeviceController _universalDeviceController;

        bool _toDungeonSelected = false;

        public void Setup(CharacterDescriptionSO data)
        {
            _character = data.character;
            _characterName.text = data.character.ToString();
            _characterSprite.sprite = data.characterSprite;
            _description.text = data.description;

            _passiveAbilitySprite.sprite = data.passiveAbility.abilitySprite;
            _passiveDesc.text = data.passiveAbility.description;

            _activeAbilitySprite.sprite = data.activeAbility.abilitySprite;
            _activeDesc.text = data.activeAbility.description;
            _changeAvatarButton.onClick.AddListener(BackToCharacterSelect);
            _enterDungeonButton.onClick.AddListener(EnterTheDungeonButtonSelected);
        }

        private void EnterTheDungeonButtonSelected()
        {
            if (!_toDungeonSelected)
            {
                _toDungeonSelected = true;
                _playerSelectionService.SelectCharacter(_character);
                _ = _sceneManager.LoadSceneAsync("GameScene");
                gameObject.SetActive(false);
            }
        }

        private void BackToCharacterSelect()
        {
            _characterSelectionView.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            _changeAvatarButton.onClick.RemoveAllListeners();
            _enterDungeonButton.onClick.RemoveAllListeners();
            UIController.LookAtUI(false, gameObject);
        }

        private void OnEnable()
        {
            UIController.LookAtUI(true, gameObject);
        }
    }
}