using System;
using System.IO;
using UnityEngine;

// Loads and saves SaveData as a JSON file in Application.persistentDataPath.
// It saves straight after every change (buying, winning, moving a slider), not only on quit,
// because Android often closes apps without calling OnApplicationQuit.
public static class SaveSystem
{
    private const string FileName = "bloombrigade_save.json";

    private static SaveData data;
    public static event Action OnChanged;   // the UI listens to refresh honey, stars and so on

    private static string FilePath => Path.Combine(Application.persistentDataPath, FileName);

    // The current save. Loaded from disk the first time it is used.
    public static SaveData Data
    {
        get
        {
            if (data == null) Load();
            return data;
        }
    }

    // Read the file, or start a new save if there is none or it is broken.
    public static void Load()
    {
        try
        {
            data = File.Exists(FilePath) ? JsonUtility.FromJson<SaveData>(File.ReadAllText(FilePath)) : null;
        }
        catch (Exception e)
        {
            Debug.LogWarning("Save file could not be read, starting fresh: " + e.Message);
            data = null;
        }
        if (data == null) data = new SaveData();
    }

    // Write the current data to disk and tell listeners.
    public static void Save()
    {
        File.WriteAllText(FilePath, JsonUtility.ToJson(Data, true));
        OnChanged?.Invoke();
    }

    // Delete all progress (Settings > Reset progress).
    public static void ResetAll()
    {
        data = new SaveData();
        Save();
    }

    // ---------- helpers so other scripts never touch the lists directly ----------

    public static int GetUpgradeLevel(string id)
    {
        foreach (UpgradeLevel u in Data.upgrades)
            if (u.id == id) return u.level;
        return 0;
    }

    public static void SetUpgradeLevel(string id, int level)
    {
        foreach (UpgradeLevel u in Data.upgrades)
            if (u.id == id) { u.level = level; return; }
        Data.upgrades.Add(new UpgradeLevel { id = id, level = level });
    }

    public static int GetStars(int levelIndex) => levelIndex < Data.bestStars.Count ? Data.bestStars[levelIndex] : 0;
    public static float GetBestTime(int levelIndex) => levelIndex < Data.bestTimes.Count ? Data.bestTimes[levelIndex] : 0f;

    // Keep the best stars and the best (lowest) time for a level.
    public static void RecordResult(int levelIndex, int stars, float time)
    {
        while (Data.bestStars.Count <= levelIndex) Data.bestStars.Add(0);
        while (Data.bestTimes.Count <= levelIndex) Data.bestTimes.Add(0f);
        Data.bestStars[levelIndex] = Mathf.Max(Data.bestStars[levelIndex], stars);
        if (Data.bestTimes[levelIndex] <= 0f || time < Data.bestTimes[levelIndex]) Data.bestTimes[levelIndex] = time;
    }
}
