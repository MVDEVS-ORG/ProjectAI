using Assets.Services;
using System;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class PlayerController : IPlayerController
{
    [Inject] IAssetService _assetService;

    private PlayerModel _playerModel;
    private CharacterView _characterView;// The players view
    private PlayerUI _playerUI;

    private bool _initialized = false;
    private bool _movementPossible = false;
    bool IPlayerController.Initialized => _initialized;
    bool IPlayerController.MovementPossible => _movementPossible;

    public void Initialize()
    {

    }

    async Awaitable IPlayerController.SpawnPlayer(Vector3 pos, PlayerCharactersSO playerCharacter)
    {
        try
        {
            string prefabAddress = null;
            switch (playerCharacter.CharacterType)
            {
                case Character.Gunner:
                    prefabAddress = AddressableIds.GunnerCharacter;
                    break;

                case Character.Shotgun:
                    prefabAddress = AddressableIds.ShotgunnerCharacter;
                    break;

                case Character.Pyro:
                    prefabAddress = AddressableIds.Pyro;
                    break;
            }
            if (prefabAddress == null)
            {
                throw new Exception("Character type not implemented in addressableIds");
            }
            var result = await _assetService.InstantiateAsync(prefabAddress);
            _characterView = result.GetComponent<CharacterView>();
            _playerModel = new PlayerModel(playerCharacter);
            Debug.Log("PlayerModel initialized");
            _characterView.Initialize(this, _playerModel);
            Debug.Log("PlayerView Initialized");
            result = await _assetService.InstantiateAsync(AddressableIds.PlayerUI);
            _playerUI = result.GetComponent<PlayerUI>();
            _playerUI.Initialize(_playerModel);
            Debug.Log("PlayerUI Initialized");
            _initialized = true;
            _movementPossible = true;
        }
        catch (Exception exception)
        {
            Debug.LogError(exception.Message);
        }
    }
}


