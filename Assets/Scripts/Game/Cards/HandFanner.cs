using UnityEngine;

[RequireComponent(typeof(Player))]
public class HandFanner : MonoBehaviour
{
    [Header("Adaptive Fan Angle")]
    public float MinFanAngle = 12f;
    public float MaxFanAngle = 60f;
    [Range(2, 12)] public int FullHandCount = 12;

    [Header("Fan Shape")]
    public float FanRadius = 2.5f;
    public float CardDepthOffset = 0.01f;

    [Header("Rotation")]
    public float MaxZRoll = 12f;
    public float BasePitch = 0f;
    public float BaseYaw = 0f;

    [Header("Offsets")]
    public Vector3 HandCenterOffset = Vector3.zero;

    [Header("Selection")]
    [Tooltip("Which card index is currently selected. -1 means none selected.")]
    public int SelectedIndex = 0;
    public bool ShowSelected = false;

    [Tooltip("How much the selected card lifts up (local Y).")]
    public float SelectedLift = 0.4f;

    [Tooltip("How much closer the selected card comes toward camera (local Z). Usually small.")]
    public float SelectedForward = -0.05f;

    [Tooltip("Optional extra scale for selected card.")]
    public float SelectedScale = 1.05f;

    [Tooltip("Optional extra roll reduction (0..1). 1 = remove roll on selected card.")]
    [Range(0f, 1f)]
    public float SelectedRollReduction = 0.35f;

    [Tooltip("Optional extra pitch applied to selected card.")]
    public float SelectedExtraPitch = -4f;

    private Player _player;

    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    private void LateUpdate()
    {
        FanHand();
    }

    /// <summary>Call when the hand changes (or let AnimateContinuously handle it).</summary>
    public void FanHand()
    {
        var hand = _player.Hand;
        int cardCount = hand.Count;

        if (cardCount == 0)
        {
            SelectedIndex = -1;
            return;
        }

        // Keep SelectedIndex valid even as cards are removed.
        if (SelectedIndex >= cardCount) SelectedIndex = cardCount - 1;

        float t = Mathf.InverseLerp(2f, FullHandCount, cardCount);
        float effectiveFanAngle = Mathf.Lerp(MinFanAngle, MaxFanAngle, t);

        if (cardCount == 1)
        {
            bool isSelected = (SelectedIndex == 0);
            ApplyLayout(hand[0], 0, 1, 0f, isSelected);
            return;
        }

        float angleStep = effectiveFanAngle / (cardCount - 1);
        float startAngle = -effectiveFanAngle / 2f;
        float mid = (cardCount - 1) / 2f;

        for (int i = 0; i < cardCount; i++)
        {
            float yawAngle = startAngle + angleStep * i;
            bool isSelected = (i == SelectedIndex);

            ApplyLayout(hand[i], i, cardCount, yawAngle, isSelected, mid);
        }
    }

    private void ApplyLayout(Card card, int i, int cardCount, float yawAngle, bool isSelected, float mid = 0f)
    {
        // Position on arc
        float radians = yawAngle * Mathf.Deg2Rad;

        Vector3 localPos = new Vector3(
            Mathf.Sin(radians) * FanRadius,
            0f,
            Mathf.Cos(radians) * FanRadius
        );

        localPos += HandCenterOffset;
        localPos += Vector3.forward * (i * CardDepthOffset);

        // Base roll
        float zRoll = 0f;
        if (cardCount > 1)
        {
            float normalizedIndex = (i - mid) / mid; // -1..+1
            zRoll = normalizedIndex * MaxZRoll;
        }

        // Selection adjustments
        float rollForThisCard = zRoll;
        float extraPitch = 0f;
        Vector3 extraPos = Vector3.zero;
        Vector3 targetScale = Vector3.one;

        if (ShowSelected && isSelected)
        {
            extraPos += Vector3.up * SelectedLift;
            extraPos += Vector3.forward * SelectedForward;

            rollForThisCard = Mathf.Lerp(zRoll, 0f, SelectedRollReduction);
            extraPitch += SelectedExtraPitch;

            targetScale = Vector3.one * SelectedScale;
        }

        localPos += extraPos;

        Quaternion localRot = Quaternion.Euler(
            BasePitch + extraPitch,
            BaseYaw + yawAngle,
            -rollForThisCard
        );

        card.positioner.SetPosition(localPos, localRot, targetScale);
    }

    // --- Selection API ---

    public void SelectNext()
    {
        ShowSelected = true;

        int count = _player.Hand.Count;
        if (count == 0) { SelectedIndex = -1; return; }
        SelectedIndex = (SelectedIndex + 1) % count;
    }

    public void SelectPrevious()
    {
        ShowSelected = true;

        int count = _player.Hand.Count;
        if (count == 0) { SelectedIndex = -1; return; }
        SelectedIndex = (SelectedIndex - 1 + count) % count;
    }

    public void SelectIndex(int index)
    {
        int count = _player.Hand.Count;
        if (count == 0) { SelectedIndex = -1; return; }
        SelectedIndex = Mathf.Clamp(index, 0, count - 1);
    }

    /// <summary>Gets the selected card instance (or null).</summary>
    public Card GetSelectedCard()
    {
        if (SelectedIndex < 0) return null;
        if (SelectedIndex >= _player.Hand.Count) return null;
        return _player.Hand[SelectedIndex];
    }
}
