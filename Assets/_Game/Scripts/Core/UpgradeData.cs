using UnityEngine;

// One asset per shop upgrade (Fire rate, Hive health, Bloom, Bee speed).
// Balance the economy by editing these assets, not code.
[CreateAssetMenu(fileName = "Upgrade", menuName = "Bloom Brigade/Upgrade")]
public class UpgradeData : ScriptableObject
{
    public string id = "fireRate";              // key used in the save file
    public string upgradeName = "Fire rate";
    [TextArea] public string description = "More bees per second";
    public string unit = "bees/s";
    public int baseCost = 50;
    public float costGrowth = 1.5f;             // +50% per level
    public float valuePerLevel = 0.5f;          // added to the base stat per level
    public int maxLevel = 10;

    // Cost to go from "level" to "level + 1": baseCost x costGrowth ^ level.
    public int CostAt(int level) => Mathf.RoundToInt(baseCost * Mathf.Pow(costGrowth, level));

    // Total bonus this upgrade gives at a level.
    public float BonusAt(int level) => valuePerLevel * level;
}
