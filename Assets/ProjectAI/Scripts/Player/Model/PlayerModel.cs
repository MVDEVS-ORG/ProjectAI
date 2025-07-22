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
        return left;
    }

    public static PlayerModel operator %(PlayerModel left,PlayerModel right)
    {
        left.MaxHealth = right.MaxHealth != 0 ? right.MaxHealth : left.MaxHealth;
        left.Speed = right.Speed != 0 ? right.Speed : left.Speed;
        left.RollDuration = right.RollDuration != 0 ? right.RollDuration : left.RollDuration;
        left.RollSpeed = right.RollSpeed != 0 ? right.RollSpeed : left.RollSpeed;
        left.MaxNoOfRolls = right.MaxNoOfRolls != 0 ? right.MaxNoOfRolls : left.MaxNoOfRolls;
        left.RolllCooldown = right.RolllCooldown != 0 ? right.RolllCooldown : left.RolllCooldown;
        left.InvincibilityTime = right.InvincibilityTime !=0 ? right.InvincibilityTime : left.InvincibilityTime;
        left.DamageKickBackSpeed = right.DamageKickBackSpeed !=0 ? right.DamageKickBackSpeed : left.DamageKickBackSpeed;
        return left;
    }
}
