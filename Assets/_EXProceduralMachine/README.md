# EXProceduralMachine —— 程序化动画工具目录

> 高度集中的程序化动画（Procedural Animation）工具集：多足/足式移动、自研 IK、
> 节奏（呼吸）系统、可视化辅助、简易武器，全部自包含于 `EXProceduralMachine` 命名空间，
> **不依赖任何第三方 IK 插件**（已移除对 FinalIK / Animation Rigging 的依赖）。

## 目录结构

```
Assets/_EXProceduralMachine/
├── EXProceduralMachine.asmdef   # 模块程序集（自包含，仅依赖 Odin/Sirenix 预编译 DLL）
├── Core/                        # 核心基础设施
│   ├── EXProceduralMachineManager.cs  # 模块单例：跨场景运行时根节点
│   ├── EXMachHelper.cs                # 通用数学/地面检测工具
│   └── SyncBodyHeight.cs              # 高度跟随工具（跟随参考体/多点平均高度）
├── IK/                          # 自研 IK 模块（替代第三方 IK 插件）
│   └── TwoBoneIK.cs                   # 二骨骼解析式 IK 解算器
├── Locomotion/                  # 移动模块（足式）
│   ├── BaseMultiLeggedLocomotion.cs  # 多足移动基类（步态参数、躯干贴地、步态驱动）
│   ├── FootMotionGroup.cs             # 足对分组（同相位一组，交替摆动）
│   ├── FootPlacement.cs               # 单足状态机：地面投射 → IK 解算 → 贴地
│   ├── FootConfig.cs                  # 单足配置（骨骼链 + 落点参数）
│   └── FourLegsSpiderLocomotion.cs    # 四足蜘蛛实现（抛物线抬脚）
├── Rhythm/                      # 节奏/呼吸系统
│   ├── RhythmSystem.cs                # 多周期混合输出（加/乘/最大/最小）
│   ├── RhythmCycle.cs                 # 单周期（时长 + 曲线 + 相位）
│   ├── RhythmPresets.cs               # 预设曲线工厂
│   └── RhythmController.cs            # 示例组件（驱动缩放/位置/旋转）
├── VisualAid/                   # 可视化辅助（Editor Gizmo）
│   ├── XVisualAid.cs                  # Box/Sphere 标记
│   └── XVisualLine.cs                 # 线段列表标记
├── SecondaryMotion/             # 次级运动系统（二阶动力学，迁移自 ExOpenSource）
│   ├── SecondOrderDynamics.cs         # 核心数学：频率/阻尼/缩放三参数二阶系统
│   ├── SecondOrderDynamicInstance.cs  # 单条次级运动配置（替身/影响属性/维度/参数）
│   ├── SecondOrderDynamicsComponent.cs# 组件容器：实例队列逐帧驱动
│   └── SecondOrderDynamicValueType.cs # Position/Rotation/Scale/Custom 枚举
└── Weapon/                      # 武器模块
    └── Gun.cs                         # 简易命中式枪械（Hitscan）
Examples/                      # 示例预制体
├── SpiderMachine.prefab              # 四足蜘蛛模板（含 IK 绑定 + 步态挂载）
├── SpiderWalkDriver.cs               # 蜘蛛调试驱动（自动行走/WASD/暂停）
├── DroneMachine.prefab               # 飞行无人机模板（扁长方体 + 次级运动）
└── DroneFlyDriver.cs                 # 无人机逻辑驱动（巡航/阻力/Banking/高度保持）
Editor/                         # 编辑器工具（独立 Editor 程序集 EXProceduralMachine.Editor）
├── EXMachTemplateWindow.cs          # 程序化动画模板集中窗口
└── SecondOrderDynamicsComponentEditor.cs  # 次级运动曲线预览（OdinEditor）
```

## 次级运动系统（SecondaryMotion）

二阶动力学模拟器（参考 GDC 演讲 *Giving Personality to Procedural Animations using Math*），
迁移自 `Assets/Plugins/ExOpenSource/ValueSecondOrderSystem`，命名空间统一为 `EXProceduralMachine`。

- **`SecondOrderDynamicsComponent`**（挂到需要次级运动的物体）→ 添加实例：
  - **使用替身**：跟随外部 `Transform`（位置/旋转/缩放），自动更新参数；
  - **不使用替身**：调用 `inst.Dynamics.SetInput(...)` 由代码喂输入；
  - **影响属性**：Position / Rotation / Scale；可逐轴开关（x/y/z）。
- **参数含义**：
  - **震荡频率 Frequency**：响应速度（0.1~10Hz，越大越快）；
  - **阻尼 Damping**：0=持续震荡，1=临界阻尼无超调（也即输出阻力）；
  - **缩放因子 Scale**：正值=正向超调（回弹幅度）。
- **调试**：Inspector 勾选「绘制值变化示例曲线」可预览该实例的阶跃响应曲线。

## 示例预制体（DroneMachine.prefab）

`Examples/DroneMachine.prefab` 是飞行无人机模板：**扁长方体机身 + 4 旋翼**（全部内置
Cube/Cylinder 网格 + 默认材质，零外部资源）。

- **架构**：`Drone`(根) 为**逻辑层**（挂 `DroneFlyDriver`），`Body` 为**视觉层**
  （挂 `SecondOrderDynamicsComponent`），`attitude` 为隐藏姿态替身。
- **次级运动**（2 个实例）：
  - 实例① `Position` 替身=根节点 → **惯性**：机身位置滞后跟随，急停前滑；
  - 实例② `Rotation` 替身=`attitude` → **回弹**：倾斜过冲后回正弹跳。
- **空气摩擦阻力**：`DroneFlyDriver.drag` 对速度做指数衰减（撤油门后滑行减速）。
- **Banking 姿态**：前进低头、后退抬头、侧移/急转侧倾（压弯），写入 `attitude` 由次级运动平滑。
- **使用**：拖入场景即**自动巡航**（环形 + 速度脉冲演示惯性/回弹）；关闭 `autoFly`
  后 W/S 前后、A/D 转向、Space/Ctrl 升降。Scene 中开启 `gizmosOn` 可看到
  逻辑位置框、速度向量、目标高度与姿态方向线。

## 编辑器工具：程序化动画模板集中窗口

菜单 **Tools → EXMach → 程序化动画模板列表** 打开 `EXMachTemplateWindow`：

- **集中列表**：自动扫描 `Examples/` 目录（可在窗口顶部修改）下所有模板预制体，
  展示名称、路径、挂载组件与预览缩略图；后续新增模板（轮式机械、双足等）放入目录后点「刷新」自动收录。
- **操作**：点击模板名/「定位资产」在 Project 窗口定位选中；「实例化到场景」一键放入当前场景（支持 Undo）。
- **扫描目录可配置**：窗口顶部的「扫描目录」输入框可直接修改并「刷新」。

## 示例预制体（SpiderMachine.prefab）

`Examples/SpiderMachine.prefab` 是可直接拖入场景的蜘蛛模板：

- **形态**：`Spider`(根) → `Body`(躯干盒体) → 4 条腿 `leg_hip_* → leg_knee_* → leg_foot_*`，
  每条腿带独立视觉网格子物体（`mesh_*`），另有 4 个 `idle_*` 待机锚点。
- **IK 绑定**：每个 `FootConfig` 的 `hip / knee / foot` 已绑定对应骨骼 Transform，
  `TwoBoneIK` 直接驱动（零第三方 IK）。
- **步态挂载**：根节点已挂 `FourLegsSpiderLocomotion`，对角足组
  （组1 = FL+BR，组2 = FR+BL），`legPhaseDifference = [0.5, 0.5]` 对角步态。
- **使用**：把预制体拖入场景，确保地面物体位于 `groundLayer`（当前为 Layer 7 / m_Bits 128），
  移动 Spider 根节点即可看到对角步态 + 躯干地形跟随 + 步态调试线（`gizmosOn = 1`）。
- 可直接修改腿部关节位置/视觉网格尺寸，或替换为自己的蜘蛛模型骨骼（保持 `hip/knee/foot` 层级与绑定）。

## 快速上手（以四足蜘蛛为例）

1. 在场景中搭建蜘蛛模型，骨骼层级建议：`body → leg_upper → leg_mid → log_lower(foot)`。
2. 在 Spider 根节点挂 `FourLegsSpiderLocomotion`。
3. 配置 Odin 面板「运动参数/绑定」：
   - `MotionGroup`：按对角分组（每组 2 足），每足填写 `hip / knee / foot` 骨骼、
     `idlePoint`（待机锚点，一般挂在身体下相对静止）、`offset`（落点偏移）、
     `pole`（膝盖弯曲参考点，可选，控制膝盖朝外/朝上弯曲）、`alignFootToGround`（贴地）。
   - `Body`：躯干；`FootTargetGroupNode`：可选，运行时挂载锚点。
   - 设置 `L`（步长）、`h`（离地间隙）、`groundLayer`、`castMaxDistance`。
4. 让身体移动（直接移动 Transform 或设置 `velocity`），步态自动驱动。

### 步态模型

| 参数 | 含义 | 关系 |
|---|---|---|
| `L` | 步长（m） | 与肢体长度、摆动角度正相关 |
| `v` | 速度（m/s） | 勾选「自动计算速度」时由躯干位移测得 |
| `T` | 步态周期（s） | `T = L / v` |
| `f` | 步频（Hz） | `f = 1 / T` |
| `h` | 离地间隙（m） | 机身最低处到地面的垂直距离 |
| `legPhaseDifference` | 组间相位差占比 | 决定步态类型（对角步态约 0.5） |

## 自研 IK 说明（TwoBoneIK）

- **解析式二骨骼 IK**：对 `hip → knee → foot` 链直接求解旋转，使脚掌到达目标点。
  由余弦定理求髋关节角、投影求膝位，`pole` 参考点控制膝盖弯曲方向。
- 落地时持续向「地面投射点」解算（跟随地形起伏），摆动时沿
  `CalculateFootPlacementMovingPoint`（子类实现，如抛物线）插值解算。
- 可选 `AlignFootToGround` 将脚掌对齐地面法线（贴地）。
- 目标不可达（过近/过远）时自动钳制到可达边界，不会产生 NaN。

## 设计约定

- 命名空间统一 `EXProceduralMachine`；程序集 `EXProceduralMachine.asmdef` 自包含，
  仅引用 Odin（Sirenix）预编译 DLL 做 Inspector 展示，运行时零第三方依赖。
- 步态驱动流程：`Update` 中「躯干高度/旋转 → 步态触发 → 足摆动判定 → 足组 Tick(IK) → 速度/停止复位」。
- 次级运动驱动流程：`SecondOrderDynamicsComponent.Update` 逐实例「自动更新参数 →
  读替身输入（或自输入）→ 二阶积分 → 按轴写回 Position/Rotation/Scale」。
- 运行时生成对象（足部锚点、组节点）统一挂在 `EXProceduralMachineManager` 的跨场景根节点下。

## 迁移说明（破坏性变更）

- 旧的 `Motion/`、`Rythm/`、`VisualAid/`、`Weapon/` 布局已重组为上述结构；
  类名、GUID 全部重新生成。
- **旧绑定会失效**：`DemoForESC/Resources/Prefabs/Monster/Spider.prefab`
  （旧版 FinalIK LimbIK 绑定）与 `DemoForESC/Scene/TrainRoom.unity`（旧 SyncAveHeight）
  需要按新结构重新绑定/重建预制体。
- `SyncAveHeight` 更名为 `SyncBodyHeight`；`ikTrack` 绑定改为 `hip/knee/foot` 骨骼链直接绑定。

## 依赖

- 运行时：仅 `UnityEngine`（可选 Odin 做 Inspector 展示）。
- 不再需要：`com.unity.animation.rigging`、`RootMotion/FinalIK`、`Unity.Mathematics`、`Unity.Entities`。
- `SecondaryMotion/` 为内建模块（迁移自 `Assets/Plugins/ExOpenSource/ValueSecondOrderSystem`，
  原 EX 开源插件管理器清单条目已移除）。
