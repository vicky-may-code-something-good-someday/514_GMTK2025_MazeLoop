using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer Instance { get; private set; }

    [Header("Audio Clips")]
    public AudioClip defaultMusic;
    public AudioClip intenseMusic;   // reserved for your intensity logic
    public AudioClip reverseMusic;

    [Header("References")]
    public RewindData rewindDate;    // (kept your name to match existing refs)

    [Header("Audio Sources")]
    [SerializeField] private AudioSource defaultSource;
    [SerializeField] private AudioSource reverseSource;

    [Header("Volume")]
    [Tooltip("Base music volume applied when that track is active.")]
    [Range(0f, 1f)][SerializeField] private float baseTrackVolume = 0.1f;

    [Tooltip("User-controlled master multiplier applied to active track(s).")]
    [Range(0f, 1f)][SerializeField] private float masterVolume = 1f;

    [Tooltip("How much to change volume when using the world buttons.")]
    [Range(0.01f, 0.5f)][SerializeField] private float volumeStep = 0.1f;

    private bool isUsingIntenseMusic = false;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Defensive: ensure sources exist
        if (!defaultSource) defaultSource = gameObject.AddComponent<AudioSource>();
        if (!reverseSource) reverseSource = gameObject.AddComponent<AudioSource>();

        // Set up default source
        defaultSource.clip = defaultMusic;
        defaultSource.loop = true;
        defaultSource.playOnAwake = false;
        defaultSource.volume = 0f; // will be set by ApplyVolumes
        defaultSource.Play();

        // Set up reverse source
        reverseSource.clip = reverseMusic;
        reverseSource.loop = true;
        reverseSource.playOnAwake = false;
        reverseSource.volume = 0f; // will be set by ApplyVolumes
        reverseSource.Play();

        // Initialize volumes based on current state
        ApplyVolumes();
    }

    private void Update()
    {
        if (rewindDate == null || GameManager.GM == null) return;

        // (Optional) if you later add intensity logic, you can branch here

        // Keep volumes in sync with rewind state
        ApplyVolumes();
    }

    /// <summary>
    /// Computes per-source volume from baseTrackVolume * masterVolume,
    /// and mutes the inactive source.
    /// </summary>
    private void ApplyVolumes()
    {
        float activeVol = Mathf.Clamp01(baseTrackVolume) * Mathf.Clamp01(masterVolume);

        if (rewindDate.isRewinding)
        {
            defaultSource.volume = 0f;
            reverseSource.volume = activeVol;
        }
        else
        {
            defaultSource.volume = activeVol;
            reverseSource.volume = 0f;
        }
    }

    /// <summary>
    /// Increase overall music volume (for world buttons).
    /// </summary>
    public void IncreaseVolume()
    {
        masterVolume = Mathf.Clamp01(masterVolume + volumeStep);
        ApplyVolumes();
    }

    /// <summary>
    /// Decrease overall music volume (for world buttons).
    /// </summary>
    public void DecreaseVolume()
    {
        masterVolume = Mathf.Clamp01(masterVolume - volumeStep);
        ApplyVolumes();
    }

    /// <summary>
    /// Optional helper if you want to set a specific volume from scripts/UI.
    /// </summary>
    public void SetMasterVolume(float value01)
    {
        masterVolume = Mathf.Clamp01(value01);
        ApplyVolumes();
    }

    public float GetMasterVolume() => masterVolume;

#if UNITY_EDITOR
    private void OnValidate()
    {
        baseTrackVolume = Mathf.Clamp01(baseTrackVolume);
        masterVolume = Mathf.Clamp01(masterVolume);
        volumeStep = Mathf.Clamp(volumeStep, 0.01f, 0.5f);

        // Keep inspector tweaks live in edit/play mode
        if (defaultSource && reverseSource)
            ApplyVolumes();
    }
#endif
}
