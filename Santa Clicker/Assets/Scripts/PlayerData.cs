using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class UpgradeLevel
{
    public string upgradeName;
    public int level;
}

public class PlayerData : MonoBehaviour
{
    //Currency Total
    public double gingerBreadAmount = 0;
    public double candyCaneAmount = 0;
    public double cookieAmount = 0;

    //clickmultiplier
    public double gingerBreadClick = 1;
    public double candyCaneClick = 1;
    public double cookieClick = 1;

    public double gingerbreadPerSecond = 0;
    public double candyCanePerSecond = 0;
    public double cookiePerSecond = 0;

    //Upgrade tracking
    public List<UpgradeLevel> upgradeLevels = new List<UpgradeLevel>();

    //Helper methods for upgrade tracking
    public int GetUpgradeLevel(string upgradeName)
    {
        var upgrade = upgradeLevels.Find(u => u.upgradeName == upgradeName);
        return upgrade != null ? upgrade.level : 0;
    }

    public void SetUpgradeLevel(string upgradeName, int level)
    {
        var upgrade = upgradeLevels.Find(u => u.upgradeName == upgradeName);
        if (upgrade != null)
        {
            upgrade.level = level;
        }
        else
        {
            upgradeLevels.Add(new UpgradeLevel { upgradeName = upgradeName, level = level });
        }
    }

    public void IncrementUpgradeLevel(string upgradeName)
    {
        SetUpgradeLevel(upgradeName, GetUpgradeLevel(upgradeName) + 1);
    }
}
