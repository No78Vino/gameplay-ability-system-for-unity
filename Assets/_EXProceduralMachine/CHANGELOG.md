# CHANGELOG

## [2.6.0] - 程序化动画工具箱浏览器（三层体系 + 预设 + 网页端数据源）

### 新增（Features）

- **工具箱浏览器升级**：`EXMachTemplateWindow` 重构为三层体系浏览器：
  - 🧰 **工具**（基础系统）：`ToolboxCatalog.BuildTools()` 内置定义——次级运动、呼吸/节奏、命中枪械、可视化辅助；
    点「实例化」创建空物体并挂载组件（默认参数开箱即用）；
  - 🥘 **预制菜**（模板）：自动扫描 `Examples/`，蜘蛛/无人机模板，实例化即用；
  - 🧸 **玩具**（Gameplay 小系统）：预留分类（震屏/受击抖动/相机跟随规划中）。
- **搜索 + 双重筛选**：🔍 关键词搜索（名称/描述/组件/路径）+ 分类筛选 + 家族筛选
  （跟随/摆动/回弹/步态/呼吸/调试，对齐全家调参体系）。
- **预设系统**：`ToolboxPreset`（参数覆写集合）+ `ToolboxParamOverride`（组件路径/类型/字段/值）。
  无人机模板带 3 个预设：「轻盈巡航（默认）/ 重载手感 / 轻快跟手」，实例化时一键应用
  （值解析支持 float/int/bool/Vector3/enum/string）。
- **清单导出（网页端数据源）**：底部按钮导出 `toolbox-manifest.json`——
  DTO 化（分类/家族为字符串），网页端零解析成本即可消费；同时支持从 JSON 回读（`ImportJson`）。

### 架构（Architecture）

- 新增 `Editor/Toolbox/`：
  - `ToolboxModels.cs`：数据模型（Item/Preset/ParamOverride/Manifest + 导出 DTO）；
  - `ToolboxCatalog.cs`：目录构建（内置工具 + 预制菜扫描 + 预设注入）、JSON 导入导出、
    实例化辅助（Tool 建物体 / Kit 实例化 + 预设覆写）、值类型解析器。
- 菜单更新：`Tools/EXMach/程序化动画模板列表` → **`Tools/EXMach/程序化动画工具箱`**。

### 修复（Fixes）

- `SecondOrderDynamicsComponentEditor.OnEnable` 隐藏基类虚方法 → 改为 `override`（消除 CS0114 警告）。

### 验证

- Roslyn 双重语义编译：编辑器语义 26 源 EXIT 0；Player 语义 22 源 EXIT 0。
- 预设覆写链路自检：Drone 预设覆写 `DroneFlyDriver.drag/hoverBobAmplitude/hoverBobFrequency`（根节点组件，路径解析正确）。

---

## [2.5.0] - SecondaryMotion 稳健性 + 能力增强（P0/P1/P2）

### 修复（Fixes）—— P0 稳健性

- **NaN/Infinity 防护**（`SecondOrderDynamics.Update`）：`deltaTime<=0` 或输入含非有限值时跳过本帧更新，
  状态一旦被 NaN 污染将无法自愈——从根上阻断；
- **启动突跳消除**：组件 `Awake` 时默认用**自身 Transform 当前值**初始化动力学状态
  （`initializeFromCurrentTransform`），不会再出现"从原点飞到目标"的首帧瞬移；
- **唤醒突跳消除**：组件 `OnEnable` 时重置状态（`resetOnEnable`），物体禁用期间状态冻结导致的
  重新启用大幅位移问题修复；
- **参数脱节静默错误**：组件 `Awake` 时对所有实例执行一次参数同步
  （`UpdateDynamicsFactors`），即使未勾 `autoUpdate`，反序列化后 Inspector 参数与内核强制一致。

### 新增（Features）—— P1 能力

- **时间模式 `timeMode`**：`ScaledDeltaTime`（默认）/ `UnscaledDeltaTime`（暂停时惯性延续）/
  `FixedDeltaTime`（物理帧同步，走 `FixedUpdate`）；
- **`QuaternionRotation` 旋转模式**：四元数对数空间二阶系统（`QuaternionLog/Exp`），
  避免万向锁与欧拉角非最短路径；逐轴遮罩在对数空间生效；适合小幅多轴旋转（大角度连续偏航仍建议欧拉模式）；
- **`Custom` 类型实现**：`customInput`（`Func<Vector3>`）输入 + `customOutput`
  （`Action<Vector3, Transform>`）回写委托（`[NonSerialized]`），纯代码驱动场景可用。
- **`IsSettled` 稳态判定**：内核级 `IsSettled(posTol, velTol)` + 实例便捷方法，
  便于外部系统感知"回弹完成"时机。

### 架构优化（Refactor）—— P2

- **Update 重构**：替身/非替身 × 类型 的巨型 switch 拆分为 `TickInstance` → 按类型分派
  （`TickPosition/TickRotationEuler/TickRotationQuaternion/TickScale/TickCustom`），消除重复；
- **运行时 Gizmos**：组件 `gizmosOn` 在 Scene 中绘制替身目标点（绿）与输出点连线（黄）、
  旋转朝向线（蓝），运行时调试不再依赖 Inspector 曲线；
- **API 明确**：`Configure`（仅更新参数不重置状态）与 `Set`（参数+重置）职责分离。

### 兼容性

- `SecondOrderDynamicInstance` 字段名**保持 `avator` 原名**（未做破坏性改名），
  DroneMachine.prefab 序列化**完全兼容**，无需改动；
- `SecondOrderDynamicValueType` 新增 `QuaternionRotation`（值 4，追加在末尾），
  已有枚举值的序列化 int（0/1/2/3）不受影响；
- 组件新增字段（timeMode/resetOnEnable/initializeFromCurrentTransform/gizmosOn）prefab 中缺失时
  全部走默认值，行为与 2.4.x 一致。

### 验证

- Roslyn 双重语义编译：编辑器语义 24 源 EXIT 0；Player 语义 22 源 EXIT 0。
- prefab 引用校验：两个脚本 GUID 正确、实例字段匹配、`m_Children` 无两行式。

---

## [2.4.2] - 无人机新增悬停漂浮浮动

### 新增（Features）

- **悬停漂浮** `DroneFlyDriver`：悬停时上下轻微浮动（默认幅度 0.08m / 频率 1.2Hz），
  可直接开关（`hoverBobOn`）；浮动直接叠加在高度上、只经 Body 二级系统柔化一次，
  避免被高度恢复低通二次衰减导致效果过弱。

### 验证

- Roslyn 双重语义编译：编辑器语义 24 源 EXIT 0；Player 语义 22 源 EXIT 0。
- prefab 序列化新增字段 `hoverBobOn` / `hoverBobAmplitude` / `hoverBobFrequency`。

---

## [2.4.1] - 修复无人机方向切换时的欧拉角回绕翻转

### 修复（Fixes）

- **无人机方向切换剧烈翻转（根因：欧拉角回绕）**：
  无人机前进/后退、左/右方向切换时，机身会翻转一大圈。
  根因——`SecondOrderDynamicsComponent` 的 Rotation 分支把 `target.localEulerAngles`
  （Unity 返回 **[0°,360°)** 范围的欧拉角）直接当作普通标量喂给二阶动力学做数值平滑；
  方向切换时姿态变号（如 pitch `-18°` 存为 `342`，切到 `+18°` 存为 `18`），
  数值跳变 **±324°** 被二阶系统误判为长距离运动 → 输出沿大弧旋转。
- **修复方式**：Rotation 分支的输入统一做**角度回绕规范化**（`WrapEuler` 映射到 `[-180°,180°)`），
  使 350°↔10° 这类相邻角度恢复真实数值差，二阶系统按短路径平滑；
  替身/非替身两条路径均修复；`SecondOrderDynamics` 新增只读属性 `CurrentInput` 供组件规范化。
- 设计文档/报告中的「姿态欧拉角回绕」已知限制已解除。

### 验证

- Roslyn 双重语义编译：编辑器语义 24 源 EXIT 0；Player 语义（无 UNITY_EDITOR/无 UnityEditor 引用）22 源 EXIT 0。

---

## [2.4.0] - 次级运动系统内建化 + 飞行无人机模板

### 迁移（Moved）

- **次级运动系统内建**：`Assets/Plugins/ExOpenSource/ValueSecondOrderSystem/`（二阶动力学模拟器）
  整体迁移至 `SecondaryMotion/`，纳入 `EXProceduralMachine` 程序集与命名空间（原 `EXToyLib`）。
- **清理**：删除遗留注释代码；`SecondOrderDynamicInstance.cs` 的 `using UnityEditor;`
  包进 `#if UNITY_EDITOR`（Runtime 程序集 + Player 构建双向编译安全）。
- **Editor 并入**：`SecondOrderDynamicsComponentEditor.cs`（曲线预览）移入 `Editor/`，
  随 `EXProceduralMachine.Editor` 程序集编译，命名空间 `EXProceduralMachine.Editor`。
- **插件清单**：`menu_ex.json` 移除「二阶动力学模拟器」条目（已内建化，非独立插件）。
- 引用面核实：全项目仅 `DemoForESC/BaseUnit.cs` 用 `EXToyLib`（且只依赖 Gravity 系统），迁移零破坏。

### 新增（Features）

- **飞行无人机模板** `Examples/DroneMachine.prefab` + `Examples/DroneFlyDriver.cs`：
  - 结构：扁长方体机身 + 4 旋翼（内置 Cube/Cylinder + 默认材质，零外部资源）；
  - **逻辑层/视觉层分离**：根节点 `DroneFlyDriver`（巡航/手动、空气阻力、Banking 姿态、高度保持），
    `Body` 挂 `SecondOrderDynamicsComponent` 通过替身滞后跟随；
  - 三个物理效果：**惯性**（位置实例 2.2/0.85/0.35）、**回弹**（姿态实例 1.4/0.35/0.55）、
    **空气摩擦阻力**（`drag=1.8` 速度指数衰减）；
  - 实例化即自动环形巡航（速度脉冲演示惯性/回弹），可关 `autoFly` 用 W/S、A/D、Space/Ctrl 手动控制；
  - Scene 调试 Gizmos：逻辑位置框、速度向量、目标高度、姿态方向线。

### 验证

- prefab：26 块定义 / 0 缺失引用；`m_Children: []` 全部同行。
- Roslyn 编译：24 源（合并编译 `EXProceduralMachine` + `EXProceduralMachine.Editor`）EXIT 0；
  **Player 语义编译**（无 UNITY_EDITOR 宏/无 UnityEditor 引用）22 源 EXIT 0。

---

## [2.3.0] - 蜘蛛模板缺陷修复（解析失败 + 连杆视觉 + 调试驱动）

### 修复（Fixes）

- **预制体 YAML 解析失败（根因）**：`m_Children:` 空数组此前写成"换行 + 裸 `[]`"两行，
  Unity YAML 子集无法解析（日志 `Parser Failure: Expect ':' between key and value`），
  导致**整个预制体加载失败**——MeshRenderer 材质引用全部丢失、结构异常。
  已改为 Unity 标准写法 `m_Children: []` 同行（对照项目内 60 个真实 prefab 确认）。
- **材质丢失**：根因即上述 YAML 解析失败；修复后内置默认材质（`10303` Default-Material）正常加载。

### 新增（Features）

- **机器人连杆视觉**：每条腿的 hip→knee、knee→foot 之间新增细长连杆
  （`link_hip_*` / `link_knee_*`），作为父关节子物体**随 IK 旋转**，始终连接两端关节，
  呈现完整机器人关节结构（关节方块 + 连杆）。
- **调试驱动器** `Examples/SpiderWalkDriver.cs`（已挂预制体根节点）：
  实例化后**自动行走**，W/S 前进后退、A/D 转向、Space 暂停/继续，便于在 Scene 中直接观察步态。
- **躯干姿态稳定**：`ComputeBodyRotation` 改为——落地脚用实际 IK 位置（保留地形跟随）、
  摆动脚回退到地面投射点，避免抬脚时两点定面被扭曲导致躯干抖动。

### 验证

- prefab：88 块定义 / 0 缺失引用；`m_Children: []` 16 处同行写法。
- Roslyn 编译：17 个 runtime 源文件 EXIT 0。

---

## [2.2.0] - 示例预制体 + 模板集中窗口

### 新增（Features）

- **示例预制体** `Examples/SpiderMachine.prefab`：可直接拖入场景的四足蜘蛛模板——
  简单形态（Body + 4 条腿 × 3 关节 + 视觉网格 + idle 待机锚点）、
  **自研 TwoBoneIK 绑定**（每条腿 hip/knee/foot 直绑）、
  **FourLegsSpiderLocomotion 挂载**（对角足组 FL+BR / FR+BL，`legPhaseDifference=[0.5,0.5]`）。
- **编辑器工具** `Editor/EXMachTemplateWindow.cs`（独立 Editor 程序集）：
  **程序化动画模板集中窗口**（菜单 `Tools/EXMach/程序化动画模板列表`），
  扫描 `Examples/` 目录集中列出模板（名称/组件/缩略图），支持定位资产与一键实例化到场景，
  扫描目录可配置，后续新模板自动收录。

### 验证

- 新 meta GUID 用 `[guid]::NewGuid()` 程序化生成（3 个，32 位合法 + 全项目 3894 个 meta 无冲突）。
- Roslyn 端到端合并编译验证：Runtime 16 源 + Editor 1 源，EXIT 0。

---

## [2.1.0] - 蜘蛛程序化动画设计优化（3 轮迭代）

### 设计优化（Round 1 - 核心逻辑）

- **躯干地形跟随**：`ComputeBodyRotation` 由"地面投射点三点定面"改为"**足部实际 IK 解算位置**三点定面"——
  三足落点存在真实高度差，才能解出地形起伏对应的俯仰/横滚（原实现三点几乎同高，法线恒为 up，躯干姿态形同虚设）。
- **落点前伸**：新增 `stepAheadRatio`（默认 0.35），`RefreshStepPoint` 沿平滑速度方向前伸 `L×ratio`，
  腿迈向前方而非原地踏步，步幅利用率与步态自然度提升。
- **速度平滑**：新增 `velocitySmoothing`（默认 12），`velocity` 做低通滤波，稳定 `T=L/v` 步态周期，避免步频抖动。
- **组级步态状态**：`FootMotionGroup.IsMoving()` 由"只看第一只足"改为"**任一只摆动即视为组在摆**"，步态调度判断更准确。

### 设计优化（Round 2 - 步态节奏）

- **步态评估内聚**：摆动触发判断移入 `FootPlacement.Tick`（cast 更新后立即评估），
  消除"基类先用旧 cast 评估、下一帧才触发"的 1 帧延迟；组刷新后组内双足天然同步（对角步态）。
- **摆动曲线**：蜘蛛摆动水平分量由匀速 `Lerp` 改为 **SmoothStep 加速-减速**（起步慢→中间快→收步慢），观感更自然。
- **贴地时机**：`AlignFootToGround` 仅在**落地钉脚**时执行——摆动中脚在空中强行对齐地面法线会乱转。
- **摆动时长下限**：新增 `minSwingDuration`（默认 0.08s），`SetMoving` 时长钳制到 `[minSwingDuration, stepTime]`，
  防止高速时步频失控。
- 移除基类中已被 Tick 内聚评估取代的 `UpdateFeetMovement`。

### 设计优化（Round 3 - 躯干平滑与收尾）

- **躯干高度平滑**：新增 `bodyHeightSmoothing`（默认 10），`body.y` 由直接赋值改为指数平滑跟随
  （`Lerp(targetY, 1-exp(-k·dt))`），避免地形起伏/足组刷新时躯干垂直瞬跳。

### 验证

- 每轮修改后均用 Unity 自带 Roslyn（`csc.dll` + 项目真实引用）对全部 16 个源文件编译验证，3 轮均 **EXIT 0**。

---

## [2.0.0] - 程序化动画模块全面升级

### 破坏性变更（Breaking Changes）

- **移除第三方 IK 依赖**：不再使用 `RootMotion/FinalIK`（LimbIK）与
  `com.unity.animation.rigging`。足部解算改由自研 `TwoBoneIK`（解析式二骨骼 IK）直接驱动骨骼。
- **`FootConfig` 绑定方式变更**：`ikTrack`（外部 IK 目标点）→ `hip / knee / foot` 骨骼链
  + 可选 `pole`（膝盖弯曲参考） + `alignFootToGround` / `footUpAxis`（脚掌贴地）。
- **类名变更**：`SyncAveHeight` → `SyncBodyHeight`。
- **目录重组**：`Motion/`、`Rythm/`、`VisualAid/`、`Weapon/` 旧布局废弃，改为
  `Core/`、`IK/`、`Locomotion/`、`Rhythm/`、`VisualAid/`、`Weapon/` 分层结构。
- **GUID 全部重新生成**：`DemoForESC/Resources/Prefabs/Monster/Spider.prefab`（FinalIK 绑定）
  与 `DemoForESC/Scene/TrainRoom.unity`（旧 SyncAveHeight）绑定失效，需按新结构重建。
- **字段删除**：`R`（转向半径，未实现）、`MeshRoot`、空壳类 `BaseWheeledLocomotion`。
- **依赖收敛**：不再需要 `com.unity.animation.rigging`（已从 `Packages/manifest.json` 移除）、
  `Unity.Mathematics`、`Unity.Entities`（仅保留 UnityEngine 与项目自带的 Odin/Sirenix 做 Inspector 展示）。
  全项目扫描确认：除待重建的 `Spider.prefab`（BoneRenderer）外，无任何代码/场景/资产引用 rigging 包。

### 修复（Fixes）

- `IsMoving => v == 0` 逻辑反转 → 改为基于实测位移（`_measuredVelocity`），
  「停止后复位」功能恢复可用。
- 移除 `FootPlacement.Move()` 中每帧 `Debug.Log` 刷屏。
- 启用此前被注释的躯干旋转：三点定面 + 指数平滑 + 偏航保留/跟随根节点
  （`bodyRotationSmoothing`、`syncRotationWithRoot`）。
- `RhythmPresets.SineBreath()` 原来返回平线（`EaseInOut(0,0,1,0)`）→ 改为真正的正弦曲线。
- `RhythmSystem.CombineValues()` 跳过禁用周期；`Multiplicative` 模式以首个启用周期为基准。
- 删除无效 `using`（`Unity.Entities.UniversalDelegates`、`Unity.Mathematics`、
  `UnityEngine.Serialization`、`System.Linq` 等）。
- `EXMachHelper.CalculateProjectionDistance` 文档与参数名对齐（A 相对 B 沿 N 投影）。
- `XVisualAid` 移除无意义的 `[ExecuteInEditMode]` 与空 `Start/Update`。

### 新增（Features）

- `IK/TwoBoneIK.cs`：自研解析式二骨骼 IK（余弦定理 + 极点投影），
  含可达性钳制、退化方向回退、`AlignFootToGround` 脚掌贴地。
- `Core/SyncBodyHeight.cs`：多点平均高度 + 平滑跟随（重写自 SyncAveHeight）。
- `Weapon/Gun.cs`：空占位类 → 完整命中式（Hitscan）枪械（射速/射程/伤害/
  命中特效/后坐与恢复），附带 `IDamageable` 接口。
- `EXProceduralMachine.asmdef`：模块程序集（`autoReferenced`，与 GAS asmdef 惯例一致）。
- `README.md`：工具目录说明 + 快速上手 + 步态模型 + 迁移说明。
- 大量空引用保护（`motionGroup` / `body` / `feet` / `idlePoint` 缺失时告警而非 NRE）。

### 目录结构

```
Assets/_EXProceduralMachine/
├── EXProceduralMachine.asmdef
├── README.md
├── CHANGELOG.md
├── Core/          # EXProceduralMachineManager / EXMachHelper / SyncBodyHeight
├── IK/            # TwoBoneIK
├── Locomotion/    # BaseMultiLeggedLocomotion / FootMotionGroup / FootPlacement / FootConfig / FourLegsSpiderLocomotion
├── Rhythm/        # RhythmSystem / RhythmCycle / RhythmPresets / RhythmController
├── VisualAid/     # XVisualAid / XVisualLine
└── Weapon/        # Gun / IDamageable
```
