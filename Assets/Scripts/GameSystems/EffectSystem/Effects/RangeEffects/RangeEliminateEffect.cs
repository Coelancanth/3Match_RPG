using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 范围消除效果
/// </summary>
public class RangeEliminateEffect : CustomizableEffect
{
    // 范围形状
    private readonly RangeShape shape;
    // 范围大小
    private readonly int range;
    // 方向（用于直线效果）
    private readonly LineDirection direction;

    public RangeEliminateEffect(EffectConfig.EffectData config) : base(config)
    {
        // 从配置中读取参数
        shape = GetCustomParameter("shape", RangeShape.Square);
        range = GetCustomParameter("range", 1);
        direction = GetCustomParameter("direction", LineDirection.Cross);
    }

    public override List<GridCell> GetAffectedCells(EffectContext context)
    {
        // 获取网格数据
        var grid = context.GridManager.gridData;
        var center = context.SourceCell;

        // 根据不同形状获取影响范围
        return shape switch
        {
            RangeShape.Point => RangeShapeHelper.GetPointRange(grid, center),
            RangeShape.Line => RangeShapeHelper.GetLineRange(grid, center, range, direction),
            RangeShape.Square => RangeShapeHelper.GetSquareRange(grid, center, range),
            RangeShape.Diamond => RangeShapeHelper.GetDiamondRange(grid, center, range),
            RangeShape.Circle => RangeShapeHelper.GetCircleRange(grid, center, range),
            RangeShape.Global => RangeShapeHelper.GetGlobalRange(grid),
            _ => new List<GridCell>()
        };
    }

    protected override void ApplyEffect(EffectContext context, List<GridCell> affectedCells)
    {
        Debug.Log($"开始执行{config.Name}范围消除效果");
        
        // 遍历并消除范围内的元素
        foreach (var cell in affectedCells)
        {
            if (cell.Element != null)
            {
                Debug.Log($"消除位置 ({cell.Row}, {cell.Column}) 的元素: {cell.Element.Type}");
                cell.Element = null;
            }
        }
    }
}

// 范围形状枚举
public enum RangeShape
{
    Point,      // 单点
    Line,       // 直线
    Square,     // 正方形
    Diamond,    // 菱形
    Circle,     // 圆形
    Global      // 全局
} 