using TMPro;
using UnityEngine;
using UnityEngine.UI;

// One upgrade in the shop: name, level, value now → next, cost and a Buy button.
public class ShopCard : MonoBehaviour
{
    [SerializeField] private UpgradeData upgrade;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Button buyButton;

    // The base value this upgrade adds to (e.g. 4 bees/s), so the card can show real numbers.
    [SerializeField] private float baseValue;

    private void Awake()
    {
        buyButton.onClick.AddListener(Buy);
    }

    // Try to buy one level; play a sound either way.
    private void Buy()
    {
        bool bought = PlayerStats.Buy(upgrade);
        if (AudioManager.Instance == null) return;
        if (bought) AudioManager.Instance.PlayBuy(); else AudioManager.Instance.PlayError();
    }

    // Redraw the card from the save data.
    public void Refresh()
    {
        int level = PlayerStats.LevelOf(upgrade);
        float now = baseValue + upgrade.BonusAt(level);
        nameText.text = $"{upgrade.upgradeName}  Lv {level}";
        if (PlayerStats.IsMaxed(upgrade))
        {
            infoText.text = $"{now:0.#} {upgrade.unit}  (max)";
            costText.text = "MAX";
            buyButton.interactable = false;
            return;
        }
        float next = baseValue + upgrade.BonusAt(level + 1);
        infoText.text = $"{upgrade.description}\n{now:0.#} → {next:0.#} {upgrade.unit}";
        costText.text = PlayerStats.NextCost(upgrade).ToString();
        buyButton.interactable = PlayerStats.CanBuy(upgrade);
    }
}
