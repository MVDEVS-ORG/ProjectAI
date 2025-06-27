using UnityEngine;

[CreateAssetMenu(fileName = "GunsSO", menuName = "Scriptable Objects/GunsSO")]
public class GunsSO : ScriptableObject
{
    public string PrimaryProjectileAddressable;
    public string SecondaryProjectileAddressable;
    public string GunUIAddressable;
    public float FireRate;
    public float OverHeatLimit;
    public float OverHeatRate;
    public float CoolDownRate;
    public float MinCooldownThreshold;
    public float GunWindUpTime;
    public float ElipseVerticalRadius;
    public float ElipseHorizontalRadius;
    public Sprite GunRight;
    public Sprite GunLeft;
    public Sprite GunUp;
    public Sprite GunDown;
}
