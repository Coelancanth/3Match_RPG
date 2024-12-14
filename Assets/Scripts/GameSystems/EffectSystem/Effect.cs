using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 效果基类
/// </summary>
public abstract class Effect
{
    // 保护级别的配置，允许子类访问
    protected readonly EffectConfig.EffectData config;

    // 基本属性
    public string ID => config.ID;
    public string Name => config.Name;
    public string Description => config.Description;
    public EffectType Type => config.Type;
    public EffectTriggerType TriggerType => config.TriggerType;

    // 构造函数
    protected Effect(EffectConfig.EffectData config)
    {
        this.config = config;
    }

    // 主要执行逻辑
    public virtual void Execute(EffectContext context)
    {
        // 1. 前置检查
        if (!CanExecute(context))
        {
            Debug.LogWarning($"效果 {Name} 无法执行");
            return;
        }

        // 2. 获取影响范围
        var affectedCells = GetAffectedCells(context);

        // 3. 应用效果
        ApplyEffect(context, affectedCells);

        // 4. 触发连锁效果
        TriggerChainEffects(context, affectedCells);
    }
    
    // 获取效果影响的范围
    public abstract List<GridCell> GetAffectedCells(EffectContext context);
    
    // 应用具体效果
    protected abstract void ApplyEffect(EffectContext context, List<GridCell> affectedCells);
    
    // 检查效果是否可以执行
    public virtual bool CanExecute(EffectContext context)
    {
        return true;
    }

    // 触发连锁效果
    protected virtual void TriggerChainEffects(EffectContext context, List<GridCell> affectedCells)
    {
        if (config.ChainEffectIDs == null || config.ChainEffectIDs.Count == 0) return;

        foreach (var chainEffectId in config.ChainEffectIDs)
        {
            EffectManager.Instance.QueueEffect(chainEffectId, context);
        }
    }
}

// 效果类型枚举
public enum EffectType
{
    Damage,         // 伤害效果
    ElementChange,  // 元素变化
    TerrainChange,  // 地形改变
    Status,         // 状态效果
    Composite       // 组合效果
}

// 效果触发类型
public enum EffectTriggerType
{
    OnEliminate,    // 消除时触发
    OnMatch,        // 匹配时触发
    OnTurnStart,    // 回合开始时触发
    OnTurnEnd,      // 回合结束时触发
    OnDamaged,      // 受到伤害时触发
    Manual          // 手动触发
} 