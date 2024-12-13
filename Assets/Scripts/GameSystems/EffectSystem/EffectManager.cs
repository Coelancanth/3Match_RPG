using System.Collections.Generic;
using UnityEngine;

public class EffectManager
{
    private static EffectManager instance;
    public static EffectManager Instance => instance ??= new EffectManager();

    private Dictionary<string, Effect> effectRegistry;
    private Queue<EffectExecutionRequest> effectQueue;

    private EffectManager()
    {
        Debug.Log("EffectManager: 初始化");
        effectRegistry = new Dictionary<string, Effect>();
        effectQueue = new Queue<EffectExecutionRequest>();
        RegisterDefaultEffects();
    }

    private void RegisterDefaultEffects()
    {
        Debug.Log("EffectManager: 注册默认效果");
        //RegisterEffect(new FireballEffect());
        
        // 打印已注册的效果
        foreach (var effect in effectRegistry)
        {
            Debug.Log($"已注册效果: {effect.Key} -> {effect.Value.GetType().Name}");
        }
    }

    public void RegisterEffect(Effect effect)
    {
        Debug.Log($"EffectManager: 注册效果 {effect.ID}");
        effectRegistry[effect.ID] = effect;
    }

    public void QueueEffect(string effectId, EffectContext context)
    {
        Debug.Log($"EffectManager: 尝试将效果 {effectId} 加入队列");
        if (effectRegistry.TryGetValue(effectId, out Effect effect))
        {
            Debug.Log($"EffectManager: 成功找到效果 {effectId}，加入队列");
            effectQueue.Enqueue(new EffectExecutionRequest(effect, context));
        }
        else
        {
            Debug.LogError($"EffectManager: 未找到效果 {effectId}");
            // 打印所有已注册的效果ID
            Debug.Log("当前已注册的效果:");
            foreach (var registeredEffect in effectRegistry)
            {
                Debug.Log($"- {registeredEffect.Key}");
            }
        }
    }

    public void ProcessEffectQueue()
    {
        Debug.Log($"EffectManager: 开始处理效果队列，当前队列长度: {effectQueue.Count}");
        while (effectQueue.Count > 0)
        {
            var request = effectQueue.Dequeue();
            Debug.Log($"EffectManager: 执行效果 {request.Effect.ID}");
            
            if (request.Effect.CanExecute(request.Context))
            {
                try
                {
                    request.Effect.Execute(request.Context);
                    Debug.Log($"EffectManager: 效果 {request.Effect.ID} 执行完成");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"EffectManager: 效果 {request.Effect.ID} 执行失败");
                    Debug.LogException(e);
                }
            }
            else
            {
                Debug.LogWarning($"EffectManager: 效果 {request.Effect.ID} 无法执行");
            }
        }
        Debug.Log("EffectManager: 效果队列处理完成");
    }

    // 用于调试的辅助方法
    public void DebugPrintRegisteredEffects()
    {
        Debug.Log("=== 已注册的效果列表 ===");
        foreach (var effect in effectRegistry)
        {
            Debug.Log($"ID: {effect.Key}");
            Debug.Log($"类型: {effect.Value.GetType().Name}");
            Debug.Log($"名称: {effect.Value.Name}");
            Debug.Log($"描述: {effect.Value.Description}");
            Debug.Log("---");
        }
    }

    public void TriggerEffect(string effectId, EffectContext context)
    {
        if (effectRegistry.TryGetValue(effectId, out Effect effect))
        {
            effect.Execute(context);
        }
    }

    // 新增方法：根据ID获取Effect
    public Effect GetEffect(string effectId)
    {
        if (effectRegistry.TryGetValue(effectId, out Effect effect))
        {
            return effect;
        }
        return null;
    }
}

public class EffectExecutionRequest
{
    public Effect Effect { get; }
    public EffectContext Context { get; }

    public EffectExecutionRequest(Effect effect, EffectContext context)
    {
        Effect = effect;
        Context = context;
    }
}