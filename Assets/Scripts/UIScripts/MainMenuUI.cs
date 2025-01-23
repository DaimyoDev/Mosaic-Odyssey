using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuUI : MonoBehaviour
{
    public VisualElement ui;
    public Button sandboxModeButton;
    public Button regularModeButton;
    public Button controlsButton;
    public Button settingsButton;
    public Button quitButton;
    public Button colorPalettesButton;


    private void Awake()
    {
        ui = GetComponent<UIDocument>().rootVisualElement;
    }

    private void OnEnable()
    {
        sandboxModeButton = ui.Q<Button>("SandboxMode");
        sandboxModeButton.clicked += OnSandboxModeButtonClicked;
        regularModeButton = ui.Q<Button>("RegularMode");
        regularModeButton.clicked += OnRegularModeButtonClicked;
        controlsButton = ui.Q<Button>("Controls");
        controlsButton.clicked += OnControlsButtonClicked;
        settingsButton = ui.Q<Button>("Settings");
        settingsButton.clicked += OnSettingsButtonClicked;
        quitButton = ui.Q<Button>("Quit");
        quitButton.clicked += OnQuitButtonClicked;
        colorPalettesButton = ui.Q<Button>("ColorPalettes");
        colorPalettesButton.clicked += OnColorPalettesButtonClicked;

    }

    private void OnSandboxModeButtonClicked()
    {
        SceneManager.LoadScene("Sandbox");
    }

    private void OnRegularModeButtonClicked()
    {
        SceneManager.LoadScene("RegularMode");
    }

    private void OnControlsButtonClicked()
    {
        SceneManager.LoadScene("Controls");
    }

    private void OnSettingsButtonClicked()
    {
        Debug.Log("Settings!");
    }

    private void OnQuitButtonClicked()
    {
        Application.Quit();
    }

    private void OnColorPalettesButtonClicked()
    {
        Debug.Log("Color Palettes!");
    }
}
