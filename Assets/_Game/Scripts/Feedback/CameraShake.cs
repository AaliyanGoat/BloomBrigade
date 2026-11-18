using UnityEngine;

// Feedback: shakes the fixed camera for a moment, then puts it back exactly where it was.
public class CameraShake : MonoBehaviour
{
    [SerializeField] private float defaultDuration = 0.4f;
    [SerializeField] private float defaultMagnitude = 0.3f;

    private Vector3 restPosition;
    private float timeLeft;
    private float duration;
    private float magnitude;

    // The camera never moves, so its start position is its rest position.
    private void Awake()
    {
        restPosition = transform.localPosition;
    }

    // Shake with the Inspector values.
    public void Shake()
    {
        Shake(defaultDuration, defaultMagnitude);
    }

    // Shake for "seconds" with a random offset up to "strength" units that fades out.
    public void Shake(float seconds, float strength)
    {
        duration = seconds;
        timeLeft = seconds;
        magnitude = strength;
    }

    // Random offset each frame while shaking.
    private void LateUpdate()
    {
        if (timeLeft <= 0f) return;
        timeLeft -= Time.deltaTime;
        float fade = Mathf.Clamp01(timeLeft / duration);
        transform.localPosition = timeLeft > 0f
            ? restPosition + Random.insideUnitSphere * (magnitude * fade)
            : restPosition;
    }
}
