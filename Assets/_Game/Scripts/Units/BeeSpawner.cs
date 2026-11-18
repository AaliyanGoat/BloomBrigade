using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

// Card M2: owns the bee pool, fires bees while the finger is held, and gives flowers their bonus bees.
public class BeeSpawner : MonoBehaviour
{
    [SerializeField] private GameTuning tuning;
    [SerializeField] private Bee beePrefab;
    [SerializeField] private Transform muzzle;        // where bees leave the cannon
    [SerializeField] private Transform beeContainer;  // keeps the Hierarchy tidy
    [SerializeField] private ScalePunch cannonPunch;  // feedback on every shot
    [SerializeField] private float laneHalfWidth = 3.75f;

    private ObjectPool<Bee> pool;
    private readonly List<Bee> activeBees = new List<Bee>();
    private float nextFireTime;
    private bool wasFiring;

    public GameTuning Tuning => tuning;
    public int ActiveCount => activeBees.Count;
    public float FireRate { get; private set; }   // base + Fire rate upgrade (from PlayerStats)
    public float BeeSpeed { get; private set; }   // base + Bee speed upgrade (from PlayerStats)

    // Build the pool and create poolPrewarm bees up front so play never has to Instantiate.
    private void Awake()
    {
        pool = new ObjectPool<Bee>(
            () => Instantiate(beePrefab, beeContainer),
            bee => bee.gameObject.SetActive(true),
            bee => bee.gameObject.SetActive(false),
            bee => { if (bee != null) Destroy(bee.gameObject); },   // already gone when Play mode stops
            true, tuning.poolPrewarm, tuning.maxActiveBees);

        var warm = new List<Bee>();
        for (int i = 0; i < tuning.poolPrewarm; i++) warm.Add(pool.Get());
        foreach (Bee bee in warm) pool.Release(bee);
    }

    // While Playing and held: one bee every 1 / fireRate seconds. Release stops at once.
    private void Update()
    {
        bool firing = GameManager.Instance.State == GameState.Playing && PointerInput.IsHeld;
        if (firing && !wasFiring) nextFireTime = Time.time;   // first bee fires immediately
        wasFiring = firing;
        if (!firing) return;

        while (Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime += 1f / FireRate;
        }
    }

    // Shoot one bee from the muzzle, with a scale punch and a pop sound.
    private void Fire()
    {
        if (SpawnBee(muzzle.position) == null) return;
        cannonPunch.Punch();
        FeedbackManager.Instance.BeeFired();
    }

    // Take a bee from the pool and launch it. Returns null if maxActiveBees are alive.
    private Bee SpawnBee(Vector3 position)
    {
        if (activeBees.Count >= tuning.maxActiveBees) return null;
        Bee bee = pool.Get();
        activeBees.Add(bee);
        bee.Launch(position, this);
        return bee;
    }

    // Called by a flower: one bonus bee next to the bee that passed, already marked as having used it.
    public bool SpawnBonusBee(Vector3 nearPosition, FlowerGate flower, float spread)
    {
        Vector3 position = nearPosition;
        position.x = Mathf.Clamp(position.x + Random.Range(-spread, spread), -laneHalfWidth, laneHalfWidth);
        Bee bee = SpawnBee(position);
        if (bee == null) return false;
        bee.MarkUsed(flower);
        return true;
    }

    // Called by Bee.Despawn: back into the pool.
    public void Release(Bee bee)
    {
        activeBees.Remove(bee);
        pool.Release(bee);
    }

    // Restart: every bee in the air goes back to the pool, and the upgraded stats are read again.
    public void ResetForNewRun()
    {
        for (int i = activeBees.Count - 1; i >= 0; i--)
            activeBees[i].Despawn();
        wasFiring = false;
        FireRate = PlayerStats.FireRate;
        BeeSpeed = PlayerStats.BeeSpeed;
    }
}
