using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Settings: music and sound volume sliders (saved), and Reset progress with a second tap to confirm.
public class SettingsScreen : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Button resetButton;
    [SerializeField] private TextMeshProUGUI resetLabel;

    private bool confirmReset;

    private void Awake()
    {
        musicSlider.onValueChanged.AddListener(v => AudioManager.Instance?.SetMusicVolume(v));
        sfxSlider.onValueChanged.AddListener(v => AudioManager.Instance?.SetSfxVolume(v));
        resetButton.onClick.AddListener(OnResetPressed);
    }

    // Show the saved volumes without triggering a save.
    private void OnEnable()
    {
        musicSlider.SetValueWithoutNotify(SaveSystem.Data.musicVolume);
        sfxSlider.SetValueWithoutNotify(SaveSystem.Data.sfxVolume);
        confirmReset = false;
        resetLabel.text = "Reset progress";
    }

    // First tap asks; second tap really deletes progress (volumes are kept).
    private void OnResetPressed()
    {
        if (!confirmReset)
        {
            confirmReset = true;
            resetLabel.text = "Tap again to reset";
            return;
        }
        float music = SaveSystem.Data.musicVolume, sfx = SaveSystem.Data.sfxVolume;
        SaveSystem.ResetAll();
        SaveSystem.Data.musicVolume = music;
        SaveSystem.Data.sfxVolume = sfx;
        SaveSystem.Save();
        confirmReset = false;
        resetLabel.text = "Progress reset";
    }
}
