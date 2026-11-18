using System;
using TMPro;
using UnityEngine;

// Feedback: a number such as "+5" that rises and fades, then returns to its pool.
public class FloatingText : MonoBehaviour
{
    [SerializeField] private TextMeshPro text;
    [SerializeField] private float riseSpeed = 1.5f;   // units per second
    [SerializeField] private float lifetime = 0.7f;

    private float age;
    private Color color;
    private Action<FloatingText> release;

    // Called by FeedbackManager when the text leaves the pool.
    public void Show(Vector3 position, string message, Color textColor, Quaternion facing, Action<FloatingText> onFinished)
    {
        transform.SetPositionAndRotation(position, facing);
        text.text = message;
        color = textColor;
        text.color = color;
        age = 0f;
        release = onFinished;
    }

    // Move up and fade; give itself back when its life is over.
    private void Update()
    {
        age += Time.deltaTime;
        transform.position += Vector3.up * (riseSpeed * Time.deltaTime);
        color.a = 1f - age / lifetime;
        text.color = color;
        if (age >= lifetime) release?.Invoke(this);
    }
}
