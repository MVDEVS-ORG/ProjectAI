using Assets.ProjectAI.Scripts.Player.Characters;
using Assets.Services;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Assets.ProjectAI.Scripts.MainMenu
{
    public class CharacterSelectionView : MonoBehaviour
    {
        [Inject] IAssetService _assetService;
        [Inject] PlayerPicker _playerPicker;
        [Inject] IUniversalDeviceController _universalDeviceController;

        [SerializeField] private Transform _cardsContainer;
        [SerializeField] private Button _backToMainMenu;

        private readonly List<CharacterDescriptionSO> _characters = new();
        private List<CharacterCardUI> _charactercards = new();
        private bool _isInitialized = false;

        [Header("Transitions to")]
        [SerializeField] private MainMenuView _mainMenu;
        [SerializeField] private CharacterDetailsUI _characterDetailsUI;

        public async Awaitable Initialize()
        {
            await LoadCharacters();
            foreach (var character in _characters)
            {
                var card = await _assetService.InstantiateWithParentAsync(AddressableIds.Character_Card_UI, _cardsContainer);
                card.TryGetComponent(out CharacterCardUI characterCardUI);
                characterCardUI.Setup(character, _characterDetailsUI, this);
                _charactercards.Add(characterCardUI);
            }
            _isInitialized = true;
            _backToMainMenu.onClick.AddListener(ToMainMenu);
        }

        private void OnDeviceChanged(ControllerType controller)
        {
            if (controller == ControllerType.GamePad)
            {
                _universalDeviceController.SetGameObjectUI(_backToMainMenu.gameObject);
            }
            else
            {
                _universalDeviceController.SetGameObjectUI(null);
            }
        }

        private async void OnEnable()
        {
            await Awaitable.EndOfFrameAsync();
            _universalDeviceController.OnDeviceChanged += OnDeviceChanged;
            _ = _universalDeviceController.OnGamePadSetUI(_backToMainMenu.gameObject);
        }

        private void OnApplicationFocus(bool focus)
        {
            if (focus)
            {
                _universalDeviceController.OnGamePadSetUI(_backToMainMenu.gameObject);
            }
        }

        private void OnDisable()
        {
            _universalDeviceController.OnDeviceChanged -= OnDeviceChanged;
        }

        private void Start()
        {
            _ = Initialize();
        }

        private async Awaitable LoadCharacters()
        {
            _characters.Add(await _assetService.LoadAssetAsync<CharacterDescriptionSO>(AddressableIds.Gunner_Detail_SO));
            _characters.Add(await _assetService.LoadAssetAsync<CharacterDescriptionSO>(AddressableIds.Shotgunner_Detail_SO));
            _characters.Add(await _assetService.LoadAssetAsync<CharacterDescriptionSO>(AddressableIds.Pyro_Detail_SO));
            await _playerPicker.SetPlayer();
        }

        private void ToMainMenu()
        {
            _mainMenu.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}