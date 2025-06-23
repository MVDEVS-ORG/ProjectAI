using Assets.Services;
using UnityEngine;
using Zenject;

public class GunsController : IGunsController
{
    [Inject] ObjectPoolManager poolManager;
    private GunsView _currentActiveGun;
    private GunsModel _gunsModel;

    void IGunsController.Fire(Vector2 direction)
    {
        if (_currentActiveGun != null)
        {
            _ = _currentActiveGun.Fire(direction);
        }
        else
        {
            Debug.LogError("Trying to fire a gun which does not exist");
        }
    }

    void IGunsController.SetCurrentActiveGun(GunsView gun, Transform playerTransform)
    {
        _currentActiveGun = gun;
        gun.InitializeGun(this,poolManager, playerTransform);
    }

    void IGunsController.SwapGuns(GunsView gun)
    {
        _currentActiveGun.DeactivateGun();
        _currentActiveGun = gun;
    }


}
