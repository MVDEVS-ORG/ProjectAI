using UnityEngine;

[CreateAssetMenu(fileName = "GunsSO", menuName = "Scriptable Objects/GunsSO")]
public class GunsSO : ScriptableObject
{
    public string PrimaryProjectileAddressable;
    public string SecondaryProjectileAddressable;
    public float FireRate;
    public float OverHeatLimit;
    public float OverHeatRate;
    public float CoolDownRate;
    public float MinCooldownThreshold;
}
