using Assets.Services;
using System;
using UnityEngine;
using Zenject;

public class GunsView : MonoBehaviour
{
    private GunsModel _gunsModel;
    private GunsController _gunsController;
    private bool _gunActive;
    public GunsSO GunsDataModel;
    private Transform _playerTransform;
    private ObjectPoolManager _poolManager;
    public Transform GunBulletSpawnTransform;
    private Transform _playerCursor;

    public GunsModel InitializeGun(GunsController controller, ObjectPoolManager objectPoolManager, Transform playerTrasform, Transform playerCursor)
    {
        Debug.Log("Gun initialized");
        _gunsController = controller;
        if (_gunsModel == null)
        {
            _gunsModel = new GunsModel(GunsDataModel);
        }
        _gunActive = true;
        _playerTransform = playerTrasform;
        _poolManager = objectPoolManager;
        _playerCursor = playerCursor;
        return _gunsModel;
    }

    public void DeactivateGun()
    {
        _gunActive = false;
    }

    public async Awaitable Fire()
    {
        try
        {
            Debug.Log(_gunsModel!=null);
            GameObject projectile = await _poolManager.SpawnObjectAsync(_gunsModel.PrimaryProjectileAddressable, GunBulletSpawnTransform.position, Quaternion.identity, ObjectPoolManager.PoolType.GameObjects);
            IGunProjectileBehavior weaponBehavior = projectile.GetComponent<IGunProjectileBehavior>();
            weaponBehavior.Initialize(_poolManager);
            weaponBehavior.SpawnProjectileAnimation();
            weaponBehavior.AddModifications();
            weaponBehavior.MoveProjectile((_playerCursor.position - GunBulletSpawnTransform.position).normalized);
            //add projectile spawn and modification logic here adn this should be called by guns controller when the projectile actually needs spawning
            //Then add the weapon overheat and so on
        }
        catch (Exception exception)
        {
            Debug.LogError($"{exception.Message}");
        }
    }

    public void Update()
    {
        if(_gunActive)
        {
            transform.position = _playerTransform.position;
        }
    }
}
