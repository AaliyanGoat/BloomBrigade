using UnityEngine;
using UnityEngine.UI;

// Put on any Button: plays the click sound through the AudioManager when it is pressed.
[RequireComponent(typeof(Button))]
public class UIButtonSound : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            if (AudioManager.Instance != null) AudioManager.Instance.PlayClick();
        });
    }
}
