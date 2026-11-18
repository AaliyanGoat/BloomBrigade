using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Level Select: one button per level. Locked levels are greyed out; won levels show their best stars.
public class LevelSelectScreen : MonoBehaviour
{
    [SerializeField] private Button[] levelButtons;      // in level order
    [SerializeField] private TextMeshProUGUI[] labels;   // "1  Meadow" or "Locked"
    [SerializeField] private TextMeshProUGUI[] starLabels;
    [SerializeField] private Color lockedColor = new Color(0.6f, 0.6f, 0.6f);
    [SerializeField] private Color openColor = Color.white;

    // Each button loads its own level.
    private void Awake()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int index = i;   // copy, so each lambda keeps its own number
            levelButtons[i].onClick.AddListener(() => SceneLoader.LoadLevel(index));
        }
    }

    // Refresh every time the screen opens (a level may have just been unlocked).
    private void OnEnable()
    {
        var levels = GameCatalog.Instance.levels;
        for (int i = 0; i < levelButtons.Length; i++)
        {
            bool unlocked = i < SaveSystem.Data.unlockedLevel && i < levels.Count;
            levelButtons[i].interactable = unlocked;
            levelButtons[i].image.color = unlocked ? openColor : lockedColor;
            labels[i].text = unlocked ? $"{i + 1}  {levels[i].levelName}" : $"{i + 1}  Locked";
            starLabels[i].text = Stars(SaveSystem.GetStars(i));
        }
    }

    // 2 → "Stars 2/3" (the font has no star symbol)
    private static string Stars(int count) => $"Stars {count}/3";
}
