# 飞行无人机程序化动画 —— 设计方案与落地计划（v2 已实施）

> 状态：**已实施**（2025 版 2.4.0，D1–D6 均按推荐确认）
> 关联需求：
> 1. 把项目里现有的「次级运动程序化动画系统」（二阶动力学模拟器）整理进 `EXProceduralMachine`
> 2. 用整理后的系统做一个飞行无人机程序化动画（扁长方体 + 次级运动，模拟**惯性 / 回弹 / 空气摩擦阻力**）
> 3. 蜘蛛动画表现问题暂缓（本次不动蜘蛛任何代码与预制体）

---

## 1. 调研结论

### 1.1 现有次级运动系统：`Assets/Plugins/ExOpenSource/ValueSecondOrderSystem/`

这就是项目里现成的「次级运动程序化动画系统」，全名**二阶动力学模拟器**（参考 GDC 演讲 *Giving Personality to Procedural Animations using Math*，T.G. Parr 的二阶系统公式）。

| 文件 | 职责 |
|---|---|
| `SecondOrderDynamics.cs` | **核心数学**。三参数系统：频率 `f`（响应速度）、阻尼比 `z`（0=震荡，1=临界阻尼）、缩放因子 `r`（超调幅度）。`Update(dt, target, velocity?)` 数值积分返回平滑输出，含 `k2_stable` 稳定性修正 |
| `SecondOrderDynamicValueType.cs` | 枚举：Position / Rotation / Scale / Custom |
| `SecondOrderDynamicInstance.cs` | 单条次级运动的配置（替身模式/自输入、影响属性、xyz 维度开关、f/z/r 参数、Odin 滑条绘制器） |
| `SecondOrderDynamicsComponent.cs` | MonoBehaviour 容器，持有 `List<SecondOrderDynamicInstance>`，`Update()` 逐帧驱动 |
| `Editor/SecondOrderDynamicsComponentEditor.cs` | `OdinEditor` 扩展，提供**阶跃响应曲线预览**（原始值 vs 二次运动值） |
| `GUIDE.md` | 使用文档 |

**迁移前必须处理的问题点：**

1. **命名空间不一致**：全部在 `EXToyLib`，而 `EXProceduralMachine` 内全部代码统一用 `EXProceduralMachine`。
2. **`SecondOrderDynamicInstance.cs` 在运行时文件里 `using UnityEditor;`**（第 3 行，裸 using）。现在它在 `Assets/Plugins/ExOpenSource/` 下属于预定义程序集 `Assembly-CSharp`，编辑器下能编译；一旦移入 `EXProceduralMachine.asmdef`（Runtime 程序集），**编辑器下也会 CS0246**，必须把 using 包进 `#if UNITY_EDITOR`。
3. **组件类里有一大段注释残留**（被注释掉的旧 avator/target 字段，与实例类重复）——整理时清理。
4. **引用面已排查**：全项目仅 `Assets/DemoForESC/_Script/Unit/BaseUnit.cs` 引用 `EXToyLib`，且**只用 `GravityForCharacterController`**（不碰二阶动力学）。→ 本次迁移把二阶动力学系统改命名空间为 `EXProceduralMachine` **不会破坏 BaseUnit**。
5. **`Assets/Plugins/ExOpenSource/menu_ex.json`**（EX 开源插件管理器清单）中有「二阶动力学模拟器」条目，`LocalPath` 指向旧路径 → 迁移后条目会失效，需同步处理（见决策点 D2）。
6. 旋翼/无人机场景里**没有现成的无人机相关代码**（grep 确认），全新开发。

### 1.2 无人机程序化动画的通用手法（调研）

业界（含上述 GDC 演讲的体系）做「飞行动画的次级运动」标准套路：

- **逻辑层 / 视觉层分离**：逻辑体负责移动转向（无渲染），视觉体用次级运动**滞后跟随**逻辑体 → 产生惯性、漂移、回弹。
- **Banking（倾斜）**：加速时前倾（pitch），侧移/急转时侧倾（roll），用二阶动力学平滑目标姿态 → 倾斜带过冲、回正带回弹。
- **三参数物理映射**（关键）：
  - **惯性（位置滞后）**：低频 + 中高阻尼 + 小 scale → 视觉体"拖着走"，停顿时继续前滑一段。
  - **回弹（姿态过冲）**：中频 + **低阻尼（0.3~0.5）** + 正 scale → 倾斜过头再弹回。
  - **空气摩擦阻力**：两层实现——① 驱动层对目标速度做指数衰减 `v *= (1 - drag·dt)`；② 二阶系统的阻尼 z 本身对输出起阻力作用（速度越快衰减越狠，天然是"空气阻力"）。

---

## 2. 总体设计

### 2.1 目录规划（整理后的 EXProceduralMachine）

```
Assets/_EXProceduralMachine/
├── EXProceduralMachine.asmdef          # Runtime（不动）
├── Core/                               # 现有
├── IK/                                 # 现有
├── Locomotion/                         # 现有（蜘蛛，本次不动）
├── Rhythm/  VisualAid/  Weapon/        # 现有
├── SecondaryMotion/                    # ★ 新增：迁移整理后的次级运动系统
│   ├── SecondOrderDynamics.cs          # 核心数学（原样保留，仅改命名空间）
│   ├── SecondOrderDynamicValueType.cs
│   ├── SecondOrderDynamicInstance.cs   # using UnityEditor 包 #if UNITY_EDITOR
│   └── SecondOrderDynamicsComponent.cs # 清理注释残留
├── Editor/
│   ├── EXMachTemplateWindow.cs         # 现有
│   └── SecondOrderDynamicsComponentEditor.cs  # ★ 迁移，并入 Editor asmdef
└── Examples/
    ├── SpiderMachine.prefab            # 现有
    └── DroneMachine.prefab             # ★ 新增：无人机模板
```

- 原 `Assets/Plugins/ExOpenSource/ValueSecondOrderSystem/` **整目录删除**（含 meta）。
- 命名空间：Runtime 全部 `EXProceduralMachine`；Editor 用 `EXProceduralMachine.Editor`（与现有一致）。
- Odin 依赖不变（`Sirenix.OdinInspector` 是 precompiled autoReferenced，Runtime/Editor 均可用）。

### 2.2 无人机架构（逻辑层 + 视觉层）

```
DroneMachine.prefab
└── Drone (root)                        # 逻辑层：挂 DroneFlyDriver（无渲染，纯逻辑体）
    ├── target (空物体, 隐藏)            # 位置替身：DroneFlyDriver 每帧写入"期望位置"
    ├── attitude (空物体, 隐藏)          # 姿态替身：每帧写入"期望欧拉角"（yaw + banking）
    └── Body (扁长方体 1.6×0.12×1.0)     # 视觉层：挂 SecondOrderDynamicsComponent
        ├── Rotor_FL / Rotor_FR / Rotor_BL / Rotor_BR   # 4 旋翼装饰（扁平圆柱，默认材质）
        └── （可选）RotorSpin 旋翼旋转动画
```

**工作流：**

1. `DroneFlyDriver`（挂 root）每帧：
   - 读输入（自动巡航 或 WASD + Space/Ctrl 升降）；
   - 目标速度做**空气阻力衰减** `v *= (1 - drag·dt)`，再叠加输入加速度；
   - 积分更新 `target.position`（逻辑位置）；root 本身也移动到该位置（逻辑层实时，无滞后）；
   - 按当前速度/加速度计算 banking 目标姿态（pitch = −前向速度·k、roll = 侧向速度·k、yaw = 转向角），写入 `attitude.localEulerAngles`；
   - 高度自动保持（hoverHeight 柔和恢复，模拟悬停）。
2. `SecondOrderDynamicsComponent`（挂 Body）配 **2 个实例**：
   - 实例① `Position`：替身 = `target` → Body 位置**惯性滞后**跟随；
   - 实例② `Rotation`：替身 = `attitude` → Body 姿态**过冲回弹**跟随。
3. 效果：加速 → Body 前倾过头再回弹；急停 → Body 前滑一小段再回正；转向 → 侧倾漂移；悬停 → 轻微上下浮动（低频率 + 小 scale 的 Position 即可自然产生）。

### 2.3 三效果 → 参数映射表（默认值，可调）

| 你要的效果 | 实现层 | 参数 | 默认值 | 说明 |
|---|---|---|---|---|
| **惯性**（移动滞后、急停前滑） | 位置次级运动 | Frequency / Damping / Scale | 2.2 / 0.85 / 0.35 | 低频慢跟 + 中等阻尼 + 小超调 |
| **回弹**（倾斜过冲、回正弹跳） | 姿态次级运动 | Frequency / Damping / Scale | 1.4 / 0.35 / 0.55 | 低阻尼 → 明显过冲；正 scale → 回弹 |
| **空气摩擦阻力** | 驱动层 | drag | 1.8 /s | 速度指数衰减（越大越"粘稠"） |
| 阻力（次级层补充） | 位置+姿态阻尼 | Damping | 如上 | 输出速度衰减，物理上即阻力 |
| 悬停浮动 | 位置次级运动 | （沿用惯性实例） | — | 悬停时目标静止，输出小幅余振 |

> 姿态输入用**局部欧拉角**（与现有系统一致，GUIDE 已注明避免万向锁）；无人机 yaw 大角度转向时若出现欧拉角回绕，姿态实例按轴拆分或改用局部增量——作为实施中的备选方案，先按简单路径做。

### 2.4 组件清单

| 组件 | 位置 | 职责 |
|---|---|---|
| `SecondOrderDynamics` | SecondaryMotion/ | 核心数学（迁移，不改逻辑） |
| `SecondOrderDynamicInstance` | SecondaryMotion/ | 实例配置（迁移 + using 修复） |
| `SecondOrderDynamicsComponent` | SecondaryMotion/ | 容器（迁移 + 注释清理） |
| `SecondOrderDynamicsComponentEditor` | Editor/ | 曲线预览（迁移） |
| `DroneFlyDriver` | Examples/ | **新增**：输入/巡航、空气阻力、banking 姿态、高度保持、Gizmos（目标点十字、速度向量、姿态线） |
| `DroneMachine.prefab` | Examples/ | **新增**：扁长方体 + 4 旋翼 + 组件配置（默认材质、内置网格，零外部资源） |

`EXMachTemplateWindow`（模板集中窗口）扫描 `Examples/`，新增 prefab **自动收录**，无需改窗口代码。

---

## 3. 落地计划

### 阶段 A：整理迁移次级运动系统（独立、先行）

| 步骤 | 动作 | 验证 |
|---|---|---|
| A1 | 新建 `SecondaryMotion/`，移动 4 个 runtime 文件（保留 .meta 或重生成合法 GUID） | — |
| A2 | 命名空间 `EXToyLib` → `EXProceduralMachine`（4 个文件） | — |
| A3 | `SecondOrderDynamicInstance.cs`：`using UnityEditor;` 包 `#if UNITY_EDITOR` | — |
| A4 | `SecondOrderDynamicsComponent.cs`：删除注释残留 | — |
| A5 | `SecondOrderDynamicsComponentEditor.cs` 移入 `Editor/`（并入 `EXProceduralMachine.Editor` asmdef） | — |
| A6 | 删除原 `ValueSecondOrderSystem/` 目录；处理 `menu_ex.json` 条目（见 D2） | — |
| A7 | Roslyn 合并编译：Runtime 全部源 + Editor 全部源，**EXIT 0** | 编译通过 |

### 阶段 B：无人机程序化动画

| 步骤 | 动作 | 验证 |
|---|---|---|
| B1 | 编写 `DroneFlyDriver.cs`（输入/巡航、drag、banking、高度保持、Gizmos） | 编译通过 |
| B2 | 生成 `DroneMachine.prefab`（YAML 手写：`m_Children: []` 同行、默认材质 10303、内置网格 10202/10207、合法 GUID） | 解析通过、0 缺失引用 |
| B3 | 模板窗口扫描确认 `DroneMachine` 出现 | Unity 侧确认 |
| B4 | 预制体实例化自检：autoFly 默认开 → 实例化即飞行 + 次级运动可见 | Unity 侧确认 |

### 阶段 C：文档与收尾

| 步骤 | 动作 |
|---|---|
| C1 | `README.md`：目录结构加 `SecondaryMotion/`、新增「飞行无人机模板」章节 + 参数调优表 |
| C2 | `CHANGELOG.md`：`[2.4.0]` 迁移次级运动系统 + 新增无人机模板 |
| C3 | 交付说明：提醒 Ctrl+R / 重启 Unity 刷新 |

---

## 4. 风险与注意事项

1. **`using UnityEditor;`**：迁移进 Runtime asmdef 后必须包 `#if UNITY_EDITOR`，否则编译失败（已列入 A3）。
2. **menu_ex.json**：EX 开源插件管理器清单条目会失效，需按 D2 处理，避免插件管理器报"未安装"。
3. **Odin 依赖**：整理后组件仍依赖 Odin Inspector（`Sirenix.OdinInspector`），项目已有惯例依赖，无新增风险。
4. ~~**姿态欧拉角回绕**：大角度偏航时局部欧拉角可能跳变（360° 翻转）~~ —— **已解决（2.4.1）**：
   `SecondOrderDynamicsComponent` Rotation 分支对输入做 `WrapEuler` 角度回绕规范化
   （映射到 [-180°,180°)），方向切换不再剧烈翻转。
5. **蜘蛛暂缓**：本次不动 `Locomotion/`、`SpiderMachine.prefab` 任何内容。
6. **预制体 YAML**：延续既定约束（`m_Children: []` 同行、32 位 GUID、默认材质/内置网格引用）。

---

## 5. 待确认决策点（D1–D6）

| # | 决策 | 推荐 | 说明 |
|---|---|---|---|
| D1 | 命名空间改为 `EXProceduralMachine`？ | **是** | 与模块统一；BaseUnit 只用 Gravity，不受影响 |
| D2 | 原目录删除 + menu_ex.json 条目？ | **删除，条目移除** | 系统已内建化，不再是独立插件；也可改为条目指向新路径（二选一） |
| D3 | 无人机用「逻辑层 root + 视觉层 Body」分离架构？ | **是** | 次级运动的标准玩法，效果最直观 |
| D4 | 驱动模式：autoFly 自动巡航默认开 + WASD/Space 可控？ | **是** | 实例化即演示；`autoFly` 可关变纯模板 |
| D5 | 加 4 旋翼装饰（扁平圆柱 + 可选旋转）？ | **是** | 纯内置资源，廉价提升辨识度 |
| D6 | 默认参数按 2.3 映射表？ | **是** | 实施后可按表现微调 |
