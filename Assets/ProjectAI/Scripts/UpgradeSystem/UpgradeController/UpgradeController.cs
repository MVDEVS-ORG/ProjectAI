using Assets.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
    private DataSerializer _dataSerializer =  new DataSerializer();

    public event Action<List<UpgradeSO>> OnUpgrade;

    void IUpgradeController.DisplayUpgrades()
    {
        Cursor.visible = true;
        _upgradesPopup.gameObject.SetActive(true);
        _upgradesCanvas.enabled = true;
        _upgradesPopup.UpdateList(GenerateUpgrades());
    }

    private List<List<UpgradeSO>> GenerateUpgrades()
    {
        List<List<UpgradeSO>> upgrades = new List<List<UpgradeSO>>();
        #region logic for tier one items

        List<UpgradeSO> possibleUpgrades = new List<UpgradeSO>(_upgradeList.Tier1);
        Debug.LogError(possibleUpgrades.Count);
        foreach (UpgradeSO upgrade in _activeUpgrades)
        {
            if (_activeUpgrades.Contains(upgrade.FuturePath))
            {
                if (_activeUpgrades.Contains(upgrade.FuturePath.FuturePath))
                {
                    possibleUpgrades.Remove(upgrade);
                }
                else
                {
                    possibleUpgrades.Remove(upgrade);
                    if (upgrade.FuturePath.FuturePath != null)
                    {
                        possibleUpgrades.Add(upgrade.FuturePath.FuturePath);
                    }
                }
            }
            else
            {
                possibleUpgrades.Remove(upgrade);
                if (upgrade.FuturePath != null)
                {
                    possibleUpgrades.Add(upgrade.FuturePath);
                }
            }
        }
        if (possibleUpgrades.Count > 3)
        {
            while (upgrades.Count < 3)
            {
                List<UpgradeSO> upgrade = new();
                int randomUpgrade = UnityEngine.Random.Range(0, possibleUpgrades.Count);
                upgrade.Add(possibleUpgrades[(int)randomUpgrade]);
                upgrades.Add(upgrade);
                possibleUpgrades.Remove(possibleUpgrades[(int)randomUpgrade]);
            }
        }
        else
        {
            foreach (UpgradeSO possibleUpgrade in possibleUpgrades)
            {
                List<UpgradeSO> upgrade = new();
                upgrade.Add(possibleUpgrade);
                upgrades.Add(upgrade);
            }
        }
        Debug.LogError(upgrades.Count);
        return upgrades;
        #endregion
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
            _path = Application.persistentDataPath + "/gameData/upgrades.txt";
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
        _activeUpgrades = _dataSerializer.LoadData<List<UpgradeSO>>("/GameData/upgrades.json");
        _cursedUpgrades = _dataSerializer.LoadData<List<UpgradeSO>>("/GameData/curses.json");
    }

    void IUpgradeController.RefreshUpgrades()
    {
        (this as IUpgradeController).RefreshUpgrades();
        OnUpgrade.Invoke(_activeUpgrades);
        OnUpgrade.Invoke(_cursedUpgrades);
    }

    void IUpgradeController.SaveUpgrades()
    {
        _dataSerializer.SaveData("/GameData/upgrades.json",_activeUpgrades);
        _dataSerializer.SaveData("/GameData/curses.json",_cursedUpgrades);
    }

    void IUpgradeController.SelectedUpgrade(List<UpgradeSO> upgrades)
    {
        Debug.LogError(upgrades[0].Header);
        foreach (UpgradeSO upgrade in upgrades)
        {
            if(upgrade.UpgradeTier!=UpgradeTier.Cursed)
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
        if(_cursedUpgrades.Count>0 && OnUpgrade!=null)
        {
            OnUpgrade.Invoke(_cursedUpgrades);
        }
        (this as IUpgradeController).SaveUpgrades();
        _upgradesPopup.ClearPreviousList();
        _upgradesCanvas.enabled = false;
        _upgradesPopup.gameObject.SetActive(false);
        Cursor.visible = false;
    }
}
