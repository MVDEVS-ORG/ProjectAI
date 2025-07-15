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
    /*public Sprite UpSprite;
    public Sprite DownSprite;
    public Sprite LeftSprite;
    public Sprite RightSprite;*/

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
        /*UpSprite = playerModelData.UpSprite;
        DownSprite = playerModelData.DownSprite;
        LeftSprite = playerModelData.LeftSprite;
        RightSprite = playerModelData.RightSprite;*/
    }

    public static PlayerModel operator +(PlayerModel left, PlayerModel right)
    {
        /*PlayerModel temp = new PlayerModel();
        temp.Character = left.Character;
        temp.CursorDistance = left.CursorDistance;
        *//*temp.UpSprite = left.UpSprite;
        temp.DownSprite = left.DownSprite;
        temp.LeftSprite = left.LeftSprite;
        temp.RightSprite = left.RightSprite;*//*
        temp.Health = left.Health;
        temp.NoOfRoll = left.NoOfRoll;*/
        //The above attributes dont change
        left.MaxHealth = left.MaxHealth + right.MaxHealth;
        left.Speed = left.Speed + right.Speed;
        left.RollDuration = left.RollDuration + right.RollDuration;
        left.RollSpeed = left.RollSpeed + right.RollSpeed;
        left.MaxNoOfRolls = left.MaxNoOfRolls + right.MaxNoOfRolls;
        left.RolllCooldown = left.RolllCooldown + right.RolllCooldown;

        return left;

    }

    public static PlayerModel operator *(PlayerModel left, PlayerModel right)
    {
        /*PlayerModel temp = new PlayerModel();
        temp.Character = left.Character;
        temp.CursorDistance = left.CursorDistance;
        *//*temp.UpSprite = left.UpSprite;
        temp.DownSprite = left.DownSprite;
        temp.LeftSprite = left.LeftSprite;
        temp.RightSprite = left.RightSprite;*//*
        temp.Health = left.Health;
        temp.NoOfRoll = left.NoOfRoll;*/

        //The above attributes dont change
        left.MaxHealth = left.MaxHealth * right.MaxHealth;
        left.Speed = left.Speed * right.Speed;
        left.RollDuration = left.RollDuration * right.RollDuration;
        left.RollSpeed = left.RollSpeed * right.RollSpeed;
        left.MaxNoOfRolls = left.MaxNoOfRolls * right.MaxNoOfRolls;
        left.RolllCooldown = left.RolllCooldown * right.RolllCooldown;

        return left;
    }

    public static PlayerModel operator %(PlayerModel left,PlayerModel right)
    {
        /*PlayerModel temp = new PlayerModel();
        temp.Character = left.Character;
        temp.CursorDistance = left.CursorDistance;
        *//*temp.UpSprite = left.UpSprite;
        temp.DownSprite = left.DownSprite;
        temp.LeftSprite = left.LeftSprite;
        temp.RightSprite = left.RightSprite;*//*
        temp.Health = left.Health;
        temp.NoOfRoll = left.NoOfRoll;*/

        //The above attributes dont change
        left.MaxHealth = right.MaxHealth != 0 ? right.MaxHealth : left.MaxHealth;
        left.Speed = right.Speed != 0 ? right.Speed : left.Speed;
        left.RollDuration = right.RollDuration != 0 ? right.RollDuration : left.RollDuration;
        left.RollSpeed = right.RollSpeed != 0 ? right.RollSpeed : left.RollSpeed;
        left.MaxNoOfRolls = right.MaxNoOfRolls != 0 ? right.MaxNoOfRolls : left.MaxNoOfRolls;
        left.RolllCooldown = right.RolllCooldown != 0 ? right.RolllCooldown : left.RolllCooldown;

        return left;
    }
}
