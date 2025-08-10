using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FootstepAudioController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Auto-resolves to the nearest CharacterController in parents.")]
    public CharacterController controller; // auto-found in Awake()

    [Header("Audio")]
    public AudioClip[] footstepClips;
    public float maxVolume = 1.0f;
    public Vector2 pitchRange = new Vector2(0.97f, 1.03f);

    [Header("Speed → Stride (cadence)")]
    [Tooltip("Meters per step as a function of horizontal speed (m/s). Higher speed → longer steps (fewer footfalls).")]
    public AnimationCurve stepDistanceBySpeed = new AnimationCurve(); // will be auto-initialized if empty
    [Tooltip("Safety clamp on evaluated step distance (meters).")]
    public Vector2 stepDistanceClamp = new Vector2(0.25f, 1.0f);

    [Header("Speed → Volume")]
    [Tooltip("Below this speed (m/s), steps are silent.")]
    public float speedThreshold = 0.1f;
    [Tooltip("This speed (m/s) maps to maxVolume. Use your top sprint speed.")]
    public float maxSpeed = 15f;

    [Header("Smoothing")]
    [Tooltip("0..1: how quickly we smooth measured speed for stable volume/cadence.")]
    [Range(0f, 1f)] public float speedSmooth = 0.25f;

    [Header("Debug")]
    public bool debugLog = false;

    private AudioSource audioSource;
    private Transform mover;
    private Vector3 lastPos;
    private float distanceAccum;
    private float smoothedSpeed;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;

        if (controller == null)
            controller = GetComponentInParent<CharacterController>();
        if (controller == null)
        {
            Debug.LogError("[Footsteps] No CharacterController found in parents. Disabling.");
            enabled = false;
            return;
        }

        mover = controller.transform;
        lastPos = mover.position;

        // Provide a good default curve if none set in Inspector
        if (stepDistanceBySpeed == null || stepDistanceBySpeed.keys == null || stepDistanceBySpeed.keys.Length < 2)
        {
            // Defaults: at rest ~0.42m, at jog(6 m/s) ~0.55m, at sprint(12 m/s) ~0.75m, at max(15) ~0.85m
            stepDistanceBySpeed = new AnimationCurve(
                new Keyframe(0f, 0.42f, 0f, 0.06f),
                new Keyframe(3f, 0.48f, 0.04f, 0.04f),
                new Keyframe(6f, 0.55f, 0.03f, 0.03f),
                new Keyframe(12f, 0.75f, 0.02f, 0.02f),
                new Keyframe(15f, 0.85f, 0.01f, 0.01f)
            );
        }

        if (footstepClips == null || footstepClips.Length == 0)
            Debug.LogWarning("[Footsteps] No clips assigned.");
    }

    void Update()
    {
        // Horizontal motion measured from the controller root
        Vector3 current = mover.position;
        Vector3 delta = current - lastPos; delta.y = 0f;

        float frameDistance = delta.magnitude;
        float dt = Time.deltaTime > 0f ? Time.deltaTime : 0.0001f;
        float rawSpeed = frameDistance / dt;

        // Smooth speed for stable volume/cadence
        smoothedSpeed = Mathf.Lerp(smoothedSpeed, rawSpeed, Mathf.Clamp01(speedSmooth));

        bool grounded = controller.isGrounded;

        if (grounded && smoothedSpeed > speedThreshold && footstepClips != null && footstepClips.Length > 0)
        {
            distanceAccum += frameDistance;

            // Evaluate stride length from curve, clamp to safety range
            float desiredStepDist = stepDistanceBySpeed.Evaluate(smoothedSpeed);
            desiredStepDist = Mathf.Clamp(desiredStepDist, stepDistanceClamp.x, stepDistanceClamp.y);

            // Don’t let degenerate tiny values spam
            desiredStepDist = Mathf.Max(0.05f, desiredStepDist);

            if (distanceAccum >= desiredStepDist)
            {
                distanceAccum = 0f;

                // Speed → 0..1 loudness
                float t = Mathf.InverseLerp(speedThreshold, maxSpeed, smoothedSpeed);
                audioSource.volume = t * maxVolume;

                // Random pitch for variety
                audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);

                // Fire
                var clip = footstepClips[Random.Range(0, footstepClips.Length)];
                audioSource.PlayOneShot(clip);

                if (debugLog)
                    Debug.Log($"[Footsteps] STEP  speed={smoothedSpeed:F2} m/s | stepDist={desiredStepDist:F2} m | vol={audioSource.volume:F2} | {clip.name}");
            }
        }
        else
        {
            // Avoid popping a step right after landing
            distanceAccum = Mathf.Max(0f, distanceAccum - frameDistance * 0.5f);
        }

        lastPos = current;
    }
}
