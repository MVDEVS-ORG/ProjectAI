using Assets.Services;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradesPopUp : MonoBehaviour
{
    [SerializeField] List<UpgradeCard> _upgradeCards;
    private IUpgradeController _upgradeController;
    [SerializeField] private ControllerNavigation _controllerNavigation;

    public void UpdateList(List<List<UpgradeSO>> ListOfUpgrades)
    {
        if (ListOfUpgrades != null)
        {
            int i = 0;
            for (i = 0; i < ListOfUpgrades.Count; i++)
            {
                _upgradeCards[i].LoadCard(ListOfUpgrades[i], this);
            }
            for(int j =i; j<3;j++)
            {
                _upgradeCards[i].gameObject.SetActive(false);
            }
        }
        //_controllerNavigation.ForcedSelectionAfterInitialization();
        Time.timeScale = 0f;
    }

    public void Initialize(IUpgradeController upgradeController, IAssetService assetService)
    {
        _upgradeController = upgradeController;
        foreach(UpgradeCard card in _upgradeCards)
        {
            card.Initialize(this, assetService);
        }

    }


    public void OnSelected(List<UpgradeSO> upgrade)
    {
        _upgradeController.SelectedUpgrade(upgrade);
        Time.timeScale = 1f;
    }

    public void ClearPreviousList()
    {
        foreach (var upgrade in _upgradeCards)
        {
            upgrade.Clear();
            upgrade.CardButton.onClick.RemoveAllListeners();
        }
    }

    public void SkipUpgrade()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    /*private void OnEnable()
    {
        //EventSystem.current.firstSelectedGameObject = _upgradeCards[0].gameObject;
    }*/
}
