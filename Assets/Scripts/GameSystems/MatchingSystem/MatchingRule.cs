using System;

public class MatchingRule
{
    public string TriggerType { get; private set; }
    public string TargetType { get; private set; }
    public Func<int, int, int, bool> Condition { get; private set; }
    public string Description { get; set; }

    public MatchingRule(string triggerType, string targetType, Func<int, int, int, bool> condition)
    {
        TriggerType = triggerType;
        TargetType = targetType;
        Condition = condition;
    }

    public bool Evaluate(string triggerType, string targetType, int triggerValue, int groupSum, int groupCount)
    {
        return TriggerType == triggerType && 
               TargetType == targetType && 
               Condition(triggerValue, groupSum, groupCount);
    }
} 