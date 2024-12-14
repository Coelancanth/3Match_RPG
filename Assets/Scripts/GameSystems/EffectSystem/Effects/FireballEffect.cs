using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 火球效果：造成范围伤害，并对不同元素有不同影响
/// </summary>
public class FireballEffect : CustomizableEffect
{
    // 效果参数
    private readonly int range;           // 影响范围
    private readonly int baseDamage;      // 基础伤害值
    private readonly RangeShape shape;    // 范围形状

    public FireballEffect(EffectConfig.EffectData config) : base(config)
    {
        // 从配置中读取参数
        range = GetCustomParameter("range", 2);
        baseDamage = GetCustomParameter("baseDamage", 1);
        shape = GetCustomParameter("shape", RangeShape.Circle);
    }

    public override List<GridCell> GetAffectedCells(EffectContext context)
    {
        var grid = context.GridManager.gridData;
        var center = context.SourceCell;

        // 使用RangeShapeHelper获取影响范围
        return shape switch
        {
            RangeShape.Circle => RangeShapeHelper.GetCircleRange(grid, center, range),
            _ => RangeShapeHelper.GetSquareRange(grid, center, range)
        };
    }

    protected override void ApplyEffect(EffectContext context, List<GridCell> affectedCells)
    {
        Debug.Log($"开始执行{config.Name}效果");
        
        foreach (var cell in affectedCells)
        {
            if (cell.Element == null) continue;
            
            // 使用配置中的元素修正值
            if (config.ElementModifiers.TryGetValue(cell.Element.Type, out float modifier))
            {
                Debug.Log($"元素修正值: {modifier}");
                ApplyElementEffect(cell, modifier);
            }
        }
    }

    private void ApplyElementEffect(GridCell cell, float modifier)
    {
        if (cell.Element == null) return;

        // 计算最终伤害值
        int finalDamage = Mathf.RoundToInt(baseDamage * modifier);
        
        // 应用伤害效果
        int newValue = cell.Element.Value - finalDamage;
        if (newValue <= 0)
        {
            cell.Element = null;
            Debug.Log($"元素被消除：({cell.Row}, {cell.Column})");
        }
        else
        {
            cell.Element = new Element(cell.Element.Type, newValue);
            Debug.Log($"元素受到伤害：({cell.Row}, {cell.Column}) -> {newValue}");
        }
    }
}