# EX-GAS Bean映射规范

## 一、设计意图

EX-GAS采用**代码驱动配置**的设计模式：
- **自定义逻辑类**定义在C#代码中（如 `CueLogging : GameplayCueBase<XParamLogging>`）
- **Luban Bean**定义在 `__beans__.xlsx` 中，用于配置表数据序列化
- **两者必须保持同步**，否则配置表无法正确解析

## 二、映射关系总览

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                           Bean映射规范                                       │
├─────────────────────────────────────────────────────────────────────────────┤
│  C# 自定义类                          Luban Bean                            │
├─────────────────────────────────────────────────────────────────────────────┤
│  CueLogging                           CueLogging : CueLogic                │
│  : GameplayCueBase<XParamLogging>       - Param: XParamLogging             │
│                                        ───────────────────────────────────  │
│  XParamLogging                        XParamLogging : XParam               │
│  : XParam                              - Value: string                      │
│                                        - Duration: float                    │
├─────────────────────────────────────────────────────────────────────────────┤
│  MMCScalableFloat                    MMCScalableFloat : MmcLogic           │
│  : ModMagnitudeCalculationBase<         - Param: XParamMMCScalable         │
│      MmcParaFloatScale>               ───────────────────────────────────  │
│                                       XParamMMCScalable : XParam           │
│  MmcParaFloatScale : XParam            - K: float                          │
│                                        - B: float                          │
├─────────────────────────────────────────────────────────────────────────────┤
│  ALMove                              ALMove : AbilityLogic                  │
│  : AbilityLogicBase<XParamMove>        - Param: XParamMove                 │
│                                       ───────────────────────────────────  │
│  XParamMove                          XParamMove : XParam                   │
│  : XParam                              - (自定义字段)                       │
└─────────────────────────────────────────────────────────────────────────────┘
```

## 三、Bean类型分类

### 3.1 参数Bean（XParam系列）

| 运行时类 | Luban Bean | 用途 |
|---------|------------|------|
| `XParamNone` | `XParamNone : XParam` | 无参数 |
| `XParamInt` | `XParamInt : XParam` | 整数参数 |
| `XParamFloat` | `XParamFloat : XParam` | 浮点数参数 |
| `XParamString` | `XParamString : XParam` | 字符串参数 |
| `XParamLogging` | `XParamLogging : XParam` | 日志参数（Value, Duration） |
| `XParamMMCScalable` | `XParamMMCScalable : XParam` | MMC缩放参数（K, B） |
| 自定义参数类 | `自定义名 : XParam` | 自定义参数 |

### 3.2 逻辑Bean

| 基类 | Bean父类 | 用途 |
|------|---------|------|
| `GameplayCueBase<T>` | `CueLogic` | Cue逻辑 |
| `ModMagnitudeCalculationBase<T>` | `MmcLogic` | MMC逻辑 |
| `AbilityLogicBase<T>` | `AbilityLogic` | Ability逻辑 |
| `AbilityTaskBase<T>` | `AbilityTask` | Ability任务 |
| `TargetCatcherBase<T>` | `TargetCatcher` | 目标捕获器 |

## 四、命名规范

### 4.1 类型命名

| 类别 | 命名规范 | 示例 |
|------|---------|------|
| Cue逻辑类 | 无强制前缀 | `CueLog`, `CueLogging`, `CLCameraFovShake` |
| MMC逻辑类 | 建议以`MMC`开头 | `MMCScalableFloat`, `MMCAttributeBased` |
| Ability逻辑类 | 建议以`AL`开头 | `ALMove`, `ALDeath`, `ALApplyEffect` |
| AbilityTask类 | 建议以`Task`开头 | `TaskPlayCue`, `TaskApplyEffects` |
| 参数类 | 建议以`XParam`开头 | `XParamLogging`, `XParamMove` |

### 4.2 Bean命名

Bean名称与运行时类名**完全一致**：

```
C# 类名              →    Luban Bean名称
CueLogging           →    CueLogging
MMCScalableFloat     →    MMCScalableFloat
XParamLogging        →    XParamLogging
```

## 五、字段映射规则

### 5.1 逻辑Bean字段

所有逻辑Bean都包含一个 `Param` 字段：

```yaml
# CueLogic子类示例
CueLogging:
  parent: CueLogic
  fields:
    - name: Param
      type: XParamLogging  # 对应泛型参数T
      comment: 参数

# MMC逻辑Bean示例
MMCScalableFloat:
  parent: MmcLogic
  fields:
    - name: Param
      type: XParamMMCScalable
      comment: 参数
```

### 5.2 参数Bean字段

参数Bean的字段直接映射自C#类的公共字段和属性：

```csharp
// C# 定义
public class XParamLogging : XParam
{
    public string Value { get; private set; }  // → string Value
    public float Duration;                      // → float Duration
}
```

```yaml
# Luban Bean定义
XParamLogging:
  parent: XParam
  fields:
    - name: Value
      type: string
      comment: 日志内容
    - name: Duration
      type: float
      comment: 持续时间
```

### 5.3 类型映射

| C# 类型 | Luban 类型 |
|--------|-----------|
| `int` | `int` |
| `long` | `long` |
| `float` | `float` |
| `double` | `double` |
| `bool` | `bool` |
| `string` | `string` |
| `Vector2` | `vector2` |
| `Vector3` | `vector3` |
| `Vector4` | `vector4` |
| `T[]` | `array<T>` |
| `List<T>` | `array<T>` |
| 自定义类型 | 类型名 |

## 六、开发流程

### 6.1 创建新的自定义类

1. **创建参数类**（如需要）：
```csharp
// Assets/YourProject/Script/Gas/Param/XParamMyData.cs
public class XParamMyData : XParam
{
    public float Value;
    public string Name;

#if UNITY_EDITOR
    public void DecodeExcelData(List<object> paramData) { ... }
    public List<object> EncodeExcelData() { ... }
#endif
}
```

2. **创建逻辑类**：
```csharp
// Assets/YourProject/Script/Gas/Cue/CueMyEffect.cs
public class CueMyEffect : GameplayCueBase<XParamMyData>
{
    public override void OnActivate(float time)
    {
        Debug.Log($"Effect: {Parameter.Name}, Value: {Parameter.Value}");
    }

    public override void Reset() { }
}
```

3. **更新Bean定义**：
```
菜单: EXTool/EX-GAS/生成脚本/更新Bean定义
```

4. **生成运行时代码**：
```
菜单: EXTool/EX-GAS/生成脚本/生成所有
```

5. **重新导出配置表**：
```
菜单: EXTool/EX-GAS/生成脚本/GAS表配置
```

### 6.2 自动化工作流

```
创建自定义类 → 更新Bean定义 → 生成运行时代码 → 导出配置表
      ↓              ↓              ↓              ↓
  [编码]       [BeanUpdater]   [CodeGenerator]  [Luban]
```

## 七、JSON数据格式

配置表JSON使用 `$type` 字段进行多态识别：

```json
{
  "ID": 1001,
  "Name": "测试Cue",
  "CueLogic": {
    "$type": "CueLogging",
    "Param": {
      "Value": "Hello World",
      "Duration": 2.5
    }
  }
}
```

## 八、常见问题

### Q1: 为什么配置表解析失败？
检查：
1. Bean定义是否与自定义类同步
2. 参数类型是否正确注册
3. 字段名是否完全匹配

### Q2: 如何添加新的参数类型？
1. 创建继承自 `XParam` 的类
2. 实现 `DecodeExcelData` 和 `EncodeExcelData` 方法
3. 运行 `更新Bean定义`

### Q3: Bean定义更新后还需要做什么？
1. 运行Luban导表工具生成新的Bean类
2. 重新生成运行时映射代码（`XCue.gen.cs` 等）
3. 重新导出配置表JSON

## 九、相关文件

| 文件 | 用途 |
|------|------|
| `__beans__.xlsx` | Luban Bean定义 |
| `XCue.gen.cs` | Cue运行时映射 |
| `XMmc.gen.cs` | MMC运行时映射 |
| `XAbility.gen.cs` | Ability运行时映射 |
| `BeanUpdater.cs` | Bean自动更新工具 |
| `CodeGenerator.cs` | 代码生成工具 |