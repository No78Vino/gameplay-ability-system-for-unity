# 飞行无人机程序化动画报告

> 模块：`EXProceduralMachine`（程序化动画工具目录）
> 版本：2.4.0 ｜ 状态：已交付
> 关联文档：[DroneSecondaryMotionDesign.md](./DroneSecondaryMotionDesign.md)（设计方案）
> 本文档总结从需求、设计、实现到验证的完整过程与使用说明。

---

## 1. 概述

### 1.1 目标

- 用**程序化动画**（非美术动画、非物理引擎）驱动一架**飞行无人机**的表现；
- 视觉形态：**扁长方体机身 + 4 旋翼**，全部使用 Unity 内置网格与默认材质，零外部资源依赖；
- 核心效果：模拟无人机移动时的**惯性**（位置滞后）、**回弹**（姿态过冲回正）、**空气摩擦阻力**（速度衰减）；
- 附带的工程任务：将项目既有「次级运动程序化动画系统」（二阶动力学模拟器）从第三方插件区**整理内建**到 `EXProceduralMachine`。

### 1.2 交付物清单

| 类型 | 文件 | 说明 |
|---|---|---|
| 模板预制体 | `Examples/DroneMachine.prefab` | 无人机模板（26 对象，可直接实例化） |
| 逻辑驱动 | `Examples/DroneFlyDriver.cs` | 巡航/手动、空气阻力、Banking 姿态、高度保持、Gizmos |
| 次级运动系统 | `SecondaryMotion/`（4 文件） | 二阶动力学系统（迁移自 ExOpenSource） |
| 编辑器 | `Editor/SecondOrderDynamicsComponentEditor.cs` | 阶跃响应曲线预览 |
| 设计文档 | `Docs/DroneSecondaryMotionDesign.md` | 设计方案与落地计划（v2 已实施） |
| 本文档 | `Docs/DroneProgrammedAnimationReport.md` | 实现报告 |

---

## 2. 技术背景

### 2.1 次级运动系统（二阶动力学）

系统迁移自 `Assets/Plugins/ExOpenSource/ValueSecondOrderSystem`，数学内核基于 T.G. Parr 的二阶系统
（GDC 演讲 *Giving Personality to Procedural Animations using Math*）。

**三参数模型**：

| 参数 | 物理含义 | 效果方向 |
|---|---|---|
| 频率 `f` | 响应速度（Hz） | 越大跟得越快 |
| 阻尼比 `z` | 振荡抑制（0~1） | 0=持续震荡，1=临界阻尼无超调 |
| 缩放因子 `r` | 超调/回弹幅度 | 正值=正向过冲 |

核心迭代（每帧）：

```
y   += T · yd                        # 位置积分
yd  += T · (x + k3·xd − y − k1·yd) / k2_stable   # 速度积分
```

其中 `k2_stable = max(k2, 1.1·(T²/4 + T·k1/2))` 保证离散化稳定性（特征根在单位圆内）。

**迁移要点**：
- 命名空间 `EXToyLib` → `EXProceduralMachine`（与模块统一）；
- `using UnityEditor;` 包入 `#if UNITY_EDITOR`（Runtime 程序集 + Player 构建双向编译安全）；
- Editor 曲线预览并入 `EXProceduralMachine.Editor` 程序集；
- 全项目引用面核验：仅 `BaseUnit.cs` 使用 `EXToyLib` 且只依赖 Gravity 系统 → **迁移零破坏**。

### 2.2 无人机程序化动画的通用手法

业界（程序化动画领域）的成熟套路：

1. **逻辑层 / 视觉层分离**：逻辑体负责运动学（移动、转向），视觉体用次级运动**滞后跟随**，
   自然产生惯性漂移、回弹；
2. **Banking（倾斜）**：根据速度/加速度计算目标俯仰（前倾低头）与滚转（侧移/压弯侧倾），
   再经二阶系统平滑 → 倾斜带过冲、回正带回弹；
3. **空气阻力**：驱动层对速度做指数衰减，配合二阶系统的阻尼，两层叠加模拟"空气摩擦"。

---

## 3. 架构设计

### 3.1 层级结构

```
DroneMachine.prefab
└── Drone (root)                    # 逻辑层：挂 DroneFlyDriver（无渲染）
    ├── attitude (空, 隐藏)          # 姿态替身：每帧写入期望欧拉角 (pitch, 0, roll)
    └── Body (扁长方体 1.6×0.12×1.0) # 视觉层：挂 SecondOrderDynamicsComponent
        ├── Rotor_FL / Rotor_FR / Rotor_BL / Rotor_BR   # 旋翼装饰（扁平圆柱）
```

### 3.2 数据流

```
输入(巡航/手动) ──► DroneFlyDriver
                     ├─ 空气阻力衰减  v *= 1-exp(-drag·dt) ──► 位置积分 ──► root 移动
                     ├─ 高度保持      y → hoverHeight（指数柔和恢复）
                     ├─ 偏航          yaw += rate·dt
                     └─ Banking       attitude.localEulerAngles = (pitch, 0, roll)
                                          ▲
Body 上的 SecondOrderDynamicsComponent ───┘
  ├─ 实例① Position  替身=root      → 位置惯性滞后（急停前滑、启动拖尾）
  └─ 实例② Rotation  替身=attitude  → 姿态过冲回弹（倾斜过头再回正）
```

### 3.3 三个物理效果的实现映射

| 效果 | 实现层 | 参数 | 默认值 | 作用机理 |
|---|---|---|---|---|
| **惯性** | 位置次级运动 | Frequency/Damping/Scale | 2.2 / 0.85 / 0.35 | 低频慢跟 + 中阻尼 + 小超调 → 位置"拖着走" |
| **回弹** | 姿态次级运动 | Frequency/Damping/Scale | 1.4 / 0.35 / 0.55 | 低阻尼 → 明显过冲；正 scale → 回正弹跳 |
| **空气摩擦阻力** | 驱动层 | drag | 1.8 /s | 速度指数趋近目标，撤油门后滑行减速 |
| 阻力（次级层） | 二阶阻尼 | Damping（同上） | — | 输出速度衰减，物理上即空气阻力 |
| 悬停浮动 | 位置次级运动 | （沿用惯性实例） | — | 悬停时目标静止，输出小幅余振 |

---

## 4. 实现细节

### 4.1 DroneFlyDriver（逻辑驱动）

关键行为（`Update` 每帧）：

1. **目标速度生成**：
   - `autoFly=true`：沿机头方向巡航 `cruiseSpeed`，叠加正弦速度脉冲（±20%，制造加减速以演示惯性回弹），持续转向 `autoTurnRate` 形成环形航线；
   - `autoFly=false`：W/S 前后、A/D 转向、Space/Ctrl 升降（`verticalSpeed`），高度目标钳制 [0.2, 50]。
2. **空气阻力**：`v = Lerp(v, v_desired, 1 - exp(-drag·dt))` —— 指数趋近目标速度，阻力系数越大越"粘稠"，松开油门即滑行减速。
3. **位置积分**：`pos += v·dt`；**高度保持**：`y = Lerp(y, heightTarget, 1 - exp(-heightRestoreSpeed·dt))`。
4. **偏航**：`yaw += rate·dt`，`rotation = Euler(0, yaw, 0)`。
5. **Banking 姿态**（写入隐藏替身 `attitude`，局部欧拉角）：
   - `pitch = -Clamp(forwardSpeed / moveSpeed) · bankPitch`（前进低头 / 后退抬头）；
   - `roll = Clamp(sideSpeed / moveSpeed) · bankRoll + Clamp(yawRate/120) · bankRoll · turnBankWeight`（侧移侧倾 + 转向压弯）。

**Scene 调试 Gizmos**（`gizmosOn`）：逻辑位置线框、速度向量（蓝）、目标高度（黄）、姿态方向线（橙）。

### 4.2 次级运动配置（Body 上的组件实例）

| 实例 | 影响属性 | 替身 | 频率 | 阻尼 | 缩放 | 表现 |
|---|---|---|---|---|---|---|
| ① | Position | root Transform | 2.2 | 0.85 | 0.35 | 惯性漂移 |
| ② | Rotation | attitude | 1.4 | 0.35 | 0.55 | 姿态回弹 |

两个实例均开启 `autoUpdate`（参数每帧同步到动力学内核），`xyz` 三轴全开。

### 4.3 预制体规格

- 全部使用**内置资源**：Cube 网格（机身）、Cylinder 网格（旋翼）、默认材质（Default-Material）；
- 机身 `Body`：`1.6 × 0.12 × 1.0`；旋翼：`0.5 × 0.02 × 0.5` @ 机身四角上方 `±0.55, 0.08, ±0.35`；
- 根节点初始位于 `(0, 0, 0)`，运行时自动爬升至悬停高度 `hoverHeight=2`（起飞效果）；
- 26 个对象定义，0 缺失引用，`m_Children: []` 全部同行（YAML 解析安全写法）。

---

## 5. 验证结果

### 5.1 编译验证（Roslyn 双重语义）

| 验证 | 范围 | 引用/宏 | 结果 |
|---|---|---|---|
| 编辑器语义编译 | 24 源（Runtime + Editor 合并） | UnityEditor 引用 + `UNITY_EDITOR` 宏 | **EXIT 0** |
| Player 语义编译 | 22 源（仅 Runtime） | 无 UnityEditor 引用、过滤 `UNITY_EDITOR` 宏 | **EXIT 0** |

### 5.2 预制体验证

- 26 对象定义 / 0 缺失引用；
- Cube 网格 ×1、Cylinder 网格 ×4、默认材质 ×5（机身 + 4 旋翼）；
- `m_Children: []` 两行式写法 0 处。

### 5.3 元数据（meta）修复

初版 meta 因 PowerShell `Set-Content -Encoding UTF8` 写入**带 BOM** 的 UTF-8 导致 Unity YAML 解析失败
（`Parser Failure`，涉及 5 个 meta）。已全部重写为**无 BOM UTF-8**，GUID 保持不变，扫描确认全模块无 BOM。

### 5.4 运行验证（需在 Unity 中确认）

- 实例化 `DroneMachine` → 自动起飞爬升 + 环形巡航；
- 观察机身：加速低头 → 过冲回弹；急停前滑；转弯侧倾压弯；悬停轻微浮动；
- 关闭 `autoFly` 手动控制验证各效果。

---

## 6. 使用指南

### 6.1 快速上手

1. Unity 中 `Ctrl+R` 刷新（或重启）导入新资产；
2. 菜单 **Tools → EXMach → 程序化动画模板列表** → 列表中选择 `DroneMachine` → **实例化到场景**；
3. 默认 `autoFly=true`，实例化即自动飞行演示。

### 6.2 控制方式

| 键位 | 动作 |
|---|---|
| W / S | 前进 / 后退（沿机头方向） |
| A / D | 左转 / 右转 |
| Space / Ctrl(C) | 上升 / 下降（调整悬停高度目标） |
| Inspector | 关闭 `autoFly` 切手动模式 |

### 6.3 效果调优

| 想调整 | 修改位置 | 方向 |
|---|---|---|
| 惯性更强（更"飘"） | 位置实例 Frequency↓ / Scale↑ | 跟得更慢、前滑更远 |
| 回弹更明显 | 姿态实例 Damping↓ / Scale↑ | 过冲更夸张 |
| 阻力更大（更"粘稠"） | `DroneFlyDriver.drag` ↑ | 加减速更迟钝 |
| 倾斜更夸张 | `bankPitch` / `bankRoll` ↑ | Banking 幅度更大 |
| 飞行高度 | `hoverHeight` | 悬停高度 |

Inspector 中勾选组件「绘制值变化示例曲线」可预览任意实例的阶跃响应曲线辅助调参。

---

## 7. 已知限制与后续优化

### 7.1 已知限制

1. **旋翼无旋转动画**：旋翼目前是静态装饰（扁平圆柱），未加旋转表现；
2. ~~**姿态用欧拉角**：大角度偏航时局部欧拉角可能回绕（360° 跳变）~~ —— **已修复（2.4.1）**：
   `SecondOrderDynamicsComponent` Rotation 分支对输入做 `WrapEuler` 角度回绕规范化
   （映射到 [-180°,180°)），方向切换（前/后、左/右）不再剧烈翻转；
3. **无物理交互**：纯运动学驱动，无刚体碰撞（撞墙会穿过）；适合表现层/演示用途；
4. **蜘蛛表现问题暂缓**：蜘蛛动画表现不佳的问题按用户要求搁置，未在本版本处理。

### 7.2 后续优化方向

- 旋翼旋转 + 转速随油门变化（简单 `Rotate` 即可）；
- 无人机受击/碰撞抖动（复用次级运动系统加扰动输入）；
- 编队飞行（多个实例共享航线参数）；
- 摄像头跟随（`SecondOrderDynamicsComponent` 挂相机即可复用）；
- 蜘蛛步态表现调优（`Locomotion/` 参数与姿态逻辑）。

---

## 8. 变更记录

| 版本 | 内容 |
|---|---|
| 2.4.1 | 修复无人机方向切换欧拉角回绕翻转（Rotation 输入 WrapEuler 规范化） |
| 2.4.0 | 次级运动系统内建化（SecondaryMotion/）；新增 DroneMachine 模板 + DroneFlyDriver；README/CHANGELOG/设计文档更新 |
| — | meta BOM 修复（无 BOM UTF-8 重写 5 个 meta，GUID 不变） |

---

*报告完*
