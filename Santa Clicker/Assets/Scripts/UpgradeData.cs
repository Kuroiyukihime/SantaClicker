using UnityEngine;
using System.Collections.Generic;

public enum UpgradeEffectType
{
	Click,
	Passive
}

[System.Serializable]
public class UpgradeEffect
{
	public UpgradeEffectType effectType;
	public Currency currency;
	public double valueIncrease;
}

[CreateAssetMenu(fileName = "UpgradeData", menuName = "Scriptable Objects/UpgradeData")]
public class UpgradeData : ScriptableObject
{
	[Header("Identity")]
	public string upgradeName;
	public Sprite Icon;

	[Header("Cost")]
	public Currency costCurrency;
	public double baseCost;
	public double costMultiplier = 1.15;

	[Header("Effects")]
	public List<UpgradeEffect> effects = new List<UpgradeEffect>();
}
