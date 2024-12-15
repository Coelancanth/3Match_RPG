using System;
using UnityEngine;

/// <summary>
/// 元素基类，定义所有元素的基本属性和行为
/// </summary>
public abstract class Element
{
    // 基础属性
    public string Type { get; protected set; }
    public int Value { get; protected set; }
    public string DisplayName { get; protected set; }
    
    // 事件处理器，而不是事件
    protected Action<Element> ValueChangedHandler;
    protected Action<Element> EffectTriggeredHandler;

    protected Element(string type, int value)
    {
        Type = type;
        Value = value;
    }

    // 虚方法，允许子类重写
    public virtual bool CanTriggerEffect() => false;
    public virtual void TriggerEffect() { }
    
    public virtual void Upgrade()
    {
        Value++;
        ValueChangedHandler?.Invoke(this);
    }
    
    public virtual void Eliminate(){}

    // 事件注册方法
    public void RegisterValueChangedHandler(Action<Element> handler)
    {
        ValueChangedHandler += handler;
    }

    public void RegisterEffectTriggeredHandler(Action<Element> handler)
    {
        EffectTriggeredHandler += handler;
    }

    public override string ToString()
    {
        return $"{Type}(Value:{Value})";
    }
}

/// <summary>
/// 基础元素，游戏中最常见的元素类型
/// </summary>
public class BasicElement : Element
{
    public BasicElement(string type, int value) : base(type, value) { }
}

/// <summary>
/// 特殊元素基类，为主动和被动特殊元素提供共同的基础功能
/// </summary>
public abstract class SpecialElement : Element
{
    public string EffectID { get; protected set; }
    public int EffectLevel { get; protected set; }
    
    protected SpecialElement(string type, int value, int effectLevel, string effectId) 
        : base(type, value)
    {
        EffectID = effectId;
        EffectLevel = effectLevel;
    }

    public virtual void UpgradeEffect()
    {
        EffectLevel++;
        ValueChangedHandler?.Invoke(this);
    }
}

/// <summary>
/// 主动特殊元素，需要玩家主动触发效果
/// </summary>
public class ActiveSpecialElement : SpecialElement
{
    public int Range { get; private set; }
    public bool IsOnCooldown { get; private set; }
    public int CooldownTurns { get; private set; }

    public ActiveSpecialElement(
        string type, 
        int value,
        int effectLevel,
        string effectId,
        int range,
        int cooldownTurns = 0
    ) : base(type, value, effectLevel, effectId)
    {
        Range = range;
        CooldownTurns = cooldownTurns;
        IsOnCooldown = false;
    }

    public override bool CanTriggerEffect()
    {
        return !IsOnCooldown;
    }

    public override void TriggerEffect()
    {
        if (!CanTriggerEffect()) return;
        
        IsOnCooldown = true;
        EffectTriggeredHandler?.Invoke(this);
    }

    public void ResetCooldown()
    {
        IsOnCooldown = false;
    }
}

/// <summary>
/// 被动特殊元素，自动触发效果
/// </summary>
public class PassiveSpecialElement : SpecialElement
{
    public float TriggerChance { get; private set; }
    public bool CanTriggerMultiple { get; private set; }
    public int TriggersThisTurn { get; private set; }
    public int MaxTriggersPerTurn { get; private set; }

    public PassiveSpecialElement(
        string type,
        int value,
        int effectLevel,
        string effectId,
        float triggerChance = 100f,
        bool canTriggerMultiple = false,
        int maxTriggersPerTurn = 1
    ) : base(type, value, effectLevel, effectId)
    {
        TriggerChance = triggerChance;
        CanTriggerMultiple = canTriggerMultiple;
        MaxTriggersPerTurn = maxTriggersPerTurn;
        TriggersThisTurn = 0;
    }

    public override bool CanTriggerEffect()
    {
        if (!CanTriggerMultiple && TriggersThisTurn >= MaxTriggersPerTurn)
            return false;

        return UnityEngine.Random.Range(0f, 100f) <= TriggerChance;
    }

    public override void TriggerEffect()
    {
        if (!CanTriggerEffect()) return;

        TriggersThisTurn++;
        EffectTriggeredHandler?.Invoke(this);
    }

    public void ResetTriggerCount()
    {
        TriggersThisTurn = 0;
    }
}

