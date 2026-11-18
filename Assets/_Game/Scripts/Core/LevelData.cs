using System.Collections.Generic;
using UnityEngine;

// One asset per level: base health, what sits on the lane, and the wasp waves.
[CreateAssetMenu(fileName = "LevelData", menuName = "Bloom Brigade/Level Data")]
public class LevelData : ScriptableObject
{
    public int levelNumber = 1;
    public string levelName = "Meadow";
    public int nestHealth = 100;
    public int hiveHealth = 100;
    public float targetTime = 45f;
    [Range(0f, 1f)] public float threeStarHive = 0.8f;   // hive health share needed for the 3rd star

    public List<FlowerSetup> flowers = new List<FlowerSetup>();
    public List<ThornSetup> thorns = new List<ThornSetup>();
    public List<Vector3> rocks = new List<Vector3>();

    public bool hasHoneyDrop = true;
    public Vector3 honeyDropPosition = new Vector3(2.5f, 0.5f, 8f);

    public List<WaveSetup> waves = new List<WaveSetup>();
}

public enum GateType { Multiply, Add }

// Where one flower goes and how it behaves.
[System.Serializable]
public class FlowerSetup
{
    public GateType type = GateType.Multiply;
    public int value = 2;              // x2 or +5
    public int petalsMax = 20;
    public bool neverWilts = false;
    public Vector3 position = new Vector3(-2f, 0f, 14f);
    public float slideRange = 0f;      // 0 = does not move; 2 = slides 2 units left and right
    public float slideSpeed = 1.5f;    // units per second
}

// A thorn gate removes every second bee that passes. It can slide like a moving flower.
[System.Serializable]
public class ThornSetup
{
    public Vector3 position = new Vector3(0f, 0f, 10f);
    public float slideRange = 2.5f;
    public float slideSpeed = 2f;
}

// A repeating wave: "count" wasps every "interval" seconds, starting after "firstDelay".
[System.Serializable]
public class WaveSetup
{
    public WaspData wasp;
    public int count = 3;
    public float interval = 4f;
    public float firstDelay = 2f;
}
