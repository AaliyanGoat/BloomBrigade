using TMPro;
using UnityEngine;

// HUD during play (section 7.2): bee count, timer, honey and "Tap to start".
// The two health bars have their own HealthBar script.
public class HudController : MonoBehaviour
{
    [SerializeField] private BeeSpawner beeSpawner;
    [SerializeField] private HoneyCounter honeyCounter;
    [SerializeField] private TextMeshProUGUI beeCountText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI honeyText;
    [SerializeField] private GameObject tapToStart;
    [SerializeField] private TextMeshProUGUI levelText;

    private void OnEnable() { honeyCounter.OnHoneyChanged += ShowHoney; }
    private void OnDisable() { honeyCounter.OnHoneyChanged -= ShowHoney; }

    // Listen to state changes and show the current state straight away.
    private void Start()
    {
        GameManager.Instance.OnStateChanged += ShowState;
        ShowState(GameManager.Instance.State);
        ShowHoney(honeyCounter.Honey);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null) GameManager.Instance.OnStateChanged -= ShowState;
    }

    // Bee count and timer change every frame, so they are refreshed here.
    private void Update()
    {
        beeCountText.text = "Bees " + beeSpawner.ActiveCount;
        timerText.text = FormatTime(GameManager.Instance.ElapsedTime);
    }

    // "Tap to start" only in the Ready state.
    private void ShowState(GameState state)
    {
        tapToStart.SetActive(state == GameState.Ready);
        LevelData level = GameManager.Instance.Level;
        levelText.text = "Level " + level.levelNumber + ": " + level.levelName;
    }

    private void ShowHoney(int honey) => honeyText.text = "Honey " + honey;

    // 75.3 seconds -> "1:15".
    public static string FormatTime(float seconds)
    {
        int whole = Mathf.FloorToInt(seconds);
        return (whole / 60) + ":" + (whole % 60).ToString("00");
    }
}
