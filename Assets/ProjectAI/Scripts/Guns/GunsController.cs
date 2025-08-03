using Assets.Services;
using Newtonsoft.Json;
using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GunsController : IGunsController
{
    [Inject] ObjectPoolManager _poolManager;
    [Inject] IAssetService _assetService;
    [Inject] IUpgradeController _upgradeController;
    private GunsView _currentActiveGun;
    private GunsModel _gunsModel;

    private Coroutine _gunFiring;

    GunsView IGunsController.View => _currentActiveGun;

    private IGunUI _gunUI;

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

    async Awaitable IGunsController.SetCurrentActiveGun(GunsView gun, Transform playerTransform, Transform playerCursor)
    {
        _currentActiveGun = gun;
        _gunsModel = gun.InitializeGun(this,_poolManager, playerTransform, playerCursor);
        var gunUIgameObject = await _assetService.InstantiateAsync(_gunsModel.GunUIAddressable);
        _gunUI = gunUIgameObject.GetComponent<IGunUI>();
        _gunUI.Initialize(_gunsModel, playerTransform);
        _upgradeController.OnUpgrade += UpgradeWeapon;
        gun.SetGunUI(_gunUI);
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
                        _gunsModel = _gunsModel + upgrade.gunsModel;
                        break;

                    case StatType.Multiplicative:
                        _gunsModel = _gunsModel * upgrade.gunsModel;
                        break;

                    case StatType.Set:
                        _gunsModel = _gunsModel % upgrade.gunsModel;
                        break;
                }
            }
        }
    }

    async Awaitable IGunsController.SwapGuns(GunsView gun, Transform playerTransform, Transform playerCursor)
    {
        _currentActiveGun.DeactivateGun(gun.transform.position);
        //_currentActiveGun.StopCoroutine(_gunFiring);
        GameObject.Destroy((_gunUI as GunUI).gameObject);
        _gunUI = null;
        await (this as IGunsController).SetCurrentActiveGun(gun, playerTransform, playerCursor);
    }
}
