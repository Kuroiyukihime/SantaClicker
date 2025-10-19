using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UpgradeItemUI : MonoBehaviour
{
	[Header("Main UI")]
	public Image iconImage;
	public TextMeshProUGUI nameText;
	public Transform effectsContainer;
	public GameObject effectEntryPrefab; // prefab with Image + TMP text
	public TextMeshProUGUI costText;
	public Image costIconImage; // optional: show currency icon for cost
	public Button buyButton;
	public Button rootButton; // optional: whole item is a button

	private Button effectiveButton; // resolved button to use (buyButton or root)

	[Header("Level UI")]
	public TextMeshProUGUI levelText;

	[Header("Affordability Visuals")]
	public CanvasGroup canvasGroup; // optional; controls overall dimming
	[Range(0f,1f)] public float notAffordableAlpha = 0.5f;
	[Range(0f,1f)] public float affordableAlpha = 1f;

	[Header("Affordability Text Colors")]
	public Color affordableTextColor = Color.white;
	public Color notAffordableTextColor = Color.red;

	private readonly List<TextMeshProUGUI> allTexts = new List<TextMeshProUGUI>();

	[Header("Effect Icons")]
	public Sprite gingerbreadIcon;
	public Sprite candyCaneIcon;
	public Sprite cookieIcon;

	private UpgradeData upgradeData;
	private UpgradeManager upgradeManager;
	private readonly List<GameObject> spawnedEffectEntries = new List<GameObject>();

	public void Setup(UpgradeData data, UpgradeManager manager)
	{
		upgradeData = data;
		upgradeManager = manager;
		if (iconImage != null) iconImage.sprite = data.Icon;
		if (nameText != null) nameText.SetText(data.upgradeName);
		// cache all TMP texts under this item for fast recolor
		allTexts.Clear();
		GetComponentsInChildren<TextMeshProUGUI>(true, allTexts);
		BuildEffectEntries();
		WireButton();
		Refresh();
	}

	public void Refresh()
	{
		if (upgradeData == null || upgradeManager == null) return;
		double cost = upgradeManager.GetUpgradeCost(upgradeData);
		bool canAfford = upgradeManager.CanAffordUpgrade(upgradeData);
		if (costText != null) costText.SetText("Cost  " + FormatNumber(cost));
		if (costIconImage != null) costIconImage.sprite = GetIconForCurrency(upgradeData.costCurrency);
		if (effectiveButton != null) effectiveButton.interactable = canAfford;
		if (levelText != null) levelText.SetText("Lvl " + upgradeManager.GetUpgradeLevel(upgradeData));
		if (canvasGroup != null)
		{
			canvasGroup.alpha = canAfford ? affordableAlpha : notAffordableAlpha;
		}
		// recolor all texts based on affordability
		var color = canAfford ? affordableTextColor : notAffordableTextColor;
		for (int i = 0; i < allTexts.Count; i++)
		{
			if (allTexts[i] != null) allTexts[i].color = color;
		}
	}

	private void BuildEffectEntries()
	{
		// Clear old
		for (int i = 0; i < spawnedEffectEntries.Count; i++)
		{
			if (spawnedEffectEntries[i] != null)
			{
				Destroy(spawnedEffectEntries[i]);
			}
		}
		spawnedEffectEntries.Clear();

		if (effectsContainer == null || effectEntryPrefab == null || upgradeData == null || upgradeData.effects == null) return;

		for (int i = 0; i < upgradeData.effects.Count; i++)
		{
			var effect = upgradeData.effects[i];
			if (effect == null) continue;
			GameObject entry = Instantiate(effectEntryPrefab, effectsContainer);
			spawnedEffectEntries.Add(entry);
			// Expect: Image + TMP under entry
			var img = entry.GetComponentInChildren<Image>();
			var txt = entry.GetComponentInChildren<TextMeshProUGUI>();
			if (img != null) img.sprite = GetIconForCurrency(effect.currency);
			if (txt != null)
			{
				if (effect.effectType == UpgradeEffectType.Click)
				{
					txt.SetText("+" + effect.valueIncrease + " click");
				}
				else
				{
					txt.SetText("+" + effect.valueIncrease + "/sec");
				}
			}
		}
	}

	private void WireButton()
	{
		// Resolve which button to use: explicit buyButton, else rootButton, else a Button on root, else any child Button
		effectiveButton = buyButton != null ? buyButton : (rootButton != null ? rootButton : GetComponent<Button>());
		if (effectiveButton == null) effectiveButton = GetComponentInChildren<Button>();
		if (effectiveButton == null) return;

		System.Action purchase = () =>
		{
			if (upgradeManager != null && upgradeData != null)
			{
				if (upgradeManager.PurchaseUpgrade(upgradeData))
				{
					Refresh();
					if (GameManager.UIController != null)
					{
						GameManager.UIController.ForceRefresh();
					}
				}
			}
		};
		effectiveButton.onClick.RemoveAllListeners();
		effectiveButton.onClick.AddListener(() => purchase());
	}

	private Sprite GetIconForCurrency(Currency currency)
	{
		switch (currency)
		{
			case Currency.GingerBread: return gingerbreadIcon;
			case Currency.CandyCane: return candyCaneIcon;
			case Currency.Cookie: return cookieIcon;
			default: return null;
		}
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


