# EX-GAS 2.0 开发辅助 Skill

## Skill 概述

你是一个 EX-GAS 2.0（Unity DOTS 版 Gameplay Ability System）的开发辅助 AI。
当用户需要创建 EX-GAS 相关的自定义逻辑类或参数类时，你必须严格遵循本文档中的约定和模板。

EX-GAS 有 5 种可扩展的逻辑类型，每种都需要配套的 XParam 参数类：

| 逻辑类型 | 基类 | 建议前缀 | 用途 |
|---------|------|---------|------|
| AbilityLogic | `AbilityLogicBase<T>` | `AL` | 技能逻辑 |
| GameplayCue | `GameplayCueBase<T>` | 无强制 | 表现效果（特效/音效等） |
| MMC | `ModMagnitudeCalculationBase<T>` | `MMC` | 数值修改器计算 |
| AbilityTask | `AbilityTaskBase<T>` | `Task` | 时间轴技能中的子任务 |
| TargetCatcher | `TargetCatcherBase<T>` | 无强制 | 目标捕获器 |

---

## 触发条件

当用户的请求涉及以下内容时，使用本 Skill：

- 创建新的 AbilityLogic / GameplayCue / MMC / AbilityTask / TargetCatcher
- 创建新的 XParam 参数类
- 询问 EX-GAS 的代码约定、命名规范、工作流
- 修改或扩展现有的 EX-GAS 逻辑类

---

## 核心规则（必须遵守）

### 规则 1：`[BeanField]` 是唯一的字段识别标记

只有标注了 `[BeanField]` 的字段/属性才会被 EX-GAS 工具链识别并写入 Luban 配置表。
未标注的成员完全被忽略。

```csharp
// ✅ 会被工具链识别
[BeanField(nameof(SetValue), Comment = "伤害值")]
public float Value { get; private set; }

// ❌ 不会被识别，是纯运行时字段
public Vector3 CachedPosition;
```

### 规则 2：每个 `[BeanField]` 必须绑定 Setter 方法

`[BeanField]` 的第一个参数是 Setter 方法名，推荐用 `nameof(SetXxx)`。
Setter 方法由用户编写，允许在赋值时执行额外操作。

```csharp
[BeanField(nameof(SetDamage), Comment = "伤害值")]
public float Damage { get; private set; }

public void SetDamage(float value) { Damage = value; }
```

### 规则 3：`EncodeExcelData` 中空数据必须用默认占位数据代替

EX-GAS 的 Luban 配置采用流式配置，不支持空占位。
`EncodeExcelData()` 中绝对不能写入 null 或空值，必须用默认占位数据代替。

```csharp
// ✅ 正确：空字符串用 XParamDefault.DefaultString 代替
result.Add(string.IsNullOrEmpty(Value) ? XParamDefault.DefaultString : Value);

// ✅ 正确：空数组用 string.Empty 代替
result.Add(IDs == null || IDs.Length == 0 ? string.Empty : string.Join(";", IDs));

// ❌ 错误：直接写入可能为空的值
result.Add(Value);  // Value 可能是 null
```

默认占位常量定义在 `XParamDefault`：
- `XParamDefault.DefaultInt` = `0`
- `XParamDefault.DefaultString` = `"\"\""`
- `XParamDefault.DefaultFloat` = `0f`
- `XParamDefault.DefaultBool` = `false`

### 规则 4：`LayerMask` 类型必须显式指定 `LubanType = "int"`

```csharp
[BeanField(nameof(SetLayer), LubanType = "int", Comment = "检测层级")]
public LayerMask layer;

// Setter 接受 int，内部赋值给 LayerMask.value
public void SetLayer(int layer) { this.layer.value = layer; }
```

### 规则 5：枚举类型必须显式指定 `LubanType = "int"`

```csharp
[BeanField(nameof(SetFromType), LubanType = "int")]
public AttributeFromType FromType { get; private set; }

// Setter 接受 int，内部做枚举转换
public void SetFromType(int v) { FromType = (AttributeFromType)v; }
```

### 规则 6：命名规范

| 类别 | 命名规范 | 示例 |
|------|---------|------|
| Cue 逻辑类 | 无强制前缀 | `CueLogging`, `CueExplosionVFX` |
| MMC 逻辑类 | 建议以 `MMC` 开头 | `MMCScalableFloat`, `MMCAttributeBased` |
| Ability 逻辑类 | 建议以 `AL` 开头 | `ALMove`, `ALDeath`, `ALFireball` |
| AbilityTask 类 | 建议以 `Task` 开头 | `TaskPlayCue`, `TaskApplyEffects` |
| TargetCatcher 类 | 无强制前缀 | `CatchAreaBox3D`, `CatchSelf` |
| 参数类 | 建议以 `XParam` 开头 | `XParamLogging`, `XParamMove` |

Bean 名称与 C# 类名完全一致，不做任何转换。

---

## 模板：XParam 参数类

### 单字段 XParam（参考 XParamFloat）

```csharp
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace GAS.Runtime
{
    public class {{ClassName}} : XParam
    {
        [LabelText("{{FieldComment}}")]
        [ShowInInspector]
        [BeanField(nameof(Set{{FieldName}}), Comment = "{{FieldComment}}")]
        public {{FieldType}} {{FieldName}} { get; private set; }

        public {{ClassName}}()
        {
            {{FieldName}} = {{DefaultValue}};
        }

        public void Set{{FieldName}}({{SetterParamType}} value)
        {
            {{SetterBody}};
        }

#if UNITY_EDITOR
        public void DecodeExcelData(List<object> paramData)
        {
            if (paramData == null || paramData.Count == 0)
            {
                {{FieldName}} = {{DefaultValue}};
                return;
            }
            // 按字段类型选择解析方式（见下方类型解析参考）
        }

        public List<object> EncodeExcelData()
        {
            var result = new List<object>();
            // 按字段类型选择编码方式（见下方类型编码参考）
            return result;
        }
#endif
    }
}
```

### 多字段 XParam（参考 XParamPlaySound）

```csharp
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace GAS.Runtime
{
    public class {{ClassName}} : XParam
    {
        [ShowInInspector]
        [LabelText("{{Field1Comment}}")]
        [BeanField(nameof(Set{{Field1Name}}), Comment = "{{Field1Comment}}")]
        public {{Field1Type}} {{Field1Name}};

        [ShowInInspector]
        [LabelText("{{Field2Comment}}")]
        [BeanField(nameof(Set{{Field2Name}}), Comment = "{{Field2Comment}}")]
        public {{Field2Type}} {{Field2Name}};

        // ... 更多字段

        public {{ClassName}}()
        {
            {{Field1Name}} = {{Default1}};
            {{Field2Name}} = {{Default2}};
        }

        public void Set{{Field1Name}}({{Setter1Type}} value) { {{Field1Name}} = value; }
        public void Set{{Field2Name}}({{Setter2Type}} value) { {{Field2Name}} = value; }

#if UNITY_EDITOR
        public void DecodeExcelData(List<object> paramData)
        {
            if (paramData == null || paramData.Count == 0)
            {
                {{Field1Name}} = {{Default1}};
                {{Field2Name}} = {{Default2}};
                return;
            }

            // 字段 0: {{Field1Name}}
            // 字段 1: {{Field2Name}}
            // ... 按索引顺序解析
        }

        public List<object> EncodeExcelData()
        {
            var result = new List<object>();
            // 按字段声明顺序编码，空值必须用默认占位代替
            return result;
        }
#endif
    }
}
```

### 类型解析/编码参考

**float 类型：**
```csharp
// Decode
K = paramData[0] as float? ?? 0;
// 或
float.TryParse(paramData[0]?.ToString(), out var val); FieldName = val;

// Encode
result.Add(K);
```

**string 类型：**
```csharp
// Decode
var strData = paramData[0] as string;
Value = (string.IsNullOrEmpty(strData) || strData == XParamDefault.DefaultString)
    ? string.Empty : strData;

// Encode（空字符串必须用占位符）
result.Add(string.IsNullOrEmpty(Value) ? XParamDefault.DefaultString : Value);
```

**int 类型：**
```csharp
// Decode
int.TryParse(paramData[0]?.ToString(), out var val); FieldName = val;
// 或
FieldName = Convert.ToInt32(paramData[0]);

// Encode
result.Add(FieldName);
```

**bool 类型：**
```csharp
// Decode
bool.TryParse(paramData[0]?.ToString(), out var val); FieldName = val;

// Encode
result.Add(FieldName);
// 或
result.Add(FieldName.ToString());
```

**int[] 数组类型（分号分隔）：**
```csharp
// Decode
var strData = paramData[0] as string;
if (string.IsNullOrEmpty(strData))
{
    IDs = Array.Empty<int>();
}
else
{
    var strArray = strData.Split(';');
    IDs = new int[strArray.Length];
    for (var i = 0; i < strArray.Length; i++)
        if (int.TryParse(strArray[i], out var val))
            IDs[i] = val;
        else
            IDs[i] = 0;
}

// Encode（空数组用 string.Empty）
result.Add(IDs == null || IDs.Length == 0 ? string.Empty : string.Join(";", IDs));
```

**Vector3 类型（逗号分隔）：**
```csharp
// Decode
var strData = paramData[index] as string;
if (!string.IsNullOrEmpty(strData))
{
    var data = strData.Split(',');
    if (data.Length == 3 &&
        float.TryParse(data[0], out var x) &&
        float.TryParse(data[1], out var y) &&
        float.TryParse(data[2], out var z))
    {
        offset = new Vector3(x, y, z);
    }
}

// Encode
result.Add($"{offset.x},{offset.y},{offset.z}");
```

**LayerMask 类型：**
```csharp
// Decode
int.TryParse(paramData[index]?.ToString(), out var layerNumber);
layer = layerNumber;

// Encode
result.Add(layer.value.ToString());
// 或
result.Add(layer.value);
```

---

## 模板：AbilityLogic 逻辑类

```csharp
using GAS.Runtime;
using Unity.Entities;

namespace {{Namespace}}
{
    /// <summary>
    /// {{Description}}
    /// </summary>
    public class {{ClassName}} : AbilityLogicBase<{{ParamClassName}}>
    {
        public {{ClassName}}(Entity ability) : base(ability)
        {
        }

        public override void ActivateAbility(GlobalTimer timer)
        {
            // 技能激活时执行
            // 通过 _param 访问参数
            // 通过 Owner 访问技能拥有者的 AbilitySystemCell
        }

        public override void CancelAbility(GlobalTimer timer)
        {
            // 技能被取消时执行（通常调用 EndAbility）
            EndAbility(timer);
        }

        public override void EndAbility(GlobalTimer timer)
        {
            // 技能结束时执行
        }

        public override void AbilityTick(GlobalTimer timer)
        {
            // 技能激活期间每帧执行（不需要可留空）
        }
    }
}
```

**可用的基类成员：**
- `_param` — 泛型参数实例（类型为 `T`）
- `Owner` — 技能拥有者的 `AbilitySystemCell`
- `Spec` — 技能的 `AbilitySpec` 包装
- `_abilityEntity` — 技能的 ECS Entity
- `_entityManager` — Unity `EntityManager`
- `TryEndSelf()` — 尝试结束自身
- `CreateGameplayEffectEntity(config)` — 创建 GE Entity
- `ApplyGameplayEffectTo(ge, target, source)` — 施加 GE 到目标
- `RemoveGameplayEffect(geEntity)` — 移除 GE

**完整示例（ALApplyEffect）：**
```csharp
public class ALApplyEffect : AbilityLogicBase<XParamEffectIDs>
{
    public ALApplyEffect(Entity ability) : base(ability) { }

    public override void ActivateAbility(GlobalTimer timer)
    {
        var owner = Owner;
        foreach (var effectCode in _param.IDs)
        {
            var effectCfg = GameplayEffectHelper.GetConfigByID(effectCode);
            var geEntity = CreateGameplayEffectEntity(effectCfg);
            ApplyGameplayEffectTo(geEntity, owner, owner);
        }
    }

    public override void CancelAbility(GlobalTimer timer) { EndAbility(timer); }

    public override void EndAbility(GlobalTimer timer)
    {
        var ownerAsc = GetOwnerAscEntity();
        var geEntities = _entityManager.GetBuffer<BGameplayEffect>(ownerAsc);
        foreach (var beEffect in geEntities)
        {
            var effect = beEffect.GameplayEffect;
            if (_entityManager.HasComponent<CCreatedByAbility>(effect))
            {
                var createdByAbility = _entityManager.GetComponentData<CCreatedByAbility>(effect);
                if (createdByAbility.sourceAbility == _abilityEntity)
                    RemoveGameplayEffect(effect);
            }
        }
    }

    public override void AbilityTick(GlobalTimer timer) { }
}
```

---

## 模板：GameplayCue 逻辑类

```csharp
using GAS.Runtime;
using UnityEngine;

namespace {{Namespace}}
{
    /// <summary>
    /// {{Description}}
    /// </summary>
    public class {{ClassName}} : GameplayCueBase<{{ParamClassName}}>
    {
        public override void OnActivate(float time)
        {
            base.OnActivate(time);
            // Cue 激活时执行（播放特效、音效等）
            // 通过 Parameter 访问参数
            // 通过 _abilitySystemCell 访问目标 ASC
        }

        public override void OnTick(float time)
        {
            base.OnTick(time);
            // 每帧执行
        }

        public override void OnRemove(float time)
        {
            base.OnRemove(time);
            // Cue 移除时执行（清理资源）
        }
    }
}
```

**可覆写的生命周期方法（全部是 virtual，按需覆写）：**
- `OnAdd(float time)` — 添加到 ASC 时
- `OnRemove(float time)` — 从 ASC 移除时
- `OnActivate(float time)` — 激活时
- `OnDeactivate(float time)` — 停用时
- `OnTick(float time)` — 每帧调用
- `OnDestroy(float time)` — 销毁时

**可用的基类成员：**
- `Parameter` — 泛型参数实例
- `_abilitySystemCell` — 目标 ASC
- `_cueEntity` — Cue 的 ECS Entity
- `Play(bool replay = false)` — 播放 Cue
- `Stop(bool immediate = false)` — 停止 Cue
- `RemoveSelf()` — 移除自身
- `KillSelf()` — 销毁自身

**完整示例（CueLogging）：**
```csharp
public class CueLogging : GameplayCueBase<XParamLogging>
{
    private float _startTime;

    public override void OnActivate(float time)
    {
        base.OnActivate(time);
        _startTime = time;
        Debug.Log($"CueLogging activated. Value:{Parameter.Value}");
    }

    public override void OnTick(float time)
    {
        base.OnTick(time);
        if (time - _startTime > Parameter.Duration)
        {
            RemoveSelf();
            KillSelf();
        }
    }
}
```

---

## 模板：MMC 逻辑类

```csharp
using GAS.Runtime;

namespace {{Namespace}}
{
    /// <summary>
    /// {{Description}}
    /// 计算公式：{{Formula}}
    /// </summary>
    public class {{ClassName}} : ModMagnitudeCalculationBase<{{ParamClassName}}>
    {
        public override float CalculateMagnitude(MmcContext mmcContext, float magnitude)
        {
            // mmcContext.Source — 效果施放者的 AbilitySystemCell
            // mmcContext.Target — 效果目标的 AbilitySystemCell
            // magnitude — Effect 配置中的基础数值
            // Parameter — 泛型参数实例
            return magnitude;
        }

        // 可选：GE 被添加到目标时调用
        // protected override void OnAdded(MmcContext context, int targetAttrSetCode, int targetAttrCode) { }

        // 可选：GE 从目标移除时调用
        // protected override void OnRemoved() { }
    }
}
```

**完整示例（MMCScalableFloat 的参数类 MmcParaFloatScale 等价于 XParamMMCScalable）：**
```csharp
public class MMCScalableFloat : ModMagnitudeCalculationBase<MmcParaFloatScale>
{
    public override float CalculateMagnitude(MmcContext context, float magnitude)
    {
        return magnitude * Parameter.K + Parameter.B;
    }
}
```

---

## 模板：AbilityTask 类

```csharp
using GAS.Runtime;

namespace {{Namespace}}
{
    /// <summary>
    /// {{Description}}
    /// </summary>
    public class {{ClassName}} : AbilityTaskBase<{{ParamClassName}}>
    {
        public {{ClassName}}(AbilityLogicBase logic) : base(logic)
        {
        }

        protected override void OnBegin(int startFrame)
        {
            // 任务开始时执行
            // 通过 Parameter 访问参数
            // 通过 Owner 访问技能拥有者
        }

        protected override void OnTick(int frameIndex)
        {
            // 每帧执行
        }

        protected override void OnFinish(int endFrame)
        {
            // 任务结束时执行
        }
    }
}
```

**可用的基类成员：**
- `Parameter` — 泛型参数实例
- `_logic` — 所属的 AbilityLogicBase
- `Spec` — 技能的 AbilitySpec
- `Owner` — 技能拥有者的 AbilitySystemCell
- `_startTime` — 任务开始时间
- `_timeUnit` — 时间单位（默认 Frame）

**完整示例（TaskPlayCue）：**
```csharp
public class TaskPlayCue : AbilityTaskBase<XParamCue>
{
    private GameplayCueUnit _cueUnit;

    public TaskPlayCue(AbilityLogicBase logic) : base(logic) { }

    public override void InitParameters(XParam parameter)
    {
        base.InitParameters(parameter);
        _cueUnit = new GameplayCueUnit(Parameter.GetCueConfig());
    }

    protected override void OnBegin(int startFrame)
    {
        _cueUnit.Create();
        _cueUnit.AddToAsc(Owner);
        _cueUnit.Play();
    }

    protected override void OnFinish(int endFrame)
    {
        _cueUnit.Stop();
        _cueUnit.RemoveFromAsc();
        _cueUnit.Destroy();
    }
}
```

---

## 模板：TargetCatcher 类

```csharp
using System.Collections.Generic;
using GAS.Runtime;

namespace {{Namespace}}
{
    /// <summary>
    /// {{Description}}
    /// </summary>
    public class {{ClassName}} : TargetCatcherBase<{{ParamClassName}}>
    {
        protected override void CatchTargetsNonAlloc(AbilitySystemCell mainTarget, List<AbilitySystemCell> results)
        {
            // mainTarget — 主目标（可能为 null）
            // results — 将捕获到的目标 Add 到此列表
            // Owner — 技能拥有者的 AbilitySystemCell
            // Parameter — 泛型参数实例
        }
    }
}
```

**完整示例（CatchSelf）：**
```csharp
public sealed class CatchSelf : TargetCatcherBase<XParamNone>
{
    protected override void CatchTargetsNonAlloc(AbilitySystemCell mainTarget, List<AbilitySystemCell> results)
    {
        results.Add(Owner);
    }
}
```

**完整示例（CatchAreaBox3D）：**
```csharp
public sealed class CatchAreaBox3D : TargetCatcherBase<XParamCatchAreaBox3D>
{
    private static readonly Collider[] Colliders = new Collider[64];

    protected override void CatchTargetsNonAlloc(AbilitySystemCell mainTarget, List<AbilitySystemCell> results)
    {
        int count;
        if (Parameter.isWorldSpace)
        {
            count = Physics.OverlapBoxNonAlloc(
                Parameter.offset, Parameter.size * 0.5f, Colliders,
                Quaternion.Euler(Parameter.rotation), Parameter.layer.value);
        }
        else
        {
            var t = mainTarget.GameObject.transform;
            count = Physics.OverlapBoxNonAlloc(
                t.TransformPoint(Parameter.offset), Parameter.size * 0.5f, Colliders,
                Quaternion.Euler(t.TransformDirection(Parameter.rotation)), Parameter.layer.value);
        }

        for (var i = 0; i < count; ++i)
        {
            var mono = Colliders[i].GetComponent<AbilitySystemComponent>();
            if (mono != null) results.Add(mono.Cell);
        }
    }
}
```

---

## 已有的通用 XParam 类型（可直接复用，无需重新创建）

| 类名 | 字段 | 用途 |
|------|------|------|
| `XParamNone` | （无） | 无参数 |
| `XParamInt` | `Value: int` | 单个整数 |
| `XParamFloat` | `Value: float` | 单个浮点数 |
| `XParamString` | `Value: string` | 单个字符串 |
| `XParamBool` | `Value: bool` | 单个布尔值 |
| `XParamVector2` | `Value: Vector2` | 二维向量 |
| `XParamVector3` | `Value: Vector3` | 三维向量 |
| `XParamArrayInt` | `Value: int[]` | 整数数组 |
| `XParamArrayFloat` | `Value: float[]` | 浮点数组 |
| `XParamArrayString` | `Value: string[]` | 字符串数组 |
| `XParamEffectIDs` | `IDs: int[]` | Effect ID 数组 |
| `XParamCueIDs` | `IDs: int[]` | Cue ID 数组 |
| `XParamLogging` | `Value: string`, `Duration: float` | 日志+持续时间 |
| `XParamMMCScalable` | `K: float`, `B: float` | 线性函数参数 |
| `XParamPlaySound` | `AudioClipPath`, `Volume`, `Speed`, `Loop`, `AudioSourceNodePath` | 音效播放 |
| `XParamCatchAreaBox3D` | `isWorldSpace`, `offset`, `size`, `rotation`, `layer` | 3D 盒形区域 |

如果用户需要的参数结构与上述某个类型完全匹配，直接使用已有类型，不要重新创建。

---

## 代码生成管线（创建新类型后必须执行）

创建新的逻辑类或参数类后，必须按顺序执行以下 4 步：

1. **更新 Bean 定义**：Unity 菜单 → `EXTool/EX-GAS/生成脚本/更新Bean定义`
    - 扫描所有 `[BeanField]` 标注，更新 `__beans__.xlsx`
2. **运行 Luban 导表**：运行配置表工程中的 `gen.bat`
    - 生成新的 `cfg.*` Bean C# 类
3. **生成运行时代码**：Unity 菜单 → `EXTool/EX-GAS/生成脚本/生成所有`
    - 生成类型映射、工厂方法等运行时代码
4. **重新导出配置表**：Unity 菜单 → `EXTool/EX-GAS/生成脚本/GAS表配置`
    - 将 Excel 数据导出为 JSON

**重要**：如果只是修改了 Excel 配置表（未创建新类型），只需执行步骤 2 和 4。

---

## 常见问题排查

| 问题 | 检查项 |  
|------|--------|  
| 配置表解析失败 | 1. `[BeanField]` 标注是否正确 2. Setter 方法是否存在且方法名匹配 3. 是否执行了完整管线（4步） 4. Bean 字段名是否与 JSON key 一致 |  
| 新类型不生效 | 1. 是否执行了"更新Bean定义" 2. 是否运行了 Luban 导表 3. 是否执行了"生成所有" 4. 是否重新导出了配置表 |  
| `[BeanField]` 标注在 private 成员上不生效 | 不会出现此问题。`BeanUpdater` 使用 `BindingFlags.NonPublic` 扫描，private 成员只要有 `[BeanField]` 就会被收录 |  
| Setter 参数类型与字段类型不一致 | 这是允许的。需要确保 `[BeanField(LubanType = "xxx")]` 显式指定 Luban 类型。典型场景：枚举用 `LubanType = "int"`，`LayerMask` 用 `LubanType = "int"` |  
| `EncodeExcelData` 写入空值导致崩溃 | EX-GAS 的流式配置不支持空占位。`string` 空值必须用 `XParamDefault.DefaultString`（即 `"\"\""`）代替，数组空值用 `string.Empty` 代替 |  
| 数组字段在 Excel 中格式错误 | 数组在 `DecodeExcelData` 中使用 `;` 分隔（如 `"1;2;3"`），在 `EncodeExcelData` 中用 `string.Join(";", array)` 编码 |  
| Vector3 字段解析失败 | Vector3 在 Excel 中使用 `,` 分隔（如 `"1.0,2.0,3.0"`），注意不是 `;` |  
  
---  

## 默认占位常量参考

定义在 `XParamDefault` 静态类中：

```csharp  
public static class XParamDefault  
{  
    public const int DefaultInt = 0;  
    public const string DefaultString = "\"\"";  
    public const float DefaultFloat = 0f;  
    public const bool DefaultBool = false;  
}  
```  
  
---  

## `[BeanField]` Attribute 完整参数

```csharp  
[BeanField(  
    setter,              // 必填：Setter 方法名，推荐 nameof(SetXxx)  
    Name = "xxx",        // 可选：覆盖 Bean 字段名（默认取成员名）  
    LubanType = "xxx",   // 可选：覆盖 Luban 类型（默认自动映射）  
    Comment = "xxx"      // 可选：Bean 字段注释  
)]  
```  

**C# 类型 → Luban 类型自动映射表：**

| C# 类型 | Luban 类型 | 需要手动指定 LubanType？ |  
|---------|-----------|------------------------|  
| `int` | `int` | 否 |  
| `long` | `long` | 否 |  
| `float` | `float` | 否 |  
| `double` | `double` | 否 |  
| `bool` | `bool` | 否 |  
| `string` | `string` | 否 |  
| `Vector2` | `vector2` | 否 |  
| `Vector3` | `vector3` | 否 |  
| `Vector4` | `vector4` | 否 |  
| `LayerMask` | — | **是**，必须 `LubanType = "int"` |  
| 枚举类型 | — | **是**，必须 `LubanType = "int"` |  
| `T[]` | `(array#sep=,),T` | 否 |  
| `List<T>` | `(array#sep=,),T` | 否 |  
  
---  

## 创建新类型后的验证清单

创建新的逻辑类或参数类后，按此清单逐项检查：

- [ ] 类名遵循命名规范（AL/MMC/Task/XParam 前缀）
- [ ] 参数类实现了 `XParam` 接口
- [ ] 所有需要配置的字段都标注了 `[BeanField(nameof(SetXxx))]`
- [ ] 每个 `[BeanField]` 都有对应的 `SetXxx` 方法
- [ ] `LayerMask` 和枚举类型的 `[BeanField]` 指定了 `LubanType = "int"`
- [ ] 参数类有默认无参构造函数，初始化所有字段为默认值
- [ ] `#if UNITY_EDITOR` 下实现了 `DecodeExcelData` 和 `EncodeExcelData`
- [ ] `EncodeExcelData` 中所有空值都用默认占位数据代替
- [ ] `DecodeExcelData` 中处理了 `paramData` 为 null 或空的情况
- [ ] 逻辑类的泛型参数 `T` 指向正确的参数类
- [ ] 执行了 4 步管线：更新Bean定义 → Luban导表 → 生成所有 → GAS表配置

---  

## 工具链相关文件

| 文件 | 用途 |  
|------|------|  
| `Assets/GAS/Runtime/General/XParam/BeanFieldAttribute.cs` | `[BeanField]` Attribute 定义 |  
| `Assets/GAS/Runtime/General/XParam/XParam.cs` | `XParam` 接口 + `XParamDefault` 常量 |  
| `Assets/GAS/Editor/CodeGen/BeanUpdater.cs` | 扫描 `[BeanField]`，自动更新 `__beans__.xlsx` |  
| `Assets/GAS/Editor/CodeGen/CodeGeneratorLubanPart.cs` | 读取 `[BeanField]` 的 Setter，生成 XLuban 赋值代码 |  
| `Assets/GAS/Editor/Helper/EXEditorHelper.cs` | `GetBeanFields()` 辅助方法 |  
| `Assets/GAS/Editor/CodeGen/CodeGenerator.cs` | 代码生成主入口 |  
| `__beans__.xlsx` | Luban Bean 定义表（在配置表工程的 Datas 目录下） |  
| `XLuban.gen.cs` | 自动生成的运行时 Luban 数据加载代码 |  
  
---  

## 基类成员速查

### AbilityLogicBase

| 成员 | 类型 | 说明 |  
|------|------|------|  
| `_param` | `T` | 泛型参数实例 |  
| `_paramRaw` | `XParam` | 原始参数引用 |  
| `_abilityEntity` | `Entity` | 技能的 ECS Entity |  
| `_entityManager` | `EntityManager` | Unity EntityManager |  
| `_code` | `int` | 技能配置 ID |  
| `Owner` | `AbilitySystemCell` | 技能拥有者的 ASC |  
| `Spec` | `AbilitySpec` | 技能的 Spec 包装 |  
| `TryEndSelf()` | `void` | 主动结束自身 |  
| `GetAscEntity()` | `Entity` | 获取 Owner ASC Entity |  
| `GetOwnerAscEntity()` | `Entity` | 同上（别名） |  
| `CreateGameplayEffectEntity(config)` | `Entity` | 创建 GE Entity |  
| `ApplyGameplayEffectTo(ge, target, source)` | `void` | 施加 GE 到目标 |  
| `RemoveGameplayEffect(geEntity)` | `void` | 移除 GE |  

### GameplayCueBase

| 成员 | 类型 | 说明 |  
|------|------|------|  
| `Parameter` | `T` | 泛型参数实例 |  
| `_cueEntity` | `Entity` | Cue 的 ECS Entity |  
| `_sourceEntity` | `Entity` | Cue 来源 Entity |  
| `_sourceType` | `CueSourceType` | Cue 来源类型 |  
| `_targetAscEntity` | `Entity` | 目标 ASC Entity |  
| `_abilitySystemCell` | `AbilitySystemCell` | 目标 ASC 实例 |  
| `Play(replay)` | `void` | 播放 Cue |  
| `Stop(immediate)` | `void` | 停止 Cue |  
| `RemoveSelf()` | `void` | 移除自身 |  
| `KillSelf()` | `void` | 销毁自身 |  

### AbilityTaskBase

| 成员 | 类型 | 说明 |  
|------|------|------|  
| `Parameter` | `T` | 泛型参数实例 |  
| `_logic` | `AbilityLogicBase` | 所属的 AbilityLogic |  
| `Spec` | `AbilitySpec` | 技能的 Spec |  
| `Owner` | `AbilitySystemCell` | 技能拥有者 |  
| `_timeUnit` | `TimeUnit` | 时间单位 |  
| `_startTime` | `int` | 任务开始时间 |  

### TargetCatcherBase

| 成员 | 类型 | 说明 |  
|------|------|------|  
| `Parameter` | `T` | 泛型参数实例 |  
| `Owner` | `AbilitySystemCell` | 技能拥有者 |  
| `Init(owner)` | `void` | 初始化 |  
| `CatchTargetsNonAllocSafe(mainTarget, results)` | `void` | 安全捕获目标 |  

### ModMagnitudeCalculationBase

| 成员 | 类型 | 说明 |  
|------|------|------|  
| `Parameter` | `T` | 泛型参数实例 |  
| `CalculateMagnitude(context, magnitude)` | `float` | 计算修改器数值（必须实现） |  
  
---  

## JSON 数据格式参考

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

`$type` 的值就是 C# 类名（Bean 名称），与类名完全一致。