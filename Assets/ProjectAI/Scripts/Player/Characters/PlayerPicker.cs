using Assets.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class PlayerPicker
{
    [Inject] private IAssetService _assetService;
    public PlayerCharactersSO selectedPlayer { get; private set; }
    private List<PlayerCharactersSO> _selectableCharacters = new List<PlayerCharactersSO>();

    public PlayerCharactersSO PickPlayer()
    {
        selectedPlayer = _selectableCharacters[Random.Range(0,_selectableCharacters.Count)];
        return selectedPlayer;
    }

    public async Task SetPlayer()
    {
        _selectableCharacters.Add(await _assetService.LoadAssetAsync<PlayerCharactersSO>(AddressableIds.GunnerDataSO));
        _selectableCharacters.Add(await _assetService.LoadAssetAsync<PlayerCharactersSO>(AddressableIds.ShotgunnerDataSO));
        _selectableCharacters.Add(await _assetService.LoadAssetAsync<PlayerCharactersSO>(AddressableIds.PyroDataSO));
    }
}
