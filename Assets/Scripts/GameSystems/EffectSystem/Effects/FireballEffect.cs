using UnityEngine;
using System.Collections.Generic;

public class FireballEffect : Effect
{
    private readonly int range;
    private readonly int baseDamage;

    public FireballEffect() : base(
        "effect_fireball",
        "火球术",
        "对目标位置造成伤害，并对周围造成溅射伤害",
        EffectType.Damage,
        EffectTriggerType.OnEliminate)
    {
        range = 2;
        baseDamage = 1;
    }

    public override List<GridCell> GetAffectedCells(EffectContext context)
    {
        var affectedCells = new List<GridCell>();
        var sourcePos = context.SourceCell.Position;
        var grid = context.GridManager.gridData;

        // 获取范围内的所有格子
        for (int x = sourcePos.Row - range; x <= sourcePos.Row + range; x++)
        {
            for (int y = sourcePos.Column - range; y <= sourcePos.Column + range; y++)
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
        var affectedCells = GetAffectedCells(context);
        
        foreach (var cell in affectedCells)
        {
            if (cell.Element == null) continue;

            // 计算伤害
            int damage = CalculateDamage(context.SourceCell, cell);
            
            // 应用效果
            ApplyDamage(cell, damage);
            
            // 检查元素相互作用
            CheckElementInteraction(cell);
        }
    }

    private int CalculateDamage(GridCell source, GridCell target)
    {
        int distance = Mathf.Abs(source.Position.Row - target.Position.Row) + 
                      Mathf.Abs(source.Position.Column - target.Position.Column);
        
        float multiplier = distance == 0 ? 1f : 0.5f;
        
        // 如果目标是火元素，伤害加倍
        if (target.Element?.Type == "Fire")
        {
            multiplier *= 2f;
        }
        
        return Mathf.RoundToInt(baseDamage * multiplier);
    }

    private void ApplyDamage(GridCell cell, int damage)
    {
        // 这里可以添加伤害效果的视觉反馈
        Debug.Log($"对位置({cell.Position.Row}, {cell.Position.Column})造成{damage}点伤害");
        cell.Element = null;
    }

    private void CheckElementInteraction(GridCell cell)
    {
        // 这里可以添加元素相互作用的逻辑
        // 比如火+水=蒸汽等
    }
} 