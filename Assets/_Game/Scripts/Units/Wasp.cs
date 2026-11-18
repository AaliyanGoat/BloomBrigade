using UnityEngine;

// Card M4: one wasp flies straight down the lane, takes hits from bees and damages the hive.
[RequireComponent(typeof(Rigidbody))]
public class Wasp : MonoBehaviour
{
    private Rigidbody body;
    private WaspSpawner spawner;
    private WaspData data;
    private int health;
    private bool isDead;

    public WaspData Data => data;

    // Cache the Rigidbody once.
    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    // Called by WaspSpawner when the wasp leaves the pool: full health, moving towards the hive.
    public void Launch(Vector3 position, WaspData waspData, WaspSpawner owner)
    {
        data = waspData;
        spawner = owner;
        health = data.health;
        isDead = false;
        transform.position = position;
        body.position = position;
        body.linearVelocity = Vector3.back * data.speed;
    }

    // A bee touched this wasp. Returns false if the wasp was already dead, so that bee survives.
    public bool TakeHit()
    {
        if (isDead) return false;
        health--;
        if (health <= 0) Despawn();
        return true;
    }

    // Reaching the hive hit zone damages the hive.
    private void OnTriggerEnter(Collider other)
    {
        if (isDead) return;
        if (other.TryGetComponent(out BaseHealth target) && !target.DamagedByBees)
        {
            target.TakeDamage(data.hiveDamage);
            Despawn();
        }
    }

    // Safety: a wasp that somehow gets behind the hive is removed.
    private void Update()
    {
        if (!isDead && transform.position.z < spawner.Tuning.waspDespawnZ)
            Despawn();
    }

    // Stop and go back to the pool (only once).
    public void Despawn()
    {
        if (isDead) return;
        isDead = true;
        body.linearVelocity = Vector3.zero;
        spawner.Release(this);
    }
}
