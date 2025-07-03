using UnityEngine;

[CreateAssetMenu(fileName = "PlayerCharactersSO", menuName = "Scriptable Objects/PlayerCharactersSO")]
public class PlayerCharactersSO : ScriptableObject
{
    public int MaxHealth;
    public Character CharacterType;
    public float Speed;
    public float CursorDistance;
    public float RollDuration;
    public float RollSpeed;
    public int MaxNoOfRolls;
    public int NoOfRolls;  
    public float RollCooldown;
    public Sprite UpSprite;
    public Sprite DownSprite;
    public Sprite LeftSprite;
    public Sprite RightSprite;
}

public enum Character
{
    Gunner,
    Shotgun,
    Pyro
}
