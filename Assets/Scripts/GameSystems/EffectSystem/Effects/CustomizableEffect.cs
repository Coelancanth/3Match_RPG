using UnityEngine;

/// <summary>
/// 可自定义参数的效果基类
/// </summary>
public abstract class CustomizableEffect : Effect
{
    protected CustomizableEffect(EffectConfig.EffectData config) : base(config)
    {
    }

    // 辅助方法：从CustomParameters获取参数
    protected T GetCustomParameter<T>(string key, T defaultValue = default)
    {
        if (config.CustomParameters != null && 
            config.CustomParameters.TryGetValue(key, out object value))
        {
            try
            {
                return (T)value;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"参数转换失败: {key}, {e.Message}");
                return defaultValue;
            }
        }
        return defaultValue;
    }
} 