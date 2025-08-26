using UnityEngine;
public abstract class CharacterAbility : ScriptableObject
{
    public abstract void Initialize(PlayerModel playerModel,IPlayerController playerController,IMeleeWeaponController meleeWeaponController,IGunsController gunsController);
    public abstract void UseAbility();
}
