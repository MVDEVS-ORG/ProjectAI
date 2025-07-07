using UnityEngine;

[CreateAssetMenu(fileName = "MeleeWeaponSO", menuName = "Scriptable Objects/MeleeWeaponSO")]
public class MeleeWeaponSO : ScriptableObject
{
    public float AttackSpeed;
    public float AttackTime;
    public int AttackChainLimit;
    public float AttackRechargeDelay;
}
