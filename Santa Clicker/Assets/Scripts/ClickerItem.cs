using UnityEngine;


public class ClickerItem : MonoBehaviour
{
    [Header("Clicker Item Settings")]
    public string itemName;
    public Currency currency;
    public double perClick;
    public double persecond; // Base passive income (usually 0, upgrades override this)
    
    /// <summary>
    /// Handles click events - applies click multiplier and adds currency
    /// </summary>
    public void OnClick()
    {
        double clickMultiplier = GameManager.GetClickMultiplier(currency);
        double gain = perClick * clickMultiplier;
        GameManager.AddCurrency(currency, gain);
        // Force immediate UI update on click
        if (GameManager.UIController != null)
        {
            GameManager.UIController.ForceRefresh();
        }
    }

    /// <summary>
    /// Generates passive income - uses PlayerData passive income rates from upgrades
    /// </summary>
    public void Generate(double deltaTime) 
    {
        double passiveIncome = GameManager.GetPassiveIncome(currency);
        double gain = passiveIncome * deltaTime;
        GameManager.AddCurrency(currency, gain);
    }
}
