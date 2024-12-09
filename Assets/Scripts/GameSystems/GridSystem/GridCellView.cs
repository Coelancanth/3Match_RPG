using UnityEngine;
using TMPro;

public class GridCellView : MonoBehaviour
{
    public int Row { get; private set; }
    public int Column { get; private set; }

    [Header("Debug Info")]
    public string ElementType; // Element type for debugging
    public int ElementLevel;   // Element level for debugging

    private SpriteRenderer spriteRenderer;
    public TextMeshPro levelText;

    // Color mappings for different element types
    public Color[] typeColors;  // Array of colors for different element types

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

        // Set default color (could be transparent or a default color)
        spriteRenderer.color = Color.white;
    }

    public void UpdateElementInfo(GridCell cell)
    {
        if (cell.Element != null)
        {
            ElementType = cell.Element.Type;
            ElementLevel = cell.Element.Level;
            
            if (levelText != null)
            {
                levelText.text = "Level: " + ElementLevel.ToString();
            }
            // Update the sprite color based on the element's type
            UpdateColor(cell.Element);
            
        }
        else
        {
            ElementType = "None";
            ElementLevel = 0;
            levelText.text = "";

            // Set default color (empty or neutral)
            spriteRenderer.color = Color.white;
        }
    }

    private void UpdateColor(Element element)
    {
        // Map the element's type to a color
        switch (element.Type)
        {
            case "Fire":
                spriteRenderer.color = Color.red;
                break;
            case "Water":
                spriteRenderer.color = Color.blue;
                break;
            case "Earth":
                spriteRenderer.color = Color.green;
                break;
            case "Grass":
                spriteRenderer.color = Color.green;
                break;
            case "Air":
                spriteRenderer.color = Color.yellow;
                break;
            // Add more types and colors as necessary
            default:
                spriteRenderer.color = Color.white;  // Default color
                break;
        }
    }

    // Subscribe to the Element change event
    public void SubscribeToCell(GridCell cell)
    {
        cell.OnElementChanged += HandleElementChanged;
    }

    // Handle the element change event and update the UI
    private void HandleElementChanged(GridCell cell)
    {
        UpdateElementInfo(cell);  // Update the visual representation when the element changes
    }
}
