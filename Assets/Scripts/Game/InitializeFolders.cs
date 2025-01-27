using UnityEngine;
using System.IO;

public class InitializeFolders : MonoBehaviour
{
    private string[] folderNames = { "Saved Builds", "Color Palettes", "Saved Worlds" };

    void Start()
    {
        CreateFolders();
    }

    void CreateFolders()
    {
        string basePath = Application.persistentDataPath;

        foreach (string folderName in folderNames)
        {
            string folderPath = Path.Combine(basePath, folderName);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }
    }
}
