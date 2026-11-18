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

    public List<FlowerSetup> flowers = new List<FlowerSetup>();
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
