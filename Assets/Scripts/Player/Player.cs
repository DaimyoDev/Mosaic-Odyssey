using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    [SerializeField] MovementSubscription GetInput;
    [SerializeField] UIInputs GetUIInput;
    [SerializeField] UIDocument InGameMenu;
    [SerializeField] Camera PlayerCamera;
    [SerializeField] float mouseSensitivity;
    [SerializeField] int speed;
    [SerializeField] int jumpPower;
    Rigidbody rb;

    private Vector3 PlayerMovement;
    private Vector2 MouseMovement;
    private float groundCheckDistance = 1.2f;
    private float xRotation = 0f;
    private bool isMenuOpen = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
    }

    private void OnEnable()
    {
        UIInputs.OnEscapePressed += ToggleInGameMenu;
    }

    private void OnDisable()
    {
        UIInputs.OnEscapePressed -= ToggleInGameMenu;
    }

    private void Start()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        MovePlayer();
        MouseRotatePlayer();
    }

    private void MovePlayer()
    {
        PlayerMovement = new Vector3(GetInput.MoveInput.x, 0, GetInput.MoveInput.z);
        PlayerMovement.Normalize();

        Vector3 movementDirection = transform.TransformDirection(PlayerMovement);

        Vector3 desiredVelocity = rb.linearVelocity;

        desiredVelocity.x = movementDirection.x * speed;
        desiredVelocity.z = movementDirection.z * speed;

        if (GetInput.Jump > 0 && IsGrounded())
        {
            desiredVelocity.y = GetInput.Jump * jumpPower;
        }

        rb.linearVelocity = desiredVelocity;
    }

    private void MouseRotatePlayer()
    {
        if (UnityEngine.Cursor.lockState == CursorLockMode.None)
        {
            return;
        }
        MouseMovement = new Vector2(GetInput.MouseMove.x, GetInput.MouseMove.y);

        float mouseX = MouseMovement.x * mouseSensitivity;
        float mouseY = MouseMovement.y * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        PlayerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);
    }
    private void ToggleInGameMenu()
    {
        isMenuOpen = !isMenuOpen;

        // Get the root visual element
        var root = InGameMenu.rootVisualElement;

        // Toggle the visibility of the UI
        if (isMenuOpen)
        {
            root.style.display = DisplayStyle.Flex; // Show the UI
        }
        else
        {
            root.style.display = DisplayStyle.None; // Hide the UI
        }

        // Toggle cursor lock state and visibility
        UnityEngine.Cursor.lockState = isMenuOpen ? CursorLockMode.None : CursorLockMode.Locked;
        UnityEngine.Cursor.visible = isMenuOpen;
    }
}