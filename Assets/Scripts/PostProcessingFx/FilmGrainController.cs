using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FilmGrainController : MonoBehaviour
{
    [Header("Assign one of these")]
    [SerializeField] private Volume volume;              // Drag a Volume here (recommended)
    [SerializeField] private string volumeObjectName;     // Or find by name at runtime

    private VolumeProfile _profile;
    private FilmGrain _filmGrain;

    private void Awake()
    {
        EnsureVolumeAndProfile();
        EnsureFilmGrain();
    }

    /// <summary>
    /// Call this to set intensity (0..1 typically).
    /// </summary>
    public void SetIntensity(float intensity)
    {
        if (!EnsureReady()) return;

        intensity = Mathf.Clamp01(intensity);

        // Make sure the override is actually applied
        _filmGrain.active = true;
        _filmGrain.intensity.overrideState = true;
        _filmGrain.intensity.value = intensity;
    }

    private bool EnsureReady()
    {
        if (volume == null || _profile == null || _filmGrain == null)
        {
            EnsureVolumeAndProfile();
            EnsureFilmGrain();
        }
        return volume != null && _profile != null && _filmGrain != null;
    }

    private void EnsureVolumeAndProfile()
    {
        if (volume == null)
        {
            if (!string.IsNullOrEmpty(volumeObjectName))
            {
                var go = GameObject.Find(volumeObjectName);
                if (go != null) volume = go.GetComponent<Volume>();
            }
            else
            {
                volume = FindFirstObjectByType<Volume>(); // Unity 2023+. Use FindObjectOfType<Volume>() for older.
            }
        }

        if (volume == null)
        {
            Debug.LogError("No Volume found/assigned.");
            return;
        }

        // This gives you an *instance* so runtime edits don't modify the asset.
        _profile = volume.profile != null ? volume.profile : volume.sharedProfile;

        // If you're using sharedProfile and want to avoid editing the asset,
        // force an instance:
        if (volume.profile == null && volume.sharedProfile != null)
        {
            volume.profile = Instantiate(volume.sharedProfile);
            _profile = volume.profile;
        }
    }

    private void EnsureFilmGrain()
    {
        if (_profile == null) return;

        if (!_profile.TryGet(out _filmGrain) || _filmGrain == null)
        {
            _filmGrain = _profile.Add<FilmGrain>(true);
        }

        // Ensure override is enabled
        _filmGrain.intensity.overrideState = true;
        _filmGrain.active = true;
    }

    [SerializeField] private AnimationCurve flickerSpeed;
    [SerializeField] private AnimationCurve flickerMin;
    [SerializeField] private AnimationCurve flickerMax;

    private void Update()
    {
        // Simple noise flicker
        float t = Time.time * flickerSpeed.Evaluate(HumanPlayer.CurrentSanityScore);
        float n = Mathf.PerlinNoise(t, 0.123f);
        SetIntensity(Mathf.Lerp(flickerMin.Evaluate(HumanPlayer.CurrentSanityScore), flickerMax.Evaluate(HumanPlayer.CurrentSanityScore), n));
    }
}
