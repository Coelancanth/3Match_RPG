using System.Collections.Generic;

public interface IMatchResolutionRule
{
    void ResolveMatch(List<FilteredGroup> matchedGroup, GridCell triggerCell);
}

public class BasicMatchResolutionRule : IMatchResolutionRule
{
    public void ResolveMatch(List<FilteredGroup> matchedGroup, GridCell triggerCell)
    {
        return;
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

