using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Win / Lose screen: slides in after resultDelay with stars, time, honey earned,
// the optional "Watch an ad for 2x honey" button, and Retry / Next / Shop / Menu.
public class ResultPanel : MonoBehaviour
{
    [SerializeField] private RectTransform panel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI detailText;
    [SerializeField] private Image[] stars;
    [SerializeField] private Color starOn = new Color(1f, 0.8f, 0.1f);
    [SerializeField] private Color starOff = new Color(1f, 1f, 1f, 0.2f);
    [SerializeField] private Button adButton;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button shopButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private MockAdPanel mockAd;
    [SerializeField] private float resultDelay = 1f;
    [SerializeField] private float slideTime = 0.35f;
    [SerializeField] private float hiddenOffsetY = -1400f;

    private Vector2 shownPosition;

    // Remember the on-screen position and start hidden.
    private void Awake()
    {
        shownPosition = panel.anchoredPosition;
        panel.gameObject.SetActive(false);
    }

    // Wire every button to the GameManager and listen for the end of a run.
    private void Start()
    {
        GameManager game = GameManager.Instance;
        retryButton.onClick.AddListener(game.Restart);
        nextButton.onClick.AddListener(game.NextLevel);
        shopButton.onClick.AddListener(game.GoToShop);
        menuButton.onClick.AddListener(game.GoToMenu);
        adButton.onClick.AddListener(() => mockAd.Show(OnAdFinished));
        game.OnStateChanged += HandleStateChanged;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null) GameManager.Instance.OnStateChanged -= HandleStateChanged;
    }

    // Won or Lost: show after the delay. Anything else: hide at once.
    private void HandleStateChanged(GameState state)
    {
        StopAllCoroutines();
        if (state == GameState.Won || state == GameState.Lost)
            StartCoroutine(ShowAfterDelay(state == GameState.Won));
        else
            panel.gameObject.SetActive(false);
    }

    private IEnumerator ShowAfterDelay(bool won)
    {
        yield return new WaitForSeconds(resultDelay);

        GameManager game = GameManager.Instance;
        titleText.text = won ? "Nest destroyed!" : "Hive destroyed!";
        for (int i = 0; i < stars.Length; i++) stars[i].color = i < game.StarsEarned ? starOn : starOff;
        RefreshDetails();
        nextButton.gameObject.SetActive(won && game.HasNextLevel);
        adButton.gameObject.SetActive(game.HoneyEarned > 0);
        adButton.interactable = true;
        if (won) FeedbackManager.Instance.Win(); else FeedbackManager.Instance.Lose();

        panel.gameObject.SetActive(true);
        Vector2 hidden = shownPosition + Vector2.up * hiddenOffsetY;
        for (float t = 0f; t < slideTime; t += Time.unscaledDeltaTime)
        {
            float eased = 1f - Mathf.Pow(1f - t / slideTime, 3f);
            panel.anchoredPosition = Vector2.LerpUnclamped(hidden, shownPosition, eased);
            yield return null;
        }
        panel.anchoredPosition = shownPosition;
    }

    // Time, honey this run (x2 after the ad) and the bank total.
    private void RefreshDetails()
    {
        GameManager game = GameManager.Instance;
        int earned = game.AdRewardClaimed ? game.HoneyEarned * 2 : game.HoneyEarned;
        string bonus = game.AdRewardClaimed ? "  (x2!)" : "";
        detailText.text = $"Time {HudController.FormatTime(game.ElapsedTime)}\nHoney earned {earned}{bonus}\nHoney total {PlayerStats.Honey}";
    }

    // The ad ran to the end: pay the reward once and grey out the button.
    private void OnAdFinished()
    {
        GameManager.Instance.ClaimAdReward();
        adButton.interactable = false;
        if (AudioManager.Instance != null) AudioManager.Instance.PlayReward();
        RefreshDetails();
    }
}
