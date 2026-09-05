# EX-GAS 2.0 项目记忆（PROJECT MEMORY）

> 本文件是面向 AI Agent / 新开发者快速上手的项目记忆。
> 权威文档入口：`README.md`（系统设计与配置说明）、`BeanMappingSpec.md`（Bean 映射规范）、`DemoFrameworkIntroduction.md`（Demo 框架说明）。
> 常规代码约定见根目录 `AGENTS.md`。

---

## 1. 项目概述

- **项目**：EX-GAS 2.0（EX Gameplay Ability System For Unity）—— 对 Unreal Engine GAS（Gameplay Ability System）的 Unity 实现。
- **一句话概括**：**WHO DO WHAT** —— WHO=`AbilitySystemCell`(ASC，运行单元)，DO=`Ability`(能力/技能)，WHAT=`GameplayEffect`(GE，属性修改的唯一途径)。
- **本质**：一套以 ECS/DOTS 为底座的属性数值管理系统；GameplayCue 是附加的表现层价值。
- **开源**：完全开源，可二次开发；反馈 QQ 群 616570103。
- **当前分支**：`EX-GAS-2.0`（仓库还有 `EX-GAS-1.0`、`EX-GAS-2Beta`、`ECS_Version`、`develop` 等分支；默认不切换到旧分支开发）。
- **UPM 版本**：2.0.1（`Assets/GAS` 目录即 UPM 包体，可通过 UPM git 方式导入）。

## 2. 技术栈与版本要求

| 项 | 要求 |
|---|---|
| Unity | 2022.3 LTS（本仓库 `ProjectVersion.txt` 为 2022.3.16f1） |
| DOTS/ECS | `com.unity.entities` **1.2.3**（README 强调前后版本不要差距太大；Entities API 在 Unity6 前基本稳定） |
| 配置工作流 | Luban（Excel → JSON），`EX_GAS_Config/ProjectConfigTable/exgas_config` 为配置工程 |
| 编辑器依赖 | Odin Inspector 3.2+（付费，GASCenter / GASWatcher 依赖） |
| 其他包 | UniTask、InputSystem、Timeline、URP 14、ProBuilder、YooAsset、LoxodonFramework（EXUI）、Newtonsoft.Json、EPPlus（编辑器读 Excel）等，见 `Packages/manifest.json` |

## 3. 目录结构

```
Assets/
├── GAS/                      ← UPM 包体（核心，分 Runtime / Editor / General / Wiki）
│   ├── Runtime/              ← 运行时（ECS/DOTS）
│   │   ├── Ability/          ← 能力系统：AbilityConfig / AbilityController / AbilitySpec / AbilityHelper / AbilityUtil
│   │   │   ├── AbilityTask/  ← AbilityTaskBase + CommonTask（TaskApplyEffects/TaskDoCost/TaskDoCooldown/TaskPlayCue/TaskDebug/TaskDoNothing）
│   │   │   ├── Component/    ← 静态组件 C*（CAbilityBaseInfo/CAbilityCost/CAbilityCooldown/Tag 类组件等）
│   │   │   │   └── AbilityLogic/ ← AbilityLogicBase + 内置 ALApplyEffect/ALDebugLog
│   │   │   ├── ComponentConfig/  ← AbilityComponentConfig（配置 → Entity 组件装配）
│   │   │   ├── TargetCatcher/    ← 目标捕获（CatchSelf/CatchTarget/CatchAreaBox3D/Box2D/Circle2D）
│   │   │   └── TimelineAbility/  ← ALTimeline / ALTimelinePlayer / Data（Track、TaskClipData、XParamTimeline）
│   │   ├── AbilitySystemCell/← ASC：AbilitySystemCell（OOP 门面）/ AbilitySystemComponent / BasicDataController / Config
│   │   ├── Attribute/        ← CAttributeData / CAttributeIsDirty（属性数据组件）
│   │   ├── AttributeSet/     ← AttrSetController（属性集控制器）
│   │   ├── Cue/              ← GameplayCueUnit + Base/GameplayCueBase<T> + Common/（CueLog/CuePlayAnimator/CuePlaySound/CueMountPrefab）+ Component/（MCCue、ECCuePlayable/ECCuePlaying）
│   │   ├── Effect/           ← GameplayEffectConfig / GameplayEffectController / GameplayEffectSpec
│   │   │   ├── Component/    ← Static C*（CDuration/CPeriod/CStacking/MCModifiers/MCGrantedAbility/CCueOn*/Tag 组件等）
│   │   │   ├── Modifier/     ← ModMagnitudeCalculationBase + MMCConfig + 内置 MMC（MMCNone/MMCScalableFloat/MMCAttributeBased）
│   │   │   ├── Aspect/       ← AspModifyBaseValue
│   │   │   └── Enum/         ← StackingType / DurationRefreshPolicy / GrantedAbility*Policy 等
│   │   ├── General/          ← GASManager（核心）/ GASEventCenter（事件）/ TurnController / TimeUnit / XParam/ / Helper/
│   │   ├── System/           ← ECS 系统：Ability（STryActivate*）/ Attribute / Cue / GameplayEffect（Create/Destroy/Operation/Running）/ SystemGroup（SGLogic/SGAbility/SGAttribute/SGEffect/SysGrpDisplay）/ Core（SGlobalTimer）
│   │   └── Tag/              ← GameplayTag / GameplayTagController / TagRequirementData / Component（BFixedTag/BTemporaryTag）
│   ├── Editor/               ← 编辑器工具
│   │   ├── GASCenterEditor/  ← GAS 中心管理器（GASCenterWindow + GASCenterView* 各配置页，EPPlus.dll 读写 Excel）
│   │   ├── CodeGen/          ← BeanUpdater（更新 __beans__.xlsx）/ CodeGenerator(+AbilityPart/TagPart/LubanPart)
│   │   ├── Ability/AbilityTimelineEditor/ ← 时间轴技能编辑器
│   │   ├── GameplayAbilitySystem/ ← GASSettingAsset（路径设置）
│   │   ├── WebEditor/        ← Python 后端 + HTML/CSS/JS 前端（Tag/Attribute/AttributeSet/ASC/Effect 5 个网页编辑器）
│   │   ├── General/ Helper/ LubanConfigTemplate/
│   ├── General/              ← 跨 Runtime/Editor 共享：GASConstDefine / GASResourceLoader / GASTimer / Util/（ReflectionHelper、Pool、TypeUtil 等）/ DataClass/
│   └── Wiki/                 ← EX-GAS.md / Ability.md / GameplayCue.md / GameplayEffect.md / MMC.md
├── DemoForESC/               ← Demo（Scene / _Script：DemoLauncher、GameManager、EventCenter、ALMove/ALDeath、Cue 类；_Script/Gen 下是生成的 X*.gen.cs 与 LubanTable 类）
├── Framework/                ← Demo 关卡框架（Core/GameEntry、GameEventBus；Level/；Unit/UnitBase；Input/PlayerController），见 DemoFrameworkIntroduction.md
├── EXUI/ _EXProceduralMachine/ Behavior Designer/ Plugins/ Resources/ StreamingAssets/ XYooAsset/
│   └── _EXProceduralMachine/  ← 自研程序化动画工具目录（程序集 EXProceduralMachine，命名空间 EXProceduralMachine，自研 TwoBoneIK 替代 FinalIK/AnimationRigging，零第三方 IK 依赖）：
│        Core/（Manager、EXMachHelper、SyncBodyHeight） IK/（TwoBoneIK） Locomotion/（BaseMultiLeggedLocomotion、FootMotionGroup、FootPlacement、FootConfig、FourLegsSpiderLocomotion） Rhythm/（RhythmSystem、RhythmCycle、RhythmPresets、RhythmController） VisualAid/（XVisualAid、XVisualLine） Weapon/（Gun）
└── _Test/                    ← 简易测试脚本（CueUnitTest/LubanTest/TestConfigAsset/TimelineEditor 等，非标准 UTF asmdef 测试）

EX_GAS_Config/
└── ProjectConfigTable/
    ├── exgas_config/         ← Luban 配置工程（Datas/ 9 张 xlsx + __beans__/__enums__/__tables__；Defines/builtin.xml；Tools/；gen.bat、gen.sh、luban.conf）
    ├── config/               ← 空目录（历史残留？）
    └── output/               ← 空目录（历史残留？）
```

## 4. 核心架构（阅读代码前必读）

### 4.1 ECS 世界与系统拓扑（`GASManager`）

- `GASManager`（静态类）是唯一入口：`Initialize()` 创建名为 `EX_GAS_World` 的专用 World，并把 World 追加到 PlayerLoop（`ScriptBehaviourUpdateOrder.AppendWorldToCurrentPlayerLoop`）。
- 系统拓扑（README 3.1 + GASManager.cs）：
  ```
  InitializationSystemGroup
  SimulationSystemGroup
  ├── FixedStepSimulationSystemGroup（RateManager = FixedRateSimpleManager(Time.fixedDeltaTime)）
  │   └── SGLogic
  │       ├── SGlobalTimer（逻辑帧计时：Frame / Turn / Time）
  │       ├── SGAbility   → STryActivateAbility / STryCancelAbility / STryEndAbility / SAbilityTick
  │       ├── SGAttribute → SUpdateAttributeCurrentValue
  │       └── SGEffect    → SGEffectCreate(SGInstantiateEffect→SInstantiateEffect)
  │                        → SGEffectOperation(SGCheckApplyEffect→SGApplyEffect→SGCheckActivateEffect→SGActivateEffect→SGDeactivateEffect→SGRemoveEffect)
  │                        → SGEffectDestroy(SDestroyEffects)
  │                        → SGEffectTick(SGRunningEffect: SEffectDurationTick/SEffectPeriodTick/SEffectStackingTick)
  └── SysGrpDisplay（表现层，挂 SimulationSystemGroup 下）
      └── SCueStart / SCueTick / SCueEnd / SCueDestroy
  PresentationSystemGroup
  ```
- 其余 API：`Run()` / `Stop()` 控制 `IsRunning`；`GetGlobalTimer()` / `CurrentFrame` / `CurrentTurn`；`TurnController`（回合制）；`BindAscToEntity` / `GetAscFromEntity` / `UnbindAscToEntity`（Entity ↔ ASC OOP 对象字典绑定）。

### 4.2 ASC（AbilitySystemCell）—— OOP 门面

- `AbilitySystemCell` 是外部干涉 GAS 的唯一入口，public API 不暴露 ECS 类型。
- 一个 ASC = 一个 ECS Entity（命名 `ASC_V{Version}_{Index}`），内部由 5 个 Controller 分工：
  - `BasicDataController`（等级等，CAscBasicData）
  - `AttrSetController`（属性集/属性读写，GetAttrCurrentValue 等）
  - `GameplayTagController`（BFixedTag 固有标签 / BTemporaryTag 临时标签）
  - `GameplayEffectController`（BGameplayEffect buffer，Apply/Remove/Clear）
  - `AbilityController`（BAbility buffer + `Dictionary<int, AbilitySpec>`）
- 常用 API：`ApplyGameplayEffectTo(spec, target)` / `ApplyGameplayEffectToSelf`、`TryActivateAbility(code, XParam)`、`TryEndAbility` / `TryCancelAbility`、`HasTag/HasAllTags/HasAnyTags`、`AddFixedTag/KillFixedTag`、`GetAttrCurrentValue/GetAttrBaseValue/SetAttrBaseValue`、`SetLevel`。
- `AbilitySpec` / `GameplayEffectSpec`：Entity 的 OOP 包装（门面无 ECS，内部随便用）。**注意**：GE 的组件增删应在 Apply 之前完成，Apply 后动态增删组件可能导致 ECS System 异常（数据修改 Set* 任何时候安全）。

### 4.3 ECS 命名约定（AGENTS.md + 实际代码）

| 前缀 | 含义 | 例子 |
|---|---|---|
| `C*` | IComponentData 组件 | CAbilityBaseInfo、CDuration、CStacking |
| `B*` | IBufferElementData 缓冲元素 | BAbility、BGameplayEffect、BFixedTag、BTemporaryTag |
| `S*` | ISystem | STryActivateAbility、SInstantiateEffect |
| `MC*` | Managed Component（存 OOP 对象） | MCAbilityLogic（存 AbilityLogicBase）、MCModifiers、MCGrantedAbility、MCCue |
| `EC*` | Enableable Component | ECCuePlayable、ECCuePlaying、ECKillCue |
| `Wip*` | 流程中间态组件 | WipApplyEffect、WipActivateEffect、WipRemoveEffect… |
| `Conf*` | 配置加载类 | ConfCueBase |
| `Asp*` | Aspect | AspModifyBaseValue |
| `SG*` | SystemGroup | SGLogic、SGAbility、SGEffect |

### 4.4 配置驱动：Excel → Luban → JSON → 生成 C#

- 配置表（`exgas_config/Datas/`）：`#exgas.gameplayTags / attribute / attributeSet / gameplayEffect / ability / gameplayCue / mmc / asc / timelineAbility .xlsx` + `__beans__.xlsx`（Bean 定义）+ `__enums__.xlsx` + `__tables__.xlsx`。
- 导出：`gen.bat`（Windows）/ `gen.sh`（macOS/Linux）→ JSON（默认输出 `Assets/DataGenerated/Luban/Json/GAS`）。
- 代码生成：`EXTool → EX-GAS → 生成脚本`（GAS 中心管理器的"一键生成所有"，或 CodeGenerator 菜单）。生成物：`XTag.gen.cs` / `XAttribute.gen.cs` / `XAttrSet.gen.cs` / `XAbility.gen.cs` / `XCue.gen.cs` / `XMmc.gen.cs` / `XLuban.gen.cs`（运行时配置加载，如 `XLuban.GetAscConfig(id)`）。Demo 中见 `Assets/DemoForESC/_Script/Gen/`。
- 流式配置：Ability / Cue / MMC 配置表自定义参数占连续 50 列，通过 `XParam` 子类的 `EncodeExcelData()` / `DecodeExcelData()` 互转；空值必须用默认占位（0/""/0f/false），顺序必须与解析一致。
- **Bean 映射**（详见 `BeanMappingSpec.md`）：`[BeanField]`（绑定 Setter，Order 默认取源码行号）与 `[BeanPolymorphicField]`（绑定 TypeSetter + ParamSetter + ParamTypeResolver）标注字段；`BeanUpdater` 扫描 6 类继承体系（`XParam`、`GameplayCueBase<T>`、`ModMagnitudeCalculationBase<T>`、`AbilityLogicBase<T>`、`AbilityTaskBase<T>`、`TargetCatcherBase<T>`）自动更新 `__beans__.xlsx`。菜单：`EXTool/EX-GAS/生成脚本/更新Bean定义`。

### 4.5 GameplayTag（标签系统）

- 树形层级（`Parent.Child.Grandchild`），替代布尔/枚举做状态判断。
- ASC 上分固有标签（Fixed，BFixedTag）与临时标签（Temporary，BTemporaryTag，带来源 Entity）。
- `TagRequirementData { all/any/none }` 三模式统一条件（2026-03 更新后，GE/Ability/Cue 的条件 Tag 组件统一为该结构）。旧字段默认映射：`ApplicationRequiredTags`/`OngoingRequiredTags`/`ActivationRequiredTags`/`RequiredTags` → `all`；`RemoveGameplayEffectsWithTags`/`ImmunityTags` → `any`；`ActivationBlockedTags`/`Cue.ImmunityTags` → `none`。
- 层级判断在 `TagHelper`（Runtime/General/Helper）。

### 4.6 GameplayEffect（GE）

- GE 是属性修改唯一途径；`GameplayEffectSpec` 为 OOP 门面。
- 组件化设计：`CDuration`（-1 无限，Frame/Turn 计时）、`CPeriod`（周期触发）、`CStacking`（叠加：BySource/ByTarget、刷新/过期策略）、`MCModifiers`（属性修改器列表）、`MCGrantedAbility`（授予技能 + 激活/移除策略）、`CCueOnApply/Add/Remove/Activate/Deactivate/Tick`、Tag 组件（`CApplicationRequiredTags`/`COngoingRequiredTags`/`CEffectImmunityTags`/`CRemoveEffectWithTags`）等。
- 类型：Instant（立即执行 Modifier，SExecuteInstantEffectModifiers）与 Durational/Infinite（入 ASC buff 列表，检查 Stacking，激活后持续 Tick）。
- MMC（Modifier Magnitude Calculation）：`ModMagnitudeCalculationBase` / 泛型 `ModMagnitudeCalculationBase<T>`；内置 `MMCNone`（直接用模值）、`MMCScalableFloat`（Magnitude×k+b）、`MMCAttributeBased`（基于属性，AttributeFromType=Source/Target，CaptureType=Track/SnapShot）；`SetByCaller`（W.I.P）。自定义：继承泛型基类重写 `CalculateMagnitude(Entity geEntity, float magnitude)`。
- 生命周期事件：GE 容器 dirty（`GASEventCenter`）、属性变化 Before/After 事件。

### 4.7 Ability

- 配置字段：Cost（消耗 GE）、CdEffect（冷却 GE）、Cd（覆盖持续时间）、AssetTags、CancelAbilityWithTags、BlockAbilityWithTags、ActivationOwnedTags、ActivationRequiredTags、ActivationBlockedTags、AbilityLogic（逻辑类型名）+ 后续 50 列自定义参数。
- `AbilitySpec` 门面：`TryActivate()`（打 `CAbilityInTryActivate` 标记，下一帧由 System 处理）、`TryEnd()` / `TryCancel()` 同理；`CanActivate` / `CheckActivation()`（Tag/Cost/CD 综合检查，返回 `AbilityActivationResult`）；`GetLogic<T>()` 取 `AbilityLogicBase`。
- `AbilityLogicBase<T>`：技能逻辑基类；内置 `ALApplyEffect`（对 TargetCatcher 捕获的目标施加 GE）、`ALDebugLog`、`ALTimeline`。
- `AbilityTaskBase<T>`：Task 生命周期 `Begin()/Tick()/Finish()`；内置 TaskApplyEffects/TaskDoCost/TaskDoCooldown/TaskPlayCue/TaskDebug/TaskDoNothing。
- `TargetCatcherBase<T>`：目标捕获多态系统（`CatchTargetsNonAlloc` 抽象实现；推荐 `CatchTargetsNonAllocSafe` 无 GC，`CatchTargets` 已 `[Obsolete]`）。内置 CatchSelf/CatchTarget/CatchAreaBox3D/CatchAreaBox2D/CatchAreaCircle2D；自定义需在 `TargetCatcherHelper` 注册（生成代码 `XAbility.gen.cs` 自动注册）。
- **TimelineAbility**：`ALTimeline : AbilityLogicBase<XParamALTimelineID>` + `ALTimelinePlayer`（帧驱动 Track/TaskClip）；编辑器 `AbilityTimelineEditor` 支持可视化编辑与预览。

### 4.8 GameplayCue（表现层，与逻辑解耦）

- 只负责表现（特效/音效/动画/UI），**禁止**修改属性/施加 GE/影响判定（边界由游戏类型决定）。
- OOP 层：`GameplayCueUnit`（控制单元）+ `GameplayCueBase<T>`（自定义逻辑基类，`Parameter` 强类型）。
- ECS 层：`MCCue`（存单元）+ Enable Component 模式（`ECCuePlayable`/`ECCuePlaying`/`ECKillCue`）实现播放/停止切换，避免频繁创建销毁；系统：SCueStart/SCueTick/SCueEnd/SCueDestroy（SysGrpDisplay，表现层）。
- 生命周期回调：OnAdd / OnActivate / OnTick / OnDeactivate / OnRemove / OnDestroy（推荐在 OnAdd 缓存组件引用）。
- 标签过滤：RequiredTags(all) / ImmunityTags(none)（底层 TagRequirementData）。
- 可在 GE 各阶段自动触发（CueOnApply/Add/Remove/Activate/Deactivate/Tick），也可在 Ability 中手动控制，甚至脱离 GAS 独立使用。

### 4.9 事件系统（GASEventCenter）

静态事件中心（Runtime/General/GASEventCenter.cs），属性变化（BaseValueChangeBefore/After、CurrentValueChangeAfter）、GE 容器 dirty、Tag dirty 等；按 `(Entity, attrSetCode, attrCode)` 注册/触发。

### 4.10 全局计时

`GlobalTimer`（Singleton ECS 组件，SGlobalTimer 驱动）：`Frame`（逻辑帧）/ `Turn`（回合）/ `Time`（逻辑秒）。`TimeUnit`：Frame / Turn。

## 5. 编辑器与工具链

| 工具 | 入口 | 说明 |
|---|---|---|
| GAS 中心管理器 | `EXTool → EX-GAS → GAS中心管理器` | 核心可视化配置工具（Tag/Attribute/AttrSet/Cue/MMC/GE/Ability/ASC/Setting 9 页），读写 Excel + 导出 JSON + 生成 C# 代码；使用 EPPlus.dll 直接读写 xlsx |
| Web 编辑器 | `EXTool/EX-GAS/Web编辑器` | Python（server.py）+ HTML/CSS/JS 的 5 个网页编辑器（Tag/Attribute/AttributeSet/ASC/Effect），首次需"一键部署编辑器环境"（install_deps.bat）；启动 start.bat |
| 监测台 GASWatcher | `EXTool/EX-GAS/监测台`（热键 Ctrl+F11，需 `EX_GAS_ENABLE_HOT_KEYS`） | 运行时 ASC 监控（属性/标签/能力/GE），100ms 轮询，Odin UI，仅播放模式可用 |
| 代码生成 | `EXTool/EX-GAS/生成脚本/…` | GAS表配置（调 gen.bat + 生成）、更新Bean定义（BeanUpdater）等 |
| 导入模板 | `EXTool/EX-GAS/导入模板Luban配置目录` | 一键部署 Luban 配置工程模板 |

**GASSettingAsset**（`ScriptableSingleton`，路径见 Assets/GAS/Editor/GameplayAbilitySystem/GASSettingAsset.cs）：
- `ConfigProjectPath`（默认 `EX_GAS_Config/ProjectConfigTable/exgas_config`）
- `TableOutpuPath`（默认 `Assets/DataGenerated/Luban/Json/GAS`）
- `TableClassCodeOutpuPath`（默认 `Assets/DataGenerated/Luban/CSharp`；注意：Luban 类生成会清空整个输出目录，勿与其他文件混放）
- `CodeGeneratePath`（默认 `Assets/Scripts/Gen`）

## 6. 构建 / 测试 / 常用命令

- 打开项目：Unity **2022.3.16f1**（见 `ProjectSettings/ProjectVersion.txt`）。
- 重新生成配置 JSON：
  - Windows：`EX_GAS_Config\ProjectConfigTable\exgas_config\gen.bat`
  - macOS/Linux：`bash EX_GAS_Config/ProjectConfigTable/exgas_config/gen.sh`
- EditMode 测试：`Unity.exe -batchmode -quit -projectPath . -runTests -testPlatform EditMode -testResults TestResults/EditMode.xml`
- PlayMode 测试：`Unity.exe -batchmode -quit -projectPath . -runTests -testPlatform PlayMode -testResults TestResults/PlayMode.xml`
- 不要提交生成缓存/构建目录：`Library/`、`Temp/`、`Logs/`、`obj/`（以及 WebEditor 的 `__pycache__`）。

## 7. 代码与提交约定

- C#：4 空格缩进、大括号换行（与现有文件一致）。
- 命名空间：`GAS.Runtime` / `GAS.Editor` / `GAS.General`（按目录意图）。
- 类型命名：`C*` 组件 / `B*` buffer / `S*` 系统 / `MC*` managed component / `Conf*` 配置加载（见 4.3）。
- 保留 `.meta` 文件。
- OOP 门面类（ASC / AbilitySpec / GameplayEffectSpec）public API 不得暴露 ECS 类型。
- 提交信息：`<scope>: <imperative summary>`，中英文均可（例：`runtime: fix effect disposal`、`docs: update GAS workflow`）；PR 需含改动原因、影响路径、验证证据。
- 测试命名：`XxxTests`；项目测试放在 `Assets/_Test` 或专用 test asmdef，避免与插件厂商测试混放（当前 `Assets/_Test` 多为简易验证脚本，非标准 UTF 结构）。

## 8. 已知问题 / 注意事项（Agent 开发时务必留意）

1. **`EX_GAS_Config` 异常目录**：`ProjectConfigTable/config` 与 `ProjectConfigTable/output` 为空目录；`exgas_config/Datas/` 与 `exgas_config/Defines/` 下存在名为 `...` 的目录。`git status` 会因此报错：`could not open directory 'EX_GAS_Config/ProjectConfigTable/config/.../.../Assets/'`。这些可能是历史残留或符号链接问题，**改动前先确认用途，不要贸然删除**；涉及 config 路径的工具链需留意。
2. **`CODEX_TASK_TODO.json`**（2026-04-01 生成）：记录了 8 项清理任务（停用 SCheckApplicationCondition 注册、收敛 GameplayEffectSpec 重复 API、删运行期调试日志、清理 GASConstDefine 未引用常量、重命名 Immunity 常量、抽取 Effect 编辑公共协议层、更新 Wiki 术语、回归检查）。git log 显示部分任务可能已完成（如 `refactor: consolidate effect helper logic`、`effect: unify tag requirement naming and schema`），**状态可能过期，执行前需核对**。
3. **暂不支持的功能**（README 第 6 节）：RPC 相关 GE 复制广播；GameplayEffect Execution（目前只有 Modifier）；Ability 触发判断用的 Source/Target Tag（暂不生效）；GE 过期时触发的效果。后续计划：3.0 支持 RPC 网络同步。
4. **Entities 版本**：要求 `com.unity.entities` 1.2.3，官方对 DOTS API 变动频繁，升级前需谨慎评估。
5. **生成代码勿手改**：`X*.gen.cs`、`XLuban.gen.cs`、LubanTable 下的类均为自动生成，应通过"改 Excel → 导出 JSON → 生成代码"流程更新。
6. **GE 组件增删时机**：应在 Apply 之前完成组件 Add/Remove，Apply 后动态增删可能导致 ECS System 运行异常；数据 Set* 方法任何时候安全。
7. **两套编辑器**：GASCenter（Unity IMGUI/UI Toolkit）与 WebEditor（Python）读写同一批 Excel，存在双份读写逻辑（CODEX_TASK_TODO TASK-006 计划抽取公共协议层收敛），改动时需保持两边一致。
8. **流式配置**：XParam 子类必须正确实现 Encode/DecodeExcelData，空值用默认占位，列序与解析顺序一致，否则配置表解析错误。
9. **TargetCatcher**：优先使用 `CatchTargetsNonAllocSafe`（无 GC 分配）；`CatchTargets` 已标记 `[Obsolete]`。

## 9. AI Agent 上手建议

1. 先读 `README.md`（尤其 2.x 系统介绍 + 3.x API 章节）、`BeanMappingSpec.md`、`DemoFrameworkIntroduction.md`。
2. 改运行时逻辑前，先看目标模块的 Component（静态 C*/动态 Wip*）与对应 System（System/GameplayEffect/Operation/…），再决定改组件、改系统还是改 Helper。
3. 新增自定义逻辑类（AbilityLogic/Cue/MMC/AbilityTask/TargetCatcher/XParam）后，运行 BeanUpdater 更新 `__beans__.xlsx`，再导出 JSON、生成代码。
4. 调试时可用 GASWatcher 监测台查看 ASC 运行时状态；Editor 日志关注 `GASManager.Initialize()` 是否已调用（Demo 中由 GameEntry/DemoLauncher 负责）。
5. 提交前自查：是否生成物被误提交、.meta 是否缺失、是否遵循命名约定。
