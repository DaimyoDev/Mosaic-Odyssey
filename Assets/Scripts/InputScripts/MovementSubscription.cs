using UnityEngine;
using UnityEngine.InputSystem;

public class MovementSubscription : MonoBehaviour
{
    //References to InputSystem
    public Vector3 MoveInput { get; private set; } = Vector3.zero;
    public float Jump { get; private set; } = 0.0f;
    public bool IsSprinting { get; private set; } = false;
    public Vector2 MouseMove { get; private set; } = Vector2.zero;

    InputMap _Input = null;

    private void OnEnable() //Listen to Inputs
    {
        _Input = new InputMap();
        _Input.Player.Enable();

        _Input.Player.Movement.performed += SetMovement;
        _Input.Player.Movement.canceled += SetMovement;

        _Input.Player.Jump.performed += SetJump;
        _Input.Player.Jump.canceled += SetJump;

        _Input.Player.MouseMove.performed += SetMouseMovement;
        _Input.Player.MouseMove.canceled += SetMouseMovement;

        _Input.Player.Sprint.performed += SetSprint;
        _Input.Player.Sprint.canceled += SetSprint;
    }

    private void OnDisable() //Stop listening to Inputs
    {
        _Input.Player.Movement.performed -= SetMovement;
        _Input.Player.Movement.canceled -= SetMovement;

        _Input.Player.Jump.performed -= SetJump;
        _Input.Player.Jump.canceled -= SetJump;

        _Input.Player.MouseMove.performed -= SetMouseMovement;
        _Input.Player.MouseMove.canceled -= SetMouseMovement;

        _Input.Player.Sprint.performed -= SetSprint;
        _Input.Player.Sprint.canceled -= SetSprint;

        _Input.Player.Disable();
    }

    //Perform context callback to listen for actions
    void SetMovement(InputAction.CallbackContext ctx)
    {
        MoveInput = ctx.ReadValue<Vector3>();
    }

    void SetJump(InputAction.CallbackContext ctx)
    {
        Jump = ctx.ReadValue<float>();
    }

    void SetMouseMovement(InputAction.CallbackContext ctx)
    {
        MouseMove = ctx.ReadValue<Vector2>();
    }

    void SetSprint(InputAction.CallbackContext ctx)
    {
        IsSprinting = ctx.ReadValue<float>() > 0;
    }
}