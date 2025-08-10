using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    [Header("Pool Settings")]
    [SerializeField] private int initialPoolSize = 8;
    [SerializeField] private bool allowPoolExpansion = true;

    [Header("Audio Defaults")]
    [Range(0f, 1f)] public float defaultVolume = 1f;
    [Range(0f, 1f)] public float spatialBlend3D = 1f; // 0 = 2D, 1 = 3D
    public AudioRolloffMode rolloffMode = AudioRolloffMode.Linear;
    public float minDistance = 1f;
    public float maxDistance = 25f;
    public UnityEngine.Audio.AudioMixerGroup outputMixerGroup;

    private readonly List<AudioSource> pool = new List<AudioSource>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Prewarm();
    }

    private void Prewarm()
    {
        for (int i = 0; i < initialPoolSize; i++)
            pool.Add(CreateSource(i));
    }

    private AudioSource CreateSource(int index = -1)
    {
        var go = new GameObject(index >= 0 ? $"SFXSource_{index}" : "SFXSource");
        go.transform.SetParent(transform);
        var src = go.AddComponent<AudioSource>();
        src.playOnAwake = false;
        src.loop = false;
        src.spatialBlend = spatialBlend3D;
        src.rolloffMode = rolloffMode;
        src.minDistance = minDistance;
        src.maxDistance = maxDistance;
        src.outputAudioMixerGroup = outputMixerGroup;
        return src;
    }

    private AudioSource GetFreeSource()
    {
        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].isPlaying) return pool[i];
        }
        if (allowPoolExpansion)
        {
            var src = CreateSource();
            pool.Add(src);
            return src;
        }
        // Fallback: reuse the first one (least ideal, but guarantees a sound)
        return pool[0];
    }

    /// <summary>
    /// Play a clip once at a world position, unaffected by the caller being disabled.
    /// </summary>
    public void PlayOneShotAt(AudioClip clip, Vector3 position, float volume = -1f, float pitch = 1f)
    {
        if (clip == null) return;

        var src = GetFreeSource();
        src.transform.position = position;
        src.pitch = pitch;
        src.volume = volume >= 0f ? volume : defaultVolume;
        src.clip = clip;
        src.Play();
        // No coroutine needed; source is returned to pool automatically when isPlaying becomes false.
    }

    /// <summary>
    /// Optional helper: small random pitch variance for variety (e.g., 0.95–1.05).
    /// </summary>
    public void PlayWithVariance(AudioClip clip, Vector3 position, float volume = -1f, float pitchCenter = 1f, float pitchVariation = 0.05f)
    {
        float pitch = Random.Range(pitchCenter - pitchVariation, pitchCenter + pitchVariation);
        PlayOneShotAt(clip, position, volume, pitch);
    }
}
