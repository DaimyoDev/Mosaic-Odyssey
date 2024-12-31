using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] MovementSubscription GetInput;
    [SerializeField] Camera PlayerCamera;
    [SerializeField] int mouseSensitivity;
    [SerializeField] int speed;
    [SerializeField] int jumpPower;
    Rigidbody rb;

    private Vector3 PlayerMovement;
    private Vector2 MouseMovement;
    private float groundCheckDistance = 1.2f;
    private float xRotation = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
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
        MouseMovement = new Vector2(GetInput.MouseMove.x, GetInput.MouseMove.y);

        float mouseX = MouseMovement.x * mouseSensitivity * Time.deltaTime;
        float mouseY = MouseMovement.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        PlayerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);
    }
}