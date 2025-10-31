using System;
using UnityEngine;

[Serializable]
public class PlayerModel
{
    public int MaxHealth;
    public int Health;
    public float Speed;
    public Character Character;
    public float CursorDistance;
    public float RollDuration;
    public float RollSpeed;
    public int NoOfRoll;
    public int MaxNoOfRolls;
    public float RolllCooldown;
    public float InvincibilityTime;
    public float DamageKickBackSpeed;
    public float DamageKickBackTime;
    [HideInInspector] public int Experience;
    [HideInInspector] public int PlayerLevel;
    public float XPCollectionRadius;
    public float MeleeDashTime;
    public float MeleeDashSpeed;

    public string CharacterAbilityAddressableId;

    public float AbilityCooldown;
    public float AbilityDuration;
    [HideInInspector] public bool AbilityAlter = false;

    public PlayerModel()
    {

    }

    public PlayerModel(PlayerCharactersSO playerModelData)
    {
        Character = playerModelData.CharacterType;
        MaxHealth = playerModelData.MaxHealth;
        Speed = playerModelData.Speed;
        Health = MaxHealth;
        CursorDistance = playerModelData.CursorDistance;
        RollDuration = playerModelData.RollDuration;
        RollSpeed = playerModelData.RollSpeed;
        NoOfRoll = playerModelData.NoOfRolls;
        RolllCooldown = playerModelData.RollCooldown;
        MaxNoOfRolls = playerModelData.MaxNoOfRolls;
        InvincibilityTime = playerModelData.InvincibilityTime;
        DamageKickBackSpeed = playerModelData.DamageKickBackSpeed;
        DamageKickBackTime = playerModelData.DamageKickBackTime;
        XPCollectionRadius = playerModelData.XPCollectionRadius;
        MeleeDashTime = playerModelData.MeleeDashTime;
        MeleeDashSpeed = playerModelData.MeleeDashSpeed;
        AbilityCooldown = playerModelData.AbilityCooldown;
        AbilityDuration = playerModelData.AbilityDuration;
        CharacterAbilityAddressableId = playerModelData.CharacterAbilityAddressableId;
    }

    public static PlayerModel operator +(PlayerModel left, PlayerModel right)
    {
        left.MaxHealth = left.MaxHealth + right.MaxHealth;
        left.Speed = left.Speed + right.Speed;
        left.RollDuration = left.RollDuration + right.RollDuration;
        left.RollSpeed = left.RollSpeed + right.RollSpeed;
        left.MaxNoOfRolls = left.MaxNoOfRolls + right.MaxNoOfRolls;
        left.RolllCooldown = left.RolllCooldown + right.RolllCooldown;
        left.InvincibilityTime = left.InvincibilityTime + right.InvincibilityTime;
        left.DamageKickBackSpeed = left.DamageKickBackSpeed + right.DamageKickBackSpeed;
        left.MeleeDashSpeed = left.MeleeDashSpeed + right.MeleeDashSpeed;
        left.AbilityCooldown = left.AbilityCooldown + right.AbilityCooldown;
        left.AbilityDuration = left.AbilityDuration + right.AbilityDuration;
        return left;

    }

    public static PlayerModel operator *(PlayerModel left, PlayerModel right)
    {
        left.MaxHealth = left.MaxHealth * right.MaxHealth;
        left.Speed = left.Speed * right.Speed;
        left.RollDuration = left.RollDuration * right.RollDuration;
        left.RollSpeed = left.RollSpeed * right.RollSpeed;
        left.MaxNoOfRolls = left.MaxNoOfRolls * right.MaxNoOfRolls;
        left.RolllCooldown = left.RolllCooldown * right.RolllCooldown;
        left.InvincibilityTime = left.InvincibilityTime * right.InvincibilityTime;
        left.DamageKickBackSpeed = left.DamageKickBackSpeed * right.DamageKickBackSpeed;
        left.MeleeDashSpeed = left.MeleeDashSpeed * right.MeleeDashSpeed;
        left.AbilityCooldown = left.AbilityCooldown * right.AbilityCooldown;
        left.AbilityDuration = left.AbilityDuration * right.AbilityDuration;
        return left;
    }

    public static PlayerModel operator %(PlayerModel left, PlayerModel right)
    {
        left.MaxHealth = right.MaxHealth != 0 ? right.MaxHealth : left.MaxHealth;
        left.Speed = right.Speed != 0 ? right.Speed : left.Speed;
        left.RollDuration = right.RollDuration != 0 ? right.RollDuration : left.RollDuration;
        left.RollSpeed = right.RollSpeed != 0 ? right.RollSpeed : left.RollSpeed;
        left.MaxNoOfRolls = right.MaxNoOfRolls != 0 ? right.MaxNoOfRolls : left.MaxNoOfRolls;
        left.RolllCooldown = right.RolllCooldown != 0 ? right.RolllCooldown : left.RolllCooldown;
        left.InvincibilityTime = right.InvincibilityTime != 0 ? right.InvincibilityTime : left.InvincibilityTime;
        left.DamageKickBackSpeed = right.DamageKickBackSpeed != 0 ? right.DamageKickBackSpeed : left.DamageKickBackSpeed;
        left.MeleeDashSpeed = right.MeleeDashSpeed != 0 ? right.MeleeDashSpeed : left.MeleeDashSpeed;
        left.AbilityCooldown = right.AbilityCooldown != 0 ? right.AbilityCooldown : left.AbilityCooldown;
        left.AbilityDuration = right.AbilityDuration != 0 ? right.AbilityCooldown : left.AbilityCooldown;
        left.AbilityAlter = right.AbilityAlter;
        return left;
    }
}
