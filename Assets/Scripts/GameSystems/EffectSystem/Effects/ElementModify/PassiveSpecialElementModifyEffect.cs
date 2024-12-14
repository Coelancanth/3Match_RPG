using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 被动特殊元素修改效果
/// </summary>
public class PassiveSpecialElementModifyEffect : SpecialElementModifyEffect
{
    private readonly float triggerChance;    // 触发概率
    private readonly bool canTriggerMultiple; // 是否可以多次触发
    private readonly int maxTriggersPerTurn;  // 每回合最大触发次数

    public PassiveSpecialElementModifyEffect(EffectConfig.EffectData config) : base(config)
    {
        triggerChance = GetCustomParameter("triggerChance", 1.0f);
        canTriggerMultiple = GetCustomParameter("canTriggerMultiple", false);
        maxTriggersPerTurn = GetCustomParameter("maxTriggersPerTurn", 1);
    }

    protected override void UpgradeToSpecialElements(List<GridCell> cells)
    {
        foreach (var cell in cells)
        {
            if (cell.Element == null) continue;

            // 创建被动特殊元素
            var element = new PassiveSpecialElement(
                cell.Element.Type,
                cell.Element.Value,
                specialLevel,
                effectID
            );
            
            cell.Element = element;
            Debug.Log($"元素升级为被动特殊元素：({cell.Row}, {cell.Column})");
        }
    }
} 