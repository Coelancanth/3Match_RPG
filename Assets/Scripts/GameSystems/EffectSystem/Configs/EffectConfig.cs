using UnityEngine;
using System.Collections.Generic;

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
        public EffectType Type;             // 效果类型
        public EffectTriggerType TriggerType; // 触发类型
        
        // 效果参数
        public int Range = 1;               // 影响范围
        public int BaseDamage = 0;          // 基础伤害
        
        [SerializeField]
        private List<ElementModifier> elementModifierList = new List<ElementModifier>(); // 可序列化的列表
        
        // 运行时使用的字典
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
        
        // 连锁效果配置
        public List<string> ChainEffectIDs = new List<string>();  // 可能触发的连锁效果ID
        
        // 自定义参数字典，用于存储特定效果的额外参数
        [SerializeField]
        // TODO: 之后删去，因为原则上Element并不需要CustomParameter， 这个是为了还未实现的sub-effect实现的，

        public List<CustomParameter> customParameterList = new List<CustomParameter>();
        private Dictionary<string, object> _customParameters = new Dictionary<string, object>();
        public Dictionary<string, object> CustomParameters 
        { 
            get => _customParameters;
            set => _customParameters = value;
        }
    }

    [System.Serializable]
    public class CustomParameter
    {
        public string Key;
        public string Value; // 使用string类型存储，需要时再转换
    }

    public List<EffectData> Effects;  // 所有效果配置列表
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

    // 创建效果实例的方法
    public Effect CreateEffect(string effectId)
    {
        var data = GetEffectData(effectId);
        if (data == null)
        {
            Debug.LogError($"未找到效果配置: {effectId}");
            return null;
        }

        return EffectFactory.CreateEffect(data);
    }

    // 获取效果数据的方法
    public EffectData GetEffectData(string effectId)
    {
        if (effectDataMap == null)
        {
            InitializeDataMap();
        }
        return effectDataMap.TryGetValue(effectId, out EffectData data) ? data : null;
    }
}