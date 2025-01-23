using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class InGameMenuUI : MonoBehaviour
{
    private VisualElement ui;
    private Button mainMenuButton;
    private Button quitButton;

    private void Start()
    {
        // Initialize the UI when the game starts
        InitializeUI();

        // Hide the UI at start
        HideUI();
    }

    private void InitializeUI()
    {
        // Get the root visual element
        ui = GetComponent<UIDocument>().rootVisualElement;

        // Ensure the root visual element is not null
        if (ui == null)
        {
            Debug.LogError("rootVisualElement is null!");
            return;
        }

        // Find the buttons and set up event listeners
        mainMenuButton = ui.Q<Button>("MainMenu");
        if (mainMenuButton != null)
        {
            mainMenuButton.clicked += OnMainMenuButtonClicked;
        }
        else
        {
            Debug.LogError("MainMenu button not found!");
        }

        quitButton = ui.Q<Button>("Quit");
        if (quitButton != null)
        {
            quitButton.clicked += OnQuitButtonClicked;
        }
        else
        {
            Debug.LogError("Quit button not found!");
        }
    }

    public void ShowUI()
    {
        // Show the UI by setting display to Flex
        ui.style.display = DisplayStyle.Flex;

        // Unlock and show the cursor
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;
    }

    public void HideUI()
    {
        // Hide the UI by setting display to None
        ui.style.display = DisplayStyle.None;

        // Lock and hide the cursor
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
    }

    private void OnMainMenuButtonClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void OnQuitButtonClicked()
    {
        Application.Quit();
    }
}