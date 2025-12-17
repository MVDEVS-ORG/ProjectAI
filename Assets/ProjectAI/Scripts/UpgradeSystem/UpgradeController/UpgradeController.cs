using System;
using System.Collections.Generic;
using System.IO;
using Assets.Services;
using Newtonsoft.Json;
using UnityEngine;
using Zenject;

public class UpgradeController : IUpgradeController
{
    [Inject] private IAssetService _assetService;
    List<UpgradeSO> _activeUpgrades = new List<UpgradeSO>();
    List<UpgradeSO> _cursedUpgrades = new List<UpgradeSO>();
    private UpgradesPopUp _upgradesPopup;
    private Canvas _upgradesCanvas;
    private UpgradesLists _upgradeList;

    private string _path;
    private DataSerializer _dataSerializer = new DataSerializer();

    public event Action<List<UpgradeSO>> OnUpgrade;

    void IUpgradeController.ClearUpgrades()
    {
        _activeUpgrades.Clear();
        _cursedUpgrades.Clear();
        (this as IUpgradeController).SaveUpgrades();
    }

    void IUpgradeController.DisplayUpgrades()
    {
        //Cursor.visible = true;
        _upgradesPopup.gameObject.SetActive(true);
        _upgradesCanvas.enabled = true;
        _upgradesPopup.UpdateList(GenerateUpgrades());
    }

    private List<List<UpgradeSO>> GenerateUpgrades()
    {
        List<List<UpgradeSO>> finalUpgrades = new();
        HashSet<UpgradeSO> activeUpgradeSet = new(_activeUpgrades);
        List<UpgradeSO> possibleUpgrades = new(_upgradeList.Tier1);
        HashSet<UpgradeSO> possibleUpgradeSet = new();

        possibleUpgrades.RemoveAll(x => activeUpgradeSet.Contains(x));

        foreach (UpgradeSO activeUpgrade in _activeUpgrades)
        {
            UpgradeSO newUpgrade = FindPossibleUpgrade(activeUpgrade, activeUpgradeSet, 2);
            if (newUpgrade != null && possibleUpgradeSet.Add(newUpgrade))
            {
                possibleUpgrades.Add(newUpgrade);
            }
        }

        List<UpgradeSO> selectedUpgrade = new();
        int maxCount = 3;
        if (possibleUpgrades.Count > maxCount)
        {
            int temp = maxCount;
            while (temp > 0)
            {
                int choice = UnityEngine.Random.Range(0, possibleUpgrades.Count);
                selectedUpgrade.Add(possibleUpgrades[choice]);
                possibleUpgrades.RemoveAt(choice);
                temp--;
            }
        }
        else
        {
            selectedUpgrade = possibleUpgrades;
        }

        foreach (UpgradeSO upgrade in selectedUpgrade)
        {
            finalUpgrades.Add(new List<UpgradeSO> { upgrade });
        }

        return finalUpgrades;
    }

    private UpgradeSO FindPossibleUpgrade(UpgradeSO upgrade, HashSet<UpgradeSO> activeUpgradeSet, int depth, HashSet<UpgradeSO> visitedUpgradeSet = null)
    {
        if (depth <= 0) return null;
        visitedUpgradeSet ??= new();

        if (!visitedUpgradeSet.Add(upgrade)) return null;

        UpgradeSO nextUpgrade = upgrade.FuturePath;
        if (nextUpgrade == null) return null;

        if (!activeUpgradeSet.Contains(nextUpgrade)) return nextUpgrade;

        return FindPossibleUpgrade(nextUpgrade, activeUpgradeSet, depth - 1, visitedUpgradeSet);
    }

    async Awaitable IUpgradeController.Initialize()
    {
        try
        {
            //TODO: create multiple upgrade lists and then add append the number before loading asset
            _upgradeList = await _assetService.LoadAssetAsync<UpgradesLists>(AddressableIds.Upgrades_List_1);
            GameObject temp = await _assetService.InstantiateAsync(AddressableIds.Upgrades_Popup);
            _upgradesCanvas = temp.GetComponent<Canvas>();
            _upgradesPopup = temp.GetComponent<UpgradesPopUp>();
            _upgradesPopup.Initialize(this, _assetService);
            _path = Application.persistentDataPath + "/upgrades.txt";
            _upgradesCanvas.enabled = false;
            _upgradesPopup.gameObject.SetActive(false);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }

    }

    void IUpgradeController.LoadUpgrades()
    {
        _activeUpgrades = _dataSerializer.LoadData<List<UpgradeSO>>(AddressableIds.Normal_Upgrades_Path);
        _cursedUpgrades = _dataSerializer.LoadData<List<UpgradeSO>>(AddressableIds.Cursed_Upgrades_Path);
    }

    (List<UpgradeSO>, List<UpgradeSO>) IUpgradeController.RefreshUpgrades()
    {
        (this as IUpgradeController).LoadUpgrades();
        return (_activeUpgrades, _cursedUpgrades);
    }

    void IUpgradeController.SaveUpgrades()
    {
        _dataSerializer.SaveData(AddressableIds.Normal_Upgrades_Path, _activeUpgrades);
        _dataSerializer.SaveData(AddressableIds.Cursed_Upgrades_Path, _cursedUpgrades);
    }



    void IUpgradeController.SelectedUpgrade(List<UpgradeSO> upgrades)
    {
        foreach (UpgradeSO upgrade in upgrades)
        {
            if (upgrade.UpgradeTier != UpgradeTier.Cursed)
            {
                _activeUpgrades.Add(upgrade);
                List<UpgradeSO> temp = new();
                temp.Add(upgrade);
                if (OnUpgrade != null)
                {
                    OnUpgrade.Invoke(temp);
                }
            }
            else
            {
                _cursedUpgrades.Add(upgrade);
            }
        }
        if (_cursedUpgrades.Count > 0 && OnUpgrade != null)
        {
            OnUpgrade.Invoke(_cursedUpgrades);
        }
        (this as IUpgradeController).SaveUpgrades();
        _upgradesPopup.ClearPreviousList();
        _upgradesCanvas.enabled = false;
        _upgradesPopup.gameObject.SetActive(false);
        Cursor.visible = false;
    }

    void IUpgradeController.SavePlayerStats(int currentXP, int playerLevel, int playerHealth)
    {
        (int, int, int) playerStats = new(currentXP, playerLevel, playerHealth);
        _dataSerializer.SaveData(AddressableIds.Player_Stats_Path, playerStats);
    }

    (int, int, int) IUpgradeController.LoadPlayerStats(PlayerModel playerModel)
    {
        (int, int, int) stats = _dataSerializer.LoadData<(int, int, int)>(AddressableIds.Player_Stats_Path);
        if (stats == (0,0,0))
        {
            stats.Item3 = playerModel.MaxHealth;
        }
        return stats;
    }

    void IUpgradeController.ClearPlayerStats()
    {
        _dataSerializer.SaveData(AddressableIds.Player_Stats_Path, (0, 0, 0));
    }
}
