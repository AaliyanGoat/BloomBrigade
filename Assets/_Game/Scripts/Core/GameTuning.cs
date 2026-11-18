using UnityEngine;

// All the numbers that tune how the game feels, in one asset (no magic numbers in code).
[CreateAssetMenu(fileName = "GameTuning", menuName = "Bloom Brigade/Game Tuning")]
public class GameTuning : ScriptableObject
{
    [Header("Cannon (card M1)")]
    public float dragSensitivity = 0.012f;   // units moved per pixel dragged
    public float maxSlideSpeed = 14f;        // units per second
    public float minX = -3.5f;
    public float maxX = 3.5f;
    public float maxTilt = 10f;              // degrees the cannon leans while moving
    public float tiltSmoothing = 12f;        // how fast the tilt follows

    [Header("Bees (card M2)")]
    public float fireRate = 4f;              // bees per second
    public float beeSpeed = 6f;              // units per second
    public int beeDamage = 1;                // damage to the nest
    public int maxActiveBees = 400;
    public int poolPrewarm = 200;
    public float beeDespawnZ = 41f;          // bees past this line go back to the pool

    [Header("Wasps (card M4)")]
    public float waspDespawnZ = -2f;         // safety line behind the hive

    [Header("Honey (section 4.4)")]
    public int honeyPerQuarter = 10;         // each quarter of nest health lost
    public int winBonusPerLevel = 30;        // x level number on a win
    public int honeyDropValue = 10;
}
