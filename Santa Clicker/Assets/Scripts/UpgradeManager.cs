using UnityEngine;
using System.Collections.Generic;

public class UpgradeManager : MonoBehaviour
{
    public PlayerData playerData;
    public List<UpgradeData> availableUpgrades = new List<UpgradeData>();
    [Header("Debugging")]
    public bool debugEffects = false;

	private PlayerData ActivePlayerData
	{
		get { return GameManager.PlayerData != null ? GameManager.PlayerData : playerData; }
	}

    void Start()
    {
        if (playerData == null)
        {
            Debug.LogError("PlayerData not assigned to UpgradeManager!");
        }
    }

    public bool CanAffordUpgrade(UpgradeData upgrade)
    {
        double currentAmount = GetCurrencyAmount(upgrade.costCurrency);
        double cost = GetUpgradeCost(upgrade);
        return currentAmount >= cost;
    }

    public double GetUpgradeCost(UpgradeData upgrade)
    {
        int currentLevel = ActivePlayerData != null ? ActivePlayerData.GetUpgradeLevel(upgrade.upgradeName) : 0;
        double rawCost = upgrade.baseCost * Mathf.Pow((float)upgrade.costMultiplier, currentLevel);
        return System.Math.Round(rawCost, 0, System.MidpointRounding.AwayFromZero);
    }

    public bool PurchaseUpgrade(UpgradeData upgrade)
    {
        if (!CanAffordUpgrade(upgrade))
        {
            Debug.Log($"Cannot afford {upgrade.upgradeName}!");
            return false;
        }

        double cost = GetUpgradeCost(upgrade);
        SpendCurrency(upgrade.costCurrency, cost);
        if (ActivePlayerData != null) ActivePlayerData.IncrementUpgradeLevel(upgrade.upgradeName);
        ApplyUpgradeEffects(upgrade);

        Debug.Log($"Purchased {upgrade.upgradeName} for {cost} {upgrade.costCurrency}!");
        return true;
    }

    private void ApplyUpgradeEffects(UpgradeData upgrade)
    {
        if (upgrade.effects == null) return;
        var pd = ActivePlayerData;
        if (pd == null) return;
        foreach (var effect in upgrade.effects)
        {
            if (effect == null) continue;
            if (effect.effectType == UpgradeEffectType.Click)
            {
                switch (effect.currency)
                {
                    case Currency.GingerBread:
                        pd.gingerBreadClick += effect.valueIncrease;
                        if (debugEffects) Debug.Log($"[Upgrade] {upgrade.upgradeName}: Click +{effect.valueIncrease} GingerBread => {pd.gingerBreadClick}");
                        break;
                    case Currency.CandyCane:
                        pd.candyCaneClick += effect.valueIncrease;
                        if (debugEffects) Debug.Log($"[Upgrade] {upgrade.upgradeName}: Click +{effect.valueIncrease} CandyCane => {pd.candyCaneClick}");
                        break;
                    case Currency.Cookie:
                        pd.cookieClick += effect.valueIncrease;
                        if (debugEffects) Debug.Log($"[Upgrade] {upgrade.upgradeName}: Click +{effect.valueIncrease} Cookie => {pd.cookieClick}");
                        break;
                }
            }
            else if (effect.effectType == UpgradeEffectType.Passive)
            {
                switch (effect.currency)
                {
                    case Currency.GingerBread:
                        pd.gingerbreadPerSecond += effect.valueIncrease;
                        if (debugEffects) Debug.Log($"[Upgrade] {upgrade.upgradeName}: Passive +{effect.valueIncrease}/sec GingerBread => {pd.gingerbreadPerSecond}");
                        break;
                    case Currency.CandyCane:
                        pd.candyCanePerSecond += effect.valueIncrease;
                        if (debugEffects) Debug.Log($"[Upgrade] {upgrade.upgradeName}: Passive +{effect.valueIncrease}/sec CandyCane => {pd.candyCanePerSecond}");
                        break;
                    case Currency.Cookie:
                        pd.cookiePerSecond += effect.valueIncrease;
                        if (debugEffects) Debug.Log($"[Upgrade] {upgrade.upgradeName}: Passive +{effect.valueIncrease}/sec Cookie => {pd.cookiePerSecond}");
                        break;
                }
            }
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
        int currentLevel = ActivePlayerData != null ? ActivePlayerData.GetUpgradeLevel(upgrade.upgradeName) : 0;
        double cost = GetUpgradeCost(upgrade);

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.Append(upgrade.upgradeName).Append(" (Lvl ").Append(currentLevel).Append(")\n");
        if (upgrade.effects != null)
        {
            for (int i = 0; i < upgrade.effects.Count; i++)
            {
                var e = upgrade.effects[i];
                if (e == null) continue;
                if (e.effectType == UpgradeEffectType.Click)
                {
                    sb.Append("Click Power +").Append(e.valueIncrease).Append(" (" ).Append(e.currency).Append(")\n");
                }
                else if (e.effectType == UpgradeEffectType.Passive)
                {
                    sb.Append("Passive Income +").Append(e.valueIncrease).Append("/sec (").Append(e.currency).Append(")\n");
                }
            }
        }
        sb.Append("Cost: ").Append(FormatNumber(cost)).Append(' ').Append(upgrade.costCurrency);
        return sb.ToString();
    }

	public int GetUpgradeLevel(UpgradeData upgrade)
	{
		return ActivePlayerData != null ? ActivePlayerData.GetUpgradeLevel(upgrade.upgradeName) : 0;
	}

	private string FormatNumber(double number)
	{
		if (number >= 1000000)
			return (number / 1000000d).ToString("F1") + "M";
		if (number >= 1000)
			return (number / 1000d).ToString("F1") + "K";
		return number.ToString("F0");
	}
}
