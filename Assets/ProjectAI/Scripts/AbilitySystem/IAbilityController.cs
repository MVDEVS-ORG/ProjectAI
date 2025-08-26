using Assets.Services;
using UnityEngine;

public interface IAbilityController
{
    Awaitable Initialize(IAssetService assetService, PlayerModel model, IPlayerController playerController, IGunsController gunsController, IMeleeWeaponController meleeWeaponController);
    void UseAbility();
}
