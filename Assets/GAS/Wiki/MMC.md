# EX-GAS Wiki -- MMC(Modifier Magnitude Calculation)
## MMC的作用
MMC(Modifier Magnitude Calculation)是一个用于计算Modifier的模值的可编辑类，它的作用是根据输入的参数计算Modifier的模值，然后在后续逻辑中将模值赋给对应的属性。
## MMC的使用
在EX-GAS中MMC只会在GameplayEffect中使用。

MMC依存于GameplayEffectModifier中，GameplayEffectModifier是包裹MMC的结构体，MMC为GameplayEffectModifier成员之一。
GameplayEffectModifier为GameplayEffect的成员之一。

**GameplayEffectModifier在EX-GAS中非常重要，它是GAS属性变化的核心，所有的游戏运行时的元素属性变化全部由他操作（除了属性初始化，或者开发人员手动设置属性值的情况）。**

GameplayEffectModifier成员组成（所有成员都需要在编辑器中设置）：
- AttributeName：Modifier作用的属性名称，一个游戏效果对一个元素属性产生影响时，需要表明影响哪个属性
- ModiferMagnitude：基础模值，这个模值是否使用依赖于MMC的类型。后文MMC的类型中会详细介绍。
- Operation：Modifier的操作类型，有加法、乘法、赋值（覆写）等
- MMC：计算Modifier模值的计算类，后文MMC的类型中会详细介绍
```
例子：伤害效果，对单位造成50点伤害
GameplayEffectModifier：
    AttributeName：AS_Fight.Health
    ModifierMagnitude：50
    Operation：Add
    MMC：ScalableFloatModCalculation -> k=-1,b=0 (下文会介绍ScalableFloatModCalculation的k,b含义)
```

## MMC的类型
MMC的基类为抽象类ModifierMagnitudeCalculation，它的子类有以下几种：
- ScalableFloatModCalculation：可缩放浮点数计算
  - 该类型是根据ModifierMagnitude计算Modifier模值的，计算公式为：`ModifierMagnitude * k + b`
    实际上就是一个线性函数，k和b为可编辑参数，可以在编辑器中设置。
- AttributeBasedModCalculation：基于属性的计算
  - 该类型是根据属性值计算Modifier模值的，计算公式为：`AttributeValue * k + b`
    计算逻辑与ScalableFloatModCalculation一致。
  - 重点在于属性值的来源，确定属性值来源的参数有3个：
    - attributeFromType：属性值从谁身上取？是从游戏效果的来源（创建者），还是目标（拥有者）。
    - attributeName：属性值的名称，比如战斗属性集里的生命值：AS_Fight.Health
    - captureType：属性值的捕获类型
      - Track: 追踪,在Modifier被执行时，当场去取属性值
      - SnapShot: 快照,在游戏效果被创建时会对来源和目标的属性进行快照。在Modifier被执行时，去取快照的属性值。
- SetByCallerModCalculation：由调用者设置的计算
  - 不使用任何值计算模值，而是在执行时由调用者给出Modifier模值。
- CustomCalculation：自定义计算（必须继承自抽象基类ModifierMagnitudeCalculation）
  - 上述3种类型显然不够方便且全面的满足游戏开发者的所有需求，所以提供了自定义计算类的功能。
  - 允许开发者自由发挥给出各种各样的计算逻辑。