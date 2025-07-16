using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradesLists", menuName = "Scriptable Objects/UpgradesLists")]
public class UpgradesLists : ScriptableObject
{
    public List<UpgradeSO> Tier1;
    public List<UpgradeSO> Tier2;
    public List<UpgradeSO> Tier3;
    public List<UpgradeSO> Cursed;
    public List<UpgradeSO> Blessed;
}
