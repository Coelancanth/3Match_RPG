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

    // 添加新的效果相关事件
    // 当效果被触发时
    public event Action<GridCell, Effect> OnEffectTriggered;
    // 当效果准备开始时
    public event Action<GridCell, Effect> OnEffectPrepare;
    // 当效果结束时
    public event Action<GridCell, Effect> OnEffectComplete;
    // 当格子被效果影响时
    public event Action<GridCell, Effect> OnAffectedByEffect;

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
        // 处理旧元素的效果清理
        if (oldElement is SpecialElement oldSpecial)
        {
            // 触发效果结束事件
            if (oldSpecial is ActiveSpecialElement oldActive)
            {
                // 获取对应的Effect并触发结束事件
                var effect = EffectManager.Instance.GetEffect(oldActive.EffectID);
                if (effect != null)
                {
                    OnEffectComplete?.Invoke(this, effect);
                }
            }
        }

        // 处理新元素的效果初始化
        if (newElement is SpecialElement newSpecial)
        {
            // 触发效果准备事件
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

    // 新增方法：触发效果
    public void TriggerEffect(Effect effect)
    {
        OnEffectTriggered?.Invoke(this, effect);
    }

    // 新增方法：接收效果影响
    public void ReceiveEffect(Effect effect)
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
