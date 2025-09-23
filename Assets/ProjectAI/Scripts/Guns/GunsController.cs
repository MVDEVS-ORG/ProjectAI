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
    [Inject] ISceneManager _sceneManager;
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

    private DataSerializer _dataSerializer = new DataSerializer();

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

    async Awaitable<GunsView> InstantiateGunAsync(string gunAddress)
    {
        GameObject obj = await _assetService.InstantiateAsync(gunAddress);
        GunsView gun = obj.GetComponent<GunsView>();
        _ = gun.InitializeGun(this, _poolManager, _playerTransform, _playerCursor);
        return gun;
    }

    async Awaitable RegisterGun(GunsView gun)
    {
        GunsModel gunModel = gun.GunsModel;
        if (!_allGuns.ContainsKey(gunModel))
        {
            _allGuns.Add(gunModel, gun);
            _orderedValues.Add(gunModel);
        }

        #region UI
        gun.SetGunUI(_currentGunUI);

        if (!_currentGunUI.Initialized)
        {
            await _currentGunUI.Initialize(gunModel, _playerTransform);
        }
        else
        {
            await _currentGunUI.AddGun(gunModel);
        }
        #endregion

        #region  Carousel
        await _gunCarousel.AddItem(gun.name, gun.GunSprite, true);
        #endregion

        gun.gameObject.SetActive(false);
    }


    async Awaitable IGunsController.IntializeOnSceneLoad(Transform playerTransform, Transform playerCursor)
    {
        //intialize the imp systems
        _playerTransform = playerTransform;
        _playerCursor = playerCursor;

        //UI
        var gunUIgameObject = await _assetService.InstantiateAsync(AddressableIds.Gun_UI_Canvas);
        _currentGunUI = gunUIgameObject.GetComponent<IGunUI>();

        //Carousel
        var carouselgameObject = await _assetService.InstantiateAsync(AddressableIds.Gun_Carousel);
        _gunCarousel = carouselgameObject.GetComponent<Carousel>();
        await _gunCarousel.Initialize(new Dictionary<string, Sprite>(), _assetService);

        //events
        _upgradeController.OnUpgrade += UpgradeWeapon;
        _sceneManager.BeforeChangeScene += ((IGunsController)this).SavePlayerGuns;
    }

    async Awaitable IGunsController.InitializeGunsOnSceneLoad(string defaultGunAddress)
    {
        //load the savegundata
        (string currentGunAddress, List<string> gunAddressableIds) = (this as IGunsController).LoadPlayerGuns();
        List<GunsView> loadedGuns = new();
        GunsView currentGun = null;

        if (gunAddressableIds.Count != 0)
        {
            foreach (string gunAddress in gunAddressableIds)
            {
                GunsView gun = await InstantiateGunAsync(gunAddress);
                await RegisterGun(gun);
                loadedGuns.Add(gun);
            }

            foreach (GunsView gun in loadedGuns)
            {
                if (gun.GunsModel.GunViewAddressableId == currentGunAddress)
                {
                    currentGun = gun;
                    break;
                }
            }
        }
        else
        {
            currentGun = await InstantiateGunAsync(defaultGunAddress);
            await RegisterGun(currentGun);
        }

        if (_currentActiveGun == null)
        {
            _currentActiveGun = currentGun;
            _currentActiveGun.gameObject.SetActive(true);
            _currentGunsModel = _currentActiveGun.GunsModel;
            _currentGunUI.SwapGun(_currentGunsModel);
            _currentActiveGun.SetGunUI(_currentGunUI);
            OnGunSwap?.Invoke(_currentActiveGun);
            _gunCarousel.MoveToIndex(_currentActiveGun.name);
            (this as IGunsController).SavePlayerGuns();
        }
        else
        {
            (this as IGunsController).SetCurrentActiveGun(currentGun);
        }

        (List<UpgradeSO> active, List<UpgradeSO> cursed) = _upgradeController.RefreshUpgrades();
        LoadWeaponUpgrades(active, cursed);
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

    void IGunsController.SavePlayerGuns()
    {
        List<string> gunAddressableIds = new();
        foreach ((GunsModel m, GunsView v) in _allGuns)
        {
            gunAddressableIds.Add(m.GunViewAddressableId);
        }
        _dataSerializer.SaveData(AddressableIds.Player_Guns_Path, (_currentGunsModel.GunViewAddressableId, gunAddressableIds));
    }

    (string, List<string>) IGunsController.LoadPlayerGuns()
    {
        (string, List<string>) playerGunDetails = _dataSerializer.LoadData<(string, List<string>)>(AddressableIds.Player_Guns_Path);
        return playerGunDetails;
    }
}
