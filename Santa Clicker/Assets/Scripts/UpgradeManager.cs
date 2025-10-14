using UnityEngine;
using System.Collections.Generic;

public class UpgradeManager : MonoBehaviour
{
    public PlayerData playerData;
    public List<UpgradeData> availableUpgrades = new List<UpgradeData>();

    void Start()
    {
        if (playerData == null)
        {
            Debug.LogError("PlayerData not assigned to UpgradeManager!");
        }
    }

    public bool CanAffordUpgrade(UpgradeData upgrade)
    {
        double currentAmount = GetCurrencyAmount(upgrade.currency);
        double cost = GetUpgradeCost(upgrade);
        return currentAmount >= cost;
    }

    public double GetUpgradeCost(UpgradeData upgrade)
    {
        int currentLevel = playerData.GetUpgradeLevel(upgrade.upgradeName);
        return upgrade.baseCost * Mathf.Pow((float)upgrade.costMultiplier, currentLevel);
    }

    public bool PurchaseUpgrade(UpgradeData upgrade)
    {
        if (!CanAffordUpgrade(upgrade))
        {
            Debug.Log($"Cannot afford {upgrade.upgradeName}!");
            return false;
        }

        double cost = GetUpgradeCost(upgrade);
        SpendCurrency(upgrade.currency, cost);
        playerData.IncrementUpgradeLevel(upgrade.upgradeName);
        ApplyUpgradeEffect(upgrade);

        Debug.Log($"Purchased {upgrade.upgradeName} for {cost} {upgrade.currency}!");
        return true;
    }

    private void ApplyUpgradeEffect(UpgradeData upgrade)
    {
        switch (upgrade.upgradeType)
        {
            case UpgradeType.Click:
                ApplyClickUpgrade(upgrade);
                break;
            case UpgradeType.Passive:
                ApplyPassiveUpgrade(upgrade);
                break;
        }
    }

    private void ApplyClickUpgrade(UpgradeData upgrade)
    {
        switch (upgrade.currency)
        {
            case Currency.GingerBread:
                playerData.gingerBreadClick += upgrade.valueIncrease;
                break;
            case Currency.CandyCane:
                playerData.candyCaneClick += upgrade.valueIncrease;
                break;
            case Currency.Cookie:
                playerData.cookieClick += upgrade.valueIncrease;
                break;
        }
    }

    private void ApplyPassiveUpgrade(UpgradeData upgrade)
    {
        switch (upgrade.currency)
        {
            case Currency.GingerBread:
                playerData.gingerbreadPerSecond += upgrade.valueIncrease;
                break;
            case Currency.CandyCane:
                playerData.candyCanePerSecond += upgrade.valueIncrease;
                break;
            case Currency.Cookie:
                playerData.cookiePerSecond += upgrade.valueIncrease;
                break;
        }
    }

    private double GetCurrencyAmount(Currency currency)
    {
        return GameManager.GetCurrency(currency);
    }

    private void SpendCurrency(Currency currency, double amount)
    {
        GameManager.SpendCurrency(currency, amount);
    }

    // Helper method for UI to check if upgrade is available
    public bool IsUpgradeAvailable(UpgradeData upgrade)
    {
        return CanAffordUpgrade(upgrade);
    }

    // Helper method to get upgrade description for UI
    public string GetUpgradeDescription(UpgradeData upgrade)
    {
        int currentLevel = playerData.GetUpgradeLevel(upgrade.upgradeName);
        double cost = GetUpgradeCost(upgrade);
        
        string effect = upgrade.upgradeType == UpgradeType.Click ? 
            $"Click Power +{upgrade.valueIncrease}" : 
            $"Passive Income +{upgrade.valueIncrease}/sec";
            
        return $"{upgrade.upgradeName} (Lvl {currentLevel})\n{effect}\nCost: {cost} {upgrade.currency}";
    }
}
