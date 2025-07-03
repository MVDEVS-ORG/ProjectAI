using System.Collections;
using System.Threading.Tasks;
using UnityEditor.EditorTools;
using UnityEngine;

public class SimpleGun : GunsView
{
    private bool StartFiring = false;
    private Awaitable FiringGun;
    private bool _overheat = false;
    public override void Fire(bool firing)
    {
        _firing = firing;
        Debug.Log("SimpleGunFire");
        if(FiringGun==null)
        {
            FiringGun = Firing();
        }
    }

    public override void DeactivateGun(Vector3 position)
    {
        base.DeactivateGun(position);
        if (FiringGun!=null)
        {
            FiringGun.Cancel();
            FiringGun = null;
        }
    }

    private async Awaitable Firing()
    {
        while (true)
        {
            if (_firing && GunsModel.OverHeatValue < GunsModel.OverHeatLimit && !_overheat)
            {
                _ = FireBullet();
                GunsModel.OverHeatValue += GunsModel.OverHeatRate;
                if(GunsModel.OverHeatValue >= GunsModel.OverHeatLimit)
                {
                    _overheat = true;
                }
                await Awaitable.WaitForSecondsAsync(1 / GunsModel.FireRate);
            }
            else
            {
                await Awaitable.EndOfFrameAsync();
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
        IGunProjectileBehavior weaponBehavior = bullet.GetComponent<IGunProjectileBehavior>();
        weaponBehavior.Initialize(PoolManager);
        weaponBehavior.SpawnProjectileAnimation();
        weaponBehavior.AddModifications();
        weaponBehavior.MoveProjectile((PlayerCursor.position - GunBulletSpawnTransform.position).normalized);
    }
}
