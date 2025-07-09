using UnityEngine;

public class MeleeWeaponModel
{
    public float AttackSpeed;
    public float AttackTime;
    public int AttackChainLimit;
    public float AttackRechargeDelay;
    public float DistanceFromPlayer;
    public int Attacks;
    public int Damage;
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
}
