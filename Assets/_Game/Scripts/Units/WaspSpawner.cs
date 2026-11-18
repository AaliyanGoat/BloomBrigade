using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

// Card M4: spawns the waves listed in the LevelData near the nest, from one pool per wasp type.
public class WaspSpawner : MonoBehaviour
{
    [SerializeField] private GameTuning tuning;
    [SerializeField] private Transform waspContainer;
    [SerializeField] private float spawnZ = 35f;
    [SerializeField] private float spawnY = 0.5f;
    [SerializeField] private float spawnHalfWidth = 3f;   // X random between -3 and 3
    [SerializeField] private int poolPrewarm = 20;

    private readonly Dictionary<WaspData, ObjectPool<Wasp>> pools = new Dictionary<WaspData, ObjectPool<Wasp>>();
    private readonly List<Wasp> activeWasps = new List<Wasp>();
    private float[] nextWaveTimes;   // when each wave in the level spawns next

    public GameTuning Tuning => tuning;

    // Spawn any wave whose time has come. Only while Playing.
    private void Update()
    {
        GameManager game = GameManager.Instance;
        if (game.State != GameState.Playing) return;

        List<WaveSetup> waves = game.Level.waves;
        for (int i = 0; i < waves.Count; i++)
        {
            if (game.ElapsedTime < nextWaveTimes[i]) continue;
            for (int n = 0; n < waves[i].count; n++) SpawnWasp(waves[i].wasp);
            nextWaveTimes[i] += waves[i].interval;
        }
    }

    // Take a wasp of this type from its pool and send it down the lane at a random X.
    private void SpawnWasp(WaspData data)
    {
        Wasp wasp = GetPool(data).Get();
        activeWasps.Add(wasp);
        Vector3 position = new Vector3(Random.Range(-spawnHalfWidth, spawnHalfWidth), spawnY, spawnZ);
        wasp.Launch(position, data, this);
    }

    // Finds the pool for a wasp type, creating (and prewarming) it the first time.
    private ObjectPool<Wasp> GetPool(WaspData data)
    {
        if (pools.TryGetValue(data, out ObjectPool<Wasp> pool)) return pool;

        pool = new ObjectPool<Wasp>(
            () => Instantiate(data.prefab, waspContainer),
            wasp => wasp.gameObject.SetActive(true),
            wasp => wasp.gameObject.SetActive(false),
            wasp => { if (wasp != null) Destroy(wasp.gameObject); },   // already gone when Play mode stops
            true, poolPrewarm);
        pools.Add(data, pool);

        var warm = new List<Wasp>();
        for (int i = 0; i < poolPrewarm; i++) warm.Add(pool.Get());
        foreach (Wasp wasp in warm) pool.Release(wasp);
        return pool;
    }

    // Called by Wasp.Despawn: back into its own pool.
    public void Release(Wasp wasp)
    {
        activeWasps.Remove(wasp);
        pools[wasp.Data].Release(wasp);
    }

    // Restart: clear every wasp and restart the wave timers from each wave's first delay.
    public void ResetForNewRun()
    {
        for (int i = activeWasps.Count - 1; i >= 0; i--)
            activeWasps[i].Despawn();

        List<WaveSetup> waves = GameManager.Instance.Level.waves;
        nextWaveTimes = new float[waves.Count];
        for (int i = 0; i < waves.Count; i++)
        {
            nextWaveTimes[i] = waves[i].firstDelay;
            GetPool(waves[i].wasp);   // prewarm now, not in the middle of play
        }
    }
}
