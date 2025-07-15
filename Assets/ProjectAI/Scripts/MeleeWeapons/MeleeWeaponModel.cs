using System;
using UnityEngine;

[Serializable]
public class MeleeWeaponModel
{
    public float AttackSpeed;
    public float AttackTime;
    public int AttackChainLimit;
    public float AttackRechargeDelay;
    public float DistanceFromPlayer;
    public int Attacks;
    public int Damage;

    public MeleeWeaponModel() { }

    public MeleeWeaponModel(MeleeWeaponSO meleeWeaponSO) 
    {
        Attacks = 0;
        AttackSpeed = meleeWeaponSO.AttackSpeed;
        AttackTime = meleeWeaponSO.AttackTime;
        AttackChainLimit = meleeWeaponSO.AttackChainLimit;
        AttackRechargeDelay = meleeWeaponSO.AttackRechargeDelay;
        DistanceFromPlayer = meleeWeaponSO.DistanceFromPlayer;
        Damage = meleeWeaponSO.Damage;
    }

    public static MeleeWeaponModel operator +(MeleeWeaponModel left, MeleeWeaponModel right)
    {
        left.AttackSpeed = left.AttackSpeed + right.AttackSpeed;
        left.AttackTime = left.AttackTime + right.AttackTime;
        left.AttackChainLimit = left.AttackChainLimit + right.AttackChainLimit;
        left.AttackRechargeDelay = left.AttackRechargeDelay + right.AttackRechargeDelay;
        left.DistanceFromPlayer = left.DistanceFromPlayer + right.DistanceFromPlayer;
        left.Damage = left.Damage + right.Damage;
        return left;
    }

    public static MeleeWeaponModel operator *(MeleeWeaponModel left, MeleeWeaponModel right)
    {
        left.AttackSpeed = left.AttackSpeed * right.AttackSpeed;
        left.AttackTime = left.AttackTime * right.AttackTime;
        left.AttackChainLimit = left.AttackChainLimit * right.AttackChainLimit;
        left.AttackRechargeDelay = left.AttackRechargeDelay * right.AttackRechargeDelay;
        left.DistanceFromPlayer = left.DistanceFromPlayer * right.DistanceFromPlayer;
        left.Damage = left.Damage * right.Damage;
        return left;
    }

    public static MeleeWeaponModel operator %(MeleeWeaponModel left, MeleeWeaponModel right)
    {
        left.AttackSpeed = right.AttackSpeed!=0?right.AttackSpeed:left.AttackSpeed;
        left.AttackTime = right.AttackTime!=0?right.AttackTime:left.AttackTime;
        left.AttackChainLimit = right.AttackChainLimit != 0 ? right.AttackChainLimit : left.AttackChainLimit;
        left.AttackRechargeDelay = right.AttackRechargeDelay != 0 ? right.AttackRechargeDelay : left.AttackRechargeDelay;
        left.DistanceFromPlayer = right.DistanceFromPlayer != 0 ? right.DistanceFromPlayer : left.DistanceFromPlayer;
        left.Damage = right.Damage != 0 ? right.Damage : left.Damage;
        return left;
    }
}
