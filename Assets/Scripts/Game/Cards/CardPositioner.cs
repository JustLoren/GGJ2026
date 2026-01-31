using UnityEngine;

public class CardPositioner : MonoBehaviour
{

    public float LerpSpeed = 12f;
    private Vector3 targetPosition, targetScale;
    private Quaternion targetRotation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Update()
    {
        PositionCard();
    }

    public void SetPosition(Vector3 targetPos, Quaternion targetRot, Vector3 targetScale)
    {
        this.targetPosition = targetPos;
        this.targetRotation = targetRot;
        this.targetScale = targetScale;
    }

    private void PositionCard()
    {
        Transform t = transform;

        if (LerpSpeed > 0f)
        {
            float k = Time.deltaTime * LerpSpeed;
            t.localPosition = Vector3.Lerp(t.localPosition, targetPosition, k);
            t.localRotation = Quaternion.Slerp(t.localRotation, targetRotation, k);
            t.localScale = Vector3.Lerp(t.localScale, targetScale, k);
        }
        else
        {
            t.localPosition = targetPosition;
            t.localRotation = targetRotation;
            t.localScale = targetScale;
        }
    }
}
