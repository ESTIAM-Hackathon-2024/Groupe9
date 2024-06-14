using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Extinguisher : MonoBehaviour
{
    [SerializeField] private float extinguishRate = 1.0f; // Amount of fire extinguished per second
    [SerializeField] private Transform raycastOrigin = null;

    [Space, Header("Steam")]
    [SerializeField] private GameObject steamObject = null;

    // Variables for camera control
    public Transform cameraTransform;
    public float mouseSensitivity = 2f;
    private float cameraVerticalRotation = 0f;
    private bool lockedCursor = true;

    // Variables for player movement
    public float speed = 5f;
    public float jumpForce = 5f;
    private CharacterController characterController;
    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        LockCursor();

        if (!steamObject)
            Debug.LogError("Please place a steam particle system on the Extinguisher's steamObject field or rewrite the Extinguisher script.", this);

        if (!cameraTransform)
            Debug.LogError("Please assign the cameraTransform field in the inspector.", this);
    }

    void Update()
    {
        HandleMouseInput();
        HandleMovement();
        HandleCursorLockToggle();
        HandleExtinguishing();
    }

    private void HandleMouseInput()
    {
        // Collect Mouse Input
        float inputX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float inputY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Rotate the Camera around its local X axis
        cameraVerticalRotation -= inputY;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -90f, 90f);
        cameraTransform.localEulerAngles = Vector3.right * cameraVerticalRotation;

        // Rotate the Player Object and the Camera around its Y axis
        transform.Rotate(Vector3.up * inputX);
    }

    private void HandleMovement()
    {
        // Collect movement input
        float inputX = Input.GetAxis("Horizontal"); // Left (-1) and Right (1) arrow keys
        float inputZ = Input.GetAxis("Vertical");   // Forward (1) and Backward (-1) keys

        // Move the player
        Vector3 move = transform.right * inputX + transform.forward * inputZ;
        characterController.Move(move * speed * Time.deltaTime);

        // Handle gravity
        isGrounded = characterController.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Handle jumping
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y);
        }

        // Apply gravity
        velocity.y += Physics.gravity.y * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
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

    private void HandleExtinguishing()
    {
        if (IsRaycastingFire(out Fire fire))
            ExtinguishFire(fire);
        else if (steamObject.activeSelf)
            steamObject.SetActive(false);
    }

    private bool IsRaycastingSomething(out RaycastHit hit)
    {
        return Physics.Raycast(raycastOrigin.position, raycastOrigin.forward, out hit, 100f);
    }

    private bool IsRaycastingFire(out Fire fire)
    {
        fire = null;
        return IsRaycastingSomething(out RaycastHit hit) && hit.collider.TryGetComponent(out fire);
    }

    private void ExtinguishFire(Fire fire)
    {
        fire.TryExtinguish(extinguishRate * Time.deltaTime);
        steamObject.transform.position = fire.transform.position;
        steamObject.SetActive(fire.GetIntensity() > 0.0f);
    }
}