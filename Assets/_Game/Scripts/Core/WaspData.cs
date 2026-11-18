using UnityEngine;

// One asset per wasp type (Worker now; Soldier, Hornet and Giant later).
[CreateAssetMenu(fileName = "WaspData", menuName = "Bloom Brigade/Wasp Data")]
public class WaspData : ScriptableObject
{
    public string displayName = "Worker";
    public int health = 1;          // bees needed to stop it
    public float speed = 2.5f;      // units per second down the lane
    public int hiveDamage = 5;      // damage when it reaches the hive
    public Wasp prefab;
}
