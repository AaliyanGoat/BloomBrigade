using UnityEngine;

// Plays short sound effects. An empty clip slot does nothing, so the game runs before audio is added.
[RequireComponent(typeof(AudioSource))]
public class SfxPlayer : MonoBehaviour
{
    [SerializeField] private float volume = 0.8f;

    private AudioSource source;

    // Cache the AudioSource.
    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    // Play a clip once, with a random pitch between 1 - jitter and 1 + jitter.
    public void Play(AudioClip clip, float pitchJitter = 0f)
    {
        if (clip == null) return;
        source.pitch = 1f + Random.Range(-pitchJitter, pitchJitter);
        source.PlayOneShot(clip, volume);
    }
}
