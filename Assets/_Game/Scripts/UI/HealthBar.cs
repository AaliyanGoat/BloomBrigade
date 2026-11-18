using TMPro;
using UnityEngine;

// HUD bar for one base: the fill is scaled on X from 0 to 1, with "current / max" text.
public class HealthBar : MonoBehaviour
{
    [SerializeField] private BaseHealth health;
    [SerializeField] private RectTransform fill;   // pivot on the left so it shrinks to the left
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private string prefix = "Nest";

    private void OnEnable() { health.OnHealthChanged += Show; }
    private void OnDisable() { health.OnHealthChanged -= Show; }

    // Update the fill and the number.
    private void Show(int current, int max)
    {
        float fraction = max > 0 ? (float)current / max : 0f;
        fill.localScale = new Vector3(fraction, 1f, 1f);
        valueText.text = prefix + "  " + current + " / " + max;
    }
}
