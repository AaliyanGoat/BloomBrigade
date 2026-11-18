using UnityEngine;
using UnityEngine.Audio;

// Music and menu sounds, and the volume settings. Lives across scenes (DontDestroyOnLoad).
// Volumes go through an AudioMixer with two exposed parameters: MusicVolume and SfxVolume.
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioMixer mixer;
    [SerializeField] private AudioSource musicSource;   // output: Music group
    [SerializeField] private AudioSource uiSource;      // output: SFX group
    [SerializeField] private AudioClip music;
    [SerializeField] private AudioClip click;
    [SerializeField] private AudioClip buy;
    [SerializeField] private AudioClip error;
    [SerializeField] private AudioClip reward;

    private const string MusicParam = "MusicVolume";
    private const string SfxParam = "SfxVolume";

    // Keep only one AudioManager: the first one survives scene loads, later copies remove themselves.
    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        musicSource.clip = music;
        musicSource.loop = true;
        musicSource.Play();
    }

    // Apply the saved volumes (the mixer can only be set after Awake).
    private void Start()
    {
        SetMusicVolume(SaveSystem.Data.musicVolume, false);
        SetSfxVolume(SaveSystem.Data.sfxVolume, false);
    }

    // Slider value 0..1 → decibels. 0 becomes -80 dB (silent); 1 becomes 0 dB (full).
    private static float ToDecibels(float value) => value <= 0.0001f ? -80f : Mathf.Log10(value) * 20f;

    public void SetMusicVolume(float value, bool save = true)
    {
        mixer.SetFloat(MusicParam, ToDecibels(value));
        SaveSystem.Data.musicVolume = value;
        if (save) SaveSystem.Save();
    }

    public void SetSfxVolume(float value, bool save = true)
    {
        mixer.SetFloat(SfxParam, ToDecibels(value));
        SaveSystem.Data.sfxVolume = value;
        if (save) SaveSystem.Save();
    }

    public void PlayClick() => Play(click);
    public void PlayBuy() => Play(buy);
    public void PlayError() => Play(error);
    public void PlayReward() => Play(reward);

    private void Play(AudioClip clip)
    {
        if (clip != null) uiSource.PlayOneShot(clip);
    }
}
