public class GridCell
{
    public int Row { get; private set; }
    public int Column { get; private set; }
    public Element Element { get; set; }
    public string EnemyType { get; set; }
    public int EnemyHealth { get; set; }

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
