using UnityEngine;

public class PlayerModel
{
    public int MaxHealth;
    public int Health;
    public float Speed;
    public Character Character;
    public float FireRate; //No of rounds per second
    public float CursorDistance;
    public float RollDuration;
    public float RollSpeed;
    public int NoOfRoll;
    public int MaxNoOfRolls;
    public float RolllCooldown;
    public PlayerModel(PlayerCharactersSO playerModelData)
    {
        Character = playerModelData.CharacterType;
        MaxHealth = playerModelData.MaxHealth;
        Speed = playerModelData.Speed;
        Health = MaxHealth;
        FireRate = playerModelData.FireRate;
        CursorDistance = playerModelData.CursorDistance;
        RollDuration = playerModelData.RollDuration;
        RollSpeed = playerModelData.RollSpeed;  
        NoOfRoll = playerModelData.NoOfRolls;
        RolllCooldown = playerModelData.RollCooldown;
        MaxNoOfRolls = playerModelData.MaxNoOfRolls;
    }
}
