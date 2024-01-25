# EX-GAS Wiki -- GameplayEffect
## GameplayEffect的作用
GameplayEffect是EX-GAS的核心之一，一切的游戏数值体系交互基于GameplayEffect。

GameplayEffect掌握了游戏内元素的属性控制权。理论上，只有它可以对游戏内元素的属性进行修改
（这里指的是修改，数值的初始化不算是修改）。当然，实际情况下，游戏开发人员当然可以手动直接修改属性值。
但是还是希望游戏开发者尽可能的不要打破EX-GAS的数值体系逻辑，因为过多的额外操作可能会导致游戏的数值体系变得混乱，难以追踪数值变化等等。

另外GameplayEffect还可以触发Cue（游戏提示）完成游戏效果的表现，以及控制获取额外的能力等。

---
> 由于GameplayEffect是被封装好的，程序开发者不会接触它的实现逻辑，所以本文Wiki将跳过对其接口以及代码逻辑的解析。
> 后续，可能会完善代码接口的介绍。

# GameplayEffect的使用
![gameplayeffect_editor_durational.png](Image%2Fgameplayeffect_editor_durational.png)

GameplayEffect的配置界面如图，接下来逐一解释各个参数的含义。
- Name：GameplayEffect的名称，纯粹用于显示，不会影响游戏逻辑。方便编辑者区分GameplayEffect。
- Description：GameplayEffect的描述，纯粹用于显示，不会影响游戏逻辑。方便编辑者阅读理解GameplayEffect。
- DurationPolicy：GameplayEffect的执行策略，有以下几种：
  - Instant：即时执行，GameplayEffect被添加时立即执行，执行完毕后销毁自身。
  - Duration：持续执行，GameplayEffect被添加时立即执行，持续时间结束后移除自身。
  - Infinite：无限执行，GameplayEffect被添加时立即执行，执行完毕后不会移除，需要手动移除。
  - None：无效果,这是默认占位符，因为GameplayEffect是结构体，None方便视作GameplayEffect的空值。
    _**GameplayEffect配置的执行策略禁止使用None！！！**_
- Duration：持续时间，只有DurationPolicy为Duration时有效。
- Every(Period)：周期，只有DurationPolicy为Duration或者Infinite时有效。每隔Period时间执行一次PeriodExecution。
- PeriodExecution：周期执行的GameplayEffect，只有DurationPolicy为Duration或者Infinite，且Period>0时有效。每隔Period时间执行一次PeriodExecution。
  _**PeriodExecution禁止为空!!**_PeriodExecution原则上只允许是Instant类型的GameplayEffect。但如果根据开发者需求，也可以使用其他类型的GameplayEffect。
- GrantedAbilities：授予的能力，只有DurationPolicy为Duration或者Infinite时有效。在GameplayEffect生命周期内，GameplayEffect的持有者会被授予这些能力。
  GameplayEffect被移除时，这些能力也会被移除。
- Modifiers: 属性修改器。GameplayEffect的核心功能，用于修改GameplayEffect持有者的属性。
  - Attribute：属性名称，需要填写属性的全名，比如战斗属性集里的生命值：AS_Fight.Health
  - ModifierMagnitude：基础模值，这个模值是否使用依赖于MMC的类型。在中有Wiki-MMC详细介绍。
  - Operation：属性修改操作，有加法、乘法、赋值（覆写）等。
  - MMC：属性修改计算类，用于计算ModifierMagnitude的值。
    EX-GAS提供了多种ModifierCalculation，开发者也可以自定义ModifierCalculation。
    ModifierCalculation的类型在Wiki-MMC详细介绍。
- Tags：标签。Tag具有非常重要的作用，合适的tag可以处理GameplayEffect之间复杂的关系。
  - AssetTags：描述性质的标签，用来描述GameplayEffect的特性表现，比如伤害、治疗、控制等。
  - GrantedTags：GameplayEffect的持有者会获得这些标签，GameplayEffect被移除时，这些标签也会被移除。
    Instant类型的GameplayEffect的GrantedTags是无效的。
  - ApplicationRequiredTags：GameplayEffect的目标单位必须拥有 **【所有】** 这些标签，否则GameplayEffect无法被施加到目标身上。
  - OngoingRequiredTags：GameplayEffect的目标单位必须拥有 **【所有】** 这些标签，否则GameplayEffect不会被激活（施加和激活是两个概念，
    如果已经被施加的GameplayEffect持续过程中，目标的tag变化了，不满足，效果就会失活；满足了，就会被激活）。
    Instant类型的GameplayEffect的OngoingRequiredTags是无效的。
  - RemoveGameplayEffectsWithTags：GameplayEffect的目标单位当前持有的所有GameplayEffect中，拥有 **【任意】** 这些标签的GameplayEffect会被移除。
- Cues：GameplayEffect的提示。GameplayEffect可以触发Cue（游戏提示）完成游戏效果的表现，以及控制获取额外的能力等。
  - DurationPolicy为Instant时
    - CueOnExecute（Instant）：GameplayEffect执行时触发。
  - DurationPolicy为Duration或者Infinite时
    - CueDurational（Durational）：生命周期完全和GameplayEffect同步
    - CueOnAdd（Instant）：GameplayEffect添加时触发
    - CueOnRemove（Instant）：GameplayEffect移除时触发
    - CueOnActivate（Instant）：GameplayEffect激活时触发。
    - CueOnDeactivate（Instant）：GameplayEffect失活时触发。

## GameplayEffect的施加（Apply）和激活（Activate）
GameplayEffect的施加（Apply）和激活（Activate）是两个概念，施加是指GameplayEffect被添加到目标身上，激活是指GameplayEffect实际生效。

为什么做区分？

举个例子：固有被动技能（Ability）是持续回血，被动技能的逻辑显然是永久激活的状态，而持续回血的效果（GameplayEffect）
来源于被动技能，那如果单位受到了外部的debuff禁止所有的回血效果，那么是不是被动技能被禁止？显然不是，被动技能还是会持续激活的。
那应该是移除回血效果吗？显然也不是，被动技能整个过程是不做任何变化，如果移除回血效果，那debuff一旦消失，谁再把回血效果加回来？
所以，这里需要区分施加和激活，被动技能的持续回血效果被施加到单位身上，而debuff做的是让回血效果失活，而不是移除回血效果，一旦debuff结束，
回血效果又被激活，而这个激活的操作可以理解为回血效果自己激活的（依赖于Tag系统）。