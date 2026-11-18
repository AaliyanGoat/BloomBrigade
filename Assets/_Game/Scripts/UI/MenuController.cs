using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Menu scene: Splash → Main Menu, and switching between Level Select, Shop and Settings.
// Only one screen is active at a time.
public class MenuController : MonoBehaviour
{
    [Header("Screens")]
    [SerializeField] private GameObject splash;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject levelSelect;
    [SerializeField] private GameObject shop;
    [SerializeField] private GameObject settings;

    [Header("Main menu buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button shopButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button[] backButtons;   // every "Back" goes to the main menu

    [SerializeField] private float splashTime = 1.6f;

    private static bool splashShown;   // the splash plays once per launch, not on every return

    // Wire buttons and choose the first screen.
    private void Start()
    {
        playButton.onClick.AddListener(() => Show(levelSelect));
        shopButton.onClick.AddListener(() => Show(shop));
        settingsButton.onClick.AddListener(() => Show(settings));
        quitButton.onClick.AddListener(Application.Quit);
        foreach (Button back in backButtons) back.onClick.AddListener(() => Show(mainMenu));

        if (SceneLoader.OpenShopOnMenu)
        {
            SceneLoader.ClearShopRequest();
            Show(shop);
        }
        else if (!splashShown)
        {
            StartCoroutine(SplashThenMenu());
        }
        else
        {
            Show(mainMenu);
        }
    }

    // Show the splash for splashTime seconds, then the main menu.
    private IEnumerator SplashThenMenu()
    {
        splashShown = true;
        Show(splash);
        yield return new WaitForSeconds(splashTime);
        Show(mainMenu);
    }

    // Turn on one screen and turn the others off.
    public void Show(GameObject screen)
    {
        foreach (GameObject s in new[] { splash, mainMenu, levelSelect, shop, settings })
            s.SetActive(s == screen);
    }
}
