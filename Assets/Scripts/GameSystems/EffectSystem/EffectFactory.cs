using UnityEngine;
using System.Collections.Generic;

public class EffectFactory
{
    private static Dictionary<string, System.Type> effectTypeMap = new Dictionary<string, System.Type>()
    {
        { "effect_fireball", typeof(FireballEffect) },
        { "effect_range_eliminate", typeof(RangeEliminateEffect) },
        { "effect_element_modify", typeof(ElementModifyEffect) },
        { "effect_active_special", typeof(ActiveSpecialElementModifyEffect) },
        { "effect_passive_special", typeof(PassiveSpecialElementModifyEffect) }
        // 在这里添加更多效果类型映射
    };

    public static IEffect CreateEffect(EffectConfig.EffectData config)
    {
        if (effectTypeMap.TryGetValue(config.ID, out System.Type effectType))
        {
            try
            {
                // 使用反射创建效果实例
                IEffect effect = (IEffect)System.Activator.CreateInstance(effectType, config);
                return effect;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"创建效果实例失败: {config.ID}, {e.Message}");
                return null;
            }
        }
        
        Debug.LogError($"未找到效果类型: {config.ID}");
        return null;
    }
}