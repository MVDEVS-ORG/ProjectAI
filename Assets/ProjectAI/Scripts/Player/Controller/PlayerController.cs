using Assets.Services;
using System;
using System.Collections;
using System.Threading.Tasks;
using Unity.Cinemachine;
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

    private CinemachineCamera _camera;

    private State _moveState = State.Moving;
    State IPlayerController.MoveState => _moveState;
    public void SetCam(CinemachineCamera cam)
    {
        _camera = cam;
    }

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
            if (_camera != null)
            {
                _camera.Target.TrackingTarget = _characterView.transform;
            }
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

    Vector2 IPlayerController.Dash(Vector2 MoveInput)
    {
        if(_moveState == State.Moving && MoveInput!= Vector2.zero && _playerModel.NoOfDashes>0) //also need to addd the stamina part here
        {
            _playerModel.NoOfDashes--;
            _characterView.StartCoroutine(RollDash());
            _characterView.StartCoroutine(DashCoolDown());
            return MoveInput;
        }
        return Vector2.zero;
    }

    IEnumerator DashCoolDown()
    {
        yield return Awaitable.WaitForSecondsAsync(_playerModel.DashCooldown);
        _playerModel.NoOfDashes = _playerModel.NoOfDashes < _playerModel.MaxNoOfDashes ? _playerModel.NoOfDashes + 1 : _playerModel.NoOfDashes ;
    }

    IEnumerator RollDash()
    {
        _moveState = State.RollDash;
        yield return Awaitable.WaitForSecondsAsync(_playerModel.RollDuration);
        _moveState = State.Moving;
    }
}


