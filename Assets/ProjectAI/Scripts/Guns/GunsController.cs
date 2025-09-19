using System;
using System.Collections.Generic;
using Assets.Services;
using UnityEngine;
using Zenject;


public class GunsController : IGunsController
{
    [Inject] ObjectPoolManager _poolManager;
    [Inject] IAssetService _assetService;
    [Inject] IUpgradeController _upgradeController;
    private Dictionary<GunsModel, GunsView> _allGuns = new();
    private List<GunsModel> _orderedValues = new();
    private int _gunLimit = 1;
    private GunsView _currentActiveGun;
    private GunsModel _currentGunsModel;

    private Coroutine _gunFiring;
    private Transform _playerTransform;
    private Transform _playerCursor;

    private Carousel _gunCarousel;

    public event Action<GunsView> OnGunSwap;

    GunsView IGunsController.View => _currentActiveGun;

    private IGunUI _currentGunUI;

    void IGunsController.Fire(bool firing)
    {
        if (_currentActiveGun != null && _currentActiveGun.gameObject.activeSelf)
        {
            _currentActiveGun.Fire(firing);
        }
        else
        {
            Debug.LogError("Trying to fire a gun which does not exist");
        }
    }

    void IGunsController.SetCurrentActiveGun(GunsView gun)
    {
        _currentActiveGun.StopAllCoroutines();
        _currentActiveGun.gameObject.SetActive(false);
        _currentActiveGun = gun;
        _currentActiveGun.gameObject.SetActive(true);
        _currentGunsModel = gun.GunsModel;
        _currentGunUI.SwapGun(gun.GunsModel);
        gun.SetGunUI(_currentGunUI);
        OnGunSwap?.Invoke(gun);
        _gunCarousel.MoveToIndex(_currentActiveGun.name);
    }

    private void LoadWeaponUpgrades(List<UpgradeSO> activeUpgrades, List<UpgradeSO> cursedUpgrades)
    {
        foreach (UpgradeSO upgrade in activeUpgrades)
        {
            if (upgrade.UpgradeType == UpgradeType.Gun)
            {
                switch (upgrade.Type)
                {
                    case StatType.Additive:
                        _currentGunsModel = _currentGunsModel + upgrade.gunsModel;
                        break;

                    case StatType.Multiplicative:
                        _currentGunsModel = _currentGunsModel * upgrade.gunsModel;
                        break;

                    case StatType.Set:
                        _currentGunsModel = _currentGunsModel % upgrade.gunsModel;
                        break;
                }
            }
        }

        foreach (UpgradeSO upgrade in cursedUpgrades)
        {
            if (upgrade.UpgradeType == UpgradeType.Gun)
            {
                switch (upgrade.Type)
                {
                    case StatType.Additive:
                        _currentGunsModel = _currentGunsModel + upgrade.gunsModel;
                        break;

                    case StatType.Multiplicative:
                        _currentGunsModel = _currentGunsModel * upgrade.gunsModel;
                        break;

                    case StatType.Set:
                        _currentGunsModel = _currentGunsModel % upgrade.gunsModel;
                        break;
                }
            }
        }

    }

    private void UpgradeWeapon(List<UpgradeSO> upgrades)
    {
        foreach (UpgradeSO upgrade in upgrades)
        {
            if (upgrade.UpgradeType == UpgradeType.Gun)
            {
                switch (upgrade.Type)
                {
                    case StatType.Additive:
                        _currentGunsModel = _currentGunsModel + upgrade.gunsModel;
                        break;

                    case StatType.Multiplicative:
                        _currentGunsModel = _currentGunsModel * upgrade.gunsModel;
                        break;

                    case StatType.Set:
                        _currentGunsModel = _currentGunsModel % upgrade.gunsModel;
                        break;
                }
            }
        }
    }

    async Awaitable IGunsController.ReplaceGuns(GunsView gun)
    {
        #region removing the current active gun
        Debug.LogError($"Deactivating gun {_currentActiveGun.name}");
        _allGuns.Remove(_currentActiveGun.GunsModel);
        _orderedValues.Remove(_currentActiveGun.GunsModel);
        _currentActiveGun.DeactivateGun(gun.transform.position);
        _currentActiveGun.StopAllCoroutines();
        if (_allGuns.ContainsKey(_currentActiveGun.GunsModel))
        {
            _currentActiveGun.gameObject.SetActive(false);
        }
        #endregion

        #region setting new current active gun
        _currentActiveGun = gun;
        _currentActiveGun.gameObject.SetActive(true);
        GunsModel toRemove = _currentGunsModel;
        _currentGunsModel = gun.InitializeGun(this, _poolManager, _playerTransform, _playerCursor);
        #endregion

        #region adding the new gun to the gun array and ordered list
        _allGuns.Add(_currentGunsModel, _currentActiveGun);
        _orderedValues.Add(_currentGunsModel);
        await _currentGunUI.AddGun(_currentGunsModel);
        _currentGunUI.RemoveGun(toRemove, gun.GunsModel);
        gun.SetGunUI(_currentGunUI);
        #endregion
    }

    async Awaitable IGunsController.InitializeOnSceneLoad(string gunAddress, Transform playerTransform, Transform playerCursor)
    {
        try
        {
            #region Instanting Default Gun for Character
            GameObject obj = await _assetService.InstantiateAsync(gunAddress);
            GunsView gun = obj.GetComponent<GunsView>();
            _playerTransform = playerTransform;
            _playerCursor = playerCursor;
            _currentActiveGun = gun;
            _currentGunsModel = gun.InitializeGun(this, _poolManager, _playerTransform, _playerCursor);
            #endregion

            #region Gun UI
            var gunUIgameObject = await _assetService.InstantiateAsync(AddressableIds.Gun_UI_Canvas);
            _currentGunUI = gunUIgameObject.GetComponent<IGunUI>();
            await _currentGunUI.Initialize(_currentGunsModel, _playerTransform);
            gun.SetGunUI(_currentGunUI);
            #endregion

            #region Adding it to multi gun system
            _allGuns.Add(_currentGunsModel, _currentActiveGun);
            _orderedValues.Add(_currentGunsModel);
            #endregion

            #region subscribing to upgrades controller
            _upgradeController.OnUpgrade += UpgradeWeapon;
            (List<UpgradeSO> tempActiveUpgrades, List<UpgradeSO> tempCursedUpgrades) = _upgradeController.RefreshUpgrades();
            LoadWeaponUpgrades(tempActiveUpgrades, tempCursedUpgrades);

            gun.SetGunUI(_currentGunUI);
            #endregion

            #region Gun Carousel
            var carousel = await _assetService.InstantiateAsync(AddressableIds.Gun_Carousel);
            _gunCarousel = carousel.GetComponent<Carousel>();
            Dictionary<string, Sprite> guns = new Dictionary<string, Sprite>();
            guns[_currentActiveGun.name] = _currentActiveGun.GunSprite; // need to add all active guns when we are switcching scene which is a TODO
            await _gunCarousel.Initialize(guns, _assetService);
            #endregion
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    async Awaitable IGunsController.AddGun(GunsView gun)
    {
        //checks if we are going over the permitted limit of guns for the character
        if (_allGuns.Count >= _gunLimit)
        {
            await (this as IGunsController).ReplaceGuns(gun);
        }
        else
        {
            #region Disabling old gun and setting new gun
            if (_allGuns.ContainsKey(_currentActiveGun.GunsModel))
            {
                _currentActiveGun.gameObject.SetActive(false);
            }
            _currentActiveGun = gun;
            _currentActiveGun.gameObject.SetActive(true);
            _currentGunsModel = gun.InitializeGun(this, _poolManager, _playerTransform, _playerCursor);

            #endregion

            #region adding the new gun to the gun array and ordered list
            _allGuns.Add(_currentGunsModel, _currentActiveGun);
            _orderedValues.Add(_currentGunsModel);
            #endregion

            #region Gun UI
            await _currentGunUI.AddGun(_currentGunsModel);
            gun.SetGunUI(_currentGunUI);
            #endregion

            #region Adding the gun to the carousel
            await _gunCarousel.AddItem(_currentActiveGun.name, _currentActiveGun.GunSprite, true);
            #endregion

            OnGunSwap?.Invoke(gun);
        }
    }

    void IGunsController.ChangeGunLimit(int limit)
    {
        _gunLimit = limit;
    }

    void IGunsController.SwapGuns(int updown)
    {
        try
        {
            if (_gunLimit > 1 && _allGuns.Count > 1)
            {
                int index = _orderedValues.IndexOf(_currentGunsModel);
                index = updown > 0 ? (index + 1) : ((index - 1) < 0 ? _orderedValues.Count - 1 : (index - 1));
                index = index % _orderedValues.Count;
                _currentActiveGun.StopAllCoroutines();
                (this as IGunsController).SetCurrentActiveGun(_allGuns[_orderedValues[index]]); // sets the current active gun after the swap
            }
        }
        catch (Exception exception)
        {
            Debug.LogError(exception);
        }
    }

    void IGunsController.FireAllGuns(bool toggle, bool alternateAbility)
    {
        if (toggle)
        {
            foreach (var gun in _allGuns)
            {
                gun.Value.gameObject.SetActive(true);
                if (!alternateAbility)
                {
                    gun.Key.DisableOverheat = true;
                }
                gun.Value.SetStartingRotation(_orderedValues.IndexOf(gun.Key), _orderedValues.Count);
                gun.Value.AlternateRotation = true;
                gun.Value.Fire(true);
            }
        }
        else
        {
            foreach (var gun in _allGuns)
            {
                if (gun.Value == _currentActiveGun)
                {
                    if (!alternateAbility)
                    {
                        gun.Key.DisableOverheat = false;
                    }
                    gun.Value.Fire(false);
                    gun.Value.AlternateRotation = false;
                }
                else
                {
                    gun.Value.gameObject.SetActive(false);
                    if (!alternateAbility)
                    {
                        gun.Key.DisableOverheat = false;
                    }
                    gun.Value.AlternateRotation = false;
                    gun.Value.Fire(false);
                }
            }
        }
    }

    void IGunsController.SetGunElements(Dictionary<ElementEnum, int> ElementalBuffs)
    {
        _currentActiveGun.ElementalBuffs = ElementalBuffs;
    }
}
