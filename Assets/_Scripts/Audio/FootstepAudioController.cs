using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FootstepAudioController : MonoBehaviour
{
    [Header("References")]
    public CharacterController_FirstPerson characterController;

    [Header("Footstep Settings")]
    public AudioClip[] footstepClips;
    public float stepInterval = 0.5f;
    public float maxVolume = 1.0f;
    public float speedThreshold = 0.1f;

    private AudioSource audioSource;
    private float stepTimer;

    void Start()
    {
        if (characterController == null)
        {
            Debug.LogError("CharacterController_FirstPerson reference is missing.");
            enabled = false;
            return;
        }

        audioSource = GetComponent<AudioSource>();
        stepTimer = 0f;
    }

    void Update()
    {
        Vector3 velocity = characterController.velocity;
        velocity.y = 0; // Ignore vertical velocity
        float speed = velocity.magnitude;

        stepTimer += Time.deltaTime;

        if (stepTimer >= stepInterval)
        {
            stepTimer = 0f;

            if (speed > speedThreshold && footstepClips.Length > 0)
            {
                AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];
                audioSource.volume = Mathf.Lerp(0f, maxVolume, speed);
                audioSource.PlayOneShot(clip);
            }
        }
    }
}