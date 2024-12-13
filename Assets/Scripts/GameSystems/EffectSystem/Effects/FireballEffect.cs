using UnityEngine;
using System.Collections.Generic;

public class FireballEffect : Effect
{
    public FireballEffect(EffectConfig.EffectData config) : base(config)
    {
        // 可以在这里进行额外的初始化
    }

    public override List<GridCell> GetAffectedCells(EffectContext context)
    {
        var affectedCells = new List<GridCell>();
        var sourcePosRow = context.SourceCell.Row;
        var sourcePosColumn = context.SourceCell.Column;
        var grid = context.GridManager.gridData;

        // 使用配置中的范围值
        for (int x = sourcePosRow - config.Range; x <= sourcePosRow + config.Range; x++)
        {
            for (int y = sourcePosColumn - config.Range; y <= sourcePosColumn + config.Range; y++)
            {
                if (grid.IsValidPosition(x, y))
                {
                    affectedCells.Add(grid.GetCell(x, y));
                }
            }
        }

        return affectedCells;
    }

    public override void Execute(EffectContext context)
    {
        Debug.Log($"开始执行{config.Name}效果");
        
        var affectedCells = GetAffectedCells(context);
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

        int newValue = Mathf.RoundToInt(cell.Element.Value * modifier);
        if (newValue <= 0)
        {
            cell.Element = null;
            Debug.Log($"元素被消除：({cell.Row}, {cell.Column})");
        }
        else
        {
            cell.Element = new Element(cell.Element.Type, newValue);
            Debug.Log($"元素修改：({cell.Row}, {cell.Column}) -> {newValue}");
        }
    }
}