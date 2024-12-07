public class Grid
{
    public int Rows { get; private set; }
    public int Columns { get; private set; }
    public GridCell[,] Cells { get; private set; }

    public Grid(int rows, int columns)
    {
        Rows = rows;
        Columns = columns;
        Cells = new GridCell[Rows, Columns];
        for (int row = 0; row < Rows; row++)
        {
            for (int column = 0; column < Columns; column++)
            {
                Cells[row, column] = new GridCell(row, column);
            }
        }
    }

    public GridCell GetCell(int row, int column)
    {
        if (row >= 0 && row < Rows && column >= 0 && column < Columns)
            return Cells[row, column];
        return null;
    }

    public void SetCellElement(int row, int column, Element element)
    {
        GridCell cell = GetCell(row, column);
        if (cell != null)
        {
            cell.Element = element;
        }
    }

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
