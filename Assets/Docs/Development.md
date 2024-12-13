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
``` 