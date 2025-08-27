using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "GunnerAbility", menuName = "Scriptable Objects/GunnerAbility")]
public class GunnerAbility : CharacterAbility
{  
    private IPlayerController _playerController;
    private IMeleeWeaponController _weaponController;
    private IGunsController _gunsController;
    private PlayerModel _playerModel;

    private bool AbilityOnCooldown = false;

    public override void Initialize(PlayerModel playerModel, IPlayerController playerController, IMeleeWeaponController meleeWeaponController, IGunsController gunsController)
    {
        _playerController = playerController;
        _weaponController = meleeWeaponController;
        _gunsController = gunsController;
        _playerModel = playerModel;
    }

    public override void UseAbility()
    {
        Debug.LogError("Reached here");
        if(AbilityOnCooldown)
        {
            return;
        }
        _ = AbilityCooldown();
        _ = AbilityDuration();
    }

    private async Awaitable AbilityCooldown()
    {
        AbilityOnCooldown = true;
        await Awaitable.WaitForSecondsAsync(_playerModel.AbilityCooldown);
        AbilityOnCooldown = false;
    }

    private async Awaitable AbilityDuration()
    {
        _gunsController.FireAllGuns(true, _playerModel.AbilityAlter);
        _playerController.GunEnabled = false;
        await Awaitable.WaitForSecondsAsync(_playerModel.AbilityDuration);
        _playerController.GunEnabled = true;
        _gunsController.FireAllGuns(false, _playerModel.AbilityAlter);
    }
}
