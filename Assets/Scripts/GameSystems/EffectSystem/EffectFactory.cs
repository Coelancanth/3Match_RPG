using UnityEngine;
using System.Collections.Generic;

public class EffectFactory
{
    private static Dictionary<string, System.Type> effectTypeMap = new Dictionary<string, System.Type>()
    {
        { "effect_fireball", typeof(FireballEffect) },
        // 在这里添加更多效果类型映射
    };

    public static Effect CreateEffect(EffectConfig.EffectData config)
    {
        if (effectTypeMap.TryGetValue(config.ID, out System.Type effectType))
        {
            // 使用反射创建效果实例
            Effect effect = (Effect)System.Activator.CreateInstance(effectType, config);
            return effect;
        }
        
        Debug.LogError($"未找到效果类型: {config.ID}");
        return null;
    }
}