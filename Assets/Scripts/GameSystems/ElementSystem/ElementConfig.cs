using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ElementConfig", menuName = "Game/Element Config")]
public class ElementConfig : ScriptableObject
{
    [System.Serializable]
    public class ActiveElementConfig
    {
        public int Range = 1;            // 影响范围
        public int CoolDown = 0;         // 冷却时间
        public int EnergyCost = 0;       // 能量消耗
        public string TargetingType;      // 目标选择类型（如"单体"、"范围"、"直线"）
        public bool NeedTarget = true;    // 是否需要选择目标
    }

    [System.Serializable]
    public class PassiveElementConfig
    {
        public float TriggerChance = 100f;  // 触发概率
        public bool CanTriggerMultiple;     // 是否可以多次触发
        public int MaxTriggersPerTurn;      // 每回合最大触发次数
    }

    [System.Serializable]
    public class SpecialElementConfig
    {
        public bool IsSpecialElement;     // 是否为特殊元素
        public string EffectID;           // 效果ID
        public EffectTriggerType TriggerType; // 触发类型
        public string Description;        // 特殊元素描述
        
        // 特殊元素类型配置
        public SpecialElementType Type;   // 特殊元素类型
        public ActiveElementConfig ActiveConfig;  // 主动特殊元素配置
        public PassiveElementConfig PassiveConfig; // 被动特殊元素配置
    }

    public enum SpecialElementType
    {
        None,
        Active,
        Passive
    }

    [System.Serializable]
    public class ElementData
    {
        public string Type;               // 元素类型
        public string DisplayName;        // 显示名称
        public Color Color;               // 元素颜色
        public Sprite Icon;               // 元素图标
        public int MaxLevel = 5;          // 最大等级
        public SpecialElementConfig SpecialConfig; // 特殊元素配置
        
        // 升级条件
        public int SpecialUpgradeLevel = 3;   // 升级为特殊元素所需等级
        public int SpecialUpgradeCount = 3;   // 升级为特殊元素所需数量
    }

    public ElementData[] Elements;
    private Dictionary<string, ElementData> elementDataMap;

    private void OnEnable()
    {
        InitializeDataMap();
    }

    private void InitializeDataMap()
    {
        elementDataMap = new Dictionary<string, ElementData>();
        foreach (var element in Elements)
        {
            elementDataMap[element.Type] = element;
        }
    }

    public Element CreateElement(string type, int level)
    {
        var data = GetElementData(type);
        if (data == null) return null;

        // 检查是否应该创建特殊元素
        if (data.SpecialConfig != null && 
            data.SpecialConfig.IsSpecialElement && 
            level >= data.SpecialUpgradeLevel)
        {
            switch (data.SpecialConfig.Type)
            {
                case SpecialElementType.Active:
                    return new ActiveSpecialElement(
                        type,
                        level,
                        1, // 初始效果等级
                        data.SpecialConfig.EffectID,
                        data.SpecialConfig.ActiveConfig.Range
                    );
                case SpecialElementType.Passive:
                    return new PassiveSpecialElement(
                        type,
                        level,
                        1, // 初始效果等级
                        data.SpecialConfig.EffectID
                    );
                default:
                    return new Element(type, level);
            }
        }

        return new Element(type, level);
    }

    public ElementData GetElementData(string type)
    {
        if (elementDataMap == null)
        {
            InitializeDataMap();
        }
        return elementDataMap.TryGetValue(type, out ElementData data) ? data : null;
    }

    // 检查是否可以升级为特殊元素
    public bool CanUpgradeToSpecial(string type, int level, int count)
    {
        var data = GetElementData(type);
        if (data?.SpecialConfig == null || !data.SpecialConfig.IsSpecialElement)
            return false;

        return level >= data.SpecialUpgradeLevel && count >= data.SpecialUpgradeCount;
    }
} 