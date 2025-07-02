using Assets.Services;
using NUnit.Framework.Constraints;
using System.Collections;
using UnityEngine;
using Zenject;

public class GunsController : IGunsController
{
    [Inject] ObjectPoolManager _poolManager;
    [Inject] IAssetService _assetService;
    private GunsView _currentActiveGun;
    private GunsModel _gunsModel;

    private Coroutine _gunFiring;
    private bool _firing = false;

    private bool _overheat=false;
    private IGunUI _gunUI;

    void IGunsController.Fire(bool firing)
    {
        if (_currentActiveGun != null)
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
        gun.SetGunUI(_gunUI);
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
