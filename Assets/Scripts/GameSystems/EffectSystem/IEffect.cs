using System.Collections.Generic;
using UnityEngine;
using GameSystems.EffectSystem;

// 1. 首先定义核心效果接口
public interface IEffect
{
    // 基本属性
    string ID { get; }
    string Name { get; }
    string Description { get; }
    EffectType Type { get; }
    EffectTriggerType TriggerType { get; }

    // 核心方法
    void Execute(EffectContext context);
    List<GridCell> GetAffectedCells(EffectContext context);
    bool CanExecute(EffectContext context);
}

// 2. 定义可自定义参数的接口（可选）
public interface ICustomizableEffect : IEffect
{
    T GetCustomParameter<T>(string key, T defaultValue = default);
}