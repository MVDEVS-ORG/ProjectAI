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
        var introObj = await _assetService.InstantiateAsync(AddressableIds.Intro_UI);
        var introController = introObj.GetComponent<IntroController>();
        while (!introController.IsIntroComplete)
        {
            await Awaitable.NextFrameAsync();
        }
        GameObject.Destroy(introObj);
        _ = _assetService.InstantiateAsync(AddressableIds.Main_Menu_UI);
    }
}
