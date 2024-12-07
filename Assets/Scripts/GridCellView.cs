using UnityEngine;

public class GridCellView : MonoBehaviour
{
    public int Row { get; private set; }
    public int Column { get; private set; }
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void Initialize(int row, int column, Color color)
    {
        Row = row;
        Column = column;
        spriteRenderer.color = color;
    }
}
