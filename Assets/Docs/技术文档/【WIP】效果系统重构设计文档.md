# 效果系统重构设计文档

## 一、重构目标

1. **配置驱动**
   - 将效果的参数从硬编码迁移到配置文件
   - 方便策划调整和平衡游戏
   - 支持快速添加和修改效果

2. **模块解耦**
   - 将效果的定义与实现分离
   - 降低修改成本
   - 提高代码复用性

3. **扩展性提升**
   - 支持效果的组合与连锁
   - 便于添加新的效果类型
   - 灵活的参数配置系统

## 二、核心架构

### 1. 配置系统 (EffectConfig)
```csharp
public class EffectConfig : ScriptableObject
{
public class EffectData
{
public string ID; // 效果唯一标识
public string Name; // 效果名称
public string Description; // 效果描述
public EffectType Type; // 效果类型
public EffectTriggerType TriggerType; // 触发类型
public int Range; // 影响范围
public int BaseDamage; // 基础伤害
public Dictionary<string, float> ElementModifiers; // 元素修正值
public List<string> ChainEffectIDs; // 连锁效果
public Dictionary<string, object> CustomParameters; // 自定义参数
}
}
```

### 2. 工厂模式 (EffectFactory)
- 负责根据配置创建效果实例
- 维护效果类型映射关系
- 支持反射创建效果实例

### 3. 效果基类 (Effect)
- 从配置读取基础属性
- 定义效果的通用接口
- 提供默认实现

### 4. 管理器 (EffectManager)
- 加载效果配置
- 注册和管理效果实例
- 处理效果队列
- 提供效果触发接口

## 三、配置示例
```yaml
Effects:
ID: effect_fireball
Name: 火球术
Description: 增强范围内的火元素，削弱范围内的水元素
Type: ElementChange
TriggerType: OnEliminate
Range: 2
BaseDamage: 1
ElementModifiers:
Fire: 1.5
Water: 0.5
ChainEffectIDs:
effect_steam
effect_burn
```


## 四、实现流程

1. **配置加载**
   - 游戏启动时加载EffectConfig
   - 解析配置数据
   - 初始化效果注册表

2. **效果创建**
   - 通过EffectFactory创建效果实例
   - 注入配置数据
   - 注册到EffectManager

3. **效果执行**
   - 检查触发条件
   - 获取影响范围
   - 应用效果修改
   - 触发连锁效果

## 五、后续优化方向

1. **效果细分**
   - 将效果进一步模块化
   - 支持更细粒度的配置
   - 实现效果组合系统

2. **参数系统**
   - 添加参数验证
   - 支持表达式计算
   - 实现动态参数

3. **连锁系统**
   - 优化连锁效果触发机制
   - 添加优先级控制
   - 支持条件触发

4. **可视化工具**
   - 开发效果编辑器
   - 提供效果预览
   - 支持实时调试

## 六、注意事项

1. **配置管理**
   - 确保配置文件放在正确的Resources目录下
   - 注意配置的版本控制
   - 做好配置的容错处理

2. **性能优化**
   - 缓存反射创建的实例
   - 优化效果队列处理
   - 控制连锁效果的递归深度

3. **调试支持**
   - 添加详细的日志输出
   - 提供效果执行追踪
   - 支持运行时调试





# 效果系统重构设计文档 【2024-12-14】


## 一、核心思路
将复杂效果拆分为基础效果的组合，通过组合模式实现更灵活的效果系统。

## 二、基础效果类型
1. **ElementValueEffect（元素数值效果）**
   - 修改元素的数值
   - 支持不同元素类型的修正值配置
   - 可用于增强/削弱特定元素

2. **DamageEffect（伤害效果）**
   - 对目标造成基础伤害
   - 支持范围伤害
   - 可配置基础伤害值

3. **StatusEffect（状态效果）**
   - 添加/移除状态效果
   - 可配置状态类型、持续时间和强度
   - 例如：燃烧、冰冻、麻痹等

4. **ElementTransformEffect（元素转换效果）**
   - 将一种元素转换为另一种元素
   - 支持转换概率配置
   - 保留原始元素的数值

5. **PushPullEffect（推拉效果）**
   - 移动格子中的元素
   - 支持推力和拉力
   - 可配置作用力大小

6. **ChainEffect（连锁效果）**
   - 触发其他效果
   - 支持连锁概率和最大连锁次数
   - 可用于实现复杂的连锁反应

## 三、组合效果实现
### CompositeEffect（组合效果）
- 继承自基础Effect类
- 包含多个子效果
- 按序执行所有子效果
- 合并所有子效果的影响范围

### 示例效果配置
1. **火球术**

```yaml
yaml
ID: effect_fireball
Name: 火球术
Type: Composite
SubEffects:
Type: Damage
BaseDamage: 10
Range: 2
Type: ElementValue
ElementModifiers:
ElementType: Fire
ModifierValue: 1.5
ElementType: Water
ModifierValue: 0.5
Range: 2
```

2. **闪电链**
```yaml
ID: effect_lightning_chain
Name: 闪电链
Type: Composite
SubEffects:
Type: Damage
BaseDamage: 8
Range: 1
Type: Chain
CustomParameters:
ChainEffectId: effect_lightning_chain
ChainChance: 0.7
MaxChains: 3
Type: Status
CustomParameters:
StatusType: Paralyzed
Duration: 2
Intensity: 1.0
```


## 四、配置工具
### EffectConfigGenerator（效果配置生成器）
- 提供静态方法快速生成各类基础效果配置
- 支持所有基础效果类型的配置生成
- 提供默认值和参数检查

### EffectConfigGeneratorWindow（编辑器工具窗口）
- Unity编辑器扩展工具
- 可视化效果配置界面
- 支持实时预览和配置保存
- 提供常用参数的快速配置

## 五、优点
1. **模块化**
   - 基础效果独立，易于维护
   - 效果逻辑清晰，便于测试

2. **可扩展**
   - 易于添加新的基础效果
   - 支持复杂效果的组合

3. **配置驱动**
   - 效果参数可配置
   - 支持运行时修改

4. **工具支持**
   - 提供配置生成工具
   - 降低配置复杂度

## 六、后续优化方向
1. 添加效果优先级系统
2. 实现效果的条件触发
3. 添加效果动画配置
4. 优化配置工具的使用体验
5. 添加效果预览功能