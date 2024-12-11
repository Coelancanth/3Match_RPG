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

    public virtual void OnMatch() 
    {
        // 基础元素匹配效果
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
    public string SkillID { get; private set; }

    public ActiveSpecialElement(string type, int value, int level, string skillID) 
        : base(type, value, level)
    {
        SkillID = skillID;
    }

    public void TriggerSkill()
    {
        // 触发主动技能
        Debug.Log($"Triggering active skill: {SkillID}");
    }

    public override void OnMatch()
    {
        base.OnMatch();
        TriggerSkill();
    }
}

// 被动特殊元素
public class PassiveSpecialElement : SpecialElement
{
    public PassiveEffect PassiveEffect { get; private set; }

    public PassiveSpecialElement(string type, int value, int level, PassiveEffect effect)
        : base(type, value, level)
    {
        PassiveEffect = effect;
    }

    public override void OnMatch()
    {
        base.OnMatch();
        ApplyPassiveEffect();
    }

    private void ApplyPassiveEffect()
    {
        // 应用被动效果
        PassiveEffect.Apply();
    }
}

// 被动效果基类
public abstract class PassiveEffect
{
    public abstract void Apply();
}
