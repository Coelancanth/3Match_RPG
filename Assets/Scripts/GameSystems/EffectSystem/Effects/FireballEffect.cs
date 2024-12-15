using UnityEngine;
using System.Collections.Generic;
using GameSystems.EffectSystem;
/// <summary>
/// 火球效果：造成范围伤害，并对不同元素有不同影响
/// </summary>
public class FireballEffect : ICustomizableEffect
{
    // 配置数据
    private readonly EffectConfig.EffectData config;
    
    // 效果参数
    private readonly int range;           
    private readonly int baseDamage;      
    private readonly RangeShape shape;    

    // 实现接口属性
    public string ID => config.ID;
    public string Name => config.Name;
    public string Description => config.Description;
    public EffectType Type => config.Type;
    public EffectTriggerType TriggerType => config.TriggerType;

    public FireballEffect(EffectConfig.EffectData config)
    {
        this.config = config;
        // 从配置中读取参数
        range = GetCustomParameter("range", 2);
        baseDamage = GetCustomParameter("baseDamage", 1);
        shape = GetCustomParameter("shape", RangeShape.Circle);
    }

    // 实现接口方法
    public void Execute(EffectContext context)
    {
        if (!CanExecute(context)) return;
        
        var affectedCells = GetAffectedCells(context);
        ApplyEffect(context, affectedCells);
    }

    public List<GridCell> GetAffectedCells(EffectContext context)
    {
        var grid = context.GridManager.gridData;
        var center = context.SourceCell;
        Debug.Log($"center: {center.Row}, {center.Column}");
        // 使用RangeShapeHelper获取影响范围
        return shape switch
        {
            RangeShape.Circle => RangeShapeHelper.GetCircleRange(grid, center, range),
            _ => RangeShapeHelper.GetSquareRange(grid, center, range)
        };
    }

    public bool CanExecute(EffectContext context)
    {
        return context != null && context.GridManager != null;
    }

    public T GetCustomParameter<T>(string key, T defaultValue = default)
    {
        if (config.CustomParameters != null && 
            config.CustomParameters.TryGetValue(key, out object value))
        {
            try
            {
                return (T)value;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"参数转换失败: {key}, {e.Message}");
                return defaultValue;
            }
        }
        return defaultValue;
    }

    // 私有方法
    private void ApplyEffect(EffectContext context, List<GridCell> affectedCells)
    {
        Debug.Log($"开始执行{config.Name}效果");
        
        foreach (var cell in affectedCells)
        {   
            if (cell.Element == null) continue;
           {
            Debug.Log($"cell: {cell.Row}, {cell.Column}");
           } 
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