using UnityEngine;
using TMPro;
using System.Collections;

public class GridCellView : MonoBehaviour
{
    public int Row { get; private set; }
    public int Column { get; private set; }

    [Header("Debug Info")]
    public string ElementType; // Element type for debugging
    public int ElementValue;   // Element level for debugging

    [SerializeField] private ElementVisualConfig elementVisualConfig;
    [SerializeField] private SpriteRenderer gridSpriteRenderer;    // 网格背景的渲染器
    [SerializeField] private SpriteRenderer elementSpriteRenderer; // 元素的渲染器

    public TextMeshPro levelText;

    private Coroutine textDisplayCoroutine;
    public void UpdateElementInfo(GridCell cell)
    {
        if (cell.Element != null)
        {
            ElementType = cell.Element.Type;
            ElementValue = cell.Element.Value;
            
            if (levelText != null)
            {
                levelText.text = $"{{Row: {Row}, Column: {Column}\nType: {ElementType}, Value: {ElementValue}}}";
            }
            UpdateVisuals(cell.Element);
        }
        else
        {
            ElementType = "None";
            ElementValue = 0;
            
            //if (textDisplayCoroutine != null)
                //StopCoroutine(textDisplayCoroutine);
            //textDisplayCoroutine = StartCoroutine(ShowTextTemporarily($"{{Row: {Row}, Column: {Column}\n Empty", 3.0f));
            levelText.text = "";
            

            UpdateVisuals(null);
        }
    }

    private void UpdateVisuals(Element element)
    {
        if (element == null)
        {
            elementSpriteRenderer.sprite = null;
            elementSpriteRenderer.color = Color.white;
            return;
        }

        var visualData = elementVisualConfig.GetVisualData(element.Type);
        if (visualData != null)
        {
            elementSpriteRenderer.sprite = visualData.sprite;
            elementSpriteRenderer.color = visualData.color;
        }
        else
        {
            elementSpriteRenderer.sprite = null;
            elementSpriteRenderer.color = Color.white;
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
    
    private IEnumerator ShowTextTemporarily(string text, float duration)
    {
        levelText.text = text;
        yield return new WaitForSeconds(duration);
        levelText.text = "";
    }

    public void Initialize(int row, int column, Color color)
    {
        Row = row;
        Column = column;
        ElementType = "None";
        ElementValue = 0;
        gridSpriteRenderer.color = color;  // 设置网格背景颜色
        elementSpriteRenderer.color = Color.white;  // 重置元素颜色
    }
}
