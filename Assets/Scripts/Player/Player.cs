using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] MovementSubscription GetInput;
    [SerializeField] Camera PlayerCamera;
    Rigidbody rb;

    private Vector3 PlayerMovement;
    private Vector2 MouseMovement;
    private float groundCheckDistance = 1.0f;
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
        //Make sure to get the look vector of the player and apply movement accordingly
        PlayerMovement = new Vector3(GetInput.MoveInput.x, 0, GetInput.MoveInput.z);
        MouseMovement = new Vector2(GetInput.MouseMove.x, GetInput.MouseMove.y);

        PlayerMovement.Normalize();

        Vector3 desiredVelocity = rb.linearVelocity;

        desiredVelocity.x = PlayerMovement.x * 5;
        desiredVelocity.z = PlayerMovement.z * 5;

        if (GetInput.Jump > 0 && IsGrounded())
        {
            desiredVelocity.y = GetInput.Jump * 5;
        }

        rb.linearVelocity = desiredVelocity;

        //Create separate function for player movement and mouse movement!
        float mouseX = MouseMovement.x * 10 * Time.deltaTime;
        float mouseY = MouseMovement.y * 10 * Time.deltaTime;

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