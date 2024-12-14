using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 主动特殊元素修改效果
/// </summary>
public class ActiveSpecialElementModifyEffect : SpecialElementModifyEffect
{
    private readonly int range;        // 影响范围
    private readonly bool needTarget;  // 是否需要目标

    public ActiveSpecialElementModifyEffect(EffectConfig.EffectData config) : base(config)
    {
        range = GetCustomParameter("range", 1);
        needTarget = GetCustomParameter("needTarget", true);
    }

    protected override void UpgradeToSpecialElements(List<GridCell> cells)
    {
        foreach (var cell in cells)
        {
            if (cell.Element == null) continue;

            // 创建主动特殊元素
            var element = elementConfig.CreateElement(cell.Element.Type, cell.Element.Value) as ActiveSpecialElement;
            if (element != null)
            {
                cell.Element = element;
                Debug.Log($"元素升级为主动特殊元素：({cell.Row}, {cell.Column})");
            }
        }
    }
} 