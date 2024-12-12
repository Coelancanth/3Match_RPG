using UnityEngine;
using System.Collections.Generic;

public class EffectContext
{
    public GridManager GridManager { get; set; }
    public GridCell SourceCell { get; set; }
    public GridCell TargetCell { get; set; }
    public Element SourceElement { get; set; }
    public List<GridCell> AffectedCells { get; set; }
    public Dictionary<string, object> CustomData { get; set; }

    public EffectContext()
    {
        CustomData = new Dictionary<string, object>();
        AffectedCells = new List<GridCell>();
    }
} 