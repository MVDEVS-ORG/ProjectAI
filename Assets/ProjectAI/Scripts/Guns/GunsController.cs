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
            _firing = firing;
        }
        else
        {
            Debug.LogError("Trying to fire a gun which does not exist");
        }
    }

    async void IGunsController.SetCurrentActiveGun(GunsView gun, Transform playerTransform, Transform playerCursor)
    {
        _currentActiveGun = gun;
        _gunsModel = gun.InitializeGun(this,_poolManager, playerTransform, playerCursor);
        var gunUIgameObject = await _assetService.InstantiateAsync(_gunsModel.GunUIAddressable);
        _gunUI = gunUIgameObject.GetComponent<GunUI>();
        _gunUI.Initialize(_gunsModel, playerTransform);
        _gunFiring = _currentActiveGun.StartCoroutine(FireGun());
    }

    void IGunsController.SwapGuns(GunsView gun)
    {
        _currentActiveGun.DeactivateGun();
        _currentActiveGun.StopCoroutine(_gunFiring);
        _currentActiveGun = gun;
    }

    IEnumerator FireGun()
    {
        while (_currentActiveGun != null && _gunsModel != null)
        {
            if (_firing && _gunsModel.OverHeatValue < _gunsModel.OverHeatLimit && !_overheat)
            {
                yield return Awaitable.WaitForSecondsAsync(_gunsModel.GunWindUpTime);
                _ = _currentActiveGun.Fire();
                _gunsModel.OverHeatValue += _gunsModel.OverHeatRate;
                if(_gunsModel.OverHeatValue>=_gunsModel.OverHeatLimit)
                {
                    _overheat = true;
                }
                yield return Awaitable.WaitForSecondsAsync(1 / _gunsModel.FireRate);
            }
            else
            {
                yield return Awaitable.EndOfFrameAsync();
                _gunsModel.OverHeatValue = _gunsModel.OverHeatValue > 0 ? _gunsModel.OverHeatValue - _gunsModel.CoolDownRate * Time.deltaTime : 0;
                if(_gunsModel.OverHeatValue<_gunsModel.MinCooldownThreshold)
                {
                    _overheat = false;
                }
            }
            _gunUI.UpdateCoolDown();
        }
    }
}
