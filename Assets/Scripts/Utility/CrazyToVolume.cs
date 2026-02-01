using UnityEngine;

public class CrazyToVolume : MonoBehaviour
{
    public AudioSource audioSource;
    public AnimationCurve volumeByCrazy;

    // Update is called once per frame
    void Update()
    {
        audioSource.volume = volumeByCrazy.Evaluate(TableCameraLook.CrazyAngle);
    }
}
