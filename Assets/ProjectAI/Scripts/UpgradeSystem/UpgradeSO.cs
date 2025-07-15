using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeSO", menuName = "Scriptable Objects/UpgradeSO")]
public class UpgradeSO : ScriptableObject
{
    public string Header;
    public string Description;
    public string SpriteAddressable;

    [Space]
    [Header("The stat Tier and Type and stat")]
    public StatType Type;
    public UpgradeTier UpgradeTier;
    public UpgradeType UpgradeType;

    [Space]
    [Header("The stat data")]
    public PlayerModel playerModel;
    public GunsModel gunsModel;
    public MeleeWeaponModel meleeWeaponModel;

    [Space]
    [Header("Upgrade Path")]
    public UpgradeSO FuturePath;
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

public enum UpgradeType
{
    Player,
    Gun,
    MeleeWeapon
}

