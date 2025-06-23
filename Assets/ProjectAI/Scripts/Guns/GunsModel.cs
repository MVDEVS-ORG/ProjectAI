using UnityEngine;

public class GunsModel
{
    public string PrimaryProjectileAddressable;
    public string SecondaryProjectileAddressable;
    public float FireRate;
    public float OverHeatLimit;
    public float OverHeatRate;
    public float CoolDownRate;
    public float MinCooldownThreshold;
    public float OverHeatValue;
    public GunsModel(GunsSO gunsData)
    {
        PrimaryProjectileAddressable = gunsData.PrimaryProjectileAddressable;
        SecondaryProjectileAddressable = gunsData.SecondaryProjectileAddressable;
        FireRate = gunsData.FireRate;
        OverHeatLimit = gunsData.OverHeatLimit;
        OverHeatRate = gunsData.OverHeatRate;
        CoolDownRate = gunsData.CoolDownRate;
        MinCooldownThreshold = gunsData.MinCooldownThreshold;
        OverHeatValue = 0;
    }
}
