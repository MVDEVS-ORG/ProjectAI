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

    [HideInInspector] public bool Empty = false;

    public GunsModel() { }

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

    public static GunsModel operator +(GunsModel left, GunsModel right)
    {
        /*GunsModel temp = new GunsModel();
        temp.PrimaryProjectileAddressable = left.PrimaryProjectileAddressable;
        temp.SecondaryProjectileAddressable = left.SecondaryProjectileAddressable;
        temp.GunUIAddressable = left.GunUIAddressable;
        temp.ElipseVerticalRadius = left.ElipseVerticalRadius;
        temp.ElipseHorizontalRadius = left.ElipseHorizontalRadius;
        temp.OverHeatValue = left.OverHeatValue;
        temp.Empty = left.Empty;*/
        // the above values do not change

        left.FireRate = left.FireRate + right.FireRate;
        left.OverHeatLimit = left.OverHeatLimit + right.OverHeatLimit;
        left.OverHeatRate = left.OverHeatRate + right.OverHeatRate;
        left.CoolDownRate = left.CoolDownRate + right.CoolDownRate;
        left.MinCooldownThreshold = left.MinCooldownThreshold + right.MinCooldownThreshold;
        left.GunWindUpTime = left.GunWindUpTime + right.GunWindUpTime;

        return left;
    }

    public static GunsModel operator *(GunsModel left, GunsModel right)
    {
        /*GunsModel temp = new GunsModel();
        temp.PrimaryProjectileAddressable = left.PrimaryProjectileAddressable;
        temp.SecondaryProjectileAddressable = left.SecondaryProjectileAddressable;
        temp.GunUIAddressable = left.GunUIAddressable;
        temp.ElipseVerticalRadius = left.ElipseVerticalRadius;
        temp.ElipseHorizontalRadius = left.ElipseHorizontalRadius;
        temp.OverHeatValue = left.OverHeatValue;
        temp.Empty = left.Empty;*/
        // the above values do not change

        left.FireRate = left.FireRate * right.FireRate;
        left.OverHeatLimit = left.OverHeatLimit * right.OverHeatLimit;
        left.OverHeatRate = left.OverHeatRate * right.OverHeatRate;
        left.CoolDownRate = left.CoolDownRate * right.CoolDownRate;
        left.MinCooldownThreshold = left.MinCooldownThreshold * right.MinCooldownThreshold;
        left.GunWindUpTime = left.GunWindUpTime * right.GunWindUpTime;

        return left;
    }

    public static GunsModel operator %(GunsModel left, GunsModel right)
    {
        /*GunsModel temp = new GunsModel();
        temp.PrimaryProjectileAddressable = left.PrimaryProjectileAddressable;
        temp.SecondaryProjectileAddressable = left.SecondaryProjectileAddressable;
        temp.GunUIAddressable = left.GunUIAddressable;
        temp.ElipseVerticalRadius = left.ElipseVerticalRadius;
        temp.ElipseHorizontalRadius = left.ElipseHorizontalRadius;
        temp.OverHeatValue = left.OverHeatValue;
        temp.Empty = left.Empty;*/
        // the above values do not change

        left.FireRate = right.FireRate != 0 ? right.FireRate : left.FireRate;
        left.OverHeatLimit = right.OverHeatLimit != 0 ? right.OverHeatLimit : left.OverHeatLimit;
        left.OverHeatRate = right.OverHeatRate != 0 ? right.OverHeatRate : left.OverHeatRate;
        left.CoolDownRate = right.CoolDownRate != 0 ? right.CoolDownRate : left.CoolDownRate;
        left.MinCooldownThreshold = right.MinCooldownThreshold != 0 ? right.MinCooldownThreshold : left.MinCooldownThreshold;
        left.GunWindUpTime = right.GunWindUpTime != 0 ? right.GunWindUpTime : left.GunWindUpTime;

        return left;
    }
}
