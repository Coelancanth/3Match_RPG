using UnityEngine;
using System.Collections.Generic;

public class FireballEffect : Effect
{
    private readonly int range;
    private readonly int baseDamage;
    private readonly int fireElementBoost = 1;  // 火元素增强值
    private readonly int waterElementReduction = 1;  // 水元素削弱值

    public FireballEffect() : base(
        "effect_fireball",
        "火球术",
        "增强范围内的火元素，削弱范围内的水元素",
        EffectType.ElementChange,
        EffectTriggerType.OnEliminate)
    {
        range = 2;
        baseDamage = 1;
    }

    public override List<GridCell> GetAffectedCells(EffectContext context)
    {
        var affectedCells = new List<GridCell>();
        var sourcePosRow = context.SourceCell.Row;
        var sourcePosColumn = context.SourceCell.Column;
        var grid = context.GridManager.gridData;

        // 获取范围内的所有格子
        for (int x = sourcePosRow - range; x <= sourcePosRow + range; x++)
        {
            for (int y = sourcePosColumn - range; y <= sourcePosColumn + range; y++)
            {
                if (grid.IsValidPosition(x, y))
                {
                    affectedCells.Add(grid.GetCell(x, y));
                }
            }
        }
        foreach (var cell in affectedCells)
        {
            Debug.Log($"Affected Cell: ({cell.Row}, {cell.Column})");
        }

        return affectedCells;
    }

    public override void Execute(EffectContext context)
    {
        Debug.Log("开始执行火球效果");
        Debug.Log($"触发位置: ({context.SourceCell.Row}, {context.SourceCell.Column})");
        
        var affectedCells = GetAffectedCells(context);
        Debug.Log($"影响范围内的格子数量: {affectedCells.Count}");
        
        foreach (var cell in affectedCells)
        {
            if (cell.Element == null) continue;
            
            Debug.Log($"处理格子 ({cell.Row}, {cell.Column}): {cell.Element.Type} Lv.{cell.Element.Value}");
            ApplyElementEffect(cell);
        }
        
        Debug.Log("火球效果执行完毕");
    }

    private void ApplyElementEffect(GridCell cell)
    {
        if (cell.Element == null) return;

        switch (cell.Element.Type)
        {
            case "Fire":
                // 增强火元素
                BoostFireElement(cell);
                break;
            case "Water":
                // 削弱水元素
                WeakenWaterElement(cell);
                break;
            default:
                // 其他元素类型不受影响
                break;
        }
    }

    private void BoostFireElement(GridCell cell)
    {
        int newValue = cell.Element.Value + fireElementBoost;
        Debug.Log($"火元素增强：({cell.Row}, {cell.Column}) {cell.Element.Value} -> {newValue}");
        cell.Element = new Element("Fire", newValue);
    }

    private void WeakenWaterElement(GridCell cell)
    {
        int newValue = Mathf.Max(1, cell.Element.Value - waterElementReduction); // 确保不会低于1
        Debug.Log($"水元素削弱：({cell.Row}, {cell.Column}) {cell.Element.Value} -> {newValue}");
        
        if (newValue <= 1)
        {
            // 如果削弱到1以下，直接消除
            cell.Element = null;
            Debug.Log($"水元素被消除：({cell.Row}, {cell.Column})");
        }
        else
        {
            cell.Element = new Element("Water", newValue);
        }
    }

    private int CalculateDistance(GridCell source, GridCell target)
    {
        return Mathf.Abs(source.Row - target.Row) + 
               Mathf.Abs(source.Column - target.Column);
    }
}