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
        else if (groupCount == 4)
        {
            // 执行4个匹配的逻辑，例如得分加倍
            DoubleScore(matchedGroup);
        }
        else if (groupCount >= 5)
        {
            // 执行5个及以上匹配的逻辑，例如生成特殊元素
            GenerateSpecialElement(matchedGroup);
        }
    }

    private void Eliminate(List<GridCell> matchedGroup)
    {
        foreach (var cell in matchedGroup)
        {
            cell.Element = null; // 消除元素
        }
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

