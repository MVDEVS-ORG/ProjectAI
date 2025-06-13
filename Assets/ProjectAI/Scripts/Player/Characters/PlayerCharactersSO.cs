using UnityEngine;

[CreateAssetMenu(fileName = "PlayerCharactersSO", menuName = "Scriptable Objects/PlayerCharactersSO")]
public class PlayerCharactersSO : ScriptableObject
{
    public int MaxHealth;
    public Character CharacterType;
    public float Speed;
}

public enum Character
{
    Gunner,
    Shotgun,
    Pyro
}
