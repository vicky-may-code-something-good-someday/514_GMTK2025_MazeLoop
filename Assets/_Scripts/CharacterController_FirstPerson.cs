using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterController_FirstPerson : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 6f;

    [Header("Sprint Settings")]
    public bool ToggleToSprint = false;           // false = hold to sprint, true = press to sprint
    public float baseSprintSpeed = 9f;          // initial sprint speed
    public float maxSprintSpeed = 15f;          // maximum sprint speed
    public float sprintAcceleration = 1.7f;       // acceleration before sprintBurstThreshold
    public float sprintBurstAcceleration = 14f; // acceleration after sprintBurstThreshold
    public float sprintDecaySpeed = 3f;          // decay speed when not sprinting
    public float sprintBurstThreshold = 2.5f;      // time in seconds before burst kicks in

    [Header("Camera FOV Settings")]
    public Camera playerCamera;                  // assign your main camera here
    public float normalFOV = 60f;
    public float maxSprintFOV = 70f;
    public float fovChangeSpeed = 8f;           // how fast FOV changes

    [Header("Other Settings")]
    public float gravity = -9.81f;

    [Header("References")]
    public Transform groundCheck;
    public LayerMask groundMask;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    public float groundDistance = 0.4f;

    // Internal sprint state
    private bool sprinting = false;
    private float currentSprintSpeed;
    private float sprintTimer = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentSprintSpeed = baseSprintSpeed;

        if (playerCamera != null)
        {
            playerCamera.fieldOfView = normalFOV;
        }
    }

    void Update()
    {
        HandleSprintInput();
        HandleMovement();
        HandleFOV();
    }

    private void HandleSprintInput()
    {
        if (ToggleToSprint)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                sprinting = !sprinting;
            }
        }
        else
        {
            sprinting = Input.GetKey(KeyCode.LeftShift);
        }
    }

    void HandleMovement()
    {
        // Ground check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Get input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        bool isMoving = x != 0 || z != 0;

        if (sprinting && isMoving)
        {
            sprintTimer += Time.deltaTime;

            if (sprintTimer < sprintBurstThreshold)
            {
                // Gradual acceleration
                currentSprintSpeed += sprintAcceleration * Time.deltaTime;
            }
            else
            {
                // Radical burst acceleration
                currentSprintSpeed += sprintBurstAcceleration * Time.deltaTime;
            }

            currentSprintSpeed = Mathf.Min(currentSprintSpeed, maxSprintSpeed);
        }
        else
        {
            // Reset sprint timer and decay speed when not sprinting or not moving
            sprintTimer = 0f;
            currentSprintSpeed -= sprintDecaySpeed * Time.deltaTime;
            currentSprintSpeed = Mathf.Max(currentSprintSpeed, baseSprintSpeed);
        }

        float speed = sprinting && isMoving ? currentSprintSpeed : walkSpeed;

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);


        // Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleFOV()
    {
        if (playerCamera == null) return;

        float targetFOV = normalFOV;

        // Change FOV only during radical sprint burst phase
        if (sprinting && sprintTimer >= sprintBurstThreshold)
        {
            targetFOV = maxSprintFOV;
        }

        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, fovChangeSpeed * Time.deltaTime);
    }
}
