using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Card M6: after a win or loss, waits resultDelay, slides the panel in, and offers Restart.
public class ResultPanel : MonoBehaviour
{
    [SerializeField] private RectTransform panel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI detailText;
    [SerializeField] private Button restartButton;
    [SerializeField] private HoneyCounter honeyCounter;
    [SerializeField] private float resultDelay = 1f;
    [SerializeField] private float slideTime = 0.35f;
    [SerializeField] private float hiddenOffsetY = -1400f;   // how far below the screen it starts

    private Vector2 shownPosition;

    // Remember the on-screen position, hook up the button, and start hidden.
    private void Awake()
    {
        shownPosition = panel.anchoredPosition;
        panel.gameObject.SetActive(false);
    }

    private void Start()
    {
        restartButton.onClick.AddListener(GameManager.Instance.Restart);
        GameManager.Instance.OnStateChanged += HandleStateChanged;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null) GameManager.Instance.OnStateChanged -= HandleStateChanged;
    }

    // Won or Lost: show after the delay. Ready (a restart): hide at once and cancel any pending show.
    private void HandleStateChanged(GameState state)
    {
        StopAllCoroutines();
        if (state == GameState.Won || state == GameState.Lost)
            StartCoroutine(ShowAfterDelay(state == GameState.Won));
        else
            panel.gameObject.SetActive(false);
    }

    // Wait, fill in the text, then slide up into place.
    private IEnumerator ShowAfterDelay(bool won)
    {
        yield return new WaitForSeconds(resultDelay);

        string time = HudController.FormatTime(GameManager.Instance.ElapsedTime);
        titleText.text = won ? "Nest destroyed!" : "Hive destroyed!";
        detailText.text = "Time " + time + "\nHoney earned " + honeyCounter.Honey;
        if (won) FeedbackManager.Instance.Win(); else FeedbackManager.Instance.Lose();

        panel.gameObject.SetActive(true);
        Vector2 hidden = shownPosition + Vector2.up * hiddenOffsetY;
        for (float t = 0f; t < slideTime; t += Time.deltaTime)
        {
            float eased = 1f - Mathf.Pow(1f - t / slideTime, 3f);   // fast then slow
            panel.anchoredPosition = Vector2.LerpUnclamped(hidden, shownPosition, eased);
            yield return null;
        }
        panel.anchoredPosition = shownPosition;
    }
}
