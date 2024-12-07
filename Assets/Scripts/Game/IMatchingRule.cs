using System.Collections.Generic;
public interface IMatchingRule
{
    bool IsMatch(GridCell cell1, GridCell cell2);
}


public class CompositeMatchingRule : IMatchingRule
{
    private readonly List<IMatchingRule> rules;

    public CompositeMatchingRule(IEnumerable<IMatchingRule> rules)
    {
        this.rules = new List<IMatchingRule>(rules);
    }

    //public bool IsMatch(GridCell cell1, GridCell cell2)
    //{
        //// 修改：任意规则匹配即返回 true
        //foreach (var rule in rules)
        //{
            //if (rule.IsMatch(cell1, cell2))
            //{
                //return true; // 如果任意规则匹配，返回 true
            //}
        //}
        //return false;
    //}

    public bool IsMatch(GridCell cell1, GridCell cell2)
    {
        foreach (var rule in rules)
        {
            if (!rule.IsMatch(cell1, cell2))
            {
                return false; // 如果任意规则不匹配，返回 false
            }
        }
        return true; // 所有规则都匹配
    }

}


public class LevelMatchingRule : IMatchingRule
{
    public bool IsMatch(GridCell cell1, GridCell cell2)
    {
        return cell1.Element != null && cell2.Element != null &&
               cell1.Element.Level == cell2.Element.Level;
    }
}

public class TypeMatchingRule: IMatchingRule
{
    public bool IsMatch(GridCell cell1, GridCell cell2)
    {
        return cell1.Element != null && cell2.Element != null &&
               cell1.Element.Type == cell2.Element.Type;
    }
    
}