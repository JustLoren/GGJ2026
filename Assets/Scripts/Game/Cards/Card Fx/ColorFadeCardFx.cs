using UnityEngine;

public class ColorFadeCardFx : MonoBehaviour, IMadnessFx
{
    public MeshRenderer CardNumber;
    private Color originalColor;

    public MadnessFxType FxType => MadnessFxType.ColorFade;

    private float currentFadeAmount = 0f;
    public float fadeRate = 1f;
    public float lightUpRate = .25f;
    private void Init()
    {
        this.enabled = true;
        currentFadeAmount = 0f;
        originalColor = CardNumber.material.color;
        CardNumber.material.color = new Color(originalColor.r, originalColor.g, originalColor.b, currentFadeAmount);
    }

    public void Engage()
    {
        Init();
    }

    public void ApplyFade()
    {
        currentFadeAmount = Mathf.Clamp01(currentFadeAmount + lightUpRate);
    }

    private void Update()
    {
        CardNumber.material.color = new Color(originalColor.r, originalColor.g, originalColor.b, currentFadeAmount);

        currentFadeAmount = Mathf.Clamp01(currentFadeAmount - fadeRate * Time.deltaTime);
    }
}
