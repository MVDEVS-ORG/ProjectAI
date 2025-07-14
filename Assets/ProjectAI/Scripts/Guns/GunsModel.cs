using System;
using UnityEngine;

[Serializable]
public class GunsModel
{
    public string PrimaryProjectileAddressable;
    public string SecondaryProjectileAddressable;
    public string GunUIAddressable;
    public float FireRate;
    public float GunWindUpTime;
    public float OverHeatLimit;
    public float OverHeatRate;
    public float CoolDownRate;
    public float MinCooldownThreshold;
    public float OverHeatValue;
    public float ElipseVerticalRadius;
    public float ElipseHorizontalRadius;
    public GunsModel(GunsSO gunsData)
    {
        PrimaryProjectileAddressable = gunsData.PrimaryProjectileAddressable;
        SecondaryProjectileAddressable = gunsData.SecondaryProjectileAddressable;
        GunUIAddressable = gunsData.GunUIAddressable;
        FireRate = gunsData.FireRate;
        OverHeatLimit = gunsData.OverHeatLimit;
        OverHeatRate = gunsData.OverHeatRate;
        CoolDownRate = gunsData.CoolDownRate;
        MinCooldownThreshold = gunsData.MinCooldownThreshold;
        GunWindUpTime = gunsData.GunWindUpTime;
        ElipseHorizontalRadius = gunsData.ElipseHorizontalRadius;
        ElipseVerticalRadius = gunsData.ElipseVerticalRadius;
        OverHeatValue = 0;
    }
}
