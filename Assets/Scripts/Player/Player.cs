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
    [SerializeField] int sprintSpeed;
    [SerializeField] int jumpPower;
    [SerializeField] int ascendPower;
    [SerializeField] int Health;
    [SerializeField] int Stamina;
    Rigidbody rb;

    private Vector3 PlayerMovement;
    private Vector2 MouseMovement;
    private readonly float groundCheckDistance = 1.2f;
    private float xRotation = 0f;
    private bool isFlying = false;
    private float lastJumpPressTime = 0f;
    private readonly float doubleTapTimeThreshold = 0.3f;
    private bool wasJumpKeyPressed = false;

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
        CheckForDoubleTap();
    }

    private void MovePlayer()
    {
        PlayerMovement = new Vector3(GetInput.MoveInput.x, 0, GetInput.MoveInput.z);
        PlayerMovement.Normalize();

        Vector3 movementDirection = transform.TransformDirection(PlayerMovement);

        Vector3 desiredVelocity = rb.linearVelocity;

        float currentSpeed = GetInput.IsSprinting ? sprintSpeed : speed;

        desiredVelocity.x = movementDirection.x * currentSpeed;
        desiredVelocity.z = movementDirection.z * currentSpeed;

        if (isFlying)
        {
            rb.useGravity = false;

            if (GetInput.Jump > 0)
            {
                desiredVelocity.y = GetInput.Jump * ascendPower;
            }
            else
            {
                desiredVelocity.y = 0;
            }
        }
        else
        {
            rb.useGravity = true;

            if (GetInput.Jump > 0 && IsGrounded())
            {
                desiredVelocity.y = GetInput.Jump * jumpPower;
            }
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

        var root = InGameMenu.rootVisualElement;

        if (root.style.display == DisplayStyle.None)
        {
            root.style.display = DisplayStyle.Flex;
            UnityEngine.Cursor.visible = true;
            UnityEngine.Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            root.style.display = DisplayStyle.None;
            UnityEngine.Cursor.visible = false;
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void CheckForDoubleTap()
    {
        if (GetInput.Jump > 0)
        {
            if (!wasJumpKeyPressed)
            {
                wasJumpKeyPressed = true;
            }
        }
        else
        {
            if (wasJumpKeyPressed)
            {
                if (Time.time - lastJumpPressTime < doubleTapTimeThreshold)
                {
                    ToggleFlying();
                }
                lastJumpPressTime = Time.time;
            }
            wasJumpKeyPressed = false;
        }
    }

    private void ToggleFlying()
    {
        isFlying = !isFlying;
        rb.useGravity = !isFlying;
    }
}