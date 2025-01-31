using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class ControlsUI : MonoBehaviour
{
    private UIDocument uiDocument;

    private VisualElement root;
    private Button exitButton;

    void Start()
    {
        Initialize();
        if (SceneManager.GetActiveScene().name != "Controls")
        {
            root.style.display = DisplayStyle.None;
        }
    }

    void OnEnable()
    {
        Initialize();
    }

   public void Initialize()
    {
        uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null)
        {
            Debug.LogError("UIDocument is not assigned.");
            return;
        }

        root = uiDocument.rootVisualElement;

        if (root == null)
        {
            Debug.LogError("Root VisualElement is null.");
            return;
        }

        exitButton = root.Q<Button>("Exit");

        if (exitButton == null)
        {
            Debug.LogError("Exit button not found.");
            return;
        }

        exitButton.clicked -= OnExitButtonClicked;

        exitButton.clicked += OnExitButtonClicked;

        root.style.display = DisplayStyle.Flex;
    }

    void OnExitButtonClicked()
    {
        if (SceneManager.GetActiveScene().name == "Controls")
        {
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            root.style.display = DisplayStyle.None;
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
        }
    }
}