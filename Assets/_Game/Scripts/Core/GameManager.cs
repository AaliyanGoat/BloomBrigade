using System;
using UnityEngine;

public enum GameState { Ready, Playing, Won, Lost }

// Owns the game state (card M6). Other scripts read State and listen to OnStateChanged.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Level to play (swap for the twist test layout)")]
    [SerializeField] private LevelData level;

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
    public LevelData Level => level;
    public float ElapsedTime { get; private set; }   // seconds since the run started

    // Set the singleton before anything else uses it.
    private void Awake()
    {
        Instance = this;
    }

    // Listen to both bases, build the level and get ready.
    private void Start()
    {
        hive.OnDestroyed += HandleHiveDestroyed;
        nest.OnDestroyed += HandleNestDestroyed;
        levelLoader.Build(level);
        ResetRun();
    }

    // Unsubscribe so no event points at a destroyed object.
    private void OnDestroy()
    {
        if (hive != null) hive.OnDestroyed -= HandleHiveDestroyed;
        if (nest != null) nest.OnDestroyed -= HandleNestDestroyed;
    }

    // Ready: wait for a tap. Playing: run the timer.
    private void Update()
    {
        if (State == GameState.Ready && PointerInput.PressedThisFrame)
            SetState(GameState.Playing);
        else if (State == GameState.Playing)
            ElapsedTime += Time.deltaTime;
    }

    // Called by the Restart button. Clears the run and goes back to Ready.
    public void Restart()
    {
        ResetRun();
    }

    // Puts every system back to its start values without leaving Play mode.
    private void ResetRun()
    {
        ElapsedTime = 0f;
        beeSpawner.ResetForNewRun();
        waspSpawner.ResetForNewRun();
        cannon.ResetForNewRun();
        levelLoader.ResetForNewRun();
        hive.ResetForNewRun(level.hiveHealth);
        nest.ResetForNewRun(level.nestHealth);
        honeyCounter.ResetForNewRun();
        SetState(GameState.Ready);
    }

    // Nest at 0 health: the player wins.
    private void HandleNestDestroyed()
    {
        if (State != GameState.Playing) return;
        honeyCounter.AddWinBonus(level.levelNumber);
        SetState(GameState.Won);
    }

    // Hive at 0 health: the player loses.
    private void HandleHiveDestroyed()
    {
        if (State != GameState.Playing) return;
        SetState(GameState.Lost);
    }

    // Changes the state and tells every listener.
    private void SetState(GameState newState)
    {
        State = newState;
        OnStateChanged?.Invoke(newState);
    }
}
