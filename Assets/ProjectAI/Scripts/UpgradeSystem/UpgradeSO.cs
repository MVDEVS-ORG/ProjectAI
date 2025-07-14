using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeSO", menuName = "Scriptable Objects/UpgradeSO")]
public class UpgradeSO : ScriptableObject
{
    public string Description;
    public Sprite Sprite;

    [Space]
    [Header("The stat Tier and Type")]
    public StatType Type;
    public UpgradeTier UpgradeTier;

    [Space]
    [Header("The stat data")]
    public PlayerModel playerModel;
    public GunsModel gunsModel;
    public MeleeWeaponModel meleeWeaponModel;
}

public enum StatType
{
    Additive,
    Multiplicative,
    Set
}

public enum UpgradeTier
{
    Tier1,
    Tier2,
    Tier3,
    Cursed,
    Blessed
}
