using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class BuildInputs : MonoBehaviour
{
    private InputMap playerInputActions;

    public bool isBuilding;
    private Vector3 buildDirection = Vector3.zero;
    private bool isDirectionUpdated = false;
    public bool placeBlockPressed = false;
    public bool leftMouseClickPressed = false;


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
        playerInputActions.Player.PlaceBlock.performed += OnPlaceBlockPerformed;
        playerInputActions.Player.LeftMouseClick.performed += OnLeftMouseClickPerformed;
    }

    private void OnDisable()
    {
        playerInputActions.Player.Build.performed -= OnBuildPerformed;
        playerInputActions.Player.Build.canceled -= OnBuildCanceled;
        playerInputActions.Player.EnableBuild.performed -= OnEnableBuildPerformed;
        playerInputActions.Player.PlaceBlock.performed -= OnPlaceBlockPerformed;
        playerInputActions.Player.LeftMouseClick.performed -= OnLeftMouseClickPerformed;

        playerInputActions.Player.Disable();
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

    private void OnPlaceBlockPerformed(InputAction.CallbackContext context)
    {
        placeBlockPressed = true;
    }

    private void OnLeftMouseClickPerformed(InputAction.CallbackContext context)
    {
        leftMouseClickPressed = true;
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

    public bool IsPlaceBlockPressed()
    {
        if (placeBlockPressed)
        {
            placeBlockPressed = false;
            return true;
        }
        return false;
    }

    public bool IsLeftMouseClickPressed()
    {
        if (leftMouseClickPressed)
        {
            leftMouseClickPressed = false;
            return true;
        }
        return false;
    }

    private void OnDestroy()
    {
        playerInputActions.Dispose();
    }
}