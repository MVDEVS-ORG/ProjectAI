using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Services;
using Newtonsoft.Json;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class PlayerController : IPlayerController
{
    [Inject] IAssetService _assetService;
    [Inject] IGunsController _gunsController;
    [Inject] IMeleeWeaponController _meleeWeaponController;
    [Inject] IUpgradeController _upgradeController;
    [Inject] IGamePauseController _gamePauseController;
    [Inject] ISceneManager _sceneManager;
    [Inject] CameraController _cameraController;
    [Inject] GamepadRumble _rumbleController;
    [Inject] SignalBus _signalBus;

    private PlayerModel _playerModel;
    private CharacterView _characterView;// The players view
    private PlayerUI _playerUI;
    private IAbilityController _abilityController = new AbilityController();

    private bool _initialized = false;
    private bool _movementPossible = false;
    private bool _isInvincible = false;
    private bool _gunEnabled = true;
    private bool _isAbilityInUse = false;

    bool IPlayerController.GunEnabled { get => _gunEnabled; set => _gunEnabled = value; }
    bool IPlayerController.Initialized => _initialized;
    bool IPlayerController.MovementPossible => _movementPossible;
    bool IPlayerController.IsInvincible => _isInvincible;
    bool IPlayerController.IsAbilityInUse { get => _isAbilityInUse; set => _isAbilityInUse = value; }

    private State _moveState = State.Moving;
    State IPlayerController.MoveState => _moveState;

    private Transform _bulletCursorUI;

    private List<int> _xpLevelMap;

    public void Initialize()
    {

    }

    private void StopControllerOnLevelChange()
    {
        _initialized = false;
    }

    async Awaitable IPlayerController.SpawnPlayer(Vector3 pos, PlayerCharactersSO playerCharacter)
    {
        try
        {
            #region player selection and instantiation
            //Get the character prefab address
            string prefabAddress = null;
            string gunAddress = null;
            switch (playerCharacter.CharacterType)
            {
                case Character.Gunner:
                    prefabAddress = AddressableIds.Gunner_Character;
                    gunAddress = AddressableIds.Simple_Gun;
                    _gunsController.ChangeGunLimit(5);
                    break;

                case Character.Shotgun:
                    prefabAddress = AddressableIds.Shotgunner_Character;
                    gunAddress = AddressableIds.Shot_Gun;
                    break;

                case Character.Pyro:
                    prefabAddress = AddressableIds.Pyro;
                    gunAddress = AddressableIds.Shot_Gun;
                    break;
            }
            //test if the prefabaddress is available
            if (prefabAddress == null)
            {
                throw new Exception("Character type not implemented in addressableIds");
            }
            //instantiate the asset
            var result = await _assetService.InstantiateWithPRAsync(prefabAddress, pos, Quaternion.identity);
            _characterView = result.GetComponent<CharacterView>();
            //Create a new _player model for that character
            _playerModel = new PlayerModel(playerCharacter);
            Debug.Log("PlayerModel initialized");
            //Assign the _player model and the controller to the view alongside the _player cursor aka reticle for shooting
            (GameObject, GameObject) bulletCursor = await PlayerCursorInitialization();
            _bulletCursorUI = bulletCursor.Item2.transform;
            _characterView.Initialize(this, _playerModel, bulletCursor.Item1, bulletCursor.Item2, _signalBus, _gamePauseController);
            Debug.Log("PlayerView Initialized");
            #endregion

            #region XP bar
            ExperienceListSO experienceList = await _assetService.LoadAssetAsync<ExperienceListSO>(AddressableIds.Player_Level_Chart);
            _xpLevelMap = new List<int>(experienceList.ExperiencePerLevel);
            _assetService.UnloadAsset(experienceList);
            #endregion

            #region player ui instantiation
            //Create the _player UI alongside the _player and pass the model for data
            result = await _assetService.InstantiateAsync(AddressableIds.Player_UI);
            _playerUI = result.GetComponent<PlayerUI>();
            _playerUI.Initialize(_playerModel, _xpLevelMap);
            Debug.Log("PlayerUI Initialized");
            _cameraController.Initialize(_characterView.transform);
            _rumbleController.Initialize(_characterView);
            #endregion

            #region player XP
            (int, int, int) playerStats = _upgradeController.LoadPlayerStats();
            int accumulatedXP = playerStats.Item1;
            LoadPlayerStats(playerStats.Item1, playerStats.Item2, playerStats.Item3);
            _sceneManager.BeforeChangeScene += () => { _upgradeController.SavePlayerStats(_playerModel.Experience, _playerModel.PlayerLevel, _playerModel.Health); };
            #endregion

            #region melee instantiation
            var melee = await _assetService.InstantiateAsync(AddressableIds.MeleeSlash);
            MeleeWeaponView meleeView = melee.GetComponent<MeleeWeaponView>();
            _meleeWeaponController.Initialize(_characterView.transform, _bulletCursorUI, this);
            _meleeWeaponController.SetupWeapon(meleeView);

            #endregion

            #region player upgrades
            _upgradeController.OnUpgrade += UpgradePlayer;
            (List<UpgradeSO> tempActiveUpgrades, List<UpgradeSO> tempCursedUpgrades) = _upgradeController.RefreshUpgrades();
            LoadPlayerUpgrades(tempActiveUpgrades, tempCursedUpgrades);
            _movementPossible = true;
            #endregion

            #region Gun instantiation
            await _gunsController.IntializeOnSceneLoad(_characterView.transform, bulletCursor.Item2.transform);
            await _gunsController.InitializeGunsOnSceneLoad(gunAddress);
            //var gun = await _assetService.InstantiateAsync(gunAddress);
            //await _gunsController.SetCurrentActiveGun(gun.GetComponent<GunsView>(), _characterView.transform, bulletCursor.Item2.transform);
            #endregion

            //Assign the player abilities
            _ = _abilityController.Initialize(_assetService, _playerModel, (this as IPlayerController), _gunsController, _meleeWeaponController);
            _sceneManager.BeforeChangeScene += StopControllerOnLevelChange;
        }
        catch (Exception exception)
        {
            Debug.LogError(exception.Message);
        }
    }

    private void LoadPlayerUpgrades(List<UpgradeSO> activeUpgrades, List<UpgradeSO> cursedUpgrades)
    {
        foreach (UpgradeSO upgrade in activeUpgrades)
        {
            switch (upgrade.Type)
            {
                case StatType.Additive:
                    _playerModel = _playerModel + upgrade.playerModel;
                    break;

                case StatType.Multiplicative:
                    _playerModel = _playerModel * upgrade.playerModel;
                    break;

                case StatType.Set:
                    _playerModel = _playerModel % upgrade.playerModel;
                    break;
            }
        }

        foreach (UpgradeSO upgrade in cursedUpgrades)
        {
            switch (upgrade.Type)
            {
                case StatType.Additive:
                    _playerModel = _playerModel + upgrade.playerModel;
                    break;

                case StatType.Multiplicative:
                    _playerModel = _playerModel * upgrade.playerModel;
                    break;

                case StatType.Set:
                    _playerModel = _playerModel % upgrade.playerModel;
                    break;
            }
        }


    }

    private void UpgradePlayer(List<UpgradeSO> upgrades)
    {
        foreach (UpgradeSO upgrade in upgrades)
        {
            switch (upgrade.Type)
            {
                case StatType.Additive:
                    _playerModel = _playerModel + upgrade.playerModel;
                    break;

                case StatType.Multiplicative:
                    _playerModel = _playerModel * upgrade.playerModel;
                    break;

                case StatType.Set:
                    _playerModel = _playerModel % upgrade.playerModel;
                    break;
            }
        }
    }

    private async Awaitable<(GameObject, GameObject)> PlayerCursorInitialization()
    {
        GameObject bulletCursor = await _assetService.InstantiateAsync(AddressableIds.Bullet_Cursor);
        GameObject bulletCursorUI = await _assetService.InstantiateAsync(AddressableIds.Bullet_Cursor_UI);
        FollowScript bulletCursorFollow = bulletCursorUI.GetComponent<FollowScript>();
        bulletCursorFollow.Initialize(bulletCursor.transform);
        return (bulletCursor, bulletCursorUI);
    }

    void IPlayerController.TakeDamage(int damage)
    {
        _playerModel.Health = Mathf.Max(0, _playerModel.Health - damage);
        _playerUI.AlterHealthBar();
        _moveState = State.TakeDamage;
        _rumbleController.Rumble(0.25f, 1f, 0.5f);
        _signalBus.Fire(new CamEffectsSignal(new CamEffectsSignal.SignalEffect().WithEffect(CamEffect.CamShakeConstant).WithFrequency(1f).WithAmplitude(5f).WithDuration(0.1f)));
        _isInvincible = true;
        if (_characterView != null)
        {
            _characterView.StartCoroutine(InvincibilityDuration());
            _characterView.StartCoroutine(DamageKickbackTimer());
        }
        else
        {
            _isInvincible = false;
        }
    }

    void IPlayerController.KickBack(float strength, float duration, Vector2 direction)
    {
        _characterView.StartCoroutine(KickbackTimer(duration));
        _characterView.SetKickBackStrength(strength, direction);
        _moveState = State.KickBack;
    }

    void IPlayerController.RestoreHealth(int health)
    {
        _playerModel.Health = Mathf.Min(_playerModel.Health + health, _playerModel.MaxHealth);
        _playerUI.AlterHealthBar();
    }

    void IPlayerController.Shoot(bool firing)
    {
        if (_gunEnabled)
        {
            _gunsController.Fire(firing);
        }
    }

    Vector2 IPlayerController.Dash(Vector2 MoveInput)
    {
        // Debug.LogError(_playerModel.GetHashCode());
        if (!_isAbilityInUse && _moveState == State.Moving && MoveInput != Vector2.zero && _playerModel.NoOfRoll < _playerModel.MaxNoOfRolls) //also need to addd the stamina part here
        {
            _playerModel.NoOfRoll++;
            _characterView.StartCoroutine(RollDash());
            _characterView.StartCoroutine(DashCoolDown());
            return MoveInput;
        }
        return Vector2.zero;
    }

    IEnumerator DashCoolDown()
    {
        yield return Awaitable.WaitForSecondsAsync(_playerModel.RolllCooldown);
        _playerModel.NoOfRoll = _playerModel.NoOfRoll > 0 ? _playerModel.NoOfRoll - 1 : 0;
    }

    IEnumerator RollDash()
    {
        _moveState = State.RollDash;
        _isInvincible = true;
        yield return Awaitable.WaitForSecondsAsync(_playerModel.RollDuration);
        _isInvincible = false;
        _moveState = State.Moving;
    }

    async Awaitable<Transform> IPlayerController.GetPlayerTransform()
    {
        while (_characterView == null)
        {
            Debug.LogError("Waiting for _player to spawn");
            await Awaitable.EndOfFrameAsync();
        }
        return _characterView.transform;
    }

    void IPlayerController.PickUpNewPlayerGun(GunsView gun)
    {
        if (_gunEnabled)
        {
            _gunsController.AddGun(gun);
        }
    }

    void IPlayerController.MeleeAttack()
    {
        if (_meleeWeaponController.Initialized && !_isAbilityInUse)
        {
            _meleeWeaponController.MeleeAttack();
        }
    }

    void IPlayerController.EnableController(bool enable)
    {
        _initialized = enable;
    }

    void IPlayerController.Test()
    {
        _upgradeController.DisplayUpgrades();
    }

    IEnumerator InvincibilityDuration()
    {
        yield return Awaitable.WaitForSecondsAsync(_playerModel.InvincibilityTime);
        _isInvincible = false;
    }

    IEnumerator DamageKickbackTimer()
    {
        yield return Awaitable.WaitForSecondsAsync(_playerModel.DamageKickBackTime);
        _moveState = State.Moving;
    }

    IEnumerator KickbackTimer(float timer)
    {
        yield return Awaitable.WaitForSecondsAsync(timer);
        _moveState = State.Moving;
    }

    void IPlayerController.AddXP(int xp)
    {
        _playerModel.Experience += xp;
        if (_playerModel.PlayerLevel < _xpLevelMap.Count && _playerModel.Experience > _xpLevelMap[_playerModel.PlayerLevel])
        {
            _playerModel.Experience = _playerModel.Experience % _xpLevelMap[_playerModel.PlayerLevel];
            _playerModel.PlayerLevel = _playerModel.PlayerLevel < _xpLevelMap.Count ? _playerModel.PlayerLevel + 1 : _xpLevelMap.Count;
            _upgradeController.DisplayUpgrades();
        }
        _playerUI.UpdateXpBar();
    }


    private void LoadPlayerStats(int xp, int level, int health)
    {
        if (xp <= 0) return;
        _playerModel.Experience = xp;
        _playerModel.PlayerLevel = level;
        _playerModel.Health = health;
        _playerUI.UpdateXpBar();
        _playerUI.AlterHealthBar();
    }

    IEnumerator MeleeDashTimer()
    {
        yield return Awaitable.WaitForSecondsAsync(_playerModel.MeleeDashTime);
        _moveState = State.Moving;
    }

    void IPlayerController.MeleeDash(Vector2 Direction)
    {
        _characterView.SetMeleeDashDirection(Direction);
        _moveState = State.MeleeDash;
        _characterView.StartCoroutine(MeleeDashTimer());
    }

    void IPlayerController.SwapWeapons(int value)
    {
        if (_gunEnabled)
        {
            _gunsController.SwapGuns(value);
        }
    }

    void IPlayerController.ActivateAbility()
    {
        _abilityController.UseAbility();
    }
}


