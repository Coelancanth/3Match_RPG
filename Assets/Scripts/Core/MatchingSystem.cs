using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class MatchingSystem
{
    private readonly GridManager gridManager;

    public MatchingSystem(GridManager gridManager)
    {
        this.gridManager = gridManager;
    }

    public List<List<GridCell>> FindConnectedGroups()
    {
        Grid grid = gridManager.gridData;
        List<List<GridCell>> connectedGroups = new List<List<GridCell>>();
        bool[,] visited = new bool[grid.Rows, grid.Columns];

        // Iterate through each cell in the grid
        for (int x = 0; x < grid.Rows; x++)
        {
            for (int y = 0; y < grid.Columns; y++)
            {
                if (!visited[x, y] && grid.GetCell(x, y).Element != null)
                {   
                    // first Element
                    string ElementType = grid.GetCell(x,y).Element.Type;
                    //int    ElementLevel = grid.GetCell(x,y).Element.Level;
                    // If the cell is not visited, perform DFS to find the connected group
                    List<GridCell> connectedGroup = new List<GridCell>();
                    DFS(x, y, visited, connectedGroup, ElementType);
                    connectedGroups.Add(connectedGroup);
                }
            }
        }
        foreach (var group in connectedGroups)
        {
            //Debug.Log($"Element: {group[0].Element.Type}, Level: {group[0].Element.Level}, Counts: {group.Count}");
        }

        return connectedGroups;
    }

    private void DFS(int x, int y, bool[,] visited, List<GridCell> connectedGroup, string type)
    {
        Grid grid = gridManager.gridData;
        // If out of bounds or already visited, return
        if (x < 0 || y < 0 || x >= grid.Rows || y >= grid.Columns|| visited[x, y])
            return;

        GridCell currentCell = grid.GetCell(x, y);

        // If the current cell has an element and is connected and is matched, process it
        if (currentCell.Element != null 
            && !visited[x, y] 
            && currentCell.Element.Type == type)
        {
            visited[x, y] = true;
            connectedGroup.Add(currentCell);

            // Recursively visit all adjacent cells (up, down, left, right)
            DFS(x + 1, y, visited, connectedGroup, type); // Right
            DFS(x - 1, y, visited, connectedGroup, type); // Left
            DFS(x, y + 1, visited, connectedGroup, type); // Down
            DFS(x, y - 1, visited, connectedGroup, type); // Up
        }
    }

    public List<List<GridCell>> GetAdjacentConnectedGroups(GridCell targetCell)
    {
        Grid grid = gridManager.gridData;
        List<List<GridCell>> adjacentGroups = new List<List<GridCell>>();
        bool[,] visited = new bool[grid.Rows, grid.Columns];

        // 获取目标格子的坐标
        int targetX = targetCell.Row;
        int targetY = targetCell.Column;

        // 检查四个相邻方向
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(1, 0),  // 右
            new Vector2Int(-1, 0), // 左
            new Vector2Int(0, 1),  // 上
            new Vector2Int(0, -1)  // 下
        };

        foreach (var dir in directions)
        {
            int newX = targetX + dir.x;
            int newY = targetY + dir.y;

            // 检查边界
            if (newX < 0 || newY < 0 || newX >= grid.Rows || newY >= grid.Columns)
                continue;

            GridCell adjacentCell = grid.GetCell(newX, newY);
            
            // 如果相邻格子有元素且未被访问过
            if (adjacentCell.Element != null && !visited[newX, newY])
            {
                List<GridCell> connectedGroup = new List<GridCell>();
                DFS(newX, newY, visited, connectedGroup, adjacentCell.Element.Type);
                
                // 只有当连通组不为空时才添加
                if (connectedGroup.Count > 0)
                {
                    adjacentGroups.Add(connectedGroup);
                }
            }
        }

        return adjacentGroups;
    }

    public void HandleMatches(List<GridCell> matchedCells)
    {
        foreach (GridCell cell in matchedCells)
        {
            cell.Element = null;
        }
    }
}
