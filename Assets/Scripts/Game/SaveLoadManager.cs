using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    private string saveFolderPath;

    private void Awake()
    {
        saveFolderPath = Path.Combine(Application.persistentDataPath, "Saved Builds");

        if (!Directory.Exists(saveFolderPath))
        {
            Directory.CreateDirectory(saveFolderPath);
        }
    }

    public void SaveBuild(string saveName, List<BlockData> buildData)
    {
        string filePath = Path.Combine(saveFolderPath, $"{saveName}.json");
        string jsonData = JsonUtility.ToJson(new BuildDataWrapper(buildData), true);
        File.WriteAllText(filePath, jsonData);
        Debug.Log($"Build saved to {filePath}");
    }

    public List<BlockData> LoadBuild(string saveName)
    {
        string filePath = Path.Combine(saveFolderPath, $"{saveName}.json");
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            BuildDataWrapper wrapper = JsonUtility.FromJson<BuildDataWrapper>(jsonData);
            return wrapper.blockDataList;
        }
        else
        {
            Debug.LogError($"File not found: {filePath}");
            return null;
        }
    }

    public void DeleteBuild(string saveName)
    {
        string filePath = Path.Combine(saveFolderPath, $"{saveName}.json");
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log($"Build deleted: {filePath}");
        }
        else
        {
            Debug.LogError($"File not found: {filePath}");
        }
    }

    public List<string> GetSavedBuilds()
    {
        List<string> savedBuilds = new List<string>();
        foreach (string file in Directory.GetFiles(saveFolderPath, "*.json"))
        {
            savedBuilds.Add(Path.GetFileNameWithoutExtension(file));
        }
        return savedBuilds;
    }

    [System.Serializable]
    private class BuildDataWrapper
    {
        public List<BlockData> blockDataList;

        public BuildDataWrapper(List<BlockData> blockDataList)
        {
            this.blockDataList = blockDataList;
        }
    }
}