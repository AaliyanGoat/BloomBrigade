using TMPro;
using UnityEngine;

// Shop: refreshes every card whenever the save changes (after a purchase), and shows the hive's evolution.
public class ShopScreen : MonoBehaviour
{
    [SerializeField] private ShopCard[] cards;
    [SerializeField] private TextMeshProUGUI evolutionText;
    [SerializeField] private string[] stageNames = { "Basic Hive", "Blooming Hive", "Golden Hive", "Royal Hive" };

    private void OnEnable()
    {
        SaveSystem.OnChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        SaveSystem.OnChanged -= Refresh;
    }

    // Redraw all cards and the evolution line.
    private void Refresh()
    {
        foreach (ShopCard card in cards) card.Refresh();
        int stage = PlayerStats.EvolutionStage;
        int[] levels = GameCatalog.Instance.evolutionLevels;
        string next = stage < levels.Length ? $"Next evolution at Fire rate Lv {levels[stage]}" : "Fully evolved!";
        evolutionText.text = $"Your hive: {stageNames[Mathf.Min(stage, stageNames.Length - 1)]}\n{next}";
    }
}
