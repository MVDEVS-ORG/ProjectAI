using Assets.Services;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class AbilityController : IAbilityController
{
    private PlayerModel _model;
    private CharacterAbility _characterAbility;

    async Awaitable IAbilityController.Initialize(IAssetService assetService, PlayerModel model, IPlayerController playerController, IGunsController gunsController, IMeleeWeaponController meleeWeaponController)
    {
        _model = model;
        _characterAbility = await assetService.LoadAssetAsync<CharacterAbility>(model.CharacterAbilityAddressableId);
        _characterAbility.Initialize(model, playerController, meleeWeaponController, gunsController);
    }

    void IAbilityController.UseAbility()
    {
        _characterAbility.UseAbility();
    }
}
