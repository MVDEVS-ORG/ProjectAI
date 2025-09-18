using Assets.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ElementalistAbility", menuName = "Scriptable Objects/ElementalistAbility")]
public class ElementalistAbility : CharacterAbility
{
    public const string Elementalist_UI = "ElementalistUI";

    private IGunsController _gunsController;
    private IMeleeWeaponController _meleeWeaponController;
    [SerializeField] private int _startingAfflictionValue;
    Dictionary<ElementEnum,int> _elementalBuffs = new Dictionary<ElementEnum,int>();
    private int _index = 0;
    private ElementalistAbilityUI _abilityUI; 
    public override void Initialize(PlayerModel playerModel, IPlayerController playerController, IMeleeWeaponController meleeWeaponController, IGunsController gunsController, IAssetService assetService)
    {
        _gunsController = gunsController;
        _meleeWeaponController = meleeWeaponController;
        _elementalBuffs.Clear();

        _ = LoadUI(assetService);

        #region initialize buffs dictionary
        foreach(ElementEnum element in Enum.GetValues(typeof(ElementEnum)))
        {
            if ((int)element == _index)
            {
                _elementalBuffs[element] = _startingAfflictionValue;
            }
            else
            {
                _elementalBuffs[element] = 0;
            }
        }
        _gunsController.SetGunElements(_elementalBuffs);
        gunsController.OnGunSwap += SetElements;
        #endregion
    }

    public override void UseAbility()
    {
        Debug.LogError("Use ability called");
        _index = (_index + 1) % (Enum.GetValues(typeof(ElementEnum)).Length);
        _abilityUI.SetImage(_index);
        foreach (ElementEnum element in Enum.GetValues(typeof(ElementEnum)))
        {
            if ((int)element == _index)
            {
                _elementalBuffs[element] = _startingAfflictionValue;
            }
            else
            {
                _elementalBuffs[element] = 0;
            }
        }
        _gunsController.SetGunElements(_elementalBuffs);
    }

    public void SetElements(GunsView view)
    {
        view.ElementalBuffs = _elementalBuffs;
    }

    private async Awaitable LoadUI(IAssetService assetService)
    {
        GameObject obj = await assetService.InstantiateAsync(Elementalist_UI);
        _abilityUI = obj.GetComponent<ElementalistAbilityUI>();
        _abilityUI.SetImage(_index);
    }
}
