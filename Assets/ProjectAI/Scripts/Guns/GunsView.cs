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
    private SpriteRenderer _spriteRenderer;
    private Vector3 _scale;
    private Vector3 _reverseScale;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _scale = transform.localScale;
        _reverseScale = new Vector3(transform.localScale.x, -transform.localScale.y, transform.localScale.z);
    }
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

    public virtual async Awaitable Fire()
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
            /*transform.position = _playerTransform.position;*/
            OrbitalMotion();
        }
    }

    public void OrbitalMotion()
    {
        float angle = MathF.Atan2(_playerCursor.position.y - _playerTransform.position.y, _playerCursor.position.x - _playerTransform.position.x);
        if (angle > 0)
        {
            _spriteRenderer.sortingOrder = 4;
        }
        else
        {
            _spriteRenderer.sortingOrder = 10;
        }
        transform.position = _playerTransform.position + new Vector3(_gunsModel.ElipseHorizontalRadius * MathF.Sin(Mathf.PI * (0.5f) - angle), _gunsModel.ElipseVerticalRadius * MathF.Cos(Mathf.PI * (0.5f) - angle), transform.position.z);
        transform.right = (_playerCursor.position - _playerTransform.position).normalized;
        if (MathF.Abs(angle) > Mathf.PI / 2)
        {
            transform.localScale = _reverseScale; 
        }
        else
        {
            transform.localScale = _scale;
        }
    }
}
