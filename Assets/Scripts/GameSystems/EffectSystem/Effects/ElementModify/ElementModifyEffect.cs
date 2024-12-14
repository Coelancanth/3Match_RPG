using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 元素修改效果：可以改变范围内元素的类型、等级或位置
/// </summary>
public class ElementModifyEffect : CustomizableEffect
{
    // 修改类型枚举
    public enum ModifyType
    {
        ChangeType,      // 改变元素类型
        ChangeValue,     // 改变元素数值
        ChangePosition,  // 改变元素位置
        UpgradeToSpecial // 升级为特殊元素
    }

    // 效果参数
    protected readonly ModifyType modifyType;     // 修改类型
    private readonly RangeShape shape;          // 范围形状
    private readonly int range;                 // 影响范围
    private readonly string targetElementType;  // 目标元素类型（用于ChangeType）
    private readonly int valueModifier;         // 数值修改值（用于ChangeValue）
    private readonly Vector2Int positionOffset; // 位置偏移（用于ChangePosition）
    protected readonly ElementConfig elementConfig; // 添加字段


    public ElementModifyEffect(EffectConfig.EffectData config) : base(config)
    {
        // 加载ElementConfig并添加错误检查
        elementConfig = Resources.Load<ElementConfig>("Configs/ElementConfig");
        if (elementConfig == null)
        {
            Debug.LogError("无法加载ElementConfig，请确保文件位于Resources/Configs/ElementConfig");
            return;
        }

        // 从配置中读取参数
        modifyType = GetCustomParameter("modifyType", ModifyType.ChangeType);
        shape = GetCustomParameter("shape", RangeShape.Square);
        range = GetCustomParameter("range", 1);
        targetElementType = GetCustomParameter("targetElementType", "Fire");
        valueModifier = GetCustomParameter("valueModifier", 1);
        
        // 获取位置偏移
        int offsetX = GetCustomParameter("offsetX", 0);
        int offsetY = GetCustomParameter("offsetY", 0);
        positionOffset = new Vector2Int(offsetX, offsetY);
    }

    public override List<GridCell> GetAffectedCells(EffectContext context)
    {
        var grid = context.GridManager.gridData;
        var center = context.SourceCell;

        // 使用RangeShapeHelper获取影响范围
        return shape switch
        {
            RangeShape.Square => RangeShapeHelper.GetSquareRange(grid, center, range),
            RangeShape.Diamond => RangeShapeHelper.GetDiamondRange(grid, center, range),
            RangeShape.Circle => RangeShapeHelper.GetCircleRange(grid, center, range),
            _ => RangeShapeHelper.GetSquareRange(grid, center, range)
        };
    }

    protected override void ApplyEffect(EffectContext context, List<GridCell> affectedCells)
    {
        Debug.Log($"开始执行{config.Name}效果，修改类型：{modifyType}");

        switch (modifyType)
        {
            case ModifyType.ChangeType:
                ChangeElementTypes(affectedCells);
                break;
            case ModifyType.ChangeValue:
                ChangeElementValues(affectedCells);
                break;
            case ModifyType.ChangePosition:
                ChangeElementPositions(context.GridManager.gridData, affectedCells);
                break;
            case ModifyType.UpgradeToSpecial:
                Debug.LogWarning("基础ElementModifyEffect不支持升级为特殊元素，请使用SpecialElementModifyEffect");
                break;
        }
    }

    private void ChangeElementTypes(List<GridCell> cells)
    {
        if (elementConfig == null)
        {
            Debug.LogError("ElementConfig未正确加载，无法修改元素类型");
            return;
        }

        foreach (var cell in cells)
        {
            if (cell.Element == null)
            {
                Debug.LogWarning($"格子({cell.Row}, {cell.Column})没有元素");
                continue;
            }
            
            try
            {
                int originalValue = cell.Element.Value;
                var newElement = elementConfig.CreateElement(targetElementType, originalValue);
                if (newElement == null)
                {
                    Debug.LogError($"创建元素失败: 类型={targetElementType}, 数值={originalValue}");
                    continue;
                }
                cell.Element = newElement;
                Debug.Log($"元素类型改变：({cell.Row}, {cell.Column}) -> {targetElementType}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"修改元素类型时发生错误: {e.Message}");
            }
        }
    }

    private void ChangeElementValues(List<GridCell> cells)
    {
        foreach (var cell in cells)
        {
            if (cell.Element == null) continue;
            
            // 计算新数值，确保不小于1
            int newValue = Mathf.Max(1, cell.Element.Value + valueModifier);
            cell.Element = new Element(cell.Element.Type, newValue);
            Debug.Log($"元素数值改变：({cell.Row}, {cell.Column}) -> {newValue}");
        }
    }

    private void ChangeElementPositions(Grid grid, List<GridCell> cells)
    {
        // 创建临时列表存储移动操作
        var moves = new List<(GridCell from, GridCell to, Element element)>();

        foreach (var cell in cells)
        {
            if (cell.Element == null) continue;

            // 计算目标位置
            int newRow = cell.Row + positionOffset.x;
            int newCol = cell.Column + positionOffset.y;

            // 检查新位置是否有效
            if (grid.IsValidPosition(newRow, newCol))
            {
                var targetCell = grid.GetCell(newRow, newCol);
                moves.Add((cell, targetCell, cell.Element));
            }
        }

        // 执行所有移动操作
        foreach (var (from, to, element) in moves)
        {
            from.Element = null;
            to.Element = element;
            Debug.Log($"元素位置改变：({from.Row}, {from.Column}) -> ({to.Row}, {to.Column})");
        }
    }
}