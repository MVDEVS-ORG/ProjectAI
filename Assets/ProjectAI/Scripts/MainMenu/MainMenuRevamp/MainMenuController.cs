using Assets.Services;
using Zenject;

public class MainMenuController
{
    [Inject] IAssetService _assetService;

    private ResetData _resetData = new();

    [Inject]
    public void Initialize()
    {
        _resetData.ClearData();
        _assetService.InstantiateAsync(AddressableIds.Main_Menu_UI);
    }
}
