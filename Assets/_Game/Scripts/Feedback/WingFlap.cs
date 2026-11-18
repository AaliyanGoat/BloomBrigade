using UnityEngine;

// Flaps two wing transforms back and forth. Cheaper than an Animator on hundreds of bees.
public class WingFlap : MonoBehaviour
{
    [SerializeField] private Transform leftWing;
    [SerializeField] private Transform rightWing;
    [SerializeField] private float flapSpeed = 30f;     // radians per second
    [SerializeField] private float flapAngle = 35f;     // degrees each side
    [SerializeField] private Vector3 flapAxis = Vector3.right;

    private Quaternion leftRest;
    private Quaternion rightRest;
    private float phase;

    // Remember the resting rotation and start each bee at a random point so they do not flap in sync.
    private void Awake()
    {
        leftRest = leftWing.localRotation;
        rightRest = rightWing.localRotation;
        phase = Random.value * Mathf.PI * 2f;
    }

    // Rotate both wings by a sine wave, mirrored.
    private void Update()
    {
        float angle = Mathf.Sin(Time.time * flapSpeed + phase) * flapAngle;
        leftWing.localRotation = leftRest * Quaternion.AngleAxis(angle, flapAxis);
        rightWing.localRotation = rightRest * Quaternion.AngleAxis(-angle, flapAxis);
    }
}
