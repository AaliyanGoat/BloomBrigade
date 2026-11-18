using UnityEngine;

// Obstacle from Level 4: every second bee that passes through is removed, so it halves the stream.
public class ThornGate : MonoBehaviour
{
    [SerializeField] private int removeEvery = 2;   // 2 = every second bee

    private int count;

    // Count each bee once; remove every Nth one with a puff.
    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out Bee bee) || bee.IsDead || bee.HasUsed(this)) return;
        bee.MarkUsed(this);
        count++;
        if (count % removeEvery != 0) return;
        FeedbackManager.Instance.BeeMeetsWasp(bee.transform.position);
        bee.Despawn();
    }

    // Restart: start counting again.
    public void ResetForNewRun()
    {
        count = 0;
    }
}
