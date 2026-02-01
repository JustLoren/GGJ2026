using UnityEngine;
using UnityEngine.UI;

public class CrazyToImageFill : MonoBehaviour
{
    public Image image;
    public Gradient colorGradient;

    void Update()
    {
        image.fillAmount = HumanPlayer.CurrentSanityScore;
        image.color = colorGradient.Evaluate(image.fillAmount);
    }
}
