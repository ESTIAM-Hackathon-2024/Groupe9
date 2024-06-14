using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    // Variables
    public Transform player;
    public float mouseSensitivity = 2f;
    private float cameraVerticalRotation = 0f;
    private bool lockedCursor = true;

    void Start()
    {
        LockCursor();

        // Ensure the player starts on solid ground
        RaycastHit hit;
        if (Physics.Raycast(player.position, Vector3.down, out hit, Mathf.Infinity))
        {
            player.position = hit.point;
        }
    }

    void Update()
    {
        HandleMouseInput();
        HandleCursorLockToggle();
    }

    private void HandleMouseInput()
    {
        // Collect Mouse Input
        float inputX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float inputY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Rotate the Camera around its local X axis
        cameraVerticalRotation -= inputY;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -90f, 90f);
        transform.localEulerAngles = Vector3.right * cameraVerticalRotation;

        // Rotate the Player Object and the Camera around its Y axis
        player.Rotate(Vector3.up * inputX);
    }

    private void HandleCursorLockToggle()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            lockedCursor = !lockedCursor;
            if (lockedCursor)
            {
                LockCursor();
            }
            else
            {
                UnlockCursor();
            }
        }
    }

    private void LockCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void UnlockCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
