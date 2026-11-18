using System.Collections.Generic;
using UnityEngine;

// One bee: flies straight up the lane and handles what it touches (wasps, the nest, rocks).
// Flowers and the honey drop handle bees themselves in their own OnTriggerEnter.
[RequireComponent(typeof(Rigidbody))]
public class Bee : MonoBehaviour
{
    private Rigidbody body;
    private BeeSpawner spawner;
    private readonly HashSet<Component> usedGates = new HashSet<Component>();   // flowers and thorns already passed
    private int obstacleLayer;

    // isDead is set first when the bee is removed, so it can never hit two things in one physics step.
    public bool IsDead { get; private set; }

    // Cache the Rigidbody and the Obstacle layer number once.
    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        obstacleLayer = LayerMask.NameToLayer("Obstacle");
    }

    // Called by BeeSpawner when the bee leaves the pool. Resets its state and sets its speed once.
    public void Launch(Vector3 position, BeeSpawner owner)
    {
        spawner = owner;
        IsDead = false;
        usedGates.Clear();
        transform.position = position;
        body.position = position;
        body.linearVelocity = Vector3.forward * owner.BeeSpeed;
    }

    // Has this bee already gone through that gate (flower or thorn)?
    public bool HasUsed(Component gate) => usedGates.Contains(gate);

    // Remember a gate so the bee cannot trigger it again.
    public void MarkUsed(Component gate) => usedGates.Add(gate);

    // A bee that passes the end of the lane goes back to the pool.
    private void Update()
    {
        if (!IsDead && transform.position.z > spawner.Tuning.beeDespawnZ)
            Despawn();
    }

    // Wasp: both lose 1 health. Nest: damage it. Rock: the bee is blocked.
    private void OnTriggerEnter(Collider other)
    {
        if (IsDead) return;

        if (other.TryGetComponent(out Wasp wasp))
        {
            if (wasp.TakeHit())
            {
                FeedbackManager.Instance.BeeMeetsWasp(transform.position);
                Despawn();
            }
            return;
        }

        if (other.TryGetComponent(out BaseHealth target) && target.DamagedByBees)
        {
            target.TakeDamage(spawner.Tuning.beeDamage);
            Despawn();
            return;
        }

        if (other.gameObject.layer == obstacleLayer)
            Despawn();
    }

    // Stop the bee and hand it back to the pool (only once).
    public void Despawn()
    {
        if (IsDead) return;
        IsDead = true;
        body.linearVelocity = Vector3.zero;
        spawner.Release(this);
    }
}
