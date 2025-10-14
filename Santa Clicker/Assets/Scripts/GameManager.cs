using UnityEngine;

public static class GameManager
{
    // Static references to main game components
    public static PlayerData PlayerData { get; private set; }
    public static UpgradeManager UpgradeManager { get; private set; }
    public static ClickerManager ClickerManager { get; private set; }
    public static GameUIController UIController { get; private set; }

    // Initialize game managers
    public static void Initialize(PlayerData playerData, UpgradeManager upgradeManager, ClickerManager clickerManager, GameUIController uiController)
    {
        PlayerData = playerData;
        UpgradeManager = upgradeManager;
        ClickerManager = clickerManager;
        UIController = uiController;
    }

    // Static access to player data
    public static double GetCurrency(Currency currency)
    {
        if (PlayerData == null) return 0;
        
        switch (currency)
        {
            case Currency.GingerBread:
                return PlayerData.gingerBreadAmount;
            case Currency.CandyCane:
                return PlayerData.candyCaneAmount;
            case Currency.Cookie:
                return PlayerData.cookieAmount;
            default:
                return 0;
        }
    }

    public static void AddCurrency(Currency currency, double amount)
    {
        if (PlayerData == null) return;
        
        switch (currency)
        {
            case Currency.GingerBread:
                PlayerData.gingerBreadAmount += amount;
                break;
            case Currency.CandyCane:
                PlayerData.candyCaneAmount += amount;
                break;
            case Currency.Cookie:
                PlayerData.cookieAmount += amount;
                break;
        }
    }

    public static void SpendCurrency(Currency currency, double amount)
    {
        if (PlayerData == null) return;
        
        switch (currency)
        {
            case Currency.GingerBread:
                PlayerData.gingerBreadAmount -= amount;
                break;
            case Currency.CandyCane:
                PlayerData.candyCaneAmount -= amount;
                break;
            case Currency.Cookie:
                PlayerData.cookieAmount -= amount;
                break;
        }
    }

    public static double GetClickMultiplier(Currency currency)
    {
        if (PlayerData == null) return 1;
        
        switch (currency)
        {
            case Currency.GingerBread:
                return PlayerData.gingerBreadClick;
            case Currency.CandyCane:
                return PlayerData.candyCaneClick;
            case Currency.Cookie:
                return PlayerData.cookieClick;
            default:
                return 1;
        }
    }

    public static double GetPassiveIncome(Currency currency)
    {
        if (PlayerData == null) return 0;
        
        switch (currency)
        {
            case Currency.GingerBread:
                return PlayerData.gingerbreadPerSecond;
            case Currency.CandyCane:
                return PlayerData.candyCanePerSecond;
            case Currency.Cookie:
                return PlayerData.cookiePerSecond;
            default:
                return 0;
        }
    }
}
