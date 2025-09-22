using System.Collections.Generic;

class ResetData
{
    private DataSerializer _dataSerializer = new();

    // Clear the save file data when loading into game from main menu
    public void ClearData()
    {
        //Player Stats
        _dataSerializer.SaveData(AddressableIds.Player_Stats_Path, (0, 0, 0));

        //Upgrades
        List<UpgradeSO> emptyListUpgradeSO = new();
        _dataSerializer.SaveData(AddressableIds.Normal_Upgrades_Path, emptyListUpgradeSO);
        _dataSerializer.SaveData(AddressableIds.Cursed_Upgrades_Path, emptyListUpgradeSO);

        //Guns
        (string, List<string>) emptyListStrings = ("", new List<string>());
        _dataSerializer.SaveData(AddressableIds.Player_Guns_Path, emptyListStrings);
    }
}