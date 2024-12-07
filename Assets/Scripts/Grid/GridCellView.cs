using UnityEngine;

public class GridCellView : MonoBehaviour
{
    public int Row { get; private set; }
    public int Column { get; private set; }

    [Header("Debug Info")]
    public string ElementType; // Element type for debugging
    public int ElementLevel;   // Element level for debugging

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

        // Clear debug info on initialization
        ElementType = "None";
        ElementLevel = 0;
    }

    public void UpdateElementInfo(GridCell cell)
    {
        if (cell.Element != null)
        {
            ElementType = cell.Element.Type;
            ElementLevel = cell.Element.Level;
        }
        else
        {
            ElementType = "None";
            ElementLevel = 0;
        }
    }

    public void HighlightCell()
    {
        spriteRenderer.color = Color.yellow;
    }
}
