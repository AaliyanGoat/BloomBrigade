using UnityEngine;

// Slides an object left and right around its start point (moving flowers and thorn gates).
// These objects have no Rigidbody, so moving the Transform is fine.
public class SlideMover : MonoBehaviour
{
    [SerializeField] private float range = 2f;   // units each side of the start point
    [SerializeField] private float speed = 1.5f; // units per second

    private Vector3 start;
    private float time;

    // Called by LevelLoader with the values from the LevelData.
    public void Setup(float slideRange, float slideSpeed)
    {
        range = slideRange;
        speed = slideSpeed;
        start = transform.position;
        time = 0f;
    }

    // PingPong goes 0 → 2·range → 0, so subtracting range gives -range → +range → -range.
    private void Update()
    {
        if (range <= 0f || GameManager.Instance.State != GameState.Playing) return;
        time += Time.deltaTime;
        float offset = Mathf.PingPong(time * speed, range * 2f) - range;
        transform.position = start + Vector3.right * offset;
    }

    // Restart: back to the start point.
    public void ResetForNewRun()
    {
        time = 0f;
        if (range > 0f) transform.position = start;
    }
}
