using UnityEngine;

// The ONE place that combines base tuning with bought upgrades (spec section 4.5),
// and the honey bank. Gameplay scripts read their numbers from here.
public static class PlayerStats
{
    private static GameCatalog Catalog => GameCatalog.Instance;

    public static int Honey => SaveSystem.Data.honey;

    public static int LevelOf(UpgradeData upgrade) => SaveSystem.GetUpgradeLevel(upgrade.id);

    // ---------- stats used in play ----------
    public static float FireRate => Catalog.tuning.fireRate + Catalog.fireRate.BonusAt(LevelOf(Catalog.fireRate));
    public static float BeeSpeed => Catalog.tuning.beeSpeed + Catalog.beeSpeed.BonusAt(LevelOf(Catalog.beeSpeed));
    public static int HiveHealthBonus => Mathf.RoundToInt(Catalog.hiveHealth.BonusAt(LevelOf(Catalog.hiveHealth)));
    public static int BonusPetals => Mathf.RoundToInt(Catalog.bloom.BonusAt(LevelOf(Catalog.bloom)));

    // Which hive look to show: 0 = basic, 1, 2, 3 = evolved at each evolution level.
    public static int EvolutionStage
    {
        get
        {
            int level = LevelOf(Catalog.fireRate), stage = 0;
            foreach (int needed in Catalog.evolutionLevels) if (level >= needed) stage++;
            return stage;
        }
    }

    // ---------- honey ----------
    public static void AddHoney(int amount)
    {
        SaveSystem.Data.honey += amount;
        SaveSystem.Save();
    }

    public static bool IsMaxed(UpgradeData upgrade) => LevelOf(upgrade) >= upgrade.maxLevel;
    public static int NextCost(UpgradeData upgrade) => upgrade.CostAt(LevelOf(upgrade));
    public static bool CanBuy(UpgradeData upgrade) => !IsMaxed(upgrade) && Honey >= NextCost(upgrade);

    // Pay for one level of an upgrade. Returns false if it is maxed or too expensive.
    public static bool Buy(UpgradeData upgrade)
    {
        if (!CanBuy(upgrade)) return false;
        SaveSystem.Data.honey -= NextCost(upgrade);
        SaveSystem.SetUpgradeLevel(upgrade.id, LevelOf(upgrade) + 1);
        SaveSystem.Save();
        return true;
    }
}
