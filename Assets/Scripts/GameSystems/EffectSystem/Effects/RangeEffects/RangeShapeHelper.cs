using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// 范围形状辅助类，用于计算不同形状的影响范围
/// </summary>
public static class RangeShapeHelper
{
    // 获取单点范围
    public static List<GridCell> GetPointRange(Grid grid, GridCell center)
    {
        return new List<GridCell> { center };
    }

    // 获取直线范围
public static List<GridCell> GetLineRange(Grid grid, GridCell center, int range, LineDirection direction)
{
    var cells = new List<GridCell>();

    switch (direction)
    {
        case LineDirection.Horizontal:
            // 横向直线（限制范围）
            for (int col = Math.Max(0, center.Column - range); 
                 col <= Math.Min(grid.Columns - 1, center.Column + range); 
                 col++)
            {
                cells.Add(grid.GetCell(center.Row, col));
            }
            break;

        case LineDirection.Vertical:
            // 纵向直线（限制范围）
            for (int row = Math.Max(0, center.Row - range); 
                 row <= Math.Min(grid.Rows - 1, center.Row + range); 
                 row++)
            {
                cells.Add(grid.GetCell(row, center.Column));
            }
            break;

        case LineDirection.Cross:
            // 十字形（分别调用横向和纵向）
            cells.AddRange(GetLineRange(grid, center, range, LineDirection.Horizontal));
            cells.AddRange(GetLineRange(grid, center, range, LineDirection.Vertical));
            break;
    }

    return cells;
}


    // 获取正方形范围
    public static List<GridCell> GetSquareRange(Grid grid, GridCell center, int range)
    {
        var cells = new List<GridCell>();
        int size = range * 2 + 1; // 计算边长
        
        for (int row = center.Row - range; row <= center.Row + range; row++)
        {
            for (int col = center.Column - range; col <= center.Column + range; col++)
            {
                if (IsValidPosition(grid, row, col))
                {
                    cells.Add(grid.GetCell(row, col));
                }
            }
        }
        
        return cells;
    }

    // 获取菱形范围
    public static List<GridCell> GetDiamondRange(Grid grid, GridCell center, int range)
    {
        var cells = new List<GridCell>();
        
        for (int row = center.Row - range; row <= center.Row + range; row++)
        {
            for (int col = center.Column - range; col <= center.Column + range; col++)
            {
                // 使用曼哈顿距离判断是否��菱形范围内
                if (IsValidPosition(grid, row, col) && 
                    Mathf.Abs(row - center.Row) + Mathf.Abs(col - center.Column) <= range)
                {
                    cells.Add(grid.GetCell(row, col));
                }
            }
        }
        
        return cells;
    }

    // 获取圆形范围
    public static List<GridCell> GetCircleRange(Grid grid, GridCell center, int range)
    {
        var cells = new List<GridCell>();
        
        for (int row = center.Row - range; row <= center.Row + range; row++)
        {
            for (int col = center.Column - range; col <= center.Column + range; col++)
            {
                // 使用欧几里得距离判断是否在圆形范围内
                if (IsValidPosition(grid, row, col) && 
                    Vector2.Distance(new Vector2(center.Row, center.Column), new Vector2(row, col)) <= range + 0.5f)
                {
                    cells.Add(grid.GetCell(row, col));
                }
            }
        }
        
        return cells;
    }

    // 获取L形范围
    public static List<GridCell> GetLShapeRange(Grid grid, GridCell center, int range, LShapeDirection direction)
    {
        var cells = new List<GridCell>();
        
        // 添加中心点
        cells.Add(center);
        
        // 根据方向添加L形的两个分支
        switch (direction)
        {
            case LShapeDirection.RightDown:
                // 向右
                for (int col = center.Column + 1; col <= center.Column + range; col++)
                {
                    if (IsValidPosition(grid, center.Row, col))
                        cells.Add(grid.GetCell(center.Row, col));
                }
                // 向下
                for (int row = center.Row + 1; row <= center.Row + range; row++)
                {
                    if (IsValidPosition(grid, row, center.Column))
                        cells.Add(grid.GetCell(row, center.Column));
                }
                break;
            // 可以添加其他L形方向的实现...
        }
        
        return cells;
    }

    // 获取全屏范围
    public static List<GridCell> GetGlobalRange(Grid grid)
    {
        var cells = new List<GridCell>();
        
        for (int row = 0; row < grid.Rows; row++)
        {
            for (int col = 0; col < grid.Columns; col++)
            {
                cells.Add(grid.GetCell(row, col));
            }
        }
        
        return cells;
    }

    // 辅助方法：检查位置是否有效
    private static bool IsValidPosition(Grid grid, int row, int col)
    {
        return row >= 0 && row < grid.Rows && col >= 0 && col < grid.Columns;
    }
}

// 直线方向枚举
public enum LineDirection
{
    Horizontal,  // 横向
    Vertical,    // 纵向
    Cross        // 十字
}

// L形方向枚举
public enum LShapeDirection
{
    RightDown,   // 右下
    RightUp,     // 右上
    LeftDown,    // 左下
    LeftUp       // 左上
} 