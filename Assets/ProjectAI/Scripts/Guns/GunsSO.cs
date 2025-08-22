using UnityEngine;

[CreateAssetMenu(fileName = "GunsSO", menuName = "Scriptable Objects/GunsSO")]
public class GunsSO : ScriptableObject
{
    public string PrimaryProjectileAddressable;
    public string SecondaryProjectileAddressable;
    public string GunUIAddressable;
    public string GunViewAddressableId;
    public float FireRate;
    public float OverHeatLimit;
    public float OverHeatRate;
    public float CoolDownRate;
    public float MinCooldownThreshold;
    public float GunWindUpTime;
    public float ElipseVerticalRadius;
    public float ElipseHorizontalRadius;
}
