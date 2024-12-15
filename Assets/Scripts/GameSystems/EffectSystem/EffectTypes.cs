using UnityEngine;

namespace GameSystems.EffectSystem
{
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
} 