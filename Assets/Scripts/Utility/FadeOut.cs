using UnityEngine;
using UnityEngine.Events;

public class FadeOut : MonoBehaviour
{
    public UnityEngine.UI.Image image;

    public AnimationCurve fadePattern;
    private float startTime;
    public UnityEvent FadeComplete = null;

    private void Awake()
    {
        startTime = Time.time;
    }
    void Update()
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Clamp01(fadePattern.Evaluate(Time.time - startTime)));
        if (image.color.a == fadePattern.keys[fadePattern.keys.Length - 1].value)
        {
            if (FadeComplete != null)
                FadeComplete.Invoke();

            Destroy(this);
        }
    }
}
