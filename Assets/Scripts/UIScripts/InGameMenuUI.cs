using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class InGameMenuUI : MonoBehaviour
{
    [SerializeField] SaveLoadUI saveLoadUI;
    [SerializeField] ControlsUI controlsUI;
    private VisualElement ui;
    private Button mainMenuButton;
    private Button saveLoadButton;
    private Button controlsButton;
    private Button quitButton;

    private void Start()
    {
        InitializeUI();

        HideUI();

        if (saveLoadUI == null)
        {
            Debug.LogError("SaveLoadUI script not found!");
        }
    }

    private void InitializeUI()
    {
        ui = GetComponent<UIDocument>().rootVisualElement;

        if (ui == null)
        {
            Debug.LogError("rootVisualElement is null!");
            return;
        }

        mainMenuButton = ui.Q<Button>("MainMenu");
        if (mainMenuButton != null)
        {
            mainMenuButton.clicked += OnMainMenuButtonClicked;
        }

        saveLoadButton = ui.Q<Button>("SaveLoad");
        if (saveLoadButton != null)
        {
            saveLoadButton.clicked += OnSaveLoadButtonClicked;
        }

        controlsButton = ui.Q<Button>("Controls");
        {
            controlsButton.clicked += OnControlsButtonClicked;
        }

        quitButton = ui.Q<Button>("Quit");
        if (quitButton != null)
        {
            quitButton.clicked += OnQuitButtonClicked;
        }
    }

    public void ShowUI()
    {
        ui.style.display = DisplayStyle.Flex;

        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;
    }

    public void HideUI()
    {
        ui.style.display = DisplayStyle.None;

        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
    }

    private void OnMainMenuButtonClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void OnSaveLoadButtonClicked()
    {
        if (saveLoadUI != null)
        {
            saveLoadUI.Initialize();
            ui.style.display = DisplayStyle.None;
        }
        else
        {
            Debug.LogError("SaveLoadUI is not assigned!");
        }
    }

    private void OnControlsButtonClicked()
    {
        if (controlsButton != null)
        {
            controlsUI.Initialize();
            ui.style.display = DisplayStyle.None;
        }
        else
        {
            Debug.LogError("ControlsUI is not assigned!");
        }
    }

    private void OnQuitButtonClicked()
    {
        Application.Quit();
    }
}