using UnityEngine.SceneManagement;
using UnityEngine;

// Moves between the Menu scene and the Game scene, and remembers which level to play.
public static class SceneLoader
{
    public const string MenuScene = "Menu";
    public const string GameScene = "Game";

    public static int LevelIndex { get; private set; }     // 0 = Level 1
    public static bool OpenShopOnMenu { get; private set; } // "Shop" pressed on the result screen

    // Start a level. timeScale goes back to 1 first, or the new scene would be frozen after Pause.
    public static void LoadLevel(int index)
    {
        LevelIndex = Mathf.Clamp(index, 0, GameCatalog.Instance.levels.Count - 1);
        Time.timeScale = 1f;
        SceneManager.LoadScene(GameScene);
    }

    // Back to the menu, optionally straight into the shop.
    public static void LoadMenu(bool openShop = false)
    {
        OpenShopOnMenu = openShop;
        Time.timeScale = 1f;
        SceneManager.LoadScene(MenuScene);
    }

    // The menu calls this after it has opened the shop, so it does not open again next time.
    public static void ClearShopRequest() => OpenShopOnMenu = false;
}
