public class Grid
{
    public int Rows { get; private set; } = 10;  // 行数
    public int Columns { get; private set; } = 10;  // 列数
    public GridCell[,] Cells { get; private set; }  // 存储所有单元格

    public Grid()
    {
        Cells = new GridCell[Rows, Columns];
        for (int row = 0; row < Rows; row++)
        {
            for (int column = 0; column < Columns; column++)
            {
                Cells[row, column] = new GridCell(row, column);
            }
        }
    }

    // 获取某个单元格
    public GridCell GetCell(int row, int column)
    {
        if (row >= 0 && row < Rows && column >= 0 && column < Columns)
            return Cells[row, column];
        return null;
    }

    // 设置某个单元格的属性
    public void SetCellElement(int row, int column, string elementType, int level, string state)
    {
        GridCell cell = GetCell(row, column);
        if (cell != null)
        {
            cell.ElementType = elementType;
            cell.ElementLevel = level;
            cell.ElementState = state;
        }
    }

    // 设置某个单元格的敌人信息
    public void SetCellEnemy(int row, int column, string enemyType, int health)
    {
        GridCell cell = GetCell(row, column);
        if (cell != null)
        {
            cell.EnemyType = enemyType;
            cell.EnemyHealth = health;
        }
    }
}
