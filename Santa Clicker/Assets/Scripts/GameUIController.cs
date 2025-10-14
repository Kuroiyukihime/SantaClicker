using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
public class GameUIController : MonoBehaviour
{
    [Header("Currency Display")]
    public TextMeshProUGUI gingerBreadText;
    public TextMeshProUGUI candyCaneText;
    public TextMeshProUGUI cookieText;
    
    [Header("Click Power Display")]
	public TextMeshProUGUI gingerBreadClickText;
	public TextMeshProUGUI candyCaneClickText;
	public TextMeshProUGUI cookieClickText;

	[Header("Click Buttons")]
	public Button gingerBreadButton;
	public Button candyCaneButton;
	public Button cookieButton;
    
    [Header("Passive Income Display")]
    public TextMeshProUGUI gingerBreadPerSecondText;
    public TextMeshProUGUI candyCanePerSecondText;
    public TextMeshProUGUI cookiePerSecondText;
    
    [Header("Upgrade System")]
    public Transform upgradeParent;
    public GameObject upgradeButtonPrefab;
    
    [Header("Managers")]
    public PlayerData playerData;
    public UpgradeManager upgradeManager;
    
	private List<GameObject> upgradeButtons = new List<GameObject>();

	// Cached upgrade UI entries to avoid repeated GetComponent calls and string churn
	private class UpgradeEntry
	{
		public Button button;
		public Text text;
		public UpgradeData data;
		public string lastDescription;
		public bool lastInteractable;
	}
	private List<UpgradeEntry> upgradeEntries = new List<UpgradeEntry>();

	// Throttle UI updates to reduce allocations (e.g., 4 times per second)
	[SerializeField] private float uiUpdateIntervalSeconds = 0.25f;
	private float nextUiUpdateTime = 0f;

	// Cached last values for currency and stats to avoid unnecessary text updates
	private double lastGingerbreadAmount = double.MinValue;
	private double lastCandyCaneAmount = double.MinValue;
	private double lastCookieAmount = double.MinValue;

	private double lastGingerbreadClick = double.MinValue;
	private double lastCandyCaneClick = double.MinValue;
	private double lastCookieClick = double.MinValue;

	private double lastGingerbreadPerSec = double.MinValue;
	private double lastCandyCanePerSec = double.MinValue;
	private double lastCookiePerSec = double.MinValue;

	// Reusable label strings to minimize GC
	private const string gingerLabel = "Gingerbread: ";
	private const string candyLabel = "Candy Canes: ";
	private const string cookieLabel = "Cookies: ";
	private const string clickPowerLabel = "Click Power: ";
	private const string perSecondLabel = "Per Second: ";

    void Start()
    {
		// Ensure GameManager has references so clicks can add currency
		if (GameManager.PlayerData == null || GameManager.UpgradeManager == null || GameManager.ClickerManager == null || GameManager.UIController == null)
		{
			var clickerMgr = GameObject.FindObjectOfType<ClickerManager>();
			GameManager.Initialize(playerData, upgradeManager, clickerMgr, this);
		}

        CreateUpgradeButtons();
		// Wire up click buttons if assigned via Inspector
		if (gingerBreadButton != null) gingerBreadButton.onClick.AddListener(OnGingerbreadButtonClicked);
		if (candyCaneButton != null) candyCaneButton.onClick.AddListener(OnCandyCaneButtonClicked);
		if (cookieButton != null) cookieButton.onClick.AddListener(OnCookieButtonClicked);
    }

	void OnDestroy()
	{
		// Unwire button listeners to avoid retained delegates
		if (gingerBreadButton != null) gingerBreadButton.onClick.RemoveListener(OnGingerbreadButtonClicked);
		if (candyCaneButton != null) candyCaneButton.onClick.RemoveListener(OnCandyCaneButtonClicked);
		if (cookieButton != null) cookieButton.onClick.RemoveListener(OnCookieButtonClicked);
		for (int i = 0; i < upgradeEntries.Count; i++)
		{
			var entry = upgradeEntries[i];
			if (entry != null && entry.button != null && entry.data != null)
			{
				// Remove all listeners to be safe; these were added with lambdas
				entry.button.onClick.RemoveAllListeners();
			}
		}
	}

    void Update()
    {
		if (Time.time >= nextUiUpdateTime)
		{
			UpdateCurrencyDisplay();
			UpdateStatsDisplay();
			UpdateUpgradeButtons();
			nextUiUpdateTime = Time.time + uiUpdateIntervalSeconds;
		}
    }

	public void ForceRefresh()
	{
		// Set next update time to now and invalidate last values so UI updates immediately
		nextUiUpdateTime = 0f;
		lastGingerbreadAmount = double.MinValue;
		lastCandyCaneAmount = double.MinValue;
		lastCookieAmount = double.MinValue;
		lastGingerbreadClick = double.MinValue;
		lastCandyCaneClick = double.MinValue;
		lastCookieClick = double.MinValue;
		lastGingerbreadPerSec = double.MinValue;
		lastCandyCanePerSec = double.MinValue;
		lastCookiePerSec = double.MinValue;
		UpdateCurrencyDisplay();
		UpdateStatsDisplay();
		UpdateUpgradeButtons();
	}

	void UpdateCurrencyDisplay()
    {
		if (playerData == null) return;
		if (gingerBreadText != null && playerData.gingerBreadAmount != lastGingerbreadAmount)
		{
			gingerBreadText.SetText(gingerLabel + FormatNumber(playerData.gingerBreadAmount));
			lastGingerbreadAmount = playerData.gingerBreadAmount;
		}
		if (candyCaneText != null && playerData.candyCaneAmount != lastCandyCaneAmount)
		{
			candyCaneText.SetText(candyLabel + FormatNumber(playerData.candyCaneAmount));
			lastCandyCaneAmount = playerData.candyCaneAmount;
		}
		if (cookieText != null && playerData.cookieAmount != lastCookieAmount)
		{
			cookieText.SetText(cookieLabel + FormatNumber(playerData.cookieAmount));
			lastCookieAmount = playerData.cookieAmount;
		}
    }

	void UpdateStatsDisplay()
    {
        if (playerData == null) return;
		if (gingerBreadClickText != null && playerData.gingerBreadClick != lastGingerbreadClick)
		{
			gingerBreadClickText.SetText(clickPowerLabel + playerData.gingerBreadClick + "x");
			lastGingerbreadClick = playerData.gingerBreadClick;
		}
		if (candyCaneClickText != null && playerData.candyCaneClick != lastCandyCaneClick)
		{
			candyCaneClickText.SetText(clickPowerLabel + playerData.candyCaneClick + "x");
			lastCandyCaneClick = playerData.candyCaneClick;
		}
		if (cookieClickText != null && playerData.cookieClick != lastCookieClick)
		{
			cookieClickText.SetText(clickPowerLabel + playerData.cookieClick + "x");
			lastCookieClick = playerData.cookieClick;
		}
		if (gingerBreadPerSecondText != null && playerData.gingerbreadPerSecond != lastGingerbreadPerSec)
		{
			gingerBreadPerSecondText.SetText(perSecondLabel + playerData.gingerbreadPerSecond + "/sec");
			lastGingerbreadPerSec = playerData.gingerbreadPerSecond;
		}
		if (candyCanePerSecondText != null && playerData.candyCanePerSecond != lastCandyCanePerSec)
		{
			candyCanePerSecondText.SetText(perSecondLabel + playerData.candyCanePerSecond + "/sec");
			lastCandyCanePerSec = playerData.candyCanePerSecond;
		}
		if (cookiePerSecondText != null && playerData.cookiePerSecond != lastCookiePerSec)
		{
			cookiePerSecondText.SetText(perSecondLabel + playerData.cookiePerSecond + "/sec");
			lastCookiePerSec = playerData.cookiePerSecond;
		}
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
			Text buttonText = buttonObj.GetComponentInChildren<Text>();
			if (button != null)
			{
				button.onClick.AddListener(() => TryPurchaseUpgrade(upgrade));
			}
			if (buttonText != null)
			{
				buttonText.text = upgrade.upgradeName;
			}
			// Cache entry
			upgradeEntries.Add(new UpgradeEntry
			{
				button = button,
				text = buttonText,
				data = upgrade,
				lastDescription = null,
				lastInteractable = false
			});
        }
    }

	void UpdateUpgradeButtons()
    {
        if (upgradeManager == null) return;
		
		for (int i = 0; i < upgradeEntries.Count; i++)
		{
			var entry = upgradeEntries[i];
			if (entry == null || entry.data == null) continue;
			
			bool interactable = upgradeManager.IsUpgradeAvailable(entry.data);
			if (entry.button != null && interactable != entry.lastInteractable)
			{
				entry.button.interactable = interactable;
				entry.lastInteractable = interactable;
			}
			
			string desc = upgradeManager.GetUpgradeDescription(entry.data);
			if (entry.text != null && !string.Equals(desc, entry.lastDescription))
			{
				entry.text.text = desc;
				entry.lastDescription = desc;
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

	// UI Button handlers for click actions
	public void OnGingerbreadButtonClicked()
	{
		if (GameManager.ClickerManager != null)
		{
			GameManager.ClickerManager.HandleClick(GameManager.ClickerManager.gingerbreadName);
		}
	}

	public void OnCandyCaneButtonClicked()
	{
		if (GameManager.ClickerManager != null)
		{
			GameManager.ClickerManager.HandleClick(GameManager.ClickerManager.candyCaneName);
		}
	}

	public void OnCookieButtonClicked()
	{
		if (GameManager.ClickerManager != null)
		{
			GameManager.ClickerManager.HandleClick(GameManager.ClickerManager.cookieName);
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
