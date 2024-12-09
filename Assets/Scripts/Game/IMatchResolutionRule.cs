using System.Collections.Generic;
public interface IMatchResolutionRule
{
    void ResolveMatch(List<GridCell> matchedGroup, GridCell triggerCell, int groupCount);
}

public class BasicMatchResolutionRule : IMatchResolutionRule
{
    public void ResolveMatch(List<GridCell> matchedGroup, GridCell triggerCell, int groupCount)
    {
        if (groupCount == 3)
        {
            // 执行3个匹配的逻辑，例如消除
            Eliminate(matchedGroup);
        }
        else if (groupCount >= 4)
        {
            // TODO To be extended
            EliminateAllAndUpgradeTrigger(matchedGroup, triggerCell);

            
        }
    }
    
    private void EliminateAllAndUpgradeTrigger(List<GridCell> matchedGroup, GridCell triggerCell)
    {
        foreach (var cell in matchedGroup)
        {
            if(cell.Element != triggerCell.Element)
            {
                cell.Element = null; // 消除元素
            }
            else
            {
                triggerCell.Element = Upgrade(triggerCell.Element);
            }
        }
    }
    

    private void Eliminate(List<GridCell> matchedGroup)
    {
        foreach (var cell in matchedGroup)
        {
            cell.Element = null; // 消除元素
        }
    }
    
    private Element Upgrade(Element element)
    {
        int level = element.Level;
        string type = element.Type;
        return new Element(type, level+1);
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

