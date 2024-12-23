using UnityEngine;
using System.Collections.Generic;
using GameSystems.EffectSystem;
/// <summary>
/// 范围消除效果
/// </summary>
public class RangeEliminateEffect : ICustomizableEffect
{
    // 配置数据
    private readonly EffectConfig.EffectData config;
    
    // 范围参数
    private readonly RangeShape shape;
    private readonly int range;
    private readonly LineDirection direction;

    // 实现接口属性
    public string ID => config.ID;
    public string Name => config.Name;
    public string Description => config.Description;
    public EffectType Type => config.Type;
    public EffectTriggerType TriggerType => config.TriggerType;

    public RangeEliminateEffect(EffectConfig.EffectData config)
    {
        this.config = config;
        // 从配置中读取参数
        shape = GetCustomParameter("shape", RangeShape.Square);
        range = GetCustomParameter("range", 1);
        direction = GetCustomParameter("direction", LineDirection.Cross);
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

    public bool CanExecute(EffectContext context)
    {
        return context != null && context.GridManager != null && context.SourceCell != null;
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