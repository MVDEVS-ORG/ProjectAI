using System;
using System.Collections.Generic;
using UnityEngine;

public interface IUpgradeController
{
    Awaitable Initialize();
    void DisplayUpgrades();
    void SelectedUpgrade(List<UpgradeSO> upgrades);

    event Action<List<UpgradeSO>> OnUpgrade;
    (List<UpgradeSO>, List<UpgradeSO>) RefreshUpgrades();
    void SaveUpgrades();
    void LoadUpgrades();
    void ClearUpgrades();
    void SavePlayerStats(int currentXP, int playerLevel, int playerHealth);
    (int, int, int) LoadPlayerStats(PlayerModel playerModel);
    void ClearPlayerStats();
}
