using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 特殊元素修改效果：继承自ElementModifyEffect，添加特殊元素相关的功能
/// </summary>
public class SpecialElementModifyEffect : ElementModifyEffect
{
    // 特殊元素相关参数
    protected readonly int specialLevel;           // 特殊元素等级
    protected readonly string effectID;            // 效果ID

    public SpecialElementModifyEffect(EffectConfig.EffectData config) : base(config)
    {
        // 从配置中读取特殊元素参数
        specialLevel = GetCustomParameter("specialLevel", 1);
        effectID = GetCustomParameter("effectID", "");
    }

    protected override void ApplyEffect(EffectContext context, List<GridCell> affectedCells)
    {
        if (modifyType == ModifyType.UpgradeToSpecial)
        {
            UpgradeToSpecialElements(affectedCells);
        }
        else
        {
            base.ApplyEffect(context, affectedCells);
        }
    }

    protected virtual void UpgradeToSpecialElements(List<GridCell> cells)
    {
        foreach (var cell in cells)
        {
            if (cell.Element == null) continue;

            if (elementConfig.CanUpgradeToSpecial(cell.Element.Type, cell.Element.Value, specialLevel))
            {
                cell.Element = elementConfig.CreateElement(cell.Element.Type, cell.Element.Value);
                Debug.Log($"元素升级为特殊元素：({cell.Row}, {cell.Column})");
            }
        }
    }
} 