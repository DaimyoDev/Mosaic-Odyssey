using UnityEngine;

[System.Serializable]
public class BlockData
{
    public Vector3 position;
    public Color color;

    public BlockData(Vector3 position, Color color)
    {
        this.position = position;
        this.color = color;
    }
}