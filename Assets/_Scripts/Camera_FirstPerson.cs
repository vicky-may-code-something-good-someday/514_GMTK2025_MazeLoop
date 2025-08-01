using UnityEngine;
using UnityEngine.InputSystem;

public class Camera_FirstPerson : MonoBehaviour
{
    public Transform playerBody;     // The character root (rotates horizontally)
    public Transform cameraPivot;    // The vertical pivot (rotates up/down)
    public float mouseSensitivity = 1.5f;

    private float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Mouse.current == null) return;

        // Get mouse movement delta
        Vector2 mouseDelta = Mouse.current.delta.ReadValue() * mouseSensitivity;

        // Vertical rotation (pitch)
        xRotation -= mouseDelta.y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Horizontal rotation (yaw)
        playerBody.Rotate(Vector3.up * mouseDelta.x);
    }
}
