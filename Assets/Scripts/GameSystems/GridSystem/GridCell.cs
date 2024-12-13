using System;

public class GridCell
{
    public GridCellView View { get; set; }
    public int Row { get; private set; }
    public int Column { get; private set; }
    private Element _element;
    public Element Element
    {
        get {return _element;}
        set 
        {
            if (_element !=value)
            {
                _element = value;
                // Trigger the event when the element changes
                OnElementChanged?.Invoke(this);
            }
        }
    }

    public event Action<GridCell> OnElementChanged;
    public string EnemyType { get; set; }
    public int EnemyHealth { get; set; }

    private bool _isHighlighted;
    public bool IsHighlighted
    {
        get { return _isHighlighted; }
        set
        {
            if (_isHighlighted != value)
            {
                _isHighlighted = value;
                OnHighlightChanged?.Invoke(this, value);
            }
        }
    }

    public event Action<GridCell, bool> OnHighlightChanged;

    public GridCell(int row, int column)
    {
        Row = row;
        Column = column;
        Element = null;
        EnemyType = null;
        EnemyHealth = 0;
    }

    public bool IsMovable()
    {
    return Element != null; // 示例逻辑：有元素时可移动
    }

}
