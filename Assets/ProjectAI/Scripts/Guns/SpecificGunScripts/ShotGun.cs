using System.Collections;
using System.Threading;
using UnityEngine;

public class ShotGun : GunsView
{
    public int NoOfPellets;
    private Coroutine _firingGun;
    private bool _overheat = false;
    private CharacterView _view;
    private CancellationTokenSource _cancellationTokenSource;

    public override void Fire(bool firing)
    {
        if(_view==null)
        {
            _view = PlayerTransform.GetComponent<CharacterView>();
        }
        _firing = firing;
        Debug.LogError(_firingGun == null);
        if (_firingGun == null)
        {
            _firingGun = StartCoroutine(Firing());
            if(_cancellationTokenSource!=null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }
            _cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = _cancellationTokenSource.Token;
            _ = Cooldown(token);
        }
    }

    public override void DeactivateGun(Vector3 position)
    {
        base.DeactivateGun(position);
        if (_firingGun != null)
        {
            StopCoroutine(_firingGun);
            _firingGun = null;
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource=null;
            _firing = false;
        }
    }

    public override void ActivateGun()
    {
        WeaponKnockback = true;
        if (GunsModel.OverHeatValue > 0)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = _cancellationTokenSource.Token;
            _ = Cooldown(token);
        }
    }

    public void OnDisable()
    {
        if (_firingGun != null)
        {
            StopCoroutine(_firingGun);
            _firingGun = null;
            _firing = false;
        }
    }

    private IEnumerator Firing()
    {
        while (true)
        {
            if (_firing && GunsModel.OverHeatValue < GunsModel.OverHeatLimit && !_overheat)
            {
                _ = FireBullet((PlayerCursor.position - GunBulletSpawnTransform.position).normalized);
                if (WeaponKnockback && !AlternateRotation)
                {
                    _view.ExternalKickBack(3, transform.position, 0.2f);
                }
                if (!GunsModel.DisableOverheat)
                {
                    GunsModel.OverHeatValue += GunsModel.OverHeatRate;
                }
                if (GunUI != null)
                {
                    GunUI.UpdateCoolDown();
                }
                if (GunsModel.OverHeatValue >= GunsModel.OverHeatLimit)
                {
                    _overheat = true;
                }
                yield return Awaitable.WaitForSecondsAsync(1 / GunsModel.FireRate);
            }
            yield return Awaitable.EndOfFrameAsync();
        }
    }

    private async Awaitable Cooldown(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await Awaitable.EndOfFrameAsync();
            if (GunsModel.OverHeatValue > 0 )//&& !_firing)
            {
                GunsModel.OverHeatValue = GunsModel.OverHeatValue > 0 ? GunsModel.OverHeatValue - GunsModel.CoolDownRate * Time.deltaTime : 0;
                if (GunsModel.OverHeatValue < GunsModel.MinCooldownThreshold)
                {
                    _overheat = false;
                }
                if (GunUI != null)
                {
                    GunUI.UpdateCoolDown();
                }
            }
        }
    }

    private async Awaitable FireBullet(Vector3 MainDirection)
    {
        float angle = Mathf.Atan2(MainDirection.y, MainDirection.x);
        float delta = Mathf.PI / 20;
        float startAngle = angle - (delta * NoOfPellets / 2);
        GameObject bullet = await PoolManager.SpawnObjectAsync(GunsModel.PrimaryProjectileAddressable, GunBulletSpawnTransform.position, Quaternion.identity, ObjectPoolManager.PoolType.GameObjects);
        bullet.transform.right = transform.right;
        IGunProjectileBehavior weaponBehavior = bullet.GetComponent<IGunProjectileBehavior>();
        weaponBehavior.Initialize(PoolManager);
        weaponBehavior.SpawnProjectileAnimation();
        weaponBehavior.AddModifications(ElementalBuffs);
        weaponBehavior.MoveProjectile(transform.right);
    }
}
