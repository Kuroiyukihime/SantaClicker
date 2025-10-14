using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameUIController : MonoBehaviour
{
    [Header("Currency Display")]
    public Text gingerBreadText;
    public Text candyCaneText;
    public Text cookieText;
    
    [Header("Click Power Display")]
    public Text gingerBreadClickText;
    public Text candyCaneClickText;
    public Text cookieClickText;
    
    [Header("Passive Income Display")]
    public Text gingerBreadPerSecondText;
    public Text candyCanePerSecondText;
    public Text cookiePerSecondText;
    
    [Header("Upgrade System")]
    public Transform upgradeParent;
    public GameObject upgradeButtonPrefab;
    
    [Header("Managers")]
    public PlayerData playerData;
    public UpgradeManager upgradeManager;
    
    private List<GameObject> upgradeButtons = new List<GameObject>();

    void Start()
    {
        CreateUpgradeButtons();
    }

    void Update()
    {
        UpdateCurrencyDisplay();
        UpdateStatsDisplay();
        UpdateUpgradeButtons();
    }

    void UpdateCurrencyDisplay()
    {
        if (gingerBreadText != null)
            gingerBreadText.text = $"Gingerbread: {FormatNumber(playerData.gingerBreadAmount)}";
            
        if (candyCaneText != null)
            candyCaneText.text = $"Candy Canes: {FormatNumber(playerData.candyCaneAmount)}";
            
        if (cookieText != null)
            cookieText.text = $"Cookies: {FormatNumber(playerData.cookieAmount)}";
    }

    void UpdateStatsDisplay()
    {
        if (playerData == null) return;
        
        if (gingerBreadClickText != null)
            gingerBreadClickText.text = $"Click Power: {playerData.gingerBreadClick}x";
            
        if (candyCaneClickText != null)
            candyCaneClickText.text = $"Click Power: {playerData.candyCaneClick}x";
            
        if (cookieClickText != null)
            cookieClickText.text = $"Click Power: {playerData.cookieClick}x";
            
        if (gingerBreadPerSecondText != null)
            gingerBreadPerSecondText.text = $"Per Second: {playerData.gingerbreadPerSecond}/sec";
            
        if (candyCanePerSecondText != null)
            candyCanePerSecondText.text = $"Per Second: {playerData.candyCanePerSecond}/sec";
            
        if (cookiePerSecondText != null)
            cookiePerSecondText.text = $"Per Second: {playerData.cookiePerSecond}/sec";
    }

    void CreateUpgradeButtons()
    {
        if (upgradeManager == null || upgradeButtonPrefab == null || upgradeParent == null) return;
        
        foreach (var upgrade in upgradeManager.availableUpgrades)
        {
            GameObject buttonObj = Instantiate(upgradeButtonPrefab, upgradeParent);
            upgradeButtons.Add(buttonObj);
            
            // Set up button
            Button button = buttonObj.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => TryPurchaseUpgrade(upgrade));
            }
            
            // Set up text (assuming the prefab has a Text component)
            Text buttonText = buttonObj.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = upgrade.upgradeName;
            }
        }
    }

    void UpdateUpgradeButtons()
    {
        if (upgradeManager == null) return;
        
        for (int i = 0; i < upgradeManager.availableUpgrades.Count && i < upgradeButtons.Count; i++)
        {
            var upgrade = upgradeManager.availableUpgrades[i];
            var button = upgradeButtons[i];
            
            Button buttonComponent = button.GetComponent<Button>();
            if (buttonComponent != null)
            {
                buttonComponent.interactable = upgradeManager.IsUpgradeAvailable(upgrade);
            }
            
            Text buttonText = button.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = upgradeManager.GetUpgradeDescription(upgrade);
            }
        }
    }

    void TryPurchaseUpgrade(UpgradeData upgrade)
    {
        if (upgradeManager.PurchaseUpgrade(upgrade))
        {
            Debug.Log($"Successfully purchased {upgrade.upgradeName}!");
        }
    }

    string FormatNumber(double number)
    {
        if (number >= 1000000)
            return $"{number / 1000000:F1}M";
        else if (number >= 1000)
            return $"{number / 1000:F1}K";
        else
            return $"{number:F0}";
    }
}
