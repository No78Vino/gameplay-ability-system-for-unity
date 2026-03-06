# EX-GAS Bean 映射规范

> 本文档是 EX-GAS 中 **C# 运行时类** 与 **Luban `__beans__.xlsx`** 之间映射关系的权威规范。
> 所有自动化工具（`BeanUpdater`、`CodeGeneratorLubanPart`）均以本文档为准。

## 一、设计意图

EX-GAS 采用 **代码驱动配置** 的设计模式：

- **自定义逻辑类** 定义在 C# 代码中（如 `CueLogging : GameplayCueBase<XParamLogging>`）
- **Luban Bean** 定义在 `__beans__.xlsx` 中，用于配置表数据序列化
- **两者必须保持同步**，否则配置表无法正确解析

### 1.1 核心原则

1. **`[BeanField]` 是唯一的字段识别标记** — 只有标注了 `[BeanField]` 的字段/属性才会被工具链识别
2. **Setter 强绑定** — 每个 `[BeanField]` 必须绑定一个用户自定义的 Setter 方法
3. **用户完全掌控赋值逻辑** — Setter 方法由用户编写，允许在赋值时执行额外操作

## 二、`[BeanField]` Attribute 规范

### 2.1 定义

```csharp
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class BeanFieldAttribute : Attribute
{
    /// <summary>
    /// 绑定的 Setter 方法名（必填，构造函数参数）
    /// </summary>
    public string Setter { get; }

    /// <summary>
    /// 覆盖 Bean 字段名（可选，默认取成员名）
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 覆盖 Luban 类型（可选，默认自动映射 C# 类型）
    /// </summary>
    public string LubanType { get; set; }

    /// <summary>
    /// Bean 字段注释（可选）
    /// </summary>
    public string Comment { get; set; }

    public BeanFieldAttribute(string setter)
    {
        Setter = setter;
    }
}
```

### 2.2 使用规则

| 规则 | 说明 |
|------|------|
| `Setter` 必填 | 构造函数的第一个参数，指定赋值方法名，推荐用 `nameof(SetXxx)` |
| `Name` 可选 | 覆盖写入 `__beans__.xlsx` 的字段名。不填则使用成员原始名 |
| `LubanType` 可选 | 覆盖 Luban 类型。不填则由 `MapCSharpTypeToLubanType` 自动映射 |
| `Comment` 可选 | Bean 字段注释 |
| 未标注的成员 | **完全忽略**，不会进入 `__beans__.xlsx`，不会生成赋值代码 |

### 2.3 使用示例

**简单字段：**

```csharp
public class XParamMMCScalable : XParam
{
    [BeanField(nameof(SetK))]
    public float K;

    [BeanField(nameof(SetB))]
    public float B;

    public void SetK(float value) { K = value; }
    public void SetB(float value) { B = value; }
}
```

**属性（private set）：**

```csharp
public class XParamFloat : XParam
{
    [BeanField(nameof(SetValue))]
    public float Value { get; private set; }

    public void SetValue(float v) { Value = v; }
}
```

**私有 backing field + get-only 属性：**

```csharp
public class XParamInt : XParam
{
    private int _value;

    [BeanField(nameof(SetValue))]
    public int Value => _value;

    public void SetValue(int value) { _value = value; }
}
```

> 注意：`[BeanField]` 可以标注在 get-only 属性上，因为 BeanUpdater 通过属性名和 `LubanType`/自动映射来确定 Bean 字段，而赋值走的是绑定的 Setter 方法。

**覆盖 Bean 字段名和类型（用于复杂/多态场景）：**

```csharp
public class XParamCatchAreaBox3D : XParam
{
    [BeanField(nameof(SetLayer), LubanType = "int", Comment = "检测层级")]
    public LayerMask layer;

    public void SetLayer(int layer) { this.layer.value = layer; }
}
```

**多态 Bean 引用（运行时结构和 Bean 结构不一致时）：**

```csharp
public class XParamCue : XParam
{
    [BeanField(nameof(SetRequiredTags))]
    public List<int> RequiredTags;

    [BeanField(nameof(SetImmunityTags))]
    public List<int> ImmunityTags;

    // CueType 和 Param 不加 [BeanField]，它们是运行时字段
    public string CueType { get; private set; }
    public XParam Param { get; set; }

    // Bean 中的 CueLogic 是一个多态字段，用 [BeanField] 标注在一个虚拟属性或由 BeanUpdater 逻辑 Bean 收集自动处理
    // 具体的 CueLogic 多态引用由逻辑 Bean 的 Param 字段完成

    public void SetRequiredTags(int[] tags) { RequiredTags = TagHelper.FilterInvalidTags(tags.ToList()); }
    public void SetImmunityTags(int[] tags) { ImmunityTags = TagHelper.FilterInvalidTags(tags.ToList()); }
    public void SetCueLogic(GameplayCueUnit cueLogic)
    {
        CueType = cueLogic.CueType.Name;
        Param = cueLogic.Param;
    }
}
```

## 三、Bean 类型分类

### 3.1 官方已经实现的参数 Bean（XParam 系列）

所有实现 `XParam` 接口的具体类，自动成为 `XParam` 的子 Bean。

| 运行时类 | Luban Bean | Bean 字段（由 `[BeanField]` 确定） |
|---------|------------|------|
| `XParamNone` | `XParamNone : XParam` | （无） |
| `XParamInt` | `XParamInt : XParam` | `Value: int` |
| `XParamFloat` | `XParamFloat : XParam` | `Value: float` |
| `XParamString` | `XParamString : XParam` | `Value: string` |
| `XParamBool` | `XParamBool : XParam` | `Value: bool` |
| `XParamVector2` | `XParamVector2 : XParam` | `Value: vector2` |
| `XParamVector3` | `XParamVector3 : XParam` | `Value: vector3` |
| `XParamArrayInt` | `XParamArrayInt : XParam` | `Value: (array#sep=,),int` |
| `XParamArrayFloat` | `XParamArrayFloat : XParam` | `Value: (array#sep=,),float` |
| `XParamArrayString` | `XParamArrayString : XParam` | `Value: (array#sep=,),string` |
| `XParamLogging` | `XParamLogging : XParam` | `Value: string`, `Duration: float` |
| `XParamMMCScalable` | `XParamMMCScalable : XParam` | `K: float`, `B: float` |
| `XParamAnimator` | `XParamAnimator : XParam` | `AnimatorNodePath: string`, `AnimationName: string` |
| `XParamPlaySound` | `XParamPlaySound : XParam` | `AudioClipPath: string`, `Volume: float`, `Speed: float`, `Loop: bool`, `AudioSourceNodePath: string` |
| `XParamCueIDs` | `XParamCueIDs : XParam` | `IDs: (array#sep=,),int` |
| `XParamEffectIDs` | `XParamEffectIDs : XParam` | `IDs: (array#sep=,),int` |
| `XParamALTimelineID` | `XParamALTimelineID : XParam` | `ID: int` |
| `XParamCue` | `XParamCue : XParam` | `RequiredTags: (array#sep=,),int`, `ImmunityTags: (array#sep=,),int`, `CueLogic: CueLogic` |
| `XParamApplyEffects` | `XParamApplyEffects : XParam` | `IDs: (array#sep=,),int`, `TargetCatcher: TargetCatcherBase` |
| `XParamCatchAreaBox3D` | `XParamCatchAreaBox3D : XParam` | `isWorldSpace: bool`, `offset: vector3`, `size: vector3`, `rotation: vector3`, `layer: int` |
| `AttributeBasedMmcParam` | `AttributeBasedMmcParam : XParam` | `AttrSetCode: int`, `AttrCode: int`, `FromType: int`, `CaptureType: int`, `K: float`, `B: float` |
| `XParamTimeline` | `XParamTimeline : XParam` | `ID: int`, `Name: string`, `LifeTime: int`, `ManualEndAbility: bool`, `Tracks: Track` |


### 3.2 逻辑 Bean

逻辑 Bean 是多态容器，每个逻辑类对应一个子 Bean，包含一个 `Param` 字段指向其参数类型。

| C# 基类 | Bean 抽象父类 | 用途 |
|---------|--------------|------|
| `GameplayCueBase<T>` | `GameplayCueBase` | Cue 逻辑 |
| `ModMagnitudeCalculationBase<T>` | `ModMagnitudeCalculationBase` | MMC 逻辑 |
| `AbilityLogicBase<T>` | `AbilityLogicBase` | Ability 逻辑 |
| `AbilityTaskBase<T>` | `AbilityTaskBase` | Ability 任务 |
| `TargetCatcherBase<T>` | `TargetCatcherBase` | 目标捕获器 |

**逻辑 Bean 的字段固定为 `Param`**，类型为泛型参数 `T` 对应的 XParam 子类：

```yaml
# 示例：CueLogging : GameplayCueBase<XParamLogging>
CueLogging:
  parent: GameplayCueBase
  fields:
    - name: Param
      type: XParamLogging

# 示例：MMCScalableFloat : ModMagnitudeCalculationBase<XParamMMCScalable>
MMCScalableFloat:
  parent: ModMagnitudeCalculationBase
  fields:
    - name: Param
      type: XParamMMCScalable
```

## 四、命名规范

### 4.1 类型命名

| 类别 | 命名规范 | 示例 |
|------|---------|------|
| Cue 逻辑类 | 无强制前缀 | `CueLogging`, `CLCameraFovShake` |
| MMC 逻辑类 | 建议以 `MMC` 开头 | `MMCScalableFloat`, `MMCAttributeBased` |
| Ability 逻辑类 | 建议以 `AL` 开头 | `ALMove`, `ALDeath`, `ALTimeline` |
| AbilityTask 类 | 建议以 `Task` 开头 | `TaskPlayCue`, `TaskApplyEffects` |
| TargetCatcher 类 | 无强制前缀 | `CatchAreaBox3D` |
| 参数类 | 建议以 `XParam` 开头 | `XParamLogging`, `XParamMove` |

### 4.2 Bean 命名

Bean 名称与运行时 C# 类名 **完全一致**：

```
C# 类名                →  Luban Bean 名称
CueLogging             →  CueLogging
MMCScalableFloat       →  MMCScalableFloat
XParamLogging          →  XParamLogging
CatchAreaBox3D         →  CatchAreaBox3D
```

## 五、字段映射规则

### 5.1 `[BeanField]` 字段收集规则

`BeanUpdater.CollectFieldsFromType()` 的扫描逻辑：

```
扫描范围：BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
收集条件：成员上存在 [BeanField] Attribute
字段名：  attr.Name ?? 成员原始名
类型：    attr.LubanType ?? MapCSharpTypeToLubanType(成员类型)
注释：    attr.Comment ?? 成员名
Setter：  attr.Setter（必填）
```

**没有 `[BeanField]` 的成员一律忽略**，无论其可见性、是否 readonly。

### 5.2 逻辑 Bean 的 `Param` 字段

逻辑 Bean 的 `Param` 字段 **不依赖 `[BeanField]`**，而是由 `BeanUpdater` 的 `Collect*Beans` 方法通过反射泛型参数自动添加：

```csharp
// BeanUpdater 自动处理
var paramType = GetGenericParamType(type, typeof(GameplayCueBase<>));
bean.Fields.Add(new BeanField { Name = "Param", Type = paramType.Name });
```

### 5.3 类型映射表

| C# 类型 | Luban 类型 | 说明 |
|--------|-----------|------|
| `int` | `int` | |
| `long` | `long` | |
| `float` | `float` | |
| `double` | `double` | |
| `bool` | `bool` | |
| `string` | `string` | |
| `Vector2` | `vector2` | |
| `Vector3` | `vector3` | |
| `Vector4` | `vector4` | |
| `LayerMask` | `int` | 需要 `[BeanField(LubanType = "int")]` 显式覆盖 |
| `T[]` | `(array#sep=,),T` | `__beans__.xlsx` 的数组格式 |
| `List<T>` | `(array#sep=,),T` | 同上 |
| 自定义类型 | 类型名 | 如 `CueLogic`, `TargetCatcherBase` |

## 六、赋值代码生成规则

### 6.1 Setter 绑定机制

`CodeGeneratorLubanPart` 生成 XLuban 赋值代码时，从运行时 XParam 类的 `[BeanField]` 中读取 `Setter` 属性，直接调用对应方法：

```csharp
// 生成的赋值代码示例（XParamMMCScalable）
var mp = mmcParam as GAS.Runtime.XParamMMCScalable;
mp?.SetK(mmcData.Param.K);
mp?.SetB(mmcData.Param.B);
```

### 6.2 类型转换

对于特殊 Luban 类型（`vector2/3/4`），`WriteFieldAssignment` 会通过 `LubanTypeConversionMap` 自动转换：

```csharp
// LubanTypeConversionMap
"cfg.vector3" → "new UnityEngine.Vector3({0}.X, {0}.Y, {0}.Z)"
"cfg.vector2" → "new UnityEngine.Vector2({0}.X, {0}.Y)"
"cfg.vector4" → "new UnityEngine.Vector4({0}.X, {0}.Y, {0}.Z, {0}.W)"
```

生成结果：

```csharp
cp?.SetOffset(new UnityEngine.Vector3(cData.Param.offset.X, cData.Param.offset.Y, cData.Param.offset.Z));
```

### 6.3 Setter 方法编写指南

Setter 方法由用户完全自定义，只需保证方法名与 `[BeanField]` 中的 `Setter` 一致：

**简单赋值：**
```csharp
public void SetK(float value) { K = value; }
```

**带类型转换的赋值：**
```csharp
public void SetFromType(int v) { FromType = (AttributeFromType)v; }
public void SetLayer(int layer) { this.layer.value = layer; }
```

**带额外逻辑的赋值：**
```csharp
public void SetRequiredTags(int[] tags)
{
    RequiredTags = TagHelper.FilterInvalidTags(tags.ToList());
}
```

**复合操作（一个 Setter 赋值多个成员）：**
```csharp
public void SetCueLogic(GameplayCueUnit cueLogic)
{
    CueType = cueLogic.CueType.Name;
    Param = cueLogic.Param;
}
```

## 七、映射关系总览

```
┌────────────────────────────────────────────────────────────────────┐
│  C# 运行时                          Luban __beans__.xlsx          │
├────────────────────────────────────────────────────────────────────┤
│  XParam (interface)                 XParam (abstract bean)        │
│  ├─ XParamNone                      ├─ XParamNone                │
│  ├─ XParamInt                       ├─ XParamInt                 │
│  │   [BeanField] Value → SetValue   │   Value: int               │
│  ├─ XParamFloat                     ├─ XParamFloat               │
│  │   [BeanField] Value → SetValue   │   Value: float             │
│  ├─ XParamCue                       ├─ XParamCue                 │
│  │   [BeanField] RequiredTags       │   RequiredTags: array,int  │
│  │   [BeanField] ImmunityTags       │   ImmunityTags: array,int  │
│  │   (无标注) CueType               │   CueLogic: CueLogic      │
│  │   (无标注) Param                  │                            │
│  └─ ...                             └─ ...                       │
├────────────────────────────────────────────────────────────────────┤
│  GameplayCueBase<T>                 CueLogic (abstract bean)     │
│  ├─ CueLogging<XParamLogging>       ├─ CueLogging               │
│  │   (Param 自动添加)                │   Param: XParamLogging    │
│  └─ ...                             └─ ...                       │
├────────────────────────────────────────────────────────────────────┤
│  ModMagnitudeCalculationBase<T>     MmcLogic (abstract bean)     │
│  AbilityLogicBase<T>                AbilityLogic (abstract bean) │
│  AbilityTaskBase<T>                 AbilityTask (abstract bean)  │
│  TargetCatcherBase<T>               TargetCatcherBase (abstract) │
└────────────────────────────────────────────────────────────────────┘
```

## 八、`__beans__.xlsx` 表结构

### 8.1 列定义

| 列 | EPPlus 索引 | 表头 (Row 1) | 用途 |
|----|------------|-------------|------|
| B | 2 | `full_name` | Bean 全名 |
| C | 3 | `parent` | 父 Bean 名 |
| G | 7 | `comment` | 注释 |
| J | 10 | `+fields` → `name` | 字段名 |
| L | 12 | `type` | 字段类型 |
| N | 14 | `comment` | 字段注释 |

### 8.2 数据布局（Row 4 起）

- 每个 Bean 的首行填写 `full_name`、`parent`、`comment` 和第一个字段
- 后续字段每个占一行，只填 `name`/`type`/`comment` 列
- 多字段 Bean 后有空行分隔
- 各大类（XParam / CueLogic / MmcLogic / ...）之间有蓝色（`#DAEEF3`）分隔行

### 8.3 示例

```
Row 4:  | XParam         |        | 通用参数基类 |            |          |         |  ← 抽象基类
Row 5:  | XParamNone     | XParam | 无参数       |            |          |         |
Row 6:  | XParamInt      | XParam | 整数参数     | Value      | int      | 值      |
Row 7:  | XParamLogging  | XParam | 日志参数     | Value      | string   | 日志内容 |
Row 8:  |                |        |              | Duration   | float    | 持续时间 |
Row 9:  (空行 — 多字段 Bean 后分隔)
Row 10: | XParamCue      | XParam | Cue参数      | RequiredTags | (array#sep=,),int | 需求标签 |
Row 11: |                |        |              | ImmunityTags | (array#sep=,),int | 免疫标签 |
Row 12: |                |        |              | CueLogic     | CueLogic          | Cue逻辑  |
Row 13: (空行)
...
Row XX: ████████████████████████████████████████████  ← 蓝色分隔行 (#DAEEF3)
Row XX: | CueLogic       |        | Cue逻辑基类  |            |          |         |  ← 新类别开始
Row XX: | CueLogging     | CueLogic | Cue:日志   | Param      | XParamLogging | 参数 |
...
```

## 九、开发流程

### 9.1 创建新的参数类

```csharp
// 1. 创建 XParam 子类
public class XParamMyData : XParam
{
    [BeanField(nameof(SetScore), Comment = "分数")]
    public float Score;

    [BeanField(nameof(SetName), Comment = "名称")]
    public string Name { get; private set; }

    public void SetScore(float v) { Score = v; }
    public void SetName(string v) { Name = v; }

#if UNITY_EDITOR
    public void DecodeExcelData(List<object> paramData) { /* ... */ }
    public List<object> EncodeExcelData() { /* ... */ }
#endif
}
```

### 9.2 创建新的逻辑类

```csharp
// 2. 创建逻辑类，泛型参数指定参数类
public class CueMyEffect : GameplayCueBase<XParamMyData>
{
    public override void OnActivate(float time)
    {
        Debug.Log($"Score: {Parameter.Score}, Name: {Parameter.Name}");
    }
    public override void Reset() { }
}
```

### 9.3 自动化工作流

```
编码(创建自定义类) → 更新Bean定义 → Luban导表 → 生成运行时代码
      ↓                ↓              ↓              ↓
  [开发者]        [BeanUpdater]     [Luban]    [CodeGenerator]
```

具体操作：

1. **更新 Bean 定义**：菜单 `EXTool/EX-GAS/生成脚本/更新Bean定义`
2. **运行 Luban 导表**：生成新的 `cfg.*` Bean C# 类
3. **生成运行时代码**：菜单 `EXTool/EX-GAS/生成脚本/生成所有`
4. **重新导出配置表**：菜单 `EXTool/EX-GAS/生成脚本/GAS表配置`

## 十、JSON 数据格式

配置表 JSON 使用 `$type` 字段进行多态识别：

```json
{
  "ID": 1001,
  "Name": "测试Cue",
  "CueLogic": {
    "$type": "CueLogging",
    "Param": {
      "$type": "XParamLogging",
      "Value": "Hello World",
      "Duration": 2.5
    }
  }
}
```

## 十一、常见问题

### Q1: 为什么配置表解析失败？

检查：
1. XParam 类中是否正确标注了 `[BeanField]`
2. `[BeanField]` 的 `Setter` 是否指向一个存在的方法
3. 是否运行了 `更新Bean定义` + Luban 导表 + `生成所有`
4. Bean 字段名是否与 JSON 中的 key 一致

### Q2: 如何添加新的参数类型？

1. 创建继承自 `XParam` 的类
2. 用 `[BeanField(nameof(SetXxx))]` 标注需要配置的字段
3. 编写对应的 `SetXxx` 方法
4. 实现 `DecodeExcelData` 和 `EncodeExcelData`（`#if UNITY_EDITOR`）
5. 运行 `更新Bean定义`

### Q3: `[BeanField]` 可以标注在 private 成员上吗？

可以。`BeanUpdater` 使用 `BindingFlags.NonPublic` 扫描，所以 `private` 字段/属性只要有 `[BeanField]` 就会被收录。

### Q4: Setter 方法的参数类型可以和字段类型不一致吗？

可以。例如 `AttributeBasedMmcParam` 中：

```csharp
[BeanField(nameof(SetFromType),LubanType = "int")]
public AttributeFromType FromType { get; private set; }

// Setter 接受 int，内部做枚举转换
public void SetFromType(int v) { FromType = (AttributeFromType)v; }
```

此时需要确保 `[BeanField(LubanType = "int")]` 显式指定 Luban 类型为 `int`，否则自动映射可能不正确。

### Q5: Bean定义更新后还需要做什么？

1. 运行 Luban 导表工具生成新的 Bean C# 类（`cfg.*`）
2. 重新生成运行时映射代码（菜单 `生成所有`）
3. 重新导出配置表 JSON

## 十二、工具链相关文件

| 文件 | 用途 |
|------|------|
| `Assets/GAS/Runtime/General/XParam/BeanFieldAttribute.cs` | `[BeanField]` Attribute 定义 |
| `Assets/GAS/Editor/CodeGen/BeanUpdater.cs` | 扫描 `[BeanField]`，自动更新 `__beans__.xlsx` |
| `Assets/GAS/Editor/CodeGen/CodeGeneratorLubanPart.cs` | 读取 `[BeanField]` 的 Setter，生成 XLuban 赋值代码 |
| `Assets/GAS/Editor/Helper/EXEditorHelper.cs` | `GetBeanFields()` 辅助方法 |
| `Assets/GAS/Editor/CodeGen/CodeGenerator.cs` | 代码生成主入口 |
| `__beans__.xlsx` | Luban Bean 定义表 |
| `XLuban.gen.cs` | 自动生成的运行时 Luban 数据加载代码 |