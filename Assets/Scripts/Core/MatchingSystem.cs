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
                    int    ElementLevel = grid.GetCell(x,y).Element.Level;
                    // If the cell is not visited, perform DFS to find the connected group
                    List<GridCell> connectedGroup = new List<GridCell>();
                    DFS(x, y, visited, connectedGroup, ElementType, ElementLevel);
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

    private void DFS(int x, int y, bool[,] visited, List<GridCell> connectedGroup, string type, int level)
    {
        Grid grid = gridManager.gridData;
        // If out of bounds or already visited, return
        if (x < 0 || y < 0 || x >= grid.Rows || y >= grid.Columns|| visited[x, y])
            return;

        GridCell currentCell = grid.GetCell(x, y);

        // If the current cell has an element and is connected and is matched, process it
        if (currentCell.Element != null 
            && !visited[x, y] 
            && currentCell.Element.Type == type
            && currentCell.Element.Level == level)
        {
            visited[x, y] = true;
            connectedGroup.Add(currentCell);

            // Recursively visit all adjacent cells (up, down, left, right)
            DFS(x + 1, y, visited, connectedGroup, type, level); // Right
            DFS(x - 1, y, visited, connectedGroup, type, level); // Left
            DFS(x, y + 1, visited, connectedGroup, type, level); // Down
            DFS(x, y - 1, visited, connectedGroup, type, level); // Up
        }
    }



    public void HandleMatches(List<GridCell> matchedCells)
    {
        foreach (GridCell cell in matchedCells)
        {
            cell.Element = null;
        }
    }
}
