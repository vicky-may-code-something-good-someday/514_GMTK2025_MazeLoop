using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer Instance { get; private set; }

    [Header("Audio Clips")]
    public AudioClip defaultMusic;
    public AudioClip intenseMusic;
    public AudioClip reverseMusic;

    [Header("References")]
    public RewindData rewindDate;

    private AudioSource defaultSource;
    private AudioSource reverseSource;

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

        // Create two audio sources
        defaultSource = gameObject.AddComponent<AudioSource>();
        reverseSource = gameObject.AddComponent<AudioSource>();

        // Set up default source
        defaultSource.clip = defaultMusic;
        defaultSource.loop = true;
        defaultSource.volume = 0.5f;
        defaultSource.Play();

        // Set up reverse source
        reverseSource.clip = reverseMusic;
        reverseSource.loop = true;
        reverseSource.volume = 0f;
        reverseSource.Play();
    }

    private void Update()
    {
        if (rewindDate == null || GameManager.GM == null) return;

        // Handle rewind audio swapping
        if (rewindDate.isRewinding)
        {
            defaultSource.volume = 0f;
            reverseSource.volume = 0.5f;
        }
        else
        {
            defaultSource.volume = 0.5f;
            reverseSource.volume = 0f;
        }

        // Handle music intensity swap
        float gameTime = GameManager.GM.gameTime;

        if (gameTime > 20f && !isUsingIntenseMusic)
        {
            defaultSource.Stop();
            defaultSource.clip = intenseMusic;
            defaultSource.Play();
            isUsingIntenseMusic = true;
        }
        else if (gameTime <= 20f && isUsingIntenseMusic)
        {
            defaultSource.Stop();
            defaultSource.clip = defaultMusic;
            defaultSource.Play();
            isUsingIntenseMusic = false;
        }
    }
}

