using System;
using UnityEngine;
using UnityEngine.EventSystems;

public enum GameState { Ready, Playing, Paused, Won, Lost }

// Owns the game state. Other scripts read State and listen to OnStateChanged.
// Also pays out honey, saves stars and unlocks the next level when a run ends.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Scene references")]
    [SerializeField] private LevelLoader levelLoader;
    [SerializeField] private CannonController cannon;
    [SerializeField] private BeeSpawner beeSpawner;
    [SerializeField] private WaspSpawner waspSpawner;
    [SerializeField] private HoneyCounter honeyCounter;
    [SerializeField] private BaseHealth hive;
    [SerializeField] private BaseHealth nest;

    public event Action<GameState> OnStateChanged;

    public GameState State { get; private set; } = GameState.Ready;
    public LevelData Level { get; private set; }
    public int LevelIndex { get; private set; }
    public float ElapsedTime { get; private set; }
    public int StarsEarned { get; private set; }
    public int HoneyEarned { get; private set; }        // honey from this run (before the ad)
    public bool AdRewardClaimed { get; private set; }
    public bool HasNextLevel => LevelIndex + 1 < GameCatalog.Instance.levels.Count;

    // Pick the level chosen in the menu (Level 1 when the Game scene is played directly).
    private void Awake()
    {
        Instance = this;
        LevelIndex = SceneLoader.LevelIndex;
        Level = GameCatalog.Instance.levels[LevelIndex];
    }

    // Listen to both bases, build the level and get ready.
    private void Start()
    {
        hive.OnDestroyed += HandleHiveDestroyed;
        nest.OnDestroyed += HandleNestDestroyed;
        levelLoader.Build(Level);
        ResetRun();
    }

    private void OnDestroy()
    {
        if (hive != null) hive.OnDestroyed -= HandleHiveDestroyed;
        if (nest != null) nest.OnDestroyed -= HandleNestDestroyed;
    }

    // Ready: wait for a tap that is not on a button. Playing: run the timer.
    private void Update()
    {
        bool overUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        if (State == GameState.Ready && PointerInput.PressedThisFrame && !overUI)
            SetState(GameState.Playing);
        else if (State == GameState.Playing)
            ElapsedTime += Time.deltaTime;
    }

    // Pause button: freeze the game. Buttons still work because UI ignores timeScale.
    public void Pause()
    {
        if (State != GameState.Playing) return;
        Time.timeScale = 0f;
        SetState(GameState.Paused);
    }

    public void Resume()
    {
        if (State != GameState.Paused) return;
        Time.timeScale = 1f;
        SetState(GameState.Playing);
    }

    // Retry button: same level again, without reloading the scene.
    public void Restart()
    {
        Time.timeScale = 1f;
        ResetRun();
    }

    public void NextLevel() => SceneLoader.LoadLevel(LevelIndex + 1);
    public void GoToMenu() => SceneLoader.LoadMenu();
    public void GoToShop() => SceneLoader.LoadMenu(true);

    // Puts every system back to its start values (upgrades are read again here).
    private void ResetRun()
    {
        ElapsedTime = 0f;
        StarsEarned = 0;
        HoneyEarned = 0;
        AdRewardClaimed = false;
        beeSpawner.ResetForNewRun();
        waspSpawner.ResetForNewRun();
        cannon.ResetForNewRun();
        levelLoader.ResetForNewRun();
        hive.ResetForNewRun(Level.hiveHealth + PlayerStats.HiveHealthBonus);
        nest.ResetForNewRun(Level.nestHealth);
        honeyCounter.ResetForNewRun();
        SetState(GameState.Ready);
    }

    private void HandleNestDestroyed()
    {
        if (State != GameState.Playing) return;
        honeyCounter.AddWinBonus(Level.levelNumber);
        StarsEarned = CountStars();
        EndRun(GameState.Won);
    }

    private void HandleHiveDestroyed()
    {
        if (State != GameState.Playing) return;
        EndRun(GameState.Lost);
    }

    // Bank the honey, save stars and unlock the next level (on a win), then change state.
    private void EndRun(GameState result)
    {
        HoneyEarned = honeyCounter.Honey;
        if (result == GameState.Won)
        {
            SaveSystem.RecordResult(LevelIndex, StarsEarned, ElapsedTime);
            SaveSystem.Data.unlockedLevel = Mathf.Max(SaveSystem.Data.unlockedLevel, LevelIndex + 2);
        }
        PlayerStats.AddHoney(HoneyEarned);   // also saves to disk
        SetState(result);
    }

    // 1 star for winning, 2 for winning under the target time, 3 for also keeping enough hive health.
    private int CountStars()
    {
        int stars = 1;
        if (ElapsedTime > Level.targetTime) return stars;
        stars++;
        if ((float)hive.CurrentHealth / hive.MaxHealth >= Level.threeStarHive) stars++;
        return stars;
    }

    // Mock rewarded ad finished: pay this run's honey a second time (2x), only once.
    public void ClaimAdReward()
    {
        if (AdRewardClaimed || (State != GameState.Won && State != GameState.Lost)) return;
        AdRewardClaimed = true;
        PlayerStats.AddHoney(HoneyEarned);
    }

    private void SetState(GameState newState)
    {
        State = newState;
        OnStateChanged?.Invoke(newState);
    }
}
