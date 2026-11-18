using System.Collections.Generic;

// Everything that must survive closing the game. Saved as JSON by SaveSystem.
[System.Serializable]
public class SaveData
{
    public int honey;                                   // the currency
    public List<UpgradeLevel> upgrades = new List<UpgradeLevel>();
    public int unlockedLevel = 1;                       // highest level the player may start
    public List<int> bestStars = new List<int>();       // per level, 0 to 3
    public List<float> bestTimes = new List<float>();   // per level, seconds (0 = never won)
    public float musicVolume = 0.8f;                    // 0 to 1
    public float sfxVolume = 0.8f;                      // 0 to 1
}

// One upgrade's level, stored by its id so the order of upgrades can change safely.
[System.Serializable]
public class UpgradeLevel
{
    public string id;
    public int level;
}
