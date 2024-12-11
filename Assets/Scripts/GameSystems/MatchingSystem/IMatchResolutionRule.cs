using System.Collections.Generic;
public interface IMatchResolutionRule
{
    void ResolveMatch(List<GridCell> matchedGroup, GridCell triggerCell);
}

public class BasicMatchResolutionRule : IMatchResolutionRule
{
    public void ResolveMatch(List<GridCell> matchedGroup, GridCell triggerCell)
    {
        int groupValueSum = 0;
        foreach(var cell in matchedGroup)
        {
            groupValueSum += cell.Element.Value;;
        }
        //if (groupValueSum == 3)
        //{
            //// 执行3个匹配的逻辑，例如消除
            //EliminateAll(matchedGroup);
        //}
        //else if (groupValueSum >= 4)
        //{
            //// TODO To be extended
            //EliminateAndIncrease(matchedGroup, triggerCell);

            //
        //}
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

