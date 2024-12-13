using System.Collections.Generic;

public class EffectManager
{
    private static EffectManager instance;
    public static EffectManager Instance => instance ??= new EffectManager();

    private Dictionary<string, Effect> effectRegistry;
    private Queue<EffectExecutionRequest> effectQueue;

    private EffectManager()
    {
        effectRegistry = new Dictionary<string, Effect>();
        effectQueue = new Queue<EffectExecutionRequest>();
        RegisterDefaultEffects();
    }

    private void RegisterDefaultEffects()
    {
        RegisterEffect(new FireballEffect());
        // 注册其他效果...
    }

    public void RegisterEffect(Effect effect)
    {
        effectRegistry[effect.ID] = effect;
    }

    public void QueueEffect(string effectId, EffectContext context)
    {
        if (effectRegistry.TryGetValue(effectId, out Effect effect))
        {
            effectQueue.Enqueue(new EffectExecutionRequest(effect, context));
        }
    }

    public void ProcessEffectQueue()
    {
        while (effectQueue.Count > 0)
        {
            var request = effectQueue.Dequeue();
            if (request.Effect.CanExecute(request.Context))
            {
                request.Effect.Execute(request.Context);
            }
        }
    }

    public void TriggerEffect(string effectId, EffectContext context)
    {
        if (effectRegistry.TryGetValue(effectId, out Effect effect))
        {
            effect.Execute(context);
        }
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