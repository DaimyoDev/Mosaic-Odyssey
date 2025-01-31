using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SaveLoadUI : MonoBehaviour
{
    [SerializeField] private SaveLoadManager saveLoadManager;

    private UIDocument uiDocument;
    private VisualElement root;
    private TextField saveNameField;
    private Button saveButton;
    private Button exitButton;
    private ListView savedBuildsList;

    private void OnEnable()
    {
        uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        saveNameField = root.Q<TextField>("saveNameField");
        saveButton = root.Q<Button>("saveButton");
        exitButton = root.Q<Button>("exitButton");
        savedBuildsList = root.Q<ListView>("savedBuildsList");

        root.style.display = DisplayStyle.None;
    }

    public void Initialize()
    {
        SetupListView();
        saveButton.clicked += OnSaveButtonClicked;
        root.style.display = DisplayStyle.Flex;

        exitButton.clicked += Hide;
    }


    private void SetupListView()
    {
        savedBuildsList.makeItem = () => savedBuildsList.itemTemplate.Instantiate();
        savedBuildsList.bindItem = (element, index) => BindSavedBuildItem(element, index);
        savedBuildsList.itemsSource = saveLoadManager.GetSavedBuilds();
        savedBuildsList.selectionType = SelectionType.None;
    }

    private void BindSavedBuildItem(VisualElement element, int index)
    {
        var buildNameLabel = element.Q<Label>("buildNameLabel");
        var loadButton = element.Q<Button>("loadButton");
        var deleteButton = element.Q<Button>("deleteButton");

        string buildName = saveLoadManager.GetSavedBuilds()[index];
        buildNameLabel.text = buildName;

        loadButton.clicked += () => OnLoadButtonClicked(buildName);
        deleteButton.clicked += () => OnDeleteButtonClicked(buildName);
    }

    private void OnSaveButtonClicked()
    {
        string saveName = saveNameField.value;
        if (string.IsNullOrEmpty(saveName))
        {
            Debug.LogError("Save name cannot be empty!");
            return;
        }

        List<BlockData> buildData = GetBuildData();
        saveLoadManager.SaveBuild(saveName, buildData);

        savedBuildsList.itemsSource = saveLoadManager.GetSavedBuilds();
        savedBuildsList.Rebuild();
    }

    private void OnLoadButtonClicked(string buildName)
    {
        List<BlockData> buildData = saveLoadManager.LoadBuild(buildName);
        if (buildData != null)
        {
            LoadBuildData(buildData);
        }
    }

    private void OnDeleteButtonClicked(string buildName)
    {
        saveLoadManager.DeleteBuild(buildName);

        savedBuildsList.itemsSource = saveLoadManager.GetSavedBuilds();
        savedBuildsList.Rebuild();
    }

    private List<BlockData> GetBuildData()
    {
        List<BlockData> blockDataList = new List<BlockData>();

        Renderer[] renderers = FindObjectsByType<Renderer>(FindObjectsSortMode.None);
        foreach (var renderer in renderers)
        {
            if (renderer.gameObject.GetComponent<MeshFilter>()?.sharedMesh.name == "Cube")
            {
                blockDataList.Add(new BlockData(renderer.transform.position, renderer.material.color));
            }
        }

        return blockDataList;
    }

    private void LoadBuildData(List<BlockData> buildData)
    {
        Renderer[] renderers = FindObjectsByType<Renderer>(FindObjectsSortMode.None);
        foreach (var renderer in renderers)
        {
            if (renderer.gameObject.GetComponent<MeshFilter>()?.sharedMesh.name == "Cube")
            {
                Destroy(renderer.gameObject);
            }
        }

        foreach (var blockData in buildData)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = blockData.position;
            cube.GetComponent<Renderer>().material.color = blockData.color;
        }
    }

    public bool IsVisible()
    {
        return root.style.display == DisplayStyle.Flex;
    }

    public void Hide()
    {
        root.style.display = DisplayStyle.None;

        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
    }
}