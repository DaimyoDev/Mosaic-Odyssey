using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuUI : MonoBehaviour
{
    public VisualElement ui;
    public Button sandboxModeButton;
    public Button regularModeButton;
    public Button controlsButton;
    public Button settingsButton;
    public Button quitButton;


    private void Awake()
    {
        ui = GetComponent<UIDocument>().rootVisualElement;
    }

    private void OnEnable()
    {
        
    }

}
