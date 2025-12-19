using Assets.ProjectAI.Scripts.MainMenu;
using Assets.Services;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class MainMenuController
{
    [Inject] IAssetService _assetService;

    private ResetData _resetData = new();

    [Inject]
    public async Task Initialize()
    {
        _resetData.ClearData();
        _ = _assetService.InstantiateAsync(AddressableIds.Main_Menu_UI);
    }
}
