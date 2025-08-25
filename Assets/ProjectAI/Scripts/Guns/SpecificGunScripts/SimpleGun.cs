using System.Collections;
using System.Threading;
using UnityEngine;

public class SimpleGun : GunsView
{
    private Coroutine _firingGun;
    private bool _overheat = false;
    private CancellationTokenSource _cancellationTokenSource;

    public override void Fire(bool firing)
    {
        _firing = firing;
        if(_firingGun==null)
        {
            _firingGun = StartCoroutine(Firing());
            if (_cancellationTokenSource != null)
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

    public override void ActivateGun()
    {
        if (GunsModel.OverHeatValue > 0)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = _cancellationTokenSource.Token;
            _ = Cooldown(token);
        }
    }

    public override void DeactivateGun(Vector3 position)
    {
        base.DeactivateGun(position);
        if (_firingGun!=null)
        {
            StopCoroutine(_firingGun);
            _firingGun = null;
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
            _firing = false;
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
                _ = FireBullet();
                if (!GunsModel.DisableOverheat)
                {
                    GunsModel.OverHeatValue += GunsModel.OverHeatRate;
                }
                if(GunsModel.OverHeatValue >= GunsModel.OverHeatLimit)
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
            if (GunsModel.OverHeatValue > 0)
            {
                GunsModel.OverHeatValue = GunsModel.OverHeatValue > 0 ? GunsModel.OverHeatValue - GunsModel.CoolDownRate * Time.deltaTime : 0;
                if (GunsModel.OverHeatValue < GunsModel.MinCooldownThreshold)
                {
                    _overheat = false;
                }
            }
            if (GunUI != null)
            {
                GunUI.UpdateCoolDown();
            }
        }
    }
    
    private async Awaitable FireBullet()
    {
        GameObject bullet = await PoolManager.SpawnObjectAsync(GunsModel.PrimaryProjectileAddressable, GunBulletSpawnTransform.position, Quaternion.identity, ObjectPoolManager.PoolType.GameObjects);
        bullet.transform.right = (PlayerCursor.position - PlayerTransform.position).normalized;
        IGunProjectileBehavior weaponBehavior = bullet.GetComponent<IGunProjectileBehavior>();
        weaponBehavior.Initialize(PoolManager);
        weaponBehavior.SpawnProjectileAnimation();
        weaponBehavior.AddModifications(ElementalBuffs);
        weaponBehavior.MoveProjectile((PlayerCursor.position - PlayerTransform.position).normalized);
    }
}
