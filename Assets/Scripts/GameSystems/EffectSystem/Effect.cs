using UnityEngine;
using System.Collections.Generic;

public abstract class Effect
{
    protected EffectConfig.EffectData config;

    public string ID => config.ID;
    public string Name => config.Name;
    public string Description => config.Description;
    public EffectType Type => config.Type;
    public EffectTriggerType TriggerType => config.TriggerType;

    protected Effect(EffectConfig.EffectData config)
    {
        this.config = config;
    }

    // 效果的主要执行逻辑
    public abstract void Execute(EffectContext context);
    
    // 获取效果影响的范围
    public abstract List<GridCell> GetAffectedCells(EffectContext context);
    
    // 检查效果是否可以执行
    public virtual bool CanExecute(EffectContext context)
    {
        return true;
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