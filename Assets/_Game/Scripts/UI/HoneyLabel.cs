using TMPro;
using UnityEngine;

// Shows the honey in the bank (menus and shop) and updates when it changes.
public class HoneyLabel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;

    private void OnEnable()
    {
        SaveSystem.OnChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        SaveSystem.OnChanged -= Refresh;
    }

    private void Refresh() => label.text = PlayerStats.Honey.ToString();
}
