using UnityEngine;

public interface IUpgradeController
{
    void Initialize();
    Awaitable DisplayUpgrades();
}
