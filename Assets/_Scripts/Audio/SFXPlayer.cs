using UnityEngine;

[DisallowMultipleComponent]
public class SFXPlayer : MonoBehaviour
{
    [Tooltip("Audio clip to be played as sound effect")]
    [SerializeField] private AudioClip sfxClip;

    [Header("Optional")]
    [Range(0f, 1f)] public float volumeOverride = -1f; // -1 uses manager default
    public bool usePitchVariance = true;
    [Range(0f, 0.5f)] public float pitchVariation = 0.05f;
    public float pitchCenter = 1f;

    /// <summary>
    /// Ask the global SFXManager to play this clip at the current position.
    /// Safe even if this object is disabled immediately afterwards.
    /// </summary>
    public void PlaySFX()
    {
        if (sfxClip == null)
        {
            Debug.LogWarning("SFXPlayer: No AudioClip assigned.");
            return;
        }

        if (SFXManager.Instance == null)
        {
            // Auto-create a manager if none exists
            var go = new GameObject("SFXManager_Auto");
            go.AddComponent<SFXManager>();
        }

        if (usePitchVariance)
        {
            SFXManager.Instance.PlayWithVariance(sfxClip, transform.position, volumeOverride, pitchCenter, pitchVariation);
        }
        else
        {
            SFXManager.Instance.PlayOneShotAt(sfxClip, transform.position, volumeOverride, pitchCenter);
        }
    }
}
