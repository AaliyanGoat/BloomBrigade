using System.Collections.Generic;
using UnityEngine;

// Places the flowers, thorn gates, rocks and honey drop listed in a LevelData asset.
// Runs once when the scene starts, so Instantiate here is fine (not during play).
public class LevelLoader : MonoBehaviour
{
    [SerializeField] private FlowerGate flowerPrefab;
    [SerializeField] private ThornGate thornPrefab;
    [SerializeField] private GameObject rockPrefab;
    [SerializeField] private HoneyDrop honeyDropPrefab;
    [SerializeField] private BeeSpawner beeSpawner;     // flowers ask it for bonus bees
    [SerializeField] private HoneyCounter honeyCounter; // the honey drop pays into it

    private readonly List<FlowerGate> flowers = new List<FlowerGate>();
    private readonly List<ThornGate> thorns = new List<ThornGate>();
    private readonly List<SlideMover> movers = new List<SlideMover>();
    private HoneyDrop honeyDrop;

    // Creates every object the level asks for, as children of this object.
    public void Build(LevelData level)
    {
        foreach (FlowerSetup setup in level.flowers)
        {
            FlowerGate flower = Instantiate(flowerPrefab, setup.position, Quaternion.identity, transform);
            flower.Setup(setup, beeSpawner);
            flowers.Add(flower);
            AddMover(flower.gameObject, setup.slideRange, setup.slideSpeed);
        }

        foreach (ThornSetup setup in level.thorns)
        {
            ThornGate thorn = Instantiate(thornPrefab, setup.position, Quaternion.identity, transform);
            thorns.Add(thorn);
            AddMover(thorn.gameObject, setup.slideRange, setup.slideSpeed);
        }

        foreach (Vector3 rockPosition in level.rocks)
            Instantiate(rockPrefab, rockPosition, Quaternion.identity, transform);

        if (level.hasHoneyDrop)
        {
            honeyDrop = Instantiate(honeyDropPrefab, level.honeyDropPosition, Quaternion.identity, transform);
            honeyDrop.Setup(honeyCounter);
        }
    }

    // Gives an object a SlideMover if the level says it should slide.
    private void AddMover(GameObject target, float range, float speed)
    {
        if (range <= 0f) return;
        SlideMover mover = target.AddComponent<SlideMover>();
        mover.Setup(range, speed);
        movers.Add(mover);
    }

    // Reopens every flower, resets thorns and sliding objects, and puts the honey drop back.
    public void ResetForNewRun()
    {
        foreach (FlowerGate flower in flowers) flower.ResetForNewRun();
        foreach (ThornGate thorn in thorns) thorn.ResetForNewRun();
        foreach (SlideMover mover in movers) mover.ResetForNewRun();
        if (honeyDrop != null) honeyDrop.ResetForNewRun();
    }
}
