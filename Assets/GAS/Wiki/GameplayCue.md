# EX-GAS Wiki -- GameplayCue
>目前EX-GAS的GameplayCue功能还未完善。功能相对简陋。
## GameplayCue的作用
GameplayCue是一个用于播放游戏提示的类，它的作用是在游戏运行时播放游戏效果，比如播放一个特效、播放一个音效等。

## GameplayCue的原则
Cue是游戏提示，他必须遵守以下原则：
- _**Cue不应该对游戏的数值体系产生影响，比如不应该对游戏的属性进行修改，不应该对游戏的Buff进行修改等。**_
- _**Cue不应该对游戏玩法产生实际影响，比如即时战斗类的游戏，Cue不应该影响角色的位移、攻击等。**_

第一条原则是所有类型游戏必须遵守的。

而第二条原则就见仁见智了，因为游戏类型和玩法决定了cue的影响范围。
比如即时战斗类游戏，cue对角色位移有操作显然就是干涉了战斗，但如果是回合制游戏，cue对角色位移的操作就可以被当成是动画表现。
（甚至，即便是即时战斗类游戏，cue对角色位移的操作也可以被当成是动画表现，只要游戏开发人员认为cue的位移操作不影响游戏的战斗结果即可。）

## GameplayCue的类型
GameplayCue的类型分两大类：
- GameplayCueInstant：瞬时性的Cue，比如播放动画，伤害UI提示等
- GameplayCueDurational：持续性的Cue，比如持续性的特效、持续性的音效等

GameplayCueInstant和GameplayCueDurational都是抽象类，它们的子类才是真正的可使用Cue类。
Cue是需要程序开发人员大量实现的，毕竟游戏不同导致游戏提示千变万化。

### 关于Cue的子类实现
Cue的完整组成为GameplayCue和GameplayCueSpec：
- GameplayCue< T >（抽象基类,T为对应的Spec类）：Cue的数据实类，是一个可编辑类，开发人员可以在编辑器中设置Cue的各种参数。该类只可以被视作数据类。
  - 必须实现CreateSpec方法：用于创建对应的Spec类
- GameplayCueSpec（抽象基类）：Cue的规格类，是Runtime下Cue的真正实例，Cue的具体逻辑在该类中实现。
  - GameplayCueInstantSpec：瞬时性Cue的规格类
    - Trigger(): 必须实现的方法，用于触发Cue
  - GameplayCueDurationalSpec：持续性Cue的规格类
    - OnAdd(): 必须实现的方法，用于Cue被添加时的逻辑
    - OnRemove(): 必须实现的方法，用于Cue被移除时的逻辑
    - OnGameplayEffectActivated(): 必须实现的方法，用于Cue所属的GameplayEffect被激活时的逻辑
    - OnGameplayEffectDeactivated(): 必须实现的方法，用于Cue所属的GameplayEffect被移除时的逻辑
    - OnTick(): 必须实现的方法，用于Cue的每帧更新逻辑

### 关于Cue的参数传递
目前EX-GAS的Cue参数传递非常简陋，依赖于结构体GameplayCueParameters，成员如下：
- GameplayEffectSpec sourceGameplayEffectSpec：Cue所属的GameplayEffect实例（如果是GE触发）
- AbilitySpec sourceAbilitySpec：Cue所属的Ability实例（如果是Ability触发）
- object[] customArguments：自定义参数，不同于GameplayCue中的数据。
customArguments是供程序开发人员在业务逻辑内自由传递参数的载体。
>注意：customArguments是一个object数组，开发人员需要自己保证传递的参数类型正确，否则会导致运行时错误。
customArguments是最暴力的设计，往后EX-GAS的Cue参数传递设计还会进行优化。
## GameplayCue的使用
GameplayCue的使用手段很多，最基础的是在GameplayEffect中使用，Cue最开始的设计基础也是依附于GameplayEffect。Ability也可以对Cue进行操作。

除此之外，Cue的使用不限制于EX-GAS的体系内。开发者可以在任何地方使用Cue，只要能获取到GameplayCue的资源实例并且遵守Cue的原则即可。

### 在GameplayEffect中使用Cue
GameplayEffect中使用Cue会根据GameplayEffect执行策略产生变化。
- 即时执行的GameplayEffect: 提供以下选项
  - CueOnExecute（Instant）：Cue都会在GameplayEffect执行时触发。
- 持续执行的GameplayEffect: 提供以下选项
  - CueDurational（Durational）：生命周期完全和GameplayEffect同步
  - CueOnAdd（Instant）：GameplayEffect添加时触发
  - CueOnRemove（Instant）：GameplayEffect移除时触发
  - CueOnActivate（Instant）：GameplayEffect激活时触发。
  - CueOnDeactivate（Instant）：GameplayEffect失活时触发。

### 在Ability中使用Cue
AbilityAsset中提供了Instant和Durational两个选项的Cue参数。
但是Cue的使用完全依赖于Ability自身的业务逻辑，因此程序开发者在AbilitySpec中实现Cue逻辑时一定要保证合理性。
特别是对于Durational类型的Cue，一定要保证Cue生命周期的合理性，切记不要出现遗漏销毁Cue的情况。

