using System;
using System.Collections.Generic;
using UnityEngine;

public interface IUpgradeController
{
    Awaitable Initialize();
    void DisplayUpgrades();
    void SelectedUpgrade(List<UpgradeSO> upgrades);

    event Action<List<UpgradeSO>> OnUpgrade;
    void RefreshUpgrades();
    void SaveUpgrades();
    void LoadUpgrades();
    void ClearUpgrades();
    void SaveXPAndHP(int currentXP, int playerLevel, int playerHealth);
    (int, int, int) RefreshXP();
    void ClearXP();
}
