using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class AberrationController : MonoBehaviour
{
    [Header("Assign one of these")]
    [SerializeField] private Volume volume;              // Drag a Volume here (recommended)
    [SerializeField] private string volumeObjectName;     // Or find by name at runtime

    private VolumeProfile _profile;
    private ChromaticAberration _aberration;

    private void Awake()
    {
        EnsureVolumeAndProfile();
        EnsureAberration();
    }

    /// <summary>
    /// Call this to set vignette intensity (0..1 typically).
    /// </summary>
    public void SetIntensity(float intensity)
    {
        if (!EnsureReady()) return;

        intensity = Mathf.Clamp01(intensity);

        // Make sure the override is actually applied
        _aberration.active = true;
        _aberration.intensity.overrideState = true;
        _aberration.intensity.value = intensity;
    }

    private bool EnsureReady()
    {
        if (volume == null || _profile == null || _aberration == null)
        {
            EnsureVolumeAndProfile();
            EnsureAberration();
        }
        return volume != null && _profile != null && _aberration != null;
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

    private void EnsureAberration()
    {
        if (_profile == null) return;

        if (!_profile.TryGet(out _aberration) || _aberration == null)
        {
            _aberration = _profile.Add<ChromaticAberration>(true);
        }

        // Ensure override is enabled
        _aberration.intensity.overrideState = true;
        _aberration.active = true;
    }

    [SerializeField] private float flickerSpeed = 25f;
    [SerializeField] private float flickerMin = 0.15f;
    [SerializeField] private float flickerMax = 0.45f;

    private void Update()
    {
        // Simple noise flicker
        float t = Time.time * flickerSpeed;
        float n = Mathf.PerlinNoise(t, 0.123f);
        SetIntensity(Mathf.Lerp(flickerMin, flickerMax, n));
    }
}
