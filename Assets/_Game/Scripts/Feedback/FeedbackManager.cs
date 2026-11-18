using UnityEngine;
using UnityEngine.Pool;

// One place that turns game moments into feedback (section 7.1): sounds, particles, number pops and shake.
// Prefabs such as Bee cannot hold scene references, so they reach it through Instance.
public class FeedbackManager : MonoBehaviour
{
    public static FeedbackManager Instance { get; private set; }

    [Header("Helpers")]
    [SerializeField] private CameraShake cameraShake;
    [SerializeField] private SfxPlayer sfx;
    [SerializeField] private ParticleSystem puffParticles;   // world-space system, we Emit at a point
    [SerializeField] private FloatingText floatingTextPrefab;
    [SerializeField] private Transform floatingTextContainer;

    [Header("Tuning")]
    [SerializeField] private int puffCount = 8;
    [SerializeField] private float popHeight = 1.2f;          // number pops start above the action
    [SerializeField] private Color bonusColor = Color.yellow;
    [SerializeField] private Color honeyColor = new Color(1f, 0.65f, 0f);
    [SerializeField] private float firePitchJitter = 0.15f;

    [Header("Sound clips (empty slots are fine)")]
    [SerializeField] private AudioClip beeFire;
    [SerializeField] private AudioClip flowerPop;
    [SerializeField] private AudioClip flowerClose;
    [SerializeField] private AudioClip flowerOpen;
    [SerializeField] private AudioClip squish;
    [SerializeField] private AudioClip baseHit;
    [SerializeField] private AudioClip boom;
    [SerializeField] private AudioClip winJingle;
    [SerializeField] private AudioClip loseJingle;

    private ObjectPool<FloatingText> textPool;
    private Quaternion textFacing;

    // Set the singleton and build the floating text pool.
    private void Awake()
    {
        Instance = this;
        textPool = new ObjectPool<FloatingText>(
            () => Instantiate(floatingTextPrefab, floatingTextContainer),
            t => t.gameObject.SetActive(true),
            t => t.gameObject.SetActive(false),
            t => { if (t != null) Destroy(t.gameObject); });   // already gone when Play mode stops
        textFacing = cameraShake.transform.rotation;   // texts face the camera
    }

    // Bee leaves the cannon: soft pop at a slightly random pitch.
    public void BeeFired() => sfx.Play(beeFire, firePitchJitter);

    // Bees multiplied: "+N" number pop and a bright pop sound.
    public void FlowerBonus(Vector3 position, int bonus)
    {
        PopText(position, "+" + bonus, bonusColor);
        sfx.Play(flowerPop, firePitchJitter);
    }

    public void FlowerClosed(Vector3 position) => sfx.Play(flowerClose);
    public void FlowerOpened(Vector3 position) => sfx.Play(flowerOpen);

    // Bee meets wasp: small particle puff and a squish.
    public void BeeMeetsWasp(Vector3 position)
    {
        var emit = new ParticleSystem.EmitParams { position = position, applyShapeToPosition = true };
        puffParticles.Emit(emit, puffCount);
        sfx.Play(squish, firePitchJitter);
    }

    public void HoneyCollected(Vector3 position, int amount) => PopText(position, "+" + amount + " honey", honeyColor);
    public void BaseHit() => sfx.Play(baseHit, firePitchJitter);

    // A base reached 0: camera shake and a boom.
    public void BaseDestroyed()
    {
        cameraShake.Shake();
        sfx.Play(boom);
    }

    public void Win() => sfx.Play(winJingle);
    public void Lose() => sfx.Play(loseJingle);

    // Take a floating text from the pool and show it.
    private void PopText(Vector3 position, string message, Color color)
    {
        FloatingText text = textPool.Get();
        text.Show(position + Vector3.up * popHeight, message, color, textFacing, textPool.Release);
    }
}
