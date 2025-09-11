using Assets.Services;
using Zenject;

public class MainMenuController
{
    [Inject] IUpgradeController _upgradeController;
    [Inject] IAssetService _assetService;
    [Inject]
    public void Initialize()
    {
        _upgradeController.ClearUpgrades();
        _upgradeController.ClearXP();
        _assetService.InstantiateAsync(AddressableIds.Main_Menu_UI);
    }
}
