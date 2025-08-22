using Assets.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using static Zenject.CheatSheet;


public class GunsController : IGunsController
{
    [Inject] ObjectPoolManager _poolManager;
    [Inject] IAssetService _assetService;
    [Inject] IUpgradeController _upgradeController;
    private Dictionary<GunsModel,GunsView> _allGuns = new();
    private List<GunsModel> _orderedValues =new();
    private int _gunLimit = 1;
    private GunsView _currentActiveGun;
    private GunsModel _currentGunsModel;

    private Coroutine _gunFiring;
    private Transform _playerTransform;
    private Transform _playerCursor;

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
        _currentActiveGun.gameObject.SetActive(false);
        _currentActiveGun = gun;
        _currentActiveGun.gameObject.SetActive(true);
        _currentGunsModel = gun.InitializeGun(this,_poolManager, _playerTransform, _playerCursor);
        _currentGunUI.SwapGun(gun.GunsModel);
        //_upgradeController.RefreshUpgrades();
        gun.SetGunUI(_currentGunUI);
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
        Debug.LogError($"Deactivating gun {_currentActiveGun.name}");
        //add the new system here
        _allGuns.Remove(_currentActiveGun.GunsModel);
        _orderedValues.Remove(_currentActiveGun.GunsModel);
         //problem lies here cause gun.GunsModel is not create yet
        _currentActiveGun.DeactivateGun(gun.transform.position);

        //new system changes above

        //await (this as IGunsController).AddGun(gun);
        if (_allGuns.ContainsKey(_currentActiveGun.GunsModel))
        {
            _currentActiveGun.gameObject.SetActive(false);
        }
        _currentActiveGun = gun;
        _currentActiveGun.gameObject.SetActive(true);
        GunsModel toRemove = _currentGunsModel;
        _currentGunsModel = gun.InitializeGun(this, _poolManager, _playerTransform, _playerCursor);

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
            GameObject obj = await _assetService.InstantiateAsync(gunAddress);
            GunsView gun = obj.GetComponent<GunsView>();
            _playerTransform = playerTransform;
            _playerCursor = playerCursor;
            _currentActiveGun = gun;
            _currentGunsModel = gun.InitializeGun(this, _poolManager, _playerTransform, _playerCursor);

            var gunUIgameObject = await _assetService.InstantiateAsync(AddressableIds.Gun_UI_Canvas);
            _currentGunUI = gunUIgameObject.GetComponent<IGunUI>();
            await _currentGunUI.Initialize(_currentGunsModel, _playerTransform);
            //new system added
            _allGuns.Add(_currentGunsModel, _currentActiveGun);
            _orderedValues.Add(_currentGunsModel);
            //new system
            _upgradeController.OnUpgrade += UpgradeWeapon;
            _upgradeController.RefreshUpgrades();
            gun.SetGunUI(_currentGunUI);
        }
        catch(Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    async Awaitable IGunsController.AddGun(GunsView gun)
    {
        
        if(_allGuns.Count>=_gunLimit)
        {
            await (this as IGunsController).ReplaceGuns(gun);
        }
        else
        {
            #region Disabling old gun and setting new gun
            if(_allGuns.ContainsKey(_currentActiveGun.GunsModel))
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
            Debug.LogError(_currentGunsModel);
            await _currentGunUI.AddGun(_currentGunsModel);
            gun.SetGunUI(_currentGunUI);
            #endregion
        }
        //Debug.LogError(JsonConvert.SerializeObject(lst) + "\n" + JsonConvert.SerializeObject(lst2));
    }

    void IGunsController.ChangeGunLimit(int limit)
    {
        _gunLimit = limit;
    }

    void IGunsController.SwapGuns(int updown)
    {
        try
        {
            if (_gunLimit > 1)
            {
                int index = _orderedValues.IndexOf(_currentGunsModel);
                index = updown > 0 ? (index + 1) : ((index - 1)<0?_orderedValues.Count-1: (index - 1));
                index = index % _orderedValues.Count;
                _currentActiveGun.StopAllCoroutines();
                (this as IGunsController).SetCurrentActiveGun(_allGuns[_orderedValues[index]]);
            }
        }
        catch (Exception ex)
        {

        }
    }
}
