using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Fake rewarded ad (no real ad SDK). Shows a full-screen "ad" for adLength seconds and only then
// calls the reward. The Close button is always visible; closing early just gives no reward.
public class MockAdPanel : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private RectTransform progressFill;    // pivot left, scaled 0 → 1 on X
    [SerializeField] private Button closeButton;
    [SerializeField] private float adLength = 3f;

    private Action onFinished;
    private float timeLeft;
    private bool running;

    private void Awake()
    {
        panel.SetActive(false);
        closeButton.onClick.AddListener(Close);
    }

    // Start the fake ad. "finished" runs only if the player waits until the end.
    public void Show(Action finished)
    {
        onFinished = finished;
        timeLeft = adLength;
        running = true;
        panel.SetActive(true);
    }

    // Count down with unscaled time so it also works if the game is paused.
    private void Update()
    {
        if (!running) return;
        timeLeft -= Time.unscaledDeltaTime;
        countdownText.text = "Reward in " + Mathf.CeilToInt(Mathf.Max(timeLeft, 0f));
        progressFill.localScale = new Vector3(1f - Mathf.Clamp01(timeLeft / adLength), 1f, 1f);
        if (timeLeft > 0f) return;

        running = false;
        panel.SetActive(false);
        onFinished?.Invoke();   // reward only after the full ad
    }

    // The player chose to stop: hide the ad, no reward, no penalty.
    private void Close()
    {
        running = false;
        panel.SetActive(false);
    }
}
