using System;
using UnityEngine;

// 基础元素类
public class Element
{
    public string Type { get; protected set; }
    public int Value { get; protected set; }

    public Element(string type, int value)
    {
        Type = type;
        Value = value;
    }
}

// 特殊元素基类
public class SpecialElement : Element
{
    public int Level { get; protected set; }

    public SpecialElement(string type, int value, int level) : base(type, value)
    {
        Level = level;
    }

    public void Upgrade()
    {
        Level++;
    }
}

// 主动特殊元素
public class ActiveSpecialElement : SpecialElement
{
    public string EffectID { get; private set; }

    public ActiveSpecialElement(string type, int value, int level, string effectID) 
        : base(type, value, level)
    {
        EffectID = effectID;
    }

    public void TriggerEffect()
    {
        // 触发主动技能
        Debug.Log($"Triggering active effect: {EffectID}");
    }
    
    public bool IsTriggered()
    {
        TriggerEffect();
        return true;
    }
    
    public bool Trigger()
    {
        TriggerEffect();
        return true;
    }
}

// 被动特殊元素
public class PassiveSpecialElement : SpecialElement
{
    public string EffectID { get; private set; }

    public PassiveSpecialElement(string type, int value, int level, string effectID)
        : base(type, value, level)
    {
        EffectID = effectID;
    }

    public void TriggerEffect()
    {

    }

    public bool IsTriggered()
    {
        TriggerEffect();
        return true;
    }

}

