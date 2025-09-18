using Assets.Services;
using UnityEngine;
public abstract class CharacterAbility : ScriptableObject
{
    public abstract void Initialize(PlayerModel playerModel, IPlayerController playerController, IMeleeWeaponController meleeWeaponController, IGunsController gunsController, IAssetService assetService);
    public abstract void UseAbility();
}
