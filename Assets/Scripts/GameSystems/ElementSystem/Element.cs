using System;
using UnityEngine;
public class Element
{
    public string Type { get; private set; }
    public int Level { get; private set; }
    public string SkillID { get; private set; }
    
    public Element(string type, int level, string skillID = null)
    {
        Type = type;
        Level = level;
        SkillID = skillID;
    }

    public void Upgrade()
    {
        Level +=1;
    }
    
    // 触发与元素相关的技能
    public void TriggerSkill()
    {
        if (!string.IsNullOrEmpty(SkillID))
        {
            Debug.Log($"Triggering skill: {SkillID} for Element: {Type}");
            // 未来可调用 SkillManager 处理技能逻辑
        }
        else
        {
            Debug.Log($"No skill linked to Element: {Type}");
        }
    }
}
