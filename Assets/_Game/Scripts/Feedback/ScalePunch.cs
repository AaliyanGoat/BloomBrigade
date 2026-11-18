using UnityEngine;

// Feedback: briefly scales an object up, then eases it back. Also used to shrink closed flowers.
public class ScalePunch : MonoBehaviour
{
    [SerializeField] private float strength = 0.25f;   // +25% size at the start of a punch
    [SerializeField] private float duration = 0.12f;

    private Vector3 originalScale;
    private float baseFactor = 1f;   // 1 = normal size, 0.6 = shrunk
    private float timer;

    // Remember the size set in the prefab.
    private void Awake()
    {
        originalScale = transform.localScale;
    }

    // Start a new punch (restarts it if one is already running).
    public void Punch()
    {
        timer = duration;
    }

    // Change the resting size, e.g. 0.6 while a flower is closed.
    public void SetBaseScale(float factor)
    {
        baseFactor = factor;
        ApplyScale(0f);
    }

    // Shrink the punch back to zero over the duration.
    private void Update()
    {
        if (timer <= 0f) return;
        timer = Mathf.Max(0f, timer - Time.deltaTime);
        ApplyScale(strength * (timer / duration));
    }

    // Resting size plus the current punch.
    private void ApplyScale(float extra)
    {
        transform.localScale = originalScale * (baseFactor * (1f + extra));
    }
}
