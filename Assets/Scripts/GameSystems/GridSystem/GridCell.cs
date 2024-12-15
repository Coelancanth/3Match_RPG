using System;

public class GridCell
{
    public GridCellView View { get; set; }
    public int Row { get; private set; }
    public int Column { get; private set; }
    private Element _element;
    public Element Element
    {
        get {return _element;}
        set 
        {
            if (_element !=value)
            {
                var oldElement = _element;
                _element = value;
                
                // 触发元素变化事件
                OnElementChanged?.Invoke(this);
                
                // 检查并触发效果相关事件
                HandleElementEffectEvents(oldElement, value);
            }
        }
    }

    // 修改事件类型
    public event Action<GridCell, IEffect> OnEffectTriggered;
    public event Action<GridCell, IEffect> OnEffectPrepare;
    public event Action<GridCell, IEffect> OnEffectComplete;
    public event Action<GridCell, IEffect> OnAffectedByEffect;

    public event Action<GridCell> OnElementChanged;
    public string EnemyType { get; set; }
    public int EnemyHealth { get; set; }

    private bool _isHighlighted;
    public bool IsHighlighted
    {
        get { return _isHighlighted; }
        set
        {
            if (_isHighlighted != value)
            {
                _isHighlighted = value;
                OnHighlightChanged?.Invoke(this, value);
            }
        }
    }

    public event Action<GridCell, bool> OnHighlightChanged;

    // 新增方法：处理元素效果事件
    private void HandleElementEffectEvents(Element oldElement, Element newElement)
    {
        if (oldElement is SpecialElement oldSpecial)
        {
            if (oldSpecial is ActiveSpecialElement oldActive)
            {
                var effect = EffectManager.Instance.GetEffect(oldActive.EffectID);
                if (effect != null)
                {
                    OnEffectComplete?.Invoke(this, effect);
                }
            }
        }

        if (newElement is SpecialElement newSpecial)
        {
            if (newSpecial is ActiveSpecialElement newActive)
            {
                var effect = EffectManager.Instance.GetEffect(newActive.EffectID);
                if (effect != null)
                {
                    OnEffectPrepare?.Invoke(this, effect);
                }
            }
        }
    }

    // 修改方法参数类型
    public void TriggerEffect(IEffect effect)
    {
        OnEffectTriggered?.Invoke(this, effect);
    }

    public void ReceiveEffect(IEffect effect)
    {
        OnAffectedByEffect?.Invoke(this, effect);
    }

    public GridCell(int row, int column)
    {
        Row = row;
        Column = column;
        Element = null;
        EnemyType = null;
        EnemyHealth = 0;
    }

    public bool IsMovable()
    {
    return Element != null; // 示例逻辑：有元素时可移动
    }

}
