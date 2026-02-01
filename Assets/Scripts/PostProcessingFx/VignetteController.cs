using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // URP Vignette lives here (also works in many HDRP setups, but HDRP may use a different namespace)

public class VignetteController : MonoBehaviour
{
    [Header("Assign one of these")]
    [SerializeField] private Volume volume;              // Drag a Volume here (recommended)
    [SerializeField] private string volumeObjectName;     // Or find by name at runtime

    private VolumeProfile _profile;
    private Vignette _vignette;

    private void Awake()
    {
        EnsureVolumeAndProfile();
        EnsureVignette();
    }

    /// <summary>
    /// Call this to set vignette intensity (0..1 typically).
    /// </summary>
    public void SetVignetteIntensity(float intensity)
    {
        if (!EnsureReady()) return;

        intensity = Mathf.Clamp01(intensity);

        // Make sure the override is actually applied
        _vignette.active = true;
        _vignette.intensity.overrideState = true;
        _vignette.intensity.value = intensity;
    }

    private bool EnsureReady()
    {
        if (volume == null || _profile == null || _vignette == null)
        {
            EnsureVolumeAndProfile();
            EnsureVignette();
        }
        return volume != null && _profile != null && _vignette != null;
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

    private void EnsureVignette()
    {
        if (_profile == null) return;

        if (!_profile.TryGet(out _vignette) || _vignette == null)
        {
            // If your profile doesn't have Vignette yet, add it.
            _vignette = _profile.Add<Vignette>(true);
        }

        // Ensure override is enabled
        _vignette.intensity.overrideState = true;
        _vignette.active = true;
    }

    [SerializeField] private float flickerSpeed = 25f;
    [SerializeField] private AnimationCurve flickerMin;
    [SerializeField] private AnimationCurve flickerMax;

    private void Update()
    {
        // Simple noise flicker
        float t = Time.time * flickerSpeed;
        float n = Mathf.PerlinNoise(t, 0.123f);
        SetVignetteIntensity(Mathf.Lerp(flickerMin.Evaluate(1f), flickerMax.Evaluate(1f), n));
    }

}
