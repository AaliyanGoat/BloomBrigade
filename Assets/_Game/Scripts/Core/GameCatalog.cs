using System.Collections.Generic;
using UnityEngine;

// Lists every level and upgrade in the game, plus the base tuning.
// Lives in a Resources folder so both scenes (Menu and Game) can load the same asset.
[CreateAssetMenu(fileName = "GameCatalog", menuName = "Bloom Brigade/Game Catalog")]
public class GameCatalog : ScriptableObject
{
    public GameTuning tuning;
    public List<LevelData> levels = new List<LevelData>();

    [Header("Upgrades (shop order)")]
    public UpgradeData fireRate;
    public UpgradeData hiveHealth;
    public UpgradeData bloom;
    public UpgradeData beeSpeed;

    [Header("Evolution: the hive cannon changes look at these Fire rate levels")]
    public int[] evolutionLevels = { 3, 6, 9 };

    private static GameCatalog instance;

    // Loaded once from Resources/GameCatalog and then reused.
    public static GameCatalog Instance
    {
        get
        {
            if (instance == null) instance = Resources.Load<GameCatalog>("GameCatalog");
            return instance;
        }
    }

    public IEnumerable<UpgradeData> Upgrades
    {
        get { yield return fireRate; yield return hiveHealth; yield return bloom; yield return beeSpeed; }
    }
}
