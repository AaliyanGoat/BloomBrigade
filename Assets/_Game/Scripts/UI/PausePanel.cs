using UnityEngine;
using UnityEngine.UI;

// Pause button on the HUD and the pause screen (Resume / Restart / Menu).
public class PausePanel : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;

    // Wire the buttons and follow the game state.
    private void Start()
    {
        GameManager game = GameManager.Instance;
        pauseButton.onClick.AddListener(game.Pause);
        resumeButton.onClick.AddListener(game.Resume);
        restartButton.onClick.AddListener(game.Restart);
        menuButton.onClick.AddListener(game.GoToMenu);
        game.OnStateChanged += Show;
        Show(game.State);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null) GameManager.Instance.OnStateChanged -= Show;
    }

    // The pause screen is open only while Paused; the pause button only while Playing.
    private void Show(GameState state)
    {
        panel.SetActive(state == GameState.Paused);
        pauseButton.gameObject.SetActive(state == GameState.Playing);
    }

    // Leaving the app on a phone pauses the game.
    private void OnApplicationPause(bool paused)
    {
        if (paused && GameManager.Instance != null) GameManager.Instance.Pause();
    }
}
