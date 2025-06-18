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
            //Get the character prefab address
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
            //test if the prefabaddress is available
            if (prefabAddress == null)
            {
                throw new Exception("Character type not implemented in addressableIds");
            }
            //instantiate the asset
            var result = await _assetService.InstantiateAsync(prefabAddress);
            _characterView = result.GetComponent<CharacterView>();
            //Create a new player model for that character
            _playerModel = new PlayerModel(playerCharacter);
            Debug.Log("PlayerModel initialized");
            //Asign the player model and the controller to the view alongside the player cursor aka reticle for shooting
            GameObject bulletCursor = await PlayerCursorInitialization();
            _characterView.Initialize(this, _playerModel, bulletCursor);
            Debug.Log("PlayerView Initialized");

            //Create the player UI alongside the player and pass the model for data
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

    private async Awaitable<GameObject> PlayerCursorInitialization()
    {
        GameObject bulletCursor=await _assetService.InstantiateAsync(AddressableIds.BulletCursor);
        GameObject bulletCursorUI = await _assetService.InstantiateAsync(AddressableIds.BullerCursorUI);
        FollowScript bulletCursorFollow = bulletCursorUI.GetComponent<FollowScript>();
        bulletCursorFollow.Initialize(bulletCursor.transform);
        return bulletCursor;
    }

    void IPlayerController.TakeDamage(int damage)
    {
        _playerModel.Health = Mathf.Max(0, _playerModel.Health - damage);
        _playerUI.AlterHealthBar();
    }

    void IPlayerController.RestoreHealth(int health)
    {
        _playerModel.Health = Mathf.Min(_playerModel.Health + health, _playerModel.MaxHealth);
        _playerUI.AlterHealthBar();
    }

    void IPlayerController.Shoot(Vector2 direction)
    {
        //instantiate characters fire here 
    }
}


