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
    protected readonly ElementConfig elementConfig; // 元素配置

    public SpecialElementModifyEffect(EffectConfig.EffectData config) : base(config)
    {
        // 从配置中读取特殊元素参数
        specialLevel = GetCustomParameter("specialLevel", 1);
        effectID = GetCustomParameter("effectID", "");
        elementConfig = Resources.Load<ElementConfig>("Configs/ElementConfig");
    }

    protected override void ApplyEffect(EffectContext context, List<GridCell> affectedCells)
    {
        // 如果是升级为特殊元素，使用新的处理逻辑
        if (modifyType == ModifyType.UpgradeToSpecial)
        {
            UpgradeToSpecialElements(affectedCells);
        }
        else
        {
            // 其他情况使用基类的处理逻��
            base.ApplyEffect(context, affectedCells);
        }
    }

    protected virtual void UpgradeToSpecialElements(List<GridCell> cells)
    {
        foreach (var cell in cells)
        {
            if (cell.Element == null) continue;

            // 检查是否满足升级条件
            if (elementConfig.CanUpgradeToSpecial(cell.Element.Type, cell.Element.Value, 1))
            {
                // 创建新的特殊元素
                cell.Element = elementConfig.CreateElement(cell.Element.Type, cell.Element.Value);
                Debug.Log($"元素升级为特殊元素：({cell.Row}, {cell.Column})");
            }
        }
    }
} 