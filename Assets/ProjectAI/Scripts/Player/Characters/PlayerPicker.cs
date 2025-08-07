using Assets.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class PlayerPicker
{
    [Inject] private IAssetService _assetService;
    public PlayerCharactersSO SelectedPlayer { get; private set; }
    private List<PlayerCharactersSO> _selectableCharacters = new List<PlayerCharactersSO>();

    public PlayerCharactersSO PickPlayer()
    {
        //SelectedPlayer = _selectableCharacters[Random.Range(0,_selectableCharacters.Count)];
        SelectedPlayer = _selectableCharacters[0];
        return SelectedPlayer;
    }

    public async Awaitable SetPlayer()
    {
        _selectableCharacters.Add(await _assetService.LoadAssetAsync<PlayerCharactersSO>(AddressableIds.Gunner_Data_SO));
        _selectableCharacters.Add(await _assetService.LoadAssetAsync<PlayerCharactersSO>(AddressableIds.Shotgunner_Data_SO));
        _selectableCharacters.Add(await _assetService.LoadAssetAsync<PlayerCharactersSO>(AddressableIds.Pyro_Data_SO));
    }

    public PlayerCharactersSO SelectPlayer(Character character)
    {
        foreach (var player in _selectableCharacters)
        {
            if (player.CharacterType == character)
            {
                return SelectedPlayer = player;
            }
        }

        Debug.LogWarning($"No player found matching character type: {character}");
        return SelectedPlayer = null;
    }
}
