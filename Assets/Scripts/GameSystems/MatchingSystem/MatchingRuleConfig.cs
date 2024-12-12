using System;
using System.Collections.Generic;

public static class MatchingRuleConfig
{
    public static List<MatchingRule> GetDefaultRules()
    {
        return new List<MatchingRule>
        {
            // 基础元素互相作用规则
            CreateRule("Fire", "Water", (trigger, sum, count) => 
                sum >=1, "【触发元素】：火，【组元素】：水，【规则】：组合总和大于等于1"),
            
            CreateRule("Fireball", "Fire", (trigger, sum, count) => 
                sum >= 2, "【触发元素】：火球，【组元素】：火，【规则】：组合总和大于等于2"),
                
            CreateRule("Water", "Grass", (trigger, sum, count) => 
                sum > trigger, "水对草：组合总和大于触发值"),
                
            CreateRule("Grass", "Fire", (trigger, sum, count) => 
                count > 3, "草对火：组合数量大于3"),
                
            // 同类元素规则
            CreateRule("Fire", "Fire", (trigger, sum, count) => 
                sum >= 3, "【触发元素】：火，【组元素】：火，【规则】：组合总和大于等于3"),
                
            CreateRule("Water", "Water", (trigger, sum, count) => 
                sum == trigger, "水对水：组合总和等于触发值"),
                
            CreateRule("Grass", "Grass", (trigger, sum, count) => 
                sum == trigger, "草对草：组合总和等于触发值"),
                
            // 高级组合规则
            CreateRule("Fire", "Grass", (trigger, sum, count) => 
                sum >= trigger * 2 && count >= 4, 
                "火对草：组合总和大于等于触发值2倍且数量大于等于4"),
                
            CreateRule("Water", "Fire", (trigger, sum, count) => 
                sum <= trigger / 2 || count > 5,
                "水对火：组合总和小于等于触发值一半或数量大于5")
        };
    }

    private static MatchingRule CreateRule(
        string triggerType, 
        string targetType, 
        Func<int, int, int, bool> condition,
        string description)
    {
        return new MatchingRule(triggerType, targetType, condition)
        {
            Description = description
        };
    }
}