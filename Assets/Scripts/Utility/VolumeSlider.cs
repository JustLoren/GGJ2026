using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class VolumeSlider : MonoBehaviour
{
    [Header("Audio")]
    public AudioMixer mixer;
    public string volumeParameter = "MasterVolume";

    [Header("Prefs")]
    public string playerPrefsKey = "MasterVolume";

    [Header("Tuning")]
    [Tooltip("dB at slider = 0. Typical: -80.")]
    public float minDb = -80f;

    [Tooltip("dB at slider = 1. Typical: 0.")]
    public float maxDb = 0f;

    [Tooltip("1 = linear feel, 2 = more resolution near the top, 0.5 = more near the bottom.")]
    public float sliderCurvePower = 2f;

    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();

        // Expect slider range 0..1
        slider.minValue = 0f;
        slider.maxValue = 1f;

        float saved = PlayerPrefs.GetFloat(playerPrefsKey, 1f);
        slider.SetValueWithoutNotify(saved);

        slider.onValueChanged.AddListener(Apply);
    }

    private void Start()
    {
        Apply(slider.value);
    }

    private void Apply(float t)
    {
        t = Mathf.Clamp01(t);

        // Shape the slider response so the user gets finer control where it matters.
        float shaped = Mathf.Pow(t, sliderCurvePower);

        // Map shaped 0..1 to minDb..maxDb
        float db = Mathf.Lerp(minDb, maxDb, shaped);

        mixer.SetFloat(volumeParameter, db);

        PlayerPrefs.SetFloat(playerPrefsKey, t);
        PlayerPrefs.Save();
    }
}
