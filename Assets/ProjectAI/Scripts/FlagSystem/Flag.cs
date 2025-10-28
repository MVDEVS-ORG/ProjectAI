using UnityEngine;

[CreateAssetMenu(fileName = "Flag", menuName = "Scriptable Objects/Flag")]
public class Flag : ScriptableObject
{
    public FlagType Type;
    public FlagPurpose Purpose;
    public string PurposeDescription;
    public string ItemDescription;
    public string ItemIconAddressable;
}

public enum FlagType
{
    Temporary,
    PersistAcrossRun,
    Permanent
}

public enum FlagPurpose
{
    ItemEntry,
    LocationUnlock,
    CharacterUnlock,
}


