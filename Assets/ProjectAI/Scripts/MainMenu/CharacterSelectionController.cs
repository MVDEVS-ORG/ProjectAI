using Assets.ProjectAI.Scripts.Player;
using Assets.ProjectAI.Scripts.Player.Characters;
using Assets.Services;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Assets.ProjectAI.Scripts.MainMenu
{
    public class CharacterSelectionController
    {
        [Inject] private PlayerSelectionService _playerSelectionService;
        [Inject] private IAssetService _assetService;
        [Inject]  private PlayerPicker _playerPicker;
        [Inject]  private ISceneManager _sceneManager;

        private CharacterSelectionView _view;
        private CharacterDetailsUI _characterDetailsUI;
        private readonly List<CharacterDescriptionSO> _characters = new();
        private readonly List<PlayerCharactersSO> _playerCharactersSOs = new();
        private List<CharacterCardUI> _charactercards = new();

        public async void Initialize(CharacterSelectionView view)
        {
            _view = view;
            await LoadCharacters();

            foreach (var character in _characters)
            {
                var card = await _assetService.InstantiateWithParentAsync(AddressableIds.Character_Card_UI, _view.cardsContainer);
                card.TryGetComponent(out CharacterCardUI characterCardUI);
                characterCardUI.Setup(character, OnCharacterCardClicked);
                _charactercards.Add(characterCardUI);
            }
            _characterDetailsUI = _view.detailsPanel;
            _characterDetailsUI.changeAvatarButton.onClick.AddListener(OnChangeCharacterClicked);
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(_charactercards[0].gameObject);
            _view.HideDetails();
        }

        private async Awaitable LoadCharacters()
        {
            _characters.Add(await _assetService.LoadAssetAsync<CharacterDescriptionSO>(AddressableIds.Gunner_Detail_SO));
            _characters.Add(await _assetService.LoadAssetAsync<CharacterDescriptionSO>(AddressableIds.Shotgunner_Detail_SO));
            _characters.Add(await _assetService.LoadAssetAsync<CharacterDescriptionSO>(AddressableIds.Pyro_Detail_SO));

            await _playerPicker.SetPlayer();

        }

        private void OnCharacterCardClicked(CharacterDescriptionSO characterData)
        {
            _view.ShowDetails(characterData, OnCharacterSelected);
        }

        private void OnChangeCharacterClicked()
        {
            Debug.LogError("Change character Button Clicked");
            _view.HideDetails();
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(_charactercards[0].gameObject);
        }

        private async void OnCharacterSelected(Character characterType)
        {
            _playerSelectionService.SelectCharacter(characterType);
            await _sceneManager.LoadSceneAsync("GameScene");
        }
    }
}