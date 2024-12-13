using System.Collections.Generic;
using UnityEngine;

public interface IMatchResolutionRule
{
    void ResolveMatch(List<FilteredGroup> matchedGroup, GridCell triggerCell);
}

public class BasicMatchResolutionRule : IMatchResolutionRule
{
    public void ResolveMatch(List<FilteredGroup> matchedGroups, GridCell triggerCell)
    {
        // 检查触发元素是否为 ActiveSpecialElement
        if (triggerCell.Element is ActiveSpecialElement activeElement)
        {
            //Debug.Log("ResolveMatch: triggerCell.Element is ActiveSpecialElement");
            // 通知 GameController 显示高亮范围
            //var gameController = GameObject.FindAnyObjectByType<GameController>();
            //if (gameController != null)
            //{
                //gameController.ShowEffectRange(triggerCell, activeElement.ReachRange);
            //}
            //return; // 暂时不执行消除，等待玩家选择释放位置
        }

        //foreach (var group in matchedGroups)
        //{
            //// 先检查是否有特殊元素需要触发效果
            //foreach (var cell in group.Group)
            //{
                //if (cell.Element is ActiveSpecialElement activeElement)
                //{
                    //var context = new EffectContext
                    //{
                        //GridManager = GameObject.FindAnyObjectByType<GridManager>(),
                        //SourceCell = cell,
                        //SourceElement = cell.Element
                    //};
                    //
                    //// 通过EffectManager触发效果
                    //EffectManager.Instance.QueueEffect(activeElement.EffectID, context);
                //}
            //}

            //// 然后执行常规的消除
            //EliminateExceptTrigger(group.Group, triggerCell);
        //}
        //
        //// 处理所有排队的效果
        //EffectManager.Instance.ProcessEffectQueue();
        //
        //// 最后升级触发元素，可能升级为特殊元素
        //if (triggerCell.Element != null)
        //{
            //triggerCell.Element = UpgradeElement(triggerCell.Element);
        //}
    }

    private Element UpgradeElement(Element element)
    {
        // 当元素达到特定条件时，升级为特殊元素
        if (element.Value >= 3 && !(element is SpecialElement))
        {
            // 根据元素类型创建对应的特殊元素
            switch (element.Type)
            {
                case "Fire":
                    return new ActiveSpecialElement(
                        "Fireball",
                        element.Value + 1,
                        1,  // 初始等级
                        "effect_fireball",  // 效果ID
                        2   // 影响范围
                    );
                // 可以添加其他元素类型的特殊元素升级
                default:
                    return new Element(element.Type, element.Value + 1);
            }
        }
        
        // 普通升级
        return new Element(element.Type, element.Value + 1);
    }

    private void EliminateAndIncrease(List<GridCell> matchedGroup, GridCell triggerCell)
    {
        EliminateExceptTrigger(matchedGroup, triggerCell);
        triggerCell.Element = UpgradeValue(triggerCell.Element);
    }


    private void EliminateExceptTrigger(List<GridCell> matchedGroup, GridCell triggerCell)
    {
        foreach(var cell in matchedGroup)
        {
            if(cell != triggerCell)
            {
                cell.Element = null;
            }
        }
    }

    private void EliminateAll(List<GridCell> matchedGroup)
    {
        foreach (var cell in matchedGroup)
        {
            cell.Element = null; // 消除元素
        }
    }
    
    private Element UpgradeValue(Element element)
    {
        int value = element.Value;
        string type = element.Type;
        return new Element(type,value+1);
    }

    private void DoubleScore(List<GridCell> matchedGroup)
    {
        // 执行得分加倍逻辑
    }

    private void GenerateSpecialElement(List<GridCell> matchedGroup)
    {
        // 根据匹配生成特殊元素
    }
}

