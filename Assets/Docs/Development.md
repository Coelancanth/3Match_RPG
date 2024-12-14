# 开发文档

## 效果系统重构

### 一、核心改动

#### 1. 配置系统改进
- 创建了`EffectConfig` ScriptableObject用于集中管理效果配置
- 实现了类似`ElementConfig`的数据管理机制
- 添加了配置数据的缓存和索引功能

#### 2. 数据结构优化

##### 2.1 ElementModifier类
```csharp
[System.Serializable]
public class ElementModifier
{
    public string ElementType;   // 元素类型
    public float ModifierValue;  // 修正值
}
```
- 将Dictionary改为可序列化的数据结构
- 支持在Unity Inspector中直接编辑
- 运行时动态转换为Dictionary以保持使用便利性

##### 2.2 CustomParameter类
```csharp
[System.Serializable]
public class CustomParameter
{
    public string Key;
    public string Value;
}
```
- 用于存储效果的自定义参数
- 支持字符串形式的参数值存储

#### 3. 效果数据管理
```csharp
public class EffectData
{
    // 基础信息
    public string ID;
    public string Name;
    public string Description;
    public EffectType Type;
    public EffectTriggerType TriggerType;
    
    // 效果参数
    public int Range = 1;
    public int BaseDamage = 0;
    
    // 可序列化的修正值列表
    [SerializeField]
    private List<ElementModifier> elementModifierList;
    
    // 运行时字典
    private Dictionary<string, float> _elementModifiers;
}
```

#### 4. 工厂模式实现
```csharp
public class EffectFactory
{
    private static Dictionary<string, System.Type> effectTypeMap;
    
    public static Effect CreateEffect(EffectConfig.EffectData config)
    {
        // 通过反射创建效果实例
    }
}
```

### 二、功能改进

#### 1. 配置管理
- 支持运行时动态加载效果配置
- 提供配置数据的缓存机制
- 实现配置数据的快速查询

#### 2. 效果创建
- 统一的效果创建接口
- 基于配置的参数注入
- 支持效果实例的复用

#### 3. 参数系统
- 可序列化的参数存储
- 支持自定义参数扩展
- 运行时参数转换机制

### 三、使用示例

#### 1. 配置定义
```yaml
Effects:
  - ID: effect_fireball
    Name: 火球术
    Description: 增强范围内的火元素，削弱范围内的水元素
    Type: ElementChange
    TriggerType: OnEliminate
    Range: 2
    BaseDamage: 1
    ElementModifiers:
      - ElementType: Fire
        ModifierValue: 1.5
      - ElementType: Water
        ModifierValue: 0.5
```

#### 2. 效果使用
```csharp
// 创建效果实例
var effect = effectConfig.CreateEffect("effect_fireball");

// 注册效果
EffectManager.Instance.RegisterEffect(effect);

// 触发效果
EffectManager.Instance.QueueEffect(effectId, context);
```

### 四、后续优化方向

#### 1. 配置工具
- 开发配置编辑器工具
- 添加配置验证机制
- 支持配置模板系统

#### 2. 效果组合
- 实现效果组合机制
- 支持条件触发系统
- 添加效果优先级控制

#### 3. 性能优化
- 效果实例池化
- 参数缓存优化
- 配置加载优化

### 五、效果系统实现细节

#### 1. 效果类层次结构
```
Effect (抽象基类)
├── CustomizableEffect (可自定义参数的效果基类)
    ├── RangeEliminateEffect (范围消除效果)
    └── FireballEffect (火球效果)
```

##### 1.1 Effect基类设计
```csharp
public abstract class Effect
{
    protected readonly EffectConfig.EffectData config;
    
    // 标准化的效果执行流程
    public virtual void Execute(EffectContext context)
    {
        // 1. 前置检查
        // 2. 获取影响范围
        // 3. 应用效果
        // 4. 触发连锁效果
    }
    
    // 强制子类实现的核心方法
    protected abstract void ApplyEffect(EffectContext context, List<GridCell> affectedCells);
    public abstract List<GridCell> GetAffectedCells(EffectContext context);
}
```
- 定义了效果系统的基本框架
- 实现了标准化的执行流程
- 提供了可扩展的抽象方法

##### 1.2 CustomizableEffect类设计
```csharp
public abstract class CustomizableEffect : Effect 
{
    protected T GetCustomParameter<T>(string key, T defaultValue = default);
}
```
- 为需要自定义参数的效果提供基础支持
- 实现了类型安全的参数获取方法
- 提供了默认值机制

##### 1.3 具体效果实现示例（FireballEffect）
```csharp
public class FireballEffect : CustomizableEffect
{
    private readonly int range;
    private readonly int baseDamage;
    private readonly RangeShape shape;

    protected override void ApplyEffect(context, affectedCells)
    {
        // 对范围内的元素造成伤害
        // 根据元素类型应用不同的伤害修正
    }
}
```
- 通过构造函数读取配置参数
- 使用RangeShapeHelper处理范围计算
- 实现元素克制系统

#### 2. 改进要点说明

##### 2.1 效果执行流程标准化
- 将效果执行拆分为清晰的步骤
- 在基类中实现通用逻辑
- 允许子类通过重写方法自定义行为

##### 2.2 参数处理优化
- 将自定义参数功能从基类中分离
- 创建专门的CustomizableEffect处理参数
- 提供类型安全的参数获取方法

##### 2.3 范围计算统一化
- 使用RangeShapeHelper统一处理范围计算
- 支持多种范围形状
- 便于扩展新的范围类型

##### 2.4 效果组合与扩展
- 支持效果之间的连锁触发
- 可以通过配置定义连锁关系
- 便于实现复杂的效果组合

#### 3. 配置示例
```yaml
# 火球效果配置
ID: effect_fireball
Type: ElementChange
CustomParameters:
  range: 2
  baseDamage: 3
  shape: Circle
ElementModifiers:
  Fire: 1.5    # 对火元素增强
  Water: 0.5   # 对水元素削弱
  Plant: 2.0   # 对植物特效
```

#### 4. 使用示例
```csharp
// 创建效果实例
var fireball = new FireballEffect(config);

// 创建效果上下文
var context = new EffectContext 
{
    GridManager = gridManager,
    SourceCell = sourceCell
};

// 执行效果
fireball.Execute(context);
```

#### 5. 后续优化方向
1. 效果预览系统
   - 实现效果范围预览
   - 显示预期的效果结果
   - 支持交互式调整

2. 效果组合系统
   - 支持多重效果叠加
   - 实现效果优先级
   - 处理效果间的互相影响

3. 参数验证系统
   - 配置数据验证
   - 运行时参数检查
   - 错误提示和处理



