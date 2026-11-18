using UnityEngine;

// Card M1: drag left / right to slide the hive cannon along X.
// The cannon has no Rigidbody, so moving its Transform directly is correct.
public class CannonController : MonoBehaviour
{
    [SerializeField] private GameTuning tuning;
    [SerializeField] private Transform model;   // the visible part that tilts

    private float startX;
    private float lastPointerX;
    private bool wasHolding;
    private float currentTilt;

    // Remember where the cannon starts so Restart can put it back.
    private void Awake()
    {
        startX = transform.position.x;
    }

    // Read the drag every frame and slide the cannon.
    private void Update()
    {
        bool playing = GameManager.Instance.State == GameState.Playing;
        bool holding = playing && PointerInput.IsHeld;
        float move = 0f;

        if (holding)
        {
            float pointerX = PointerInput.ScreenX;
            if (!wasHolding) lastPointerX = pointerX;   // new touch: no jump

            float wanted = (pointerX - lastPointerX) * tuning.dragSensitivity;
            float maxStep = tuning.maxSlideSpeed * Time.deltaTime;
            move = Mathf.Clamp(wanted, -maxStep, maxStep);
            lastPointerX = pointerX;

            Vector3 position = transform.position;
            position.x = Mathf.Clamp(position.x + move, tuning.minX, tuning.maxX);
            transform.position = position;
        }

        wasHolding = holding;
        UpdateTilt(move);
    }

    // Lean the model towards the direction it is moving (feedback for M1).
    private void UpdateTilt(float move)
    {
        float speed = Time.deltaTime > 0f ? move / Time.deltaTime : 0f;
        float targetTilt = -Mathf.Clamp(speed / tuning.maxSlideSpeed, -1f, 1f) * tuning.maxTilt;
        currentTilt = Mathf.Lerp(currentTilt, targetTilt, tuning.tiltSmoothing * Time.deltaTime);
        model.localRotation = Quaternion.Euler(0f, 0f, currentTilt);
    }

    // Back to the middle, standing straight.
    public void ResetForNewRun()
    {
        Vector3 position = transform.position;
        position.x = startX;
        transform.position = position;
        currentTilt = 0f;
        wasHolding = false;
        model.localRotation = Quaternion.identity;
    }
}
