using UnityEngine;

[CreateAssetMenu(fileName = "PlayerCharactersSO", menuName = "Scriptable Objects/PlayerCharactersSO")]
public class PlayerCharactersSO : ScriptableObject
{
    public int MaxHealth;
    public Character CharacterType;
    public float Speed;
    public float FireRate;
    public float CursorDistance;
    public float RollDuration;
    public float RollSpeed;
    public int MaxNoOfDashes;
    public int NoOfDashes;  
    public float DashCooldown;
}

public enum Character
{
    Gunner,
    Shotgun,
    Pyro
}
