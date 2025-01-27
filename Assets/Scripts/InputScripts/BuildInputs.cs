using UnityEngine;
using UnityEngine.InputSystem;

public class BuildInputs : MonoBehaviour
{
    private InputMap playerInputActions;

    public bool isBuilding;
    private Vector3 buildDirection = Vector3.zero;
    private bool isDirectionUpdated = false;

    private void Awake()
    {
        playerInputActions = new InputMap();

        playerInputActions.Player.Enable();
    }

    private void OnEnable()
    {
        playerInputActions.Player.Build.performed += OnBuildPerformed;
        playerInputActions.Player.Build.canceled += OnBuildCanceled;
        playerInputActions.Player.EnableBuild.performed += OnEnableBuildPerformed;
    }

    private void OnDisable()
    {
        playerInputActions.Player.Build.performed -= OnBuildPerformed;
        playerInputActions.Player.Build.canceled -= OnBuildCanceled;
        playerInputActions.Player.EnableBuild.performed -= OnEnableBuildPerformed;
    }

    private void OnBuildPerformed(InputAction.CallbackContext context)
    {
        Vector3 inputDirection = context.ReadValue<Vector3>();

        if (inputDirection != buildDirection)
        {
            buildDirection = inputDirection;
            isDirectionUpdated = true;
        }
    }

    private void OnBuildCanceled(InputAction.CallbackContext context)
    {
        buildDirection = Vector3.zero;
        isDirectionUpdated = false;
    }

    private void OnEnableBuildPerformed(InputAction.CallbackContext context)
    {
        isBuilding = !isBuilding;
    }

    public Vector3 GetBuildDirection()
    {
        if (isDirectionUpdated)
        {
            isDirectionUpdated = false;
            return buildDirection;
        }
        return Vector3.zero;
    }

    private void OnDestroy()
    {
        playerInputActions.Dispose();
    }
}