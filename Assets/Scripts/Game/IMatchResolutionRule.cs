using System.Collections.Generic;
using UnityEngine;
public interface IMatchResolutionRule
{
    void ResolveMatch(List<GridCell> matchedCells, GridCell triggerCell);
}

public class ThreeMatchRule : IMatchResolutionRule
{
    public void ResolveMatch(List<GridCell> matchedCells, GridCell triggerCell)
    {
        foreach (GridCell cell in matchedCells)
        {
            cell.Element = null;
        }
        Debug.Log("Resolving 3 match rule triggered by cell: " + triggerCell);
        // Handle 3-matching case
    }
}

public class FourMatchRule : IMatchResolutionRule
{
    public void ResolveMatch(List<GridCell> matchedCells, GridCell triggerCell)
    {
        Debug.Log("Resolving 4 match rule triggered by cell: " + triggerCell);
        // Handle 4-matching case
    }
}

public class FiveMatchRule : IMatchResolutionRule
{
    public void ResolveMatch(List<GridCell> matchedCells, GridCell triggerCell)
    {
        Debug.Log("Resolving 5 match rule triggered by cell: " + triggerCell);
        // Handle 5-matching case
    }
}

public class SixMatchRule : IMatchResolutionRule
{
    public void ResolveMatch(List<GridCell> matchedCells, GridCell triggerCell)
    {
        Debug.Log("Resolving 6 match rule triggered by cell: " + triggerCell);
        // Handle 6-matching case
    }
}

public class SevenMatchRule : IMatchResolutionRule
{
    public void ResolveMatch(List<GridCell> matchedCells, GridCell triggerCell)
    {
        Debug.Log("Resolving 7 match rule triggered by cell: " + triggerCell);
        // Handle 7-matching case
    }
}

public class EightMatchRule : IMatchResolutionRule
{
    public void ResolveMatch(List<GridCell> matchedCells, GridCell triggerCell)
    {
        Debug.Log("Resolving 8 match rule triggered by cell: " + triggerCell);
        // Handle 8-matching case
    }
}

public class NineMatchRule : IMatchResolutionRule
{
    public void ResolveMatch(List<GridCell> matchedCells, GridCell triggerCell)
    {
        Debug.Log("Resolving 9 match rule triggered by cell: " + triggerCell);
        // Handle 9-matching case
    }
}

public class MatchResolutionFactory
{
    public static IMatchResolutionRule GetRule(int matchCount)
    {
        switch (matchCount)
        {
            case 3:
                return new ThreeMatchRule();
            case 4:
                return new FourMatchRule();
            case 5:
                return new FiveMatchRule();
            case 6:
                return new SixMatchRule();
            case 7:
                return new SevenMatchRule();
            case 8:
                return new EightMatchRule();
            case 9:
                return new NineMatchRule();
            default:
                return null; // No special rule for other match counts
        }
    }
}