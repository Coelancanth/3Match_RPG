using UnityEngine;
using System.Collections.Generic;
//using GameSystems.EffectSystem;

[CreateAssetMenu(fileName = "EffectConfig", menuName = "Game/Effect Config")]
public class EffectConfig : ScriptableObject
{
    [System.Serializable]
    public class ElementModifier
    {
        public string ElementType;  // 元素类型
        public float ModifierValue; // 修正值
    }

    [System.Serializable]
    public class EffectData
    {
        public string ID;                    // 效果唯一标识
        public string Name;                  // 效果名称
        public string Description;           // 效果描述
        //public EffectType Type;             // 效果类型
        //public EffectTriggerType TriggerType; // 触发类型
        
        // 基础参数
        public int Range = 1;               // 影响范围
        public int BaseDamage = 0;          // 基础伤害
        
        // 特殊元素相关参数
        public bool IsSpecialElementEffect;  // 是否为特殊元素效果
        public int DefaultSpecialLevel = 1;  // 默认特殊等级
        public bool CanStackSpecialLevel;    // 是否可叠加特殊等级
        public int MaxSpecialLevel = 3;      // 最大特殊等级
        
        [SerializeField]
        private List<ElementModifier> elementModifierList = new List<ElementModifier>();
        
        private Dictionary<string, float> _elementModifiers;
        public Dictionary<string, float> ElementModifiers
        {
            get
            {
                if (_elementModifiers == null)
                {
                    _elementModifiers = new Dictionary<string, float>();
                    foreach (var modifier in elementModifierList)
                    {
                        _elementModifiers[modifier.ElementType] = modifier.ModifierValue;
                    }
                }
                return _elementModifiers;
            }
        }
        
        public List<string> ChainEffectIDs = new List<string>();
        
        [SerializeField]
        public List<CustomParameter> customParameterList = new List<CustomParameter>();
        
        private Dictionary<string, object> _customParameters;
        public Dictionary<string, object> CustomParameters 
        { 
            get
            {
                if (_customParameters == null)
                {
                    InitializeCustomParameters();
                }
                return _customParameters;
            }
            set => _customParameters = value;
        }

        private void InitializeCustomParameters()
        {
            _customParameters = new Dictionary<string, object>();
            foreach (var param in customParameterList)
            {
                // 尝试解析不同类型的参数
                if (bool.TryParse(param.Value, out bool boolValue))
                {
                    _customParameters[param.Key] = boolValue;
                }
                else if (int.TryParse(param.Value, out int intValue))
                {
                    _customParameters[param.Key] = intValue;
                }
                else if (float.TryParse(param.Value, out float floatValue))
                {
                    _customParameters[param.Key] = floatValue;
                }
                else
                {
                    _customParameters[param.Key] = param.Value;
                }
            }
        }

        // 验证效果配置的有效性
        public bool Validate(out string error)
        {
            error = string.Empty;
            
            if (string.IsNullOrEmpty(ID))
            {
                error = "效果ID不能为空";
                return false;
            }

            if (string.IsNullOrEmpty(Name))
            {
                error = $"效果 {ID} 的名称不能为空";
                return false;
            }

            if (Range < 0)
            {
                error = $"效果 {ID} 的范围不能小于0";
                return false;
            }

            if (IsSpecialElementEffect)
            {
                if (DefaultSpecialLevel < 1)
                {
                    error = $"效果 {ID} 的默认特殊等级不能小于1";
                    return false;
                }

                if (MaxSpecialLevel < DefaultSpecialLevel)
                {
                    error = $"效果 {ID} 的最大特殊等级不能小于默认特殊等级";
                    return false;
                }
            }

            return true;
        }
    }

    [System.Serializable]
    public class CustomParameter
    {
        public string Key;
        public string Value;
    }

    public List<EffectData> Effects;
    private Dictionary<string, EffectData> effectDataMap;

    private void OnEnable()
    {
        InitializeDataMap();
    }

    private void InitializeDataMap()
    {
        effectDataMap = new Dictionary<string, EffectData>();
        foreach (var effect in Effects)
        {
            effectDataMap[effect.ID] = effect;
        }
    }

    // 修改返回类型为IEffect
    //public IEffect CreateEffect(string effectId)
    //{
        //var data = GetEffectData(effectId);
        //if (data == null)
        //{
            //Debug.LogError($"未找到效果配置: {effectId}");
            //return null;
        //}

        //// 验证效果配置
        //if (!data.Validate(out string error))
        //{
            //Debug.LogError($"效果配置无效: {error}");
            //return null;
        //}

        //return EffectFactory.CreateEffect(data);
    //}

    public EffectData GetEffectData(string effectId)
    {
        if (effectDataMap == null)
        {
            InitializeDataMap();
        }
        return effectDataMap.TryGetValue(effectId, out EffectData data) ? data : null;
    }

    // 添加批量验证方法
    public bool ValidateAllEffects(out List<string> errors)
    {
        errors = new List<string>();
        bool isValid = true;

        foreach (var effect in Effects)
        {
            if (!effect.Validate(out string error))
            {
                errors.Add($"效果 {effect.ID} 验证失败: {error}");
                isValid = false;
            }
        }

        return isValid;
    }
}