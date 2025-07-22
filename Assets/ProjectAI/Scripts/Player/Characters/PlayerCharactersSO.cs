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
    public float InvincibilityTime;
    public float DamageKickBackSpeed;
    public float DamageKickBackTime;
}

public enum Character
{
    Gunner,
    Shotgun,
    Pyro
}
