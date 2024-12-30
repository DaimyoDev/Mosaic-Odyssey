using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] MovementSubscription GetInput;
    Rigidbody rb;

    Vector3 PlayerMovement;
    float Jump;
    float groundCheckDistance = 1.0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
    }

    private void Update()
    {
        PlayerMovement = new Vector3(GetInput.MoveInput.x, 0, GetInput.MoveInput.z);

        PlayerMovement.Normalize();

        Vector3 desiredVelocity = rb.linearVelocity;

        desiredVelocity.x = PlayerMovement.x * 5;
        desiredVelocity.z = PlayerMovement.z * 5;

        if (GetInput.Jump > 0 && IsGrounded())
        {
            desiredVelocity.y = GetInput.Jump * 5;
        }

        rb.linearVelocity = desiredVelocity;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }
    private bool IsGrounded()
    {
        // Cast a ray downward from the player's position
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);
    }

}