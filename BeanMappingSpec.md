# EX-GAS Bean 映射规范

> 本文档是 EX-GAS 中 **C# 运行时类** 与 **Luban `__beans__.xlsx`** 之间映射关系的权威规范。
> 所有自动化工具（`BeanUpdater`、`CodeGeneratorLubanPart`）均以本文档为准。

## 一、设计意图

EX-GAS 采用 **代码驱动配置** 的设计模式：

- **自定义逻辑类** 定义在 C# 代码中（如 `CueLogging : GameplayCueBase<XParamLogging>`）
- **Luban Bean** 定义在 `__beans__.xlsx` 中，用于配置表数据序列化
- **两者必须保持同步**，否则配置表无法正确解析

### 1.1 核心原则

1. **`[BeanField]` 和 `[BeanPolymorphicField]` 是字段识别标记** — 只有标注了这两个 Attribute 的字段/属性才会被工具链识别
2. **Setter 强绑定** — 每个 `[BeanField]` 必须绑定一个用户自定义的 Setter 方法；`[BeanPolymorphicField]` 则同时绑定 `TypeSetter` 和 `ParamSetter`
3. **用户完全掌控赋值逻辑** — Setter 方法由用户编写，允许在赋值时执行额外操作
4. **Order 排序** — 字段在 Bean 中的排列顺序由 `Order` 属性决定，默认通过 `[CallerLineNumber]` 自动获取源码行号

## 二、Attribute 规范

### 2.1 `[BeanField]` 定义

```csharp
using System;
using System.Runtime.CompilerServices;

namespace GAS.Runtime
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class BeanFieldAttribute : Attribute
    {
        /// <summary>
        /// 绑定的 Set 方法名（必填，推荐用 nameof(SetXxx) 传入）
        /// </summary>
        public string Setter { get; }

        /// <summary>
        /// 覆盖 Bean 字段名（默认取成员名）
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 覆盖 Luban 类型（默认自动映射 C# 类型）
        /// </summary>
        public string LubanType { get; set; }

        /// <summary>
        /// Bean 字段注释
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// 字段在 Bean 中的排序权重。
        /// 默认值由 [CallerLineNumber] 自动填入源码行号，保证声明顺序。
        /// 也可显式指定: [BeanField("SetXxx", Order = 100)]
        /// </summary>
        public int Order { get; set; }

        public BeanFieldAttribute(string setter, [CallerLineNumber] int order = 0)
        {
            Setter = setter;
            Order = order;
        }
    }
}
```

### 2.2 `[BeanPolymorphicField]` 定义

当 Luban 中用单一多态 Bean 字段（如 `CueLogic: GameplayCueBase`、`TargetCatcher: TargetCatcherBase`）表示的数据，在运行时需要拆解为 `(TypeName + Param)` 两个字段时使用此 Attribute。代码生成器会自动处理多态 Bean → 运行时字段的拆解。

```csharp
using System;
using System.Runtime.CompilerServices;

namespace GAS.Runtime
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class BeanPolymorphicFieldAttribute : Attribute
    {
        /// <summary>
        /// 写入 __beans__.xlsx 的字段名（如 "CueLogic"、"TargetCatcher"）
        /// </summary>
        public string BeanFieldName { get; }

        /// <summary>
        /// Luban 多态抽象 Bean 类型名（如 "GameplayCueBase"、"TargetCatcherBase"）
        /// </summary>
        public string LubanPolymorphicType { get; }

        /// <summary>
        /// 类型判别符的 Setter 方法名（如 "SetCueType"、"SetCatcherType"）
        /// </summary>
        public string TypeSetter { get; }

        /// <summary>
        /// 关联的 Param 字段的 Setter 方法名（如 "SetParam"）
        /// </summary>
        public string ParamSetter { get; }

        /// <summary>
        /// 运行时获取 Param 类型的静态方法全路径
        /// 如 "CueHelper.GetCueLogicParamType"
        /// 或 "TargetCatcherHelper.GetCatcherParamType"
        /// </summary>
        public string ParamTypeResolver { get; set; }

        /// <summary>
        /// Editor 侧获取所有子类的 Helper 类别标识
        /// 可选值: "Cue", "TargetCatcher" 等
        /// </summary>
        public string HelperCategory { get; set; }

        /// <summary>
        /// 字段在 Bean 中的排序权重（同 BeanFieldAttribute.Order）
        /// </summary>
        public int Order { get; set; }

        public BeanPolymorphicFieldAttribute(
            string beanFieldName,
            string lubanPolymorphicType,
            string typeSetter,
            string paramSetter,
            [CallerLineNumber] int order = 0)
        {
            BeanFieldName = beanFieldName;
            LubanPolymorphicType = lubanPolymorphicType;
            TypeSetter = typeSetter;
            ParamSetter = paramSetter;
            Order = order;
        }
    }
}
```

### 2.3 使用规则

| 规则 | 说明 |
|------|------|
| `Setter` 必填 | `[BeanField]` 构造函数的第一个参数，指定赋值方法名，推荐用 `nameof(SetXxx)` |
| `Name` 可选 | 覆盖写入 `__beans__.xlsx` 的字段名。不填则使用成员原始名 |
| `LubanType` 可选 | 覆盖 Luban 类型。不填则由 `MapCSharpTypeToLubanType` 自动映射 |
| `Comment` 可选 | Bean 字段注释 |
| `Order` 可选 | 字段排序权重。默认由 `[CallerLineNumber]` 自动填入源码行号。也可显式指定 `[BeanField("SetXxx", Order = 100)]` |
| 未标注的成员 | **完全忽略**，不会进入 `__beans__.xlsx`，不会生成赋值代码（`[BeanField]` 和 `[BeanPolymorphicField]` 均未标注时） |

### 2.4 使用示例

**简单字段（属性 + private set）：**

```csharp
public class XParamMMCScalable : XParam
{
    [ShowInInspector]
    [BeanField(nameof(SetK))]
    public float K { get; private set; }

    [ShowInInspector]
    [BeanField(nameof(SetB))]
    public float B { get; private set; }

    public void SetK(float value) { K = value; }
    public void SetB(float value) { B = value; }
}
```

**带 Comment 的属性：**

```csharp
public class XParamInt : XParam
{
    [BeanField(nameof(SetValue), Comment = "值")]
    public int Value { get; private set; }

    public void SetValue(int value) { Value = value; }
}
```

**覆盖 Bean 字段名和类型（用于特殊类型映射）：**

```csharp
public class XParamCatchAreaBox3D : XParam
{
    [BeanField(nameof(SetLayer), LubanType = "int", Comment = "检测层级")]
    public LayerMask layer;

    public void SetLayer(int layer) { this.layer.value = layer; }
}
```

**多态 Bean 引用 — `[BeanPolymorphicField]`：**

当 Luban 中的一个多态字段（如 `CueLogic: GameplayCueBase`）在运行时被拆解为 `CueType (string)` + `Param (XParam)` 两个字段时，用 `[BeanPolymorphicField]` 标注在类型判别符字段上。关联的 `Param` 字段 **不标注任何 Attribute**，由 `[BeanPolymorphicField]` 的 `ParamSetter` 自动关联。

```csharp
public class XParamCue : XParam
{
    [BeanField(nameof(SetRequiredTags), Order = 1)]
    public List<int> RequiredTags;

    [BeanField(nameof(SetImmunityTags), Order = 2)]
    public List<int> ImmunityTags;

    [BeanPolymorphicField(
        beanFieldName: "CueLogic",
        lubanPolymorphicType: nameof(GameplayCueBase),
        typeSetter: nameof(SetCueType),
        paramSetter: nameof(SetParam),
        ParamTypeResolver = "CueHelper.GetCueLogicParamType",
        HelperCategory = "Cue",
        Order = 3)]
    public string CueType { get; private set; }

    // Param 不标注任何 Attribute — 由 [BeanPolymorphicField] 的 ParamSetter 自动关联
    public XParam Param { get; set; }

    public void SetRequiredTags(int[] tags) { RequiredTags = TagHelper.FilterInvalidTags(tags.ToList()); }
    public void SetImmunityTags(int[] tags) { ImmunityTags = TagHelper.FilterInvalidTags(tags.ToList()); }
    public void SetCueType(string cueType) { CueType = cueType; }
    public void SetParam(XParam param) { Param = param; }
}
```

**另一个多态 Bean 引用示例（XParamApplyEffects）：**

```csharp
public class XParamApplyEffects : XParam
{
    [BeanField(nameof(SetIDs), Comment = "buff效果ID")]
    public int[] IDs;

    [BeanPolymorphicField(
        beanFieldName: "TargetCatcher",
        lubanPolymorphicType: nameof(TargetCatcherBase),
        typeSetter: nameof(SetCatcherType),
        paramSetter: nameof(SetParam),
        ParamTypeResolver = "TargetCatcherHelper.GetCatcherParamType",
        HelperCategory = "TargetCatcher")]
    public string CatcherType { get; private set; }

    // Param 不标注任何 Attribute
    public XParam Param { get; set; }

    public void SetIDs(int[] value) { IDs = value; }
    public void SetCatcherType(string catcherType) { CatcherType = catcherType; }
    public void SetParam(XParam param) { Param = param; }
}
```

> **注意**：`[BeanPolymorphicField]` 的 `beanFieldName` 和 `lubanPolymorphicType` 决定了写入 `__beans__.xlsx` 的字段名和类型。运行时的 `CueType`/`CatcherType` 字段本身不出现在 Bean 定义中，它们是纯运行时字段。

## 三、Bean 类型分类

### 3.1 官方已经实现的参数 Bean（XParam 系列）

所有实现 `XParam` 接口的具体类，自动成为 `XParam` 的子 Bean。

| 运行时类 | Luban Bean | Bean 字段（由 `[BeanField]` / `[BeanPolymorphicField]` 确定） |
|---------|------------|------|
| `XParamNone` | `XParamNone : XParam` | （无） |
| `XParamInt` | `XParamInt : XParam` | `Value: int` |
| `XParamFloat` | `XParamFloat : XParam` | `Value: float` |
| `XParamString` | `XParamString : XParam` | `Value: string` |
| `XParamBool` | `XParamBool : XParam` | `Value: bool` |
| `XParamVector2` | `XParamVector2 : XParam` | `Value: vector2` |
| `XParamVector3` | `XParamVector3 : XParam` | `Value: vector3` |
| `XParamArrayInt` | `XParamArrayInt : XParam` | （无 `[BeanField]`，不被 BeanUpdater 收集）\* |
| `XParamArrayFloat` | `XParamArrayFloat : XParam` | `Value: (array#sep=;),float` |
| `XParamLogging` | `XParamLogging : XParam` | `Value: string`, `Duration: float` |
| `XParamMMCScalable` | `XParamMMCScalable : XParam` | `K: float`, `B: float` |
| `XParamAnimator` | `XParamAnimator : XParam` | `AnimatorNodePath: string`, `AnimationName: string` |
| `XParamPlaySound` | `XParamPlaySound : XParam` | `AudioClipPath: string`, `Volume: float`, `Speed: float`, `Loop: bool`, `AudioSourceNodePath: string` |
| `XParamCueIDs` | `XParamCueIDs : XParam` | `IDs: (array#sep=;),int`（Setter 为 `SetValue`，非 `SetIDs`） |
| `XParamEffectIDs` | `XParamEffectIDs : XParam` | `IDs: (array#sep=;),int` |
| `XParamALTimelineID` | `XParamALTimelineID : XParam` | `ID: int` |
| `XParamCue` | `XParamCue : XParam` | `RequiredTags: (array#sep=;),int`, `ImmunityTags: (array#sep=;),int`, `CueLogic: GameplayCueBase`（via `[BeanPolymorphicField]`） |
| `XParamApplyEffects` | `XParamApplyEffects : XParam` | `IDs: (array#sep=;),int`, `TargetCatcher: TargetCatcherBase`（via `[BeanPolymorphicField]`） |
| `XParamCatchAreaBox3D` | `XParamCatchAreaBox3D : XParam` | `isWorldSpace: bool`, `offset: vector3`, `size: vector3`, `rotation: vector3`, `layer: int` |
| `AttributeBasedMmcParam` | `AttributeBasedMmcParam : XParam` | `AttrSetCode: int`, `AttrCode: int`, `FromType: int`, `CaptureType: int`, `K: float`, `B: float` |
| `XParamTimeline` | `XParamTimeline : XParam` | `ID: int`, `Name: string`, `LifeTime: int`, `ManualEndAbility: bool`, `Tracks: Track` |

> \* `XParamArrayInt` 的 `Value` 属性只有 `[ShowInInspector]`，没有 `[BeanField]`，因此不会被 `BeanUpdater.CollectFieldsFromType()` 收集。它仍然有 `SetValue` 方法，可手动使用。

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

### 5.1 字段收集规则

`BeanUpdater.CollectFieldsFromType()` 同时扫描 `[BeanField]` 和 `[BeanPolymorphicField]`，按 `Order` 排序后写入 Bean 定义：

```
扫描范围：BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance

1. 扫描 [BeanField]：
   收集条件：成员上存在 [BeanField] Attribute
   字段名：  attr.Name ?? 成员原始名
   类型：    attr.LubanType ?? MapCSharpTypeToLubanType(成员类型)
   注释：    attr.Comment ?? 成员名
   排序：    attr.Order

2. 扫描 [BeanPolymorphicField]：
   收集条件：成员上存在 [BeanPolymorphicField] Attribute
   字段名：  polyAttr.BeanFieldName
   类型：    polyAttr.LubanPolymorphicType
   注释：    polyAttr.BeanFieldName
   排序：    polyAttr.Order

3. 合并后按 Order 升序排列（LINQ OrderBy 稳定排序，Order 相同时保持收集顺序）
```

**没有 `[BeanField]` 也没有 `[BeanPolymorphicField]` 的成员一律忽略**，无论其可见性、是否 readonly。

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
| `T[]` | `(array#sep=;),T` | `__beans__.xlsx` 的数组格式（分号分隔） |
| `List<T>` | `(array#sep=;),T` | 同上 |
| 自定义类型 | 类型名 | 如 `GameplayCueBase`, `TargetCatcherBase` |

## 六、赋值代码生成规则

### 6.1 普通字段 Setter 绑定

`CodeGeneratorLubanPart` 生成 XLuban 赋值代码时，从运行时 XParam 类的 `[BeanField]` 中读取 `Setter` 属性，直接调用对应方法。

**字段名 PascalCase 转换**：代码生成器使用 `ToPascalCase()` 将 Bean 字段名首字母大写，匹配 Luban 的 `format_property_name` PascalCase 规则。例如 `requiredTags` → `RequiredTags`。

```csharp
// 生成的赋值代码示例（XParamMMCScalable）
var mp = mmcParam as GAS.Runtime.XParamMMCScalable;
mp?.SetK(mmcData.Param.K);
mp?.SetB(mmcData.Param.B);
```

### 6.2 多态字段拆解代码生成（`WritePolymorphicFieldAssignment`）

当遇到 `[BeanPolymorphicField]` 标注的成员时，`CodeGeneratorLubanPart.WritePolymorphicFieldAssignment()` 生成以下通用多态拆解逻辑：

1. 从 Luban 数据获取多态 Bean 实例 `polyBean`
2. 调用 `TypeSetter(polyBean.GetType().Name)` 设置类型判别符
3. 通过 `ParamTypeResolver` 解析运行时 XParam 类型
4. 生成 `switch-case` 逐子类赋值 — 对每个子类的 `Param` 调用 `GetBeanFields()` 获取 `[BeanField]` 列表，逐一调用 `WriteFieldAssignment`
5. 调用 `ParamSetter(resolvedParam)` 设置关联的 Param

```csharp
// 生成的代码示例（XParamCue 的 CueLogic 多态字段拆解）
// [BeanPolymorphicField] CueLogic
var polyBean = taskData.Param.CueLogic;
tp?.SetCueType(polyBean.GetType().Name);
var resolvedParamType = CueHelper.GetCueLogicParamType(polyBean.GetType().Name);
var resolvedParam = Activator.CreateInstance(resolvedParamType) as XParam;
if (resolvedParam != null)
{
    switch (polyBean)
    {
        case cfg.CueLogging pData:
        {
            var rp = resolvedParam as GAS.Runtime.XParamLogging;
            rp?.SetValue(pData.Param.Value);
            rp?.SetDuration(pData.Param.Duration);
            resolvedParam = rp;
            break;
        }
        // ... 其他子类
        default:
        {
            Debug.LogError($"[XLuban] Unknown CueLogic type: {polyBean.GetType().Name}");
            break;
        }
    }
}
tp?.SetParam(resolvedParam);
```

### 6.3 类型转换

对于特殊 Luban 类型（`vector2/3/4`），`WriteFieldAssignment` 会通过 `LubanTypeConversionMap` 自动转换。Map 的 key 使用 C# 运行时类型 FullName：

```csharp
// LubanTypeConversionMap
"UnityEngine.Vector3" → "new UnityEngine.Vector3({0}.X, {0}.Y, {0}.Z)"
"UnityEngine.Vector2" → "new UnityEngine.Vector2({0}.X, {0}.Y)"
"UnityEngine.Vector4" → "new UnityEngine.Vector4({0}.X, {0}.Y, {0}.Z, {0}.W)"
```

生成结果：

```csharp
cp?.SetOffset(new UnityEngine.Vector3(cData.Param.Offset.X, cData.Param.Offset.Y, cData.Param.Offset.Z));
```

### 6.4 Setter 方法编写指南

Setter 方法由用户完全自定义，只需保证方法名与 Attribute 中指定的一致：

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

## 七、映射关系总览

```
┌─────────────────────────────────────────────────────────────────────────────┐
│  C# 运行时                                    Luban __beans__.xlsx         │
├─────────────────────────────────────────────────────────────────────────────┤
│  XParam (interface)                            XParam (abstract bean)      │
│  ├─ XParamNone                                 ├─ XParamNone              │
│  ├─ XParamInt                                  ├─ XParamInt               │
│  │   [BeanField] Value → SetValue              │   Value: int             │
│  ├─ XParamFloat                                ├─ XParamFloat             │
│  │   [BeanField] Value → SetValue              │   Value: float           │
│  ├─ XParamCue                                  ├─ XParamCue               │
│  │   [BeanField] RequiredTags (Order=1)        │   RequiredTags: arr;int  │
│  │   [BeanField] ImmunityTags (Order=2)        │   ImmunityTags: arr;int  │
│  │   [BeanPolymorphicField] (Order=3)          │   CueLogic:              │
│  │     → TypeSetter: SetCueType                │     GameplayCueBase      │
│  │     → ParamSetter: SetParam                 │                          │
│  │   (无标注) Param                             │                          │
│  ├─ XParamApplyEffects                         ├─ XParamApplyEffects      │
│  │   [BeanField] IDs → SetIDs                  │   IDs: arr;int           │
│  │   [BeanPolymorphicField]                    │   TargetCatcher:         │
│  │     → TypeSetter: SetCatcherType            │     TargetCatcherBase    │
│  │     → ParamSetter: SetParam                 │                          │
│  │   (无标注) Param                             │                          │
│  ├─ XParamMMCScalable                          ├─ XParamMMCScalable       │
│  │   [BeanField] K → SetK                      │   K: float               │
│  │   [BeanField] B → SetB                      │   B: float               │
│  └─ ...                                        └─ ...                     │
├─────────────────────────────────────────────────────────────────────────────┤
│  GameplayCueBase<T>                            GameplayCueBase (abstract)  │
│  ├─ CueLogging<XParamLogging>                  ├─ CueLogging              │
│  │   (Param 自动添加)                           │   Param: XParamLogging  │
│  └─ ...                                        └─ ...                     │
├─────────────────────────────────────────────────────────────────────────────┤
│  ModMagnitudeCalculationBase<T>        ModMagnitudeCalculationBase (abs)   │
│  ├─ MMCScalableFloat<XParamMMCScalable>        ├─ MMCScalableFloat        │
│  │   (Param 自动添加)                           │   Param: XParamMMCScalable│
│  └─ ...                                        └─ ...                     │
├─────────────────────────────────────────────────────────────────────────────┤
│  AbilityLogicBase<T>                           AbilityLogicBase (abstract) │
│  AbilityTaskBase<T>                            AbilityTaskBase (abstract)  │
│  TargetCatcherBase<T>                          TargetCatcherBase (abstract)│
└─────────────────────────────────────────────────────────────────────────────┘
```

### 关键说明

- **`[BeanField]`** 标注的字段直接映射为 Bean 字段，一对一关系
- **`[BeanPolymorphicField]`** 标注的字段在 Luban 侧表现为一个多态 Bean 字段（如 `CueLogic: GameplayCueBase`），运行时拆解为 `TypeName (string)` + `Param (XParam)` 两个字段
- **`(无标注) Param`** 不出现在 Bean 定义中，由 `[BeanPolymorphicField]` 的 `ParamSetter` 在代码生成时自动关联
- **逻辑 Bean 的 `Param`** 由 `BeanUpdater.Collect*Beans()` 通过反射泛型参数自动添加，不依赖 `[BeanField]`

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
- 各大类（XParam / GameplayCueBase / ModMagnitudeCalculationBase / ...）之间有蓝色（`#DAEEF3`）分隔行

### 8.3 示例

```
Row 4:  | XParam         |        | 通用参数基类 |              |                    |          |  ← 抽象基类
Row 5:  | XParamNone     | XParam | 无参数       |              |                    |          |
Row 6:  | XParamInt      | XParam | 整数参数     | Value        | int                | 值       |
Row 7:  | XParamLogging  | XParam | 日志参数     | Value        | string             | 日志内容  |
Row 8:  |                |        |              | Duration     | float              | 持续时间  |
Row 9:  (空行 — 多字段 Bean 后分隔)
Row 10: | XParamCue      | XParam | Cue参数      | RequiredTags | (array#sep=;),int  | 需求标签  |
Row 11: |                |        |              | ImmunityTags | (array#sep=;),int  | 免疫标签  |
Row 12: |                |        |              | CueLogic     | GameplayCueBase    | CueLogic  |
Row 13: (空行)
...
Row XX: ████████████████████████████████████████████  ← 蓝色分隔行 (#DAEEF3)
Row XX: | GameplayCueBase    |                | Cue逻辑基类      |       |               |      |  ← 新类别
Row XX: | CueLogging         | GameplayCueBase | Cue:日志        | Param | XParamLogging | 参数  |
...
Row XX: ████████████████████████████████████████████  ← 蓝色分隔行
Row XX: | ModMagnitudeCalculationBase |       | MMC逻辑基类      |       |                    |      |
Row XX: | MMCScalableFloat | ModMagnitudeCalculationBase | MMC:线性缩放 | Param | XParamMMCScalable | 参数 |
...
Row XX: ████████████████████████████████████████████
Row XX: | AbilityLogicBase   |                | Ability逻辑基类  |       |               |      |
...
Row XX: ████████████████████████████████████████████
Row XX: | AbilityTaskBase    |                | AbilityTask基类  |       |               |      |
...
Row XX: ████████████████████████████████████████████
Row XX: | TargetCatcherBase  |                | TargetCatcher基类|       |               |      |
Row XX: | CatchAreaBox3D     | TargetCatcherBase | 目标捕获:Box3D | Param | XParamCatchAreaBox3D | 参数 |
...
```

> **注意**：`CueLogic: GameplayCueBase` 是通过 `[BeanPolymorphicField]` 收集的，在 `__beans__.xlsx` 中表现为普通的多态字段。抽象基类名已从旧版的 `CueLogic`、`MmcLogic`、`AbilityLogic`、`AbilityTask` 更新为 `GameplayCueBase`、`ModMagnitudeCalculationBase`、`AbilityLogicBase`、`AbilityTaskBase`。

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

### 9.3 创建带多态引用的参数类

当参数类需要内嵌一个多态逻辑引用时，使用 `[BeanPolymorphicField]`：

```csharp
public class XParamMyComposite : XParam
{
    [BeanField(nameof(SetID), Comment = "ID")]
    public int ID;

    [BeanPolymorphicField(
        beanFieldName: "CueLogic",
        lubanPolymorphicType: nameof(GameplayCueBase),
        typeSetter: nameof(SetCueType),
        paramSetter: nameof(SetCueParam),
        ParamTypeResolver = "CueHelper.GetCueLogicParamType",
        HelperCategory = "Cue")]
    public string CueType { get; private set; }

    // Param 不标注任何 Attribute
    public XParam CueParam { get; set; }

    public void SetID(int v) { ID = v; }
    public void SetCueType(string v) { CueType = v; }
    public void SetCueParam(XParam v) { CueParam = v; }
}
```

### 9.4 自动化工作流

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
1. XParam 类中是否正确标注了 `[BeanField]` 和/或 `[BeanPolymorphicField]`
2. `[BeanField]` 的 `Setter` 是否指向一个存在的方法
3. `[BeanPolymorphicField]` 的 `TypeSetter`、`ParamSetter`、`ParamTypeResolver` 是否均正确
4. 是否运行了 `更新Bean定义` + Luban 导表 + `生成所有`
5. Bean 字段名是否与 JSON 中的 key 一致

### Q2: 如何添加新的参数类型？

1. 创建继承自 `XParam` 的类
2. 用 `[BeanField(nameof(SetXxx))]` 标注需要配置的字段
3. 如果字段是多态引用，改用 `[BeanPolymorphicField(...)]` 标注类型判别符字段
4. 编写对应的 Setter 方法
5. 实现 `DecodeExcelData` 和 `EncodeExcelData`（`#if UNITY_EDITOR`）
6. 运行 `更新Bean定义`

### Q3: `[BeanField]` / `[BeanPolymorphicField]` 可以标注在 private 成员上吗？

可以。`BeanUpdater` 和 `EXEditorHelper` 均使用 `BindingFlags.NonPublic` 扫描，所以 `private` 字段/属性只要有对应 Attribute 就会被收录。

### Q4: Setter 方法的参数类型可以和字段类型不一致吗？

可以。例如 `AttributeBasedMmcParam` 中：

```csharp
[BeanField(nameof(SetFromType), LubanType = "int")]
public AttributeFromType FromType { get; private set; }

// Setter 接受 int，内部做枚举转换
public void SetFromType(int v) { FromType = (AttributeFromType)v; }
```

此时需要确保 `[BeanField(LubanType = "int")]` 显式指定 Luban 类型为 `int`，否则自动映射可能不正确。

### Q5: Bean 定义更新后还需要做什么？

1. 运行 Luban 导表工具生成新的 Bean C# 类（`cfg.*`）
2. 重新生成运行时映射代码（菜单 `生成所有`）
3. 重新导出配置表 JSON

### Q6: `[BeanPolymorphicField]` 的 `HelperCategory` 有哪些可选值？

当前支持：

| HelperCategory | 子类来源 | 参数类型映射 |
|---------------|---------|------------|
| `"Cue"` | `EditorCueHelper.GetCachedCueTypes()` | `EditorCueHelper.CueToCueParamTypeMap()` |
| `"TargetCatcher"` | `EditorTargetCatcherHelper.GetCachedTargetCatcherTypes()` | `EditorTargetCatcherHelper.CatcherToParamTypeMap()` |

如需添加新的 HelperCategory，需在 `CodeGeneratorLubanPart.GetPolymorphicHelperInfo()` 中添加对应的 `case` 分支。

### Q7: Order 的默认值是怎么确定的？

`[BeanField]` 和 `[BeanPolymorphicField]` 的构造函数均使用 `[CallerLineNumber] int order = 0` 参数。编译器会自动将调用处的源码行号作为默认值。因此，只要 `[BeanField]` 按声明顺序排列在源文件中，字段就会按声明顺序写入 Bean 定义，无需手动指定 `Order`。

如需显式控制排序，可手动指定：`[BeanField(nameof(SetX), Order = 100)]`

## 十二、工具链相关文件

| 文件 | 用途 |
|------|------|
| `Assets/GAS/Runtime/General/XParam/BeanFieldAttribute.cs` | `[BeanField]` Attribute 定义 |
| `Assets/GAS/Runtime/General/XParam/BeanPolymorphicFieldAttribute.cs` | `[BeanPolymorphicField]` Attribute 定义 |
| `Assets/GAS/Editor/CodeGen/BeanUpdater.cs` | 扫描 `[BeanField]` 和 `[BeanPolymorphicField]`，自动更新 `__beans__.xlsx` |
| `Assets/GAS/Editor/CodeGen/CodeGeneratorLubanPart.cs` | 读取 `[BeanField]` 的 Setter 和 `[BeanPolymorphicField]` 的多态信息，生成 XLuban 赋值代码 |
| `Assets/GAS/Editor/Helper/EXEditorHelper.cs` | `GetBeanFields()` / `GetBeanPolymorphicFields()` 辅助方法 |
| `Assets/GAS/Editor/CodeGen/CodeGenerator.cs` | 代码生成主入口 |
| `__beans__.xlsx` | Luban Bean 定义表 |
| `XLuban.gen.cs` | 自动生成的运行时 Luban 数据加载代码 |