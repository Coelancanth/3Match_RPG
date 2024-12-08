using System.Collections.Generic;

using UnityEngine;
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
        //Debug.Log("set element");
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

    public void RandomSpawn(int number)
{
    // 获取所有空白格子
    List<GridCell> emptyCells = new List<GridCell>();
    for (int row = 0; row < Rows; row++)
    {
        for (int col = 0; col < Columns; col++)
        {
            if (Cells[row, col].Element == null) // 空白格子
            {
                emptyCells.Add(Cells[row, col]);
            }
        }
    }

    if (emptyCells.Count > 0)
    {
        for (int i =0; i < number; i++)
        {
            // 随机选择一个空白格子
            var randomCell = emptyCells[UnityEngine.Random.Range(0, emptyCells.Count)];
            
            // 创建一个随机元素类型和level=1的元素
            Element newElement = new Element("Fire", 1); // 这里可以扩展生成不同类型的元素
            randomCell.Element = newElement;
            //SetCellElement(randomCell.Row, randomCell.Column, newElement);
            
        }
    }
}

}
