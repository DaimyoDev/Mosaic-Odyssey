using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class InitializeFolders : MonoBehaviour
{
    private string[] folderNames = { "Saved Builds", "Color Palettes", "Saved Worlds" };

    void Start()
    {
        CreateFolders();
        GenerateDefaultColorPalettes();
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

    void GenerateDefaultColorPalettes()
    {
        string colorPalettesPath = Path.Combine(Application.persistentDataPath, "Color Palettes");

        // Define three default color palettes with 36 colors each
        List<Color[]> defaultPalettes = new List<Color[]>
        {
            CreatePrimaryAndSecondaryPalette(), // Vibrant and Classic
            CreatePastelAndMutedPalette(),     // Soft and Subtle
            CreateNeonAndBrightPalette()       // Bold and Electric
        };

        for (int i = 0; i < defaultPalettes.Count; i++)
        {
            string palettePath = Path.Combine(colorPalettesPath, $"Palette_{i + 1}.json");

            if (!File.Exists(palettePath))
            {
                string json = JsonUtility.ToJson(new ColorPalette(defaultPalettes[i]), true);
                File.WriteAllText(palettePath, json);
            }
        }
    }

    Color[] CreatePrimaryAndSecondaryPalette()
    {
        return new Color[]
        {
            Color.red, Color.green, Color.blue, Color.yellow, Color.cyan, Color.magenta,
            new Color(1f, 0.5f, 0f), // Orange
            new Color(0.5f, 0f, 0.5f), // Purple
            new Color(0f, 0.5f, 0f), // Dark Green
            new Color(0.5f, 0.5f, 0f), // Olive
            new Color(0f, 0.5f, 0.5f), // Teal
            new Color(0.5f, 0f, 0f), // Maroon
            new Color(1f, 0.75f, 0f), // Gold
            new Color(0.75f, 0f, 0.75f), // Pink
            new Color(0f, 0.75f, 0.75f), // Sky Blue
            new Color(0.75f, 0.75f, 0f), // Lime
            new Color(0.25f, 0.25f, 0.25f), // Dark Gray
            new Color(0.75f, 0.25f, 0f), // Brown
            new Color(0.25f, 0f, 0.75f), // Indigo
            new Color(0f, 0.25f, 0.75f), // Navy
            new Color(0.75f, 0f, 0.25f), // Crimson
            new Color(0.25f, 0.75f, 0f), // Forest Green
            new Color(0f, 0.75f, 0.25f), // Mint
            new Color(0.75f, 0.25f, 0.75f), // Lavender
            new Color(0.25f, 0.75f, 0.75f), // Aqua
            new Color(0.5f, 0.25f, 0f), // Rust
            new Color(0f, 0.25f, 0.5f), // Steel Blue
            new Color(0.5f, 0f, 0.25f), // Burgundy
            new Color(0.25f, 0f, 0.5f), // Plum
            new Color(0f, 0.5f, 0.25f), // Jade
            new Color(0.5f, 0.25f, 0.5f), // Mauve
            new Color(0.25f, 0.5f, 0.5f), // Turquoise
            new Color(0.75f, 0.5f, 0f), // Amber
            new Color(0f, 0.5f, 0.75f), // Cerulean
            new Color(0.75f, 0f, 0.5f), // Fuchsia
            new Color(0.5f, 0.75f, 0f), // Chartreuse
            new Color(0f, 0.75f, 0.5f), // Spring Green
            new Color(0.5f, 0f, 0.75f), // Violet
            new Color(0.75f, 0.5f, 0.5f), // Salmon
            new Color(0.5f, 0.75f, 0.5f), // Pistachio
            new Color(0.5f, 0.5f, 0.75f) // Periwinkle
        };
    }

    // Soft and Subtle Palette (Pastel and Muted Colors)
    Color[] CreatePastelAndMutedPalette()
    {
        return new Color[]
        {
            new Color(1f, 0.8f, 0.8f), // Light Pink
            new Color(0.8f, 1f, 0.8f), // Light Mint
            new Color(0.8f, 0.8f, 1f), // Light Blue
            new Color(1f, 1f, 0.8f), // Light Yellow
            new Color(0.8f, 1f, 1f), // Light Cyan
            new Color(1f, 0.8f, 1f), // Light Magenta
            new Color(0.9f, 0.7f, 0.7f), // Dusty Rose
            new Color(0.7f, 0.9f, 0.7f), // Pale Green
            new Color(0.7f, 0.7f, 0.9f), // Pale Blue
            new Color(0.9f, 0.9f, 0.7f), // Beige
            new Color(0.7f, 0.9f, 0.9f), // Pale Turquoise
            new Color(0.9f, 0.7f, 0.9f), // Pale Purple
            new Color(0.8f, 0.6f, 0.6f), // Muted Pink
            new Color(0.6f, 0.8f, 0.6f), // Muted Green
            new Color(0.6f, 0.6f, 0.8f), // Muted Blue
            new Color(0.8f, 0.8f, 0.6f), // Muted Yellow
            new Color(0.6f, 0.8f, 0.8f), // Muted Cyan
            new Color(0.8f, 0.6f, 0.8f), // Muted Magenta
            new Color(0.7f, 0.5f, 0.5f), // Muted Red
            new Color(0.5f, 0.7f, 0.5f), // Muted Olive
            new Color(0.5f, 0.5f, 0.7f), // Muted Indigo
            new Color(0.7f, 0.7f, 0.5f), // Muted Gold
            new Color(0.5f, 0.7f, 0.7f), // Muted Teal
            new Color(0.7f, 0.5f, 0.7f), // Muted Lavender
            new Color(0.6f, 0.4f, 0.4f), // Dusty Red
            new Color(0.4f, 0.6f, 0.4f), // Dusty Green
            new Color(0.4f, 0.4f, 0.6f), // Dusty Blue
            new Color(0.6f, 0.6f, 0.4f), // Dusty Yellow
            new Color(0.4f, 0.6f, 0.6f), // Dusty Cyan
            new Color(0.6f, 0.4f, 0.6f), // Dusty Magenta
            new Color(0.5f, 0.3f, 0.3f), // Muted Maroon
            new Color(0.3f, 0.5f, 0.3f), // Muted Forest
            new Color(0.3f, 0.3f, 0.5f), // Muted Navy
            new Color(0.5f, 0.5f, 0.3f), // Muted Olive Drab
            new Color(0.3f, 0.5f, 0.5f), // Muted Slate
            new Color(0.5f, 0.3f, 0.5f) // Muted Plum
        };
    }

    // Bold and Electric Palette (Neon and Bright Colors)
    Color[] CreateNeonAndBrightPalette()
    {
        return new Color[]
        {
            new Color(1f, 0f, 0f), // Neon Red
            new Color(0f, 1f, 0f), // Neon Green
            new Color(0f, 0f, 1f), // Neon Blue
            new Color(1f, 1f, 0f), // Neon Yellow
            new Color(0f, 1f, 1f), // Neon Cyan
            new Color(1f, 0f, 1f), // Neon Magenta
            new Color(1f, 0.5f, 0f), // Neon Orange
            new Color(0.5f, 0f, 1f), // Neon Purple
            new Color(0f, 1f, 0.5f), // Neon Lime
            new Color(0.5f, 1f, 0f), // Neon Chartreuse
            new Color(0f, 0.5f, 1f), // Neon Sky Blue
            new Color(1f, 0f, 0.5f), // Neon Pink
            new Color(0.5f, 0f, 0.5f), // Neon Violet
            new Color(0.5f, 0.5f, 0f), // Neon Olive
            new Color(0f, 0.5f, 0.5f), // Neon Teal
            new Color(1f, 0.5f, 0.5f), // Neon Salmon
            new Color(0.5f, 1f, 0.5f), // Neon Pistachio
            new Color(0.5f, 0.5f, 1f), // Neon Periwinkle
            new Color(1f, 0.75f, 0f), // Neon Amber
            new Color(0.75f, 0f, 1f), // Neon Indigo
            new Color(0f, 1f, 0.75f), // Neon Mint
            new Color(0.75f, 1f, 0f), // Neon Lime Green
            new Color(0f, 0.75f, 1f), // Neon Cerulean
            new Color(1f, 0f, 0.75f), // Neon Fuchsia
            new Color(0.75f, 0f, 0.75f), // Neon Lavender
            new Color(0.75f, 0.75f, 0f), // Neon Gold
            new Color(0f, 0.75f, 0.75f), // Neon Aqua
            new Color(1f, 0.75f, 0.75f), // Neon Peach
            new Color(0.75f, 1f, 0.75f), // Neon Pastel Green
            new Color(0.75f, 0.75f, 1f), // Neon Pastel Blue
            new Color(1f, 0.25f, 0f), // Neon Red-Orange
            new Color(0.25f, 0f, 1f), // Neon Deep Purple
            new Color(0f, 1f, 0.25f), // Neon Spring Green
            new Color(0.25f, 1f, 0f), // Neon Lime Yellow
            new Color(0f, 0.25f, 1f), // Neon Royal Blue
            new Color(1f, 0f, 0.25f) // Neon Hot Pink
        };
    }
}

[System.Serializable]
public class ColorPalette
{
    public Color[] colors;

    public ColorPalette(Color[] colors)
    {
        this.colors = colors;
    }
}