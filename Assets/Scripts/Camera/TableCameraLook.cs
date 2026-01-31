using UnityEngine;
using UnityEngine.InputSystem;

public class TableCameraLook : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private string lookActionName = "Look";

    [Header("Sensitivity")]
    [SerializeField] private float sensitivityX = 0.12f; // degrees per pixel-ish (tune)
    [SerializeField] private float sensitivityY = 0.12f;
    [SerializeField] private bool invertY = false;

    [Header("Limits (degrees)")]
    [SerializeField] private float maxYawLeft = 45f;
    [SerializeField] private float maxYawRight = 45f;
    [SerializeField] private float maxPitchUp = 20f;
    [SerializeField] private float maxPitchDown = 25f;

    [Header("Smoothing (optional)")]
    [Tooltip("0 = no smoothing. Higher = snappier smoothing.")]
    [SerializeField] private float smoothing = 0f;

    [Header("Cursor")]
    [SerializeField] private bool lockCursor = true;

    private InputAction lookAction;

    // We store yaw/pitch relative to the starting rotation (seated orientation).
    private float yaw;
    private float pitch;

    private float yawVelocity;
    private float pitchVelocity;

    private void Reset()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    private void Awake()
    {
        if (!playerInput)
            playerInput = GetComponentInParent<PlayerInput>();

        if (!playerInput)
            Debug.LogError($"{nameof(TableCameraLook)} needs a PlayerInput reference (or on same GameObject).");
    }

    private void OnEnable()
    {
        if (playerInput)
        {
            lookAction = playerInput.actions[lookActionName];
            if (lookAction == null)
                Debug.LogError($"Could not find action '{lookActionName}'. Check your Input Actions asset.");
            else
                lookAction.Enable();
        }

        ApplyCursorState(lockCursor);
    }

    private void OnDisable()
    {
        lookAction?.Disable();
        ApplyCursorState(false);
    }

    private void Start()
    {
        // Initialize relative yaw/pitch as 0 (looking straight ahead from initial pose).
        yaw = 0f;
        pitch = 0f;
    }

    private void Update()
    {
        if (lookAction == null) return;

        // Mouse delta is typically in pixels per frame; multiply by sensitivity.
        Vector2 look = lookAction.ReadValue<Vector2>();

        float dx = look.x * sensitivityX;
        float dy = look.y * sensitivityY * (invertY ? 1f : -1f);

        float targetYaw = yaw + dx;
        float targetPitch = pitch + dy;

        // Clamp to "human at table" limits
        targetYaw = Mathf.Clamp(targetYaw, -maxYawLeft, maxYawRight);
        targetPitch = Mathf.Clamp(targetPitch, -maxPitchUp, maxPitchDown);

        if (smoothing <= 0f)
        {
            yaw = targetYaw;
            pitch = targetPitch;
        }
        else
        {
            // SmoothDampAngle gives nice easing; smoothing is "response speed" here.
            float smoothTime = 1f / Mathf.Max(0.0001f, smoothing);
            yaw = Mathf.SmoothDampAngle(yaw, targetYaw, ref yawVelocity, smoothTime);
            pitch = Mathf.SmoothDampAngle(pitch, targetPitch, ref pitchVelocity, smoothTime);
        }

        // Apply relative rotation on top of the original orientation
        // For a seated view, local rotation is usually what you want.
        transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    private static void ApplyCursorState(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
}
