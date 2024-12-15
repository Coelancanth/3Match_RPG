## [2024-12-15 22:33:56] Element系统事件处理重构

### 重大改动
1. 事件系统改造
   - 将Element中的事件(event)改为事件处理器(Action)
   - 添加事件注册/注销机制
   - 改进事件触发方式

2. 属性命名统一
   - 将Level属性改回Value,保持与原有系统一致性
   - 统一使用Value相关命名

3. 元素-网格交互优化
   - 重构GridCell中的元素变化处理逻辑
   - 添加自动事件注册/注销机制
   - 分离值变化和效果触发的处理逻辑

4. 删去了Effect，准备重新开始书写相关的代码

### 具体改动说明

#### 1. Element基类改动
```csharp
public abstract class Element
{
    // 改用处理器替代事件
    protected Action<Element> ValueChangedHandler;
    protected Action<Element> EffectTriggeredHandler;

    // 提供注册方法
    public void RegisterValueChangedHandler(Action<Element> handler)
    {
        ValueChangedHandler += handler;
    }

    public void RegisterEffectTriggeredHandler(Action<Element> handler)
    {
        EffectTriggeredHandler += handler;
    }
}
```
- 使用Action替代event,提供更灵活的处理器注册机制
- 支持多处理器注册和注销
- 避免事件订阅可能导致的内存泄漏

#### 2. GridCell改动
```csharp
public Element Element
{
    set 
    {
        if (_element != value)
        {
            // 注销旧元素事件
            if (_element != null)
            {
                _element.RegisterValueChangedHandler(null);
                _element.RegisterEffectTriggeredHandler(null);
            }

            _element = value;

            // 注册新元素事件
            if (_element != null)
            {
                _element.RegisterValueChangedHandler(OnElementValueChanged);
                _element.RegisterEffectTriggeredHandler(OnElementEffectTriggered);
            }

            OnElementChanged?.Invoke(this);
        }
    }
}
```
- 自动管理元素事件的注册和注销
- 分离值变化和效果触发的处理逻辑
- 保持对外事件通知的一致性

#### 3. 事件处理方法
```csharp
private void OnElementValueChanged(Element element)
{
    // 更新显示
    View?.UpdateElementInfo(this);
    
    // 检查特殊效果触发条件
    if (element is SpecialElement specialElement && element.Value >= 5)
    {
        specialElement.TriggerEffect();
    }
}

private void OnElementEffectTriggered(Element element)
{
    // 处理不同类型元素的效果触发
    if (element is ActiveSpecialElement activeElement)
    {
        OnEffectTriggered?.Invoke(this, new EffectContext {...});
    }
    else if (element is PassiveSpecialElement passiveElement)
    {
        OnEffectTriggered?.Invoke(this, new EffectContext {...});
    }
}
```
- 清晰分离不同类型的事件处理逻辑
- 提供类型安全的效果触发机制
- 支持特殊元素的自定义行为

### 改进优势
1. 更好的内存管理
   - 自动清理不再使用的事件处理器
   - 避免循环引用导致的内存泄漏
   - 支持处理器的动态添加和移除

2. 更灵活的事件处理
   - 支持多个处理器注册
   - 可以动态改变处理逻辑
   - 便于调试和测试

3. 更清晰的代码结构
   - 职责分明的处理方法
   - 类型安全的事件处理
   - 统一的命名规范

4. 更好的扩展性
   - 易于添加新的处理逻辑
   - 支持不同上下文的处理方式
   - 便于添加新的元素类型

### 使用示例
```csharp
// 在DebugConsole中监视元素
public void WatchElement(Element element)
{
    element.RegisterValueChangedHandler(OnElementDebugValueChanged);
    element.RegisterEffectTriggeredHandler(OnElementDebugEffectTriggered);
}

// 在MatchingSystem中处理匹配
private void ProcessMatch(List<GridCell> matchedCells)
{
    var highestValueCell = matchedCells
        .OrderByDescending(cell => cell.Element?.Value ?? 0)
        .First();

    if (highestValueCell.Element != null)
    {
        highestValueCell.Element.RegisterValueChangedHandler(OnMatchUpgradeComplete);
        highestValueCell.Element.Upgrade();
    }
}
```

### 后续优化方向
1. 添加事件优先级系统
2. 实现事件处理器池化
3. 添加异步事件支持
4. 优化事件触发性能
5. 添加事件处理监控工具

### 注意事项
1. 使用RegisterXXXHandler(null)注销所有处理器
2. 避免在处理器中执行耗时操作
3. 注意处理器的注册和注销时机
4. 合理使用处理器组合
