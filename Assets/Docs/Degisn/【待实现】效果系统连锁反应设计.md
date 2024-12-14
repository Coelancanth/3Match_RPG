# 效果系统连锁反应改进设计

## 核心思路
将效果的连锁反应从预设模式改为动态检测模式,实现真正的涌现式玩法。效果的连锁不再是预先定义的,而是根据当前游戏状态动态产生。

## 主要改动

### 1. 新增连锁效果信息类

```csharp
public class ChainEffectInfo
{
public string EffectID { get; set; } // 要触发的效果ID
public GridCell SourceCell { get; set; } // 效果的源格子
public GridCell TargetCell { get; set; } // 效果的目标格子
public string Reason { get; set; } // 触发原因
public Dictionary<string, object> CustomData { get; set; } // 额外数据
}
```
### 2. 新增元素反应规则配置
```csharp
public static class ElementReactionRules
{
private static readonly Dictionary<(string, string), string> ReactionEffects =
new Dictionary<(string, string), string>
{
{ ("Fire", "Water"), "effect_steam" },
{ ("Fire", "Ice"), "effect_melt" },
{ ("Lightning", "Water"), "effect_electrified" }
};
}
```


### 3. Effect基类改动
- 移除预设的ChainEffectIDs列表
- 新增动态检测连锁效果的方法

```csharp
public virtual IEnumerable<ChainEffectInfo> DetectChainEffects(EffectContext context)
```


### 4. EffectManager改动
- 新增处理连锁效果的方法
- 修改效果队列处理逻辑,支持动态连锁
- 新增连锁效果上下文创建方法

## 具体效果实现示例

以FireballEffect为例:
```csharp

public override IEnumerable<ChainEffectInfo> DetectChainEffects(EffectContext context)
{
var chainEffects = new List<ChainEffectInfo>();
foreach (var cell in GetAffectedCells(context))
{
// 1. 检查元素反应
var reactionEffect = ElementReactionRules.GetReactionEffect("Fire", cell.Element?.Type);
if (reactionEffect != null) {
chainEffects.Add(new ChainEffectInfo {
EffectID = reactionEffect,
SourceCell = context.SourceCell,
TargetCell = cell,
Reason = $"火元素与{cell.Element.Type}发生反应"
});
}
// 2. 检查地形效果
if (cell.IsFlammable()) {
chainEffects.Add(new ChainEffectInfo {
EffectID = "effect_burn",
SourceCell = cell,
TargetCell = cell,
Reason = "地形可燃"
});
}
// 3. 检查特殊状态
if (cell.HasStatus("Oiled")) {
chainEffects.Add(new ChainEffectInfo {
EffectID = "effect_explosion",
SourceCell = cell,
TargetCell = cell,
Reason = "油状态被点燃"
});
}
}
return chainEffects;
}
```


## 设计优点

1. **真正的涌现式玩法**
   - 效果连锁基于实时状态动态产生
   - 玩家可以发现意想不到的组合效果

2. **更灵活的反应系统**
   - 支持多种触发条件(元素类型、地形、状态等)
   - 可以产生复杂的多重连锁反应

3. **良好的扩展性**
   - 易于添加新的元素反应规则
   - 支持自定义连锁效果触发条件

4. **便于调试**
   - 每个连锁效果都有明确的触发原因
   - 可以追踪完整的连锁反应链

## 后续扩展方向

1. 添加更多元素反应规则
2. 实现地形系统,支持更多地形效果
3. 添加状态系统,实现更丰富的状态效果
4. 开发可视化工具,方便策划配置和调试
5. 添加效果优先级系统,控制连锁效果的触发顺序