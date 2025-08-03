using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXPlayer : MonoBehaviour
{
    [Tooltip("Audio clip to be played as sound effect")]
    [SerializeField] private AudioClip sfxClip;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    /// <summary>
    /// Plays the assigned sound effect once.
    /// </summary>
    public void PlaySFX()
    {
        if (sfxClip != null)
        {
            audioSource.PlayOneShot(sfxClip);
        }
        else
        {
            Debug.LogWarning("SFXPlayer: No AudioClip assigned.");
        }
    }
}
