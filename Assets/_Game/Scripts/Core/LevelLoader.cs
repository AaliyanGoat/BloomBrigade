using System.Collections.Generic;
using UnityEngine;

// Places the flowers, rocks and honey drop listed in a LevelData asset.
// Runs once when the scene starts, so Instantiate here is fine (not during play).
public class LevelLoader : MonoBehaviour
{
    [SerializeField] private FlowerGate flowerPrefab;
    [SerializeField] private GameObject rockPrefab;
    [SerializeField] private HoneyDrop honeyDropPrefab;
    [SerializeField] private BeeSpawner beeSpawner;     // flowers ask it for bonus bees
    [SerializeField] private HoneyCounter honeyCounter; // the honey drop pays into it

    private readonly List<FlowerGate> flowers = new List<FlowerGate>();
    private HoneyDrop honeyDrop;

    // Creates every object the level asks for, as children of this object.
    public void Build(LevelData level)
    {
        foreach (FlowerSetup setup in level.flowers)
        {
            FlowerGate flower = Instantiate(flowerPrefab, setup.position, Quaternion.identity, transform);
            flower.Setup(setup, beeSpawner);
            flowers.Add(flower);
        }

        foreach (Vector3 rockPosition in level.rocks)
            Instantiate(rockPrefab, rockPosition, Quaternion.identity, transform);

        if (level.hasHoneyDrop)
        {
            honeyDrop = Instantiate(honeyDropPrefab, level.honeyDropPosition, Quaternion.identity, transform);
            honeyDrop.Setup(honeyCounter);
        }
    }

    // Reopens every flower with full petals and puts the honey drop back.
    public void ResetForNewRun()
    {
        foreach (FlowerGate flower in flowers)
            flower.ResetForNewRun();

        if (honeyDrop != null)
            honeyDrop.ResetForNewRun();
    }
}
