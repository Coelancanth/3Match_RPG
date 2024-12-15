using System.Collections.Generic;
using UnityEngine;
using GameSystems.EffectSystem;
/// <summary>
/// 主动特殊元素修改效果
/// </summary>
public class ActiveSpecialElementModifyEffect : ICustomizableEffect
{
    // 配置数据
    private readonly EffectConfig.EffectData config;
    private readonly ElementConfig elementConfig;
    
    // 效果参数
    private readonly int range;        // 影响范围
    private readonly bool needTarget;  // 是否需要目标
    private readonly int specialLevel; // 特殊元素等级
    private readonly string effectID;  // 效果ID

    // 实现接口属性
    public string ID => config.ID;
    public string Name => config.Name;
    public string Description => config.Description;
    public EffectType Type => config.Type;
    public EffectTriggerType TriggerType => config.TriggerType;

    public ActiveSpecialElementModifyEffect(EffectConfig.EffectData config)
    {
        this.config = config;
        
        // 加载ElementConfig
        elementConfig = Resources.Load<ElementConfig>("Configs/ElementConfig");
        if (elementConfig == null)
        {
            Debug.LogError("无法加载ElementConfig");
            return;
        }

        // 从配置中读取参数
        range = GetCustomParameter("range", 1);
        needTarget = GetCustomParameter("needTarget", true);
        specialLevel = GetCustomParameter("specialLevel", 1);
        effectID = GetCustomParameter("effectID", "");
    }

    public void Execute(EffectContext context)
    {
        if (!CanExecute(context)) return;
        
        var affectedCells = GetAffectedCells(context);
        ApplyEffect(context, affectedCells);
    }

    public List<GridCell> GetAffectedCells(EffectContext context)
    {
        return RangeShapeHelper.GetSquareRange(
            context.GridManager.gridData, 
            context.SourceCell, 
            range
        );
    }

    public bool CanExecute(EffectContext context)
    {
        return context != null && 
               context.GridManager != null && 
               elementConfig != null &&
               (!needTarget || context.TargetCell != null);
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

    private void ApplyEffect(EffectContext context, List<GridCell> cells)
    {
        foreach (var cell in cells)
        {
            if (cell.Element == null) continue;

            // 创建主动特殊元素
            var element = new ActiveSpecialElement(
                cell.Element.Type,
                cell.Element.Value,
                specialLevel,
                effectID,
                range
            );
            
            cell.Element = element;
            Debug.Log($"元素升级为主动特殊元素：({cell.Row}, {cell.Column})");
        }
    }
} 