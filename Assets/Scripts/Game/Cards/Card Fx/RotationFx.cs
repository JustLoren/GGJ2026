using UnityEngine;

public class RotationFx : MonoBehaviour, IMadnessFx
{
    public GameObject CardObject;

    [Header("Player Input")]
    public float RotationForce = 120f;      // impulse added to angular velocity (deg/sec) per Engage()

    [Header("Spring Back To Neutral")]
    public float ReturnStrength = 30f;      // higher = stronger pull back toward neutral (like a spring)
    public float RotationDecay = 15f;       // higher = more damping / less oscillation

    private float CurrentRotationMomentum;  // angular velocity around Y, in deg/sec
    private float neutralYaw;               // neutral local yaw angle (degrees)

    public MadnessFxType FxType => MadnessFxType.Rotation;

    private void Awake()
    {
        if (CardObject == null) CardObject = gameObject;
        var neutralRotation = CardObject.transform.localRotation * Quaternion.AngleAxis(180, Vector3.up);

        // Store a single neutral yaw angle so we can measure error cleanly.
        neutralYaw = NormalizeAngle(neutralRotation.eulerAngles.y);
    }

    public void Engage()
    {
        this.enabled = true;
    }

    public void AddRotation()
    {
        // Add momentum. Use RotationForce sign to decide direction.
        // (If you need left/right buttons, call Engage(+1) / Engage(-1) or set RotationForce accordingly.)
        CurrentRotationMomentum += RotationForce;
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        // Current yaw & error from neutral
        float yaw = NormalizeAngle(CardObject.transform.localEulerAngles.y);
        float error = NormalizeAngle(yaw - neutralYaw); // positive means rotated +Y away from neutral

        // Spring force (accelerates momentum back toward neutral)
        // This is the "continually trying to return to neutral" part.
        // Think: angularAcceleration = -k * error
        float springAccel = -ReturnStrength * error;

        // Integrate spring into momentum (angular velocity)
        CurrentRotationMomentum += springAccel * dt;

        // Damping: bleeds off momentum over time (prevents endless oscillation)
        CurrentRotationMomentum = Mathf.MoveTowards(CurrentRotationMomentum, 0f, RotationDecay * dt * 90f);

        // Apply momentum to rotation
        yaw += CurrentRotationMomentum * dt;

        CardObject.transform.localRotation = Quaternion.Euler(0f, yaw, 0f);
    }

    private static float NormalizeAngle(float degrees)
    {
        // Convert 0..360 into -180..180 for stable error math
        degrees %= 360f;
        if (degrees > 180f) degrees -= 360f;
        if (degrees < -180f) degrees += 360f;
        return degrees;
    }
}
