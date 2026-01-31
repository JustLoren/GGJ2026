using UnityEngine;

[RequireComponent(typeof(Player))]
public class HandFanner : MonoBehaviour
{
    [Header("Adaptive Fan Angle")]
    [Tooltip("Fan angle when the hand is very small (e.g., 1-2 cards).")]
    public float MinFanAngle = 12f;

    [Tooltip("Fan angle when the hand is 'full' (FullHandCount cards).")]
    public float MaxFanAngle = 60f;

    [Tooltip("How many cards counts as a 'full hand' for the purpose of MaxFanAngle.")]
    [Range(2, 12)]
    public int FullHandCount = 12;

    [Header("Fan Shape")]
    [Tooltip("Radius of the arc used to position cards.")]
    public float FanRadius = 2.5f;

    [Tooltip("Vertical offset per card (prevents Z-fighting).")]
    public float CardDepthOffset = 0.01f;

    [Header("Rotation")]
    [Tooltip("Max roll (Z axis) applied to the outermost cards. Middle card will be near 0.")]
    public float MaxZRoll = 12f;

    [Tooltip("Optional extra pitch so the hand faces camera a bit (X axis).")]
    public float BasePitch = 0f;

    [Tooltip("Optional base yaw for the whole hand (Y axis).")]
    public float BaseYaw = 0f;

    [Header("Offsets")]
    public Vector3 HandCenterOffset = Vector3.zero;

    [Header("Smoothing")]
    [Tooltip("If 0, snap instantly. If > 0, use smoothing.")]
    public float LerpSpeed = 12f;

    [Tooltip("If true, will keep animating toward target positions/rotations every frame.")]
    public bool AnimateContinuously = true;

    private Player _player;

    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    private void LateUpdate()
    {
        if (AnimateContinuously)
            FanHand();
    }

    /// <summary>
    /// Call this whenever the hand changes (or let AnimateContinuously do it).
    /// </summary>
    public void FanHand()
    {
        var hand = _player.Hand;
        int cardCount = hand.Count;

        if (cardCount == 0)
            return;

        // Determine adaptive fan angle based on card count.
        // 2 cards => MinFanAngle-ish, FullHandCount cards => MaxFanAngle.
        float t = Mathf.InverseLerp(2f, FullHandCount, cardCount);
        float effectiveFanAngle = Mathf.Lerp(MinFanAngle, MaxFanAngle, t);

        // Single card: center it
        if (cardCount == 1)
        {
            PositionCard(hand[0], HandCenterOffset, Quaternion.Euler(BasePitch, BaseYaw, 0f));
            return;
        }

        float angleStep = effectiveFanAngle / (cardCount - 1);
        float startAngle = -effectiveFanAngle / 2f;

        // Used to create a -1..+1 index range for Z roll
        float mid = (cardCount - 1) / 2f;

        for (int i = 0; i < cardCount; i++)
        {
            float yawAngle = startAngle + angleStep * i;

            // Arc position
            float radians = yawAngle * Mathf.Deg2Rad;
            Vector3 localPos = new Vector3(
                Mathf.Sin(radians) * FanRadius,
                0f,
                Mathf.Cos(radians) * FanRadius
            );

            localPos += HandCenterOffset;
            localPos += Vector3.forward * (i * CardDepthOffset);

            // Roll (Z axis): outer cards tilt more, center tilts least.
            // normalizedIndex: -1 (left edge) to +1 (right edge)
            float normalizedIndex = (i - mid) / mid;
            float zRoll = normalizedIndex * MaxZRoll;

            Quaternion localRot = Quaternion.Euler(
                BasePitch,
                BaseYaw + yawAngle,
                -zRoll
            );

            PositionCard(hand[i], localPos, localRot);
        }
    }

    private void PositionCard(Card card, Vector3 localPos, Quaternion localRot)
    {
        Transform t = card.transform;

        if (LerpSpeed > 0f)
        {
            float k = Time.deltaTime * LerpSpeed;
            t.localPosition = Vector3.Lerp(t.localPosition, localPos, k);
            t.localRotation = Quaternion.Slerp(t.localRotation, localRot, k);
        }
        else
        {
            t.localPosition = localPos;
            t.localRotation = localRot;
        }
    }
}
