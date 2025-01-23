using UnityEngine;
using UnityEngine.InputSystem;

public class UIInputs : MonoBehaviour
{
    InputMap _Input = null;

    private void OnEnable()
    {
        _Input = new InputMap();
        _Input.UIGame.Enable();

        _Input.UIGame.InGameMenu.performed += OnInGameMenuPerformed;
    }

    private void OnDisable()
    {
        _Input.UIGame.InGameMenu.performed -= OnInGameMenuPerformed;
        _Input.UIGame.Disable();
    }

    private void OnInGameMenuPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("In game menu triggered!");
    }
}
