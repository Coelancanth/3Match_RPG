using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class MatchingSystem
{
    private readonly GridManager gridManager;
    private readonly IMatchingRule matchingRule;

    public MatchingSystem(GridManager gridManager, IMatchingRule matchingRule)
    {
        this.gridManager = gridManager;
        this.matchingRule = matchingRule;
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
                    // If the cell is not visited, perform DFS to find the connected group
                    List<GridCell> connectedGroup = new List<GridCell>();
                    DFS(x, y, visited, connectedGroup);
                    connectedGroups.Add(connectedGroup);
                }
            }
        }
        foreach (var group in connectedGroups)
        {
            Debug.Log($"Element: {group[0].Element.Type}, Counts: {group.Count}");
        }

        return connectedGroups;
    }

    private void DFS(int x, int y, bool[,] visited, List<GridCell> connectedGroup)
    {
        Grid grid = gridManager.gridData;
        // If out of bounds or already visited, return
        if (x < 0 || y < 0 || x >= grid.Rows || y >= grid.Columns|| visited[x, y])
            return;

        GridCell currentCell = grid.GetCell(x, y);

        // If the current cell has an element and is connected, process it
        if (currentCell.Element != null && !visited[x, y])
        {
            visited[x, y] = true;
            connectedGroup.Add(currentCell);

            // Recursively visit all adjacent cells (up, down, left, right)
            DFS(x + 1, y, visited, connectedGroup); // Right
            DFS(x - 1, y, visited, connectedGroup); // Left
            DFS(x, y + 1, visited, connectedGroup); // Down
            DFS(x, y - 1, visited, connectedGroup); // Up
        }
    }




    //public List<GridCell> DetectMatches()
    //{
        //int gridWidth = GridConstants.Columns;
        //int gridHeight = GridConstants.Rows;

        //bool[,] visited = new bool[gridWidth, gridHeight];
        //List<GridCell> matchedCells = new List<GridCell>();

        //for (int x = 0; x < gridWidth; x++)
        //{
            //for (int y = 0; y < gridHeight; y++)
            //{
                //GridCell currentCell = gridManager.GetCell(y, x);
                //if (currentCell != null && currentCell.Element != null && !visited[x, y])
                //{
                    //List<GridCell> currentMatch = new List<GridCell>();
                    //CheckAdjacentCells(x, y, visited, currentMatch);

                    //if (currentMatch.Count >= 3)
                    //{
                        //matchedCells.AddRange(currentMatch);
                    //}
                //}
            //}
        //}

        //return matchedCells;
    //}

    //private void CheckAdjacentCells(int x, int y, bool[,] visited, List<GridCell> currentMatch)
    //{
        //if (x < 0 || x >= GridConstants.Columns || y < 0 || y >= GridConstants.Rows || visited[x, y])
            //return;

        //GridCell currentCell = gridManager.GetCell(y, x);
        //if (currentCell == null || currentCell.Element == null)
            //return;

        //visited[x, y] = true;
        //currentMatch.Add(currentCell);

        //foreach (var direction in new[] { (0, 1), (1, 0), (0, -1), (-1, 0) })
        //{
            //int newX = x + direction.Item1;
            //int newY = y + direction.Item2;

            //GridCell adjacentCell = gridManager.GetCell(newY, newX);
            //if (adjacentCell != null && matchingRule.IsMatch(currentCell, adjacentCell))
            //{
                //CheckAdjacentCells(newX, newY, visited, currentMatch);
            //}
        //}
    //}

    public void HandleMatches(List<GridCell> matchedCells)
    {
        foreach (GridCell cell in matchedCells)
        {
            cell.Element = null;

            //GridCellView cellView = gridManager.GetCellView(cell.Row, cell.Column);
            //if (cellView != null)
            //{
                //cellView.UpdateElementInfo(cell);
                //cellView.HighlightCell();
            //}
        }
    }
}
