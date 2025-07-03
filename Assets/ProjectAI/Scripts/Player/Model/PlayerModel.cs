using UnityEngine;

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
    public Sprite UpSprite;
    public Sprite DownSprite;
    public Sprite LeftSprite;
    public Sprite RightSprite;
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
        UpSprite = playerModelData.UpSprite;
        DownSprite = playerModelData.DownSprite;
        LeftSprite = playerModelData.LeftSprite;
        RightSprite = playerModelData.RightSprite;
    }
}
