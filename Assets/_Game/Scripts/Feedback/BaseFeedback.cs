using UnityEngine;

// Feedback for card M5: the base flashes white when hit, and bursts + shakes the camera at 0 health.
public class BaseFeedback : MonoBehaviour
{
    [SerializeField] private BaseHealth health;
    [SerializeField] private Renderer baseRenderer;
    [SerializeField] private ParticleSystem destroyBurst;
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashTime = 0.1f;
    [SerializeField] private float minSoundGap = 0.08f;   // stops many hits in one frame from stacking sounds

    private MaterialPropertyBlock block;
    private float flashTimeLeft;
    private float nextSoundTime;

    // Create the property block used to tint without a new material.
    private void Awake()
    {
        block = new MaterialPropertyBlock();
    }

    private void OnEnable()
    {
        health.OnDamaged += HandleDamaged;
        health.OnDestroyed += HandleDestroyed;
    }

    private void OnDisable()
    {
        health.OnDamaged -= HandleDamaged;
        health.OnDestroyed -= HandleDestroyed;
    }

    // Hit: turn white for flashTime, and a thud (not more often than minSoundGap).
    private void HandleDamaged()
    {
        flashTimeLeft = flashTime;
        block.SetColor("_BaseColor", flashColor);
        baseRenderer.SetPropertyBlock(block);

        if (Time.time < nextSoundTime) return;
        nextSoundTime = Time.time + minSoundGap;
        FeedbackManager.Instance.BaseHit();
    }

    // 0 health: big particle burst, camera shake and boom.
    private void HandleDestroyed()
    {
        destroyBurst.Play();
        FeedbackManager.Instance.BaseDestroyed();
    }

    // End the flash by removing the tint (back to the material's own colour).
    private void Update()
    {
        if (flashTimeLeft <= 0f) return;
        flashTimeLeft -= Time.deltaTime;
        if (flashTimeLeft <= 0f) baseRenderer.SetPropertyBlock(null);
    }
}
