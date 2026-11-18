using UnityEngine;

// Optional pickup: the first bee to touch it gives honey, once per run. The bee keeps flying.
public class HoneyDrop : MonoBehaviour
{
    [SerializeField] private GameTuning tuning;
    [SerializeField] private GameObject visual;

    private HoneyCounter honeyCounter;
    private bool collected;

    // Called once by LevelLoader.
    public void Setup(HoneyCounter counter)
    {
        honeyCounter = counter;
    }

    // First bee only: pay honey, hide the drop, pop a number.
    private void OnTriggerEnter(Collider other)
    {
        if (collected || !other.TryGetComponent(out Bee bee) || bee.IsDead) return;
        collected = true;
        visual.SetActive(false);
        honeyCounter.Add(tuning.honeyDropValue);
        FeedbackManager.Instance.HoneyCollected(transform.position, tuning.honeyDropValue);
    }

    // Restart: the drop comes back.
    public void ResetForNewRun()
    {
        collected = false;
        visual.SetActive(true);
    }
}
