using UnityEngine;

public class ShotGun : GunsView
{


    public int NoOfPellets;

    private bool StartFiring = false;
    private Awaitable FiringGun;
    private bool _overheat = false;
    public override void Fire(bool firing)
    {
        _firing = firing;
        Debug.Log("SimpleGunFire");
        if (FiringGun == null)
        {
            FiringGun = Firing();
        }
    }

    public override void DeactivateGun(Vector3 position)
    {
        base.DeactivateGun(position);
        if (FiringGun != null)
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
                _ = FireBullet((PlayerCursor.position - GunBulletSpawnTransform.position).normalized);
                GunsModel.OverHeatValue += GunsModel.OverHeatRate;
                if (GunsModel.OverHeatValue >= GunsModel.OverHeatLimit)
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
    private async Awaitable FireBullet(Vector3 MainDirection)
    {
        float angle = Mathf.Atan2(MainDirection.y, MainDirection.x);
        float delta = Mathf.PI / 20;
        float startAngle = angle - (delta * NoOfPellets/2);
        for (int i = 1; i <= NoOfPellets; i++)
        {
            GameObject bullet = await PoolManager.SpawnObjectAsync(GunsModel.PrimaryProjectileAddressable, GunBulletSpawnTransform.position, Quaternion.identity, ObjectPoolManager.PoolType.GameObjects);
            IGunProjectileBehavior weaponBehavior = bullet.GetComponent<IGunProjectileBehavior>();
            weaponBehavior.Initialize(PoolManager);
            weaponBehavior.SpawnProjectileAnimation();
            weaponBehavior.AddModifications();
            weaponBehavior.MoveProjectile(new Vector3(Mathf.Cos(startAngle+(i*delta)),Mathf.Sin(startAngle+(i*delta)),0));
        }
    }
}
