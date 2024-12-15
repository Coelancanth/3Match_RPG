using System;

public class GridCell
{
    public GridCellView View { get; set; }
    public int Row { get; private set; }
    public int Column { get; private set; }
    private Element _element;
    public Element Element
    {
        get { return _element; }
        set 
        {
            if (_element != value)
            {
                // 取消旧元素的事件注册
                if (_element != null)
                {
                    _element.RegisterValueChangedHandler(null);
                    _element.RegisterEffectTriggeredHandler(null);
                }

                _element = value;

                // 为新元素注册事件处理
                if (_element != null)
                {
                    // 注册值变化处理器
                    _element.RegisterValueChangedHandler(OnElementValueChanged);
                    // 注册效果触发处理器
                }

                // 触发格子的元素变化事件
                OnElementChanged?.Invoke(this);
            }
        }
    }

    // 修改事件类型

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

    // 修改方法参数类型

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

    // 处理元素值变化
    private void OnElementValueChanged(Element element)
    {
        // 更新显示
        View?.UpdateElementInfo(this);
        
        // 检查是否需要触发特殊效果
        if (element is SpecialElement specialElement && element.Value >= 5)
        {
            specialElement.TriggerEffect();
        }
    }
}
