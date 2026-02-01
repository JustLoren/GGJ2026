using UnityEngine;

public class CutoutButton : MonoBehaviour
{
    public UnityEngine.UI.Image buttonGraphic;
    public float alphaTestThreshold;
    private void Awake()
    {
        if (buttonGraphic == null) buttonGraphic = GetComponentInChildren<UnityEngine.UI.Image>();
        buttonGraphic.alphaHitTestMinimumThreshold = alphaTestThreshold;
    }
}
