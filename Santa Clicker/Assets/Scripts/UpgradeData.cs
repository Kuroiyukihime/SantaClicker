using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeData", menuName = "Scriptable Objects/UpgradeData")]
public class UpgradeData : ScriptableObject
{
    public string upgradeName;
    public Currency currency;
    public UpgradeType upgradeType;
    public double baseCost;
    public double costMultiplier;
    public double valueIncrease;

    public Sprite Icon;
}
