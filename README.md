# EX Gameplay Ability System For Unity
## 前言
该项目为Unreal Engine的Gameplay Ability System的Unity实现，目前实现了部分功能，后续会继续完善。

该项目完全开源，欢迎大家一起参与开发，提出建议，共同完善。可以基于该项目进行二次开发。
## ！！提醒！！请注意！！
__*该项目依赖Odin Inspector插件（付费），请自行解决!!!!!!!!*__

__*Odin Inspector插件请使用3.2+版本*__

```若没有方法解决，可以加反馈qq群（616570103），群内提供帮助 ```

目前EX-GAS没有非常全面的测试，所以存在不可知的大量bug和性能问题。所以对于打算使用该插件的朋友请谨慎考虑，当然我是希望更多人用EX-GAS，毕竟相当于是变相的QA。

但是要讲良心嘛，现在的EX-GAS算不上很稳定的版本，如果你是业余时间开发rpg类的独立游戏，开发时间十分充裕，我当然建议你试试EX-GAS，我会尽可能修复bug，提供使用上的帮助。

如果有好兄弟确实打算用，那么请务必加反馈群（616570103）。我会尽力抽时间修复提出的bug。

>我非常希望EX-GAS能早日稳定，为更多游戏提供支持帮助。

[//]: # (## 参考案例 [Demo]&#40;Assets/Demo&#41;)
## 入门教学案例系列文章
俯视角2D弹幕射击游戏
- PART.1 准备工作：https://zhuanlan.zhihu.com/p/688111182
- PART.2 基础逻辑：https://zhuanlan.zhihu.com/p/689241379
- PART.3 使用EX-GAS完成交互逻辑：https://zhuanlan.zhihu.com/p/689386644
- PART.4 使用EX-GAS进行润色：https://zhuanlan.zhihu.com/p/689660650

## 目录
- 1.[快速开始](#快速开始)
  - [安装](#安装)
  - [使用](#使用)
- 2.[GAS系统介绍](#GAS系统介绍)
  - [2.1 EX-GAS概述](#21-ex-gas概述)
  - [2.2 GameplayTag](#22-gameplaytag)
  - [2.3 Attribute](#23-attribute)
  - [2.4 AttributeSet](#24-attributeset)
  - [2.5 ModifierMagnitudeCalculation](#25-modifiermagnitudecalculation)
  - [2.6 GameplayCue](#26-gameplaycue)
  - [2.7 GameplayEffect](#27-gameplayeffect)
  - [2.8 Ability](#28-ability)
  - [2.9 AbilitySystemComponent](#29-abilitysystemcomponent)
- 3.[API && Source Code Documentation (W.I.P 施工中)](#3api--source-code-documentation)
- 4.[可视化功能](#4可视化功能)
  - [GAS Base Manager (GAS基础配置管理器)](#1-gas-base-manager-gas基础配置管理器)
  - [GAS Asset Aggregator (GAS配置资源聚合器)](#2-gas-asset-aggregator-gas配置资源聚合器)
  - [GAS Runtime Watcher (GAS运行时监视器)](#3-gas-runtime-watcher-gas运行时监视器)
- 5.[如果...我想...,应该怎么做?(持续补充)](#5如果我想应该怎么做wip)
- 6.[暂不支持的功能（可能有遗漏）](#6暂不支持的功能可能有遗漏)
- 7.[后续计划](#7后续计划)
- 8.[特别感谢](#8特别感谢)
- 9.[插件反馈渠道](#9插件反馈渠道)
## 1.快速开始
### 安装
1. 导入Odin Inspector插件(付费),Odin Inspector来源请自行解决。建议使用3.2+版本。
2. 导入本插件，建议以下两种方式：
- 使用Unity Package Manager安装
在Unity Package Manager中添加git地址:https://github.com/No78Vino/gameplay-ability-system-for-unity.git?path=Assets/GAS
>【国内镜像】https://gitee.com/exhard/gameplay-ability-system-for-unity.git?path=Assets/GAS
- 使用git clone本仓库[镜像同上]，然后将Assets/GAS文件夹拷贝到你的项目中即可

### 使用
GAS十分复杂，使用门槛较高。因为本项目是对UE的GAS的模仿移植，所以实现逻辑基本一致。建议先粗略了解一下UE版本的GAS整体逻辑，参考项目文档：https://github.com/BillEliot/GASDocumentation_Chinese

#### *使用流程*
1. 基础设置

在ProjectSetting中（或者Edit Menu栏入口：EX-GAS -> Setting），找到EX Gameplay Ability System的基本设置界面：

![S0X2@(E97LP_SWIJY2SJ@F3.png](Wiki%2FS0X2%40%28E97LP_SWIJY2SJ%40F3.png)

设置好以下两个路径
- 配置文件Asset路径: 这是该项目有关GAS的配置的路径，包括MMC,Cue,GameplayEffect,Ability,ASCPreset。
- 脚本生成路径: GAS的基础配置（Tag，Attribute，AttributeSet）都会有对应的脚本生成。

首次设置完路径后,点击检查子目录文件夹，确保必要的子文件夹都已生成。
>【生成AbilitySystemComponentExtension类脚本】这个按钮，请在生成了Attribute，AttributeSet，Ability的Lib集合类之后再点击。特别提醒，AbilitySystemComponentExtension是工具类，理论上只生成一次即可。


2. 配置Tag:  Tag是GAS核心逻辑运作的依赖,非常重要。关于Tag的使用及运作逻辑详见章节([GameplayTag](#22-gameplaytag))

3. 配置Attribute:  Attribute是GAS运行时数据单位。关于Attribute的使用及运作逻辑详见章节([Attribute](#23-attribute))

4. 配置AttributeSet:  AttributeSet是GAS运行时数据单位集合，合理的AttributeSet设计能够帮助程序解耦，提高开发效率。关于AttributeSet的使用及运作逻辑详见章节([AttributeSet](#24-attributeset))

5. 设计MMC,Cue:  详见[MMC](#25-modifiermagnitudecalculation), [GameplayCue](#26-gameplaycue)

6. 设计Gameplay Effect:  详见 [Gameplay Effect](#27-gameplayeffect)

7. 设计Ability:  详见 [Ability](#28-ability)

8. 设计ASC预设（可选）:  详见 [AbilitySystemComponent](#29-abilitysystemcomponent)

#### GAS的预缓存
- 为了解决EX-GAS框架中关于Type.Name造成的GC问题。
  _**可以在游戏内合适的地方（一般是游戏初始化阶段）调用GasCache.CacheAttributeSetName(GAttrSetLib.TypeToName)
  这行代码。他会将GAttrSetLib.TypeToName中的所有Key-Value对缓存起来，避开Type.Name的使用。**_
  如果你不调用这个方法，也不会对运行逻辑造成影响，不过会有额外的GC。

---
## 2.EX-GAS系统介绍
### 2.1 EX-GAS概述
>EX-GAS是对UnrealEngine的GAS（Gameplay Ability System）的模仿和实现。

GAS 是 "Gameplay Ability System" 的缩写，是一套游戏能力系统。
这个系统的目的是为开发者提供一种灵活而强大的框架，用于实现和管理游戏中的各种角色能力、技能和效果。

如果把EX-GAS高度概括为一句话，那就是：**WHO DO WHAT**。
- Who：AbilitySystemComponent（ASC）,EX-GAS的实例对象，是体系运转的基础单位
- Do：Ability，是游戏中可以触发的一切行为和技能
- What：GameplayEffect(GE)，掌握了游戏内元素的属性实际控制权，GameplayEffect本身应该理解为结果

GAS本质是一套属性数值的管理系统，GameplayCue我个人理解为附加价值（虽然这个附加价值很有分量）。
纵使GAS的Tag体系解决复杂的GameplayEffect和Ability的逻辑，但最终的结果目的也只是掌握属性数值变化。
而属性的最底层修改权力交由了GameplayEffect。所以我把GE理解为结果。

UE的GAS的使用门槛很高，这一点在我构筑完EX-GAS雏形后更是深有体会。
所以在EX-GAS的设计上，我尽可能的做简化，优化，来降低了使用门槛。
我制作了几个关键的编辑器，来帮助开发者快速的使用EX-GAS。
但即便如此，GAS本身的繁多参数依然让编辑器的界面看上去十分臃肿，这很难简化，没有哪个参数是可以被删除的。
甚至，雏形阶段的EX-GAS还有很多功能还未实现，也就是说还有更多的参数是没有被编辑器暴露出来的。

_**GAS的使用者必须至少有一名程序开发人员，因为GAS的使用需要编写大量自定义业务逻辑。
Ability，Cue，MMC等都是必须根据游戏类型和内容玩法而定的。
非程序开发人员则需要完全理解EX-GAS的运作逻辑，才能更好的配合程序开发人员快速配置出各种各样的技能，完善玩法表现。**_

### 2.2 GameplayTag
>Gameplay Tag,标签,它用于分类和描述对象的状态，非常有用于控制游戏逻辑。

- Gameplay Tag以树形层级结构（如Parent.Child.Grandchild）组织，用于描述角色状态/事件/属性/等，如眩晕（State.Debuff.Stun）。
- Gameplay Tag主要用于替代布尔值或枚举值，进行逻辑判断。
- 通常将Gameplay Tag添加到AbilitySystemComponent（ASC）以与系统（GameplayEffect，GameplayCue,Ability）交互。

Gameplay Tag在GAS中的使用涉及到标签的添加、移除以及对标签变化的响应。
开发者可以通过[GameplayTag Manager](#22a-gameplaytag-manager)在项目设置中管理这些标签，无需手动编辑配置文件。
Gameplay Tag的灵活性和高效性使其成为GAS中控制游戏逻辑的重要工具。
它不仅可以用于简单的状态描述，还可以用于复杂的游戏逻辑和事件触发。

>举个例子，GameplayEffect中有一个字段RequiredTags，其含义是当前GameplayEffect生效的AbilitySystemComponent（ASC）
需要拥有【所有】的RequiredTags（需求标签）。

上述例子，如果用传统的思路去做，可能需要写很多if-else判断，同时元素的实例脚本可能会增加很多状态标记的变量，
而且还需要考虑多个游戏效果的交互，这使得代码的设计和实现变得复杂，耦合。

GameplayTag的使用可以大大简化这些逻辑，使得代码更加清晰，易于维护。
他把状态和标记全部抽象成了一个独立的Tag系统，而且最巧妙的是树形结构的设计。
他解决了很多Gameplay设计上的问题，常见的问题比如：移除所有Debuff，传统的做法可能是让（中毒，减速，灼伤，等等）继承自Debuff类/接口；
而GameplayTag只需要添加一个Tag（中毒:Debuff.Poison，减速:Debuff.SpeedDown，灼伤:Debuff.Burning）

GameplayTag自身可以作为一个独立的系统去使用。
我在开发Demo的过程中就发现了GameplayTag的强大之处，他几乎替代了我的所有状态值。
甚至我设计了一个全局ASC，专门用来管理全局状态，我不需要对每个系统的状态管理，转而维护一个ASC即可。（虽然最后并没有落地这个设计，因为DEMO没有那么复杂。）

#### 2.2.a GameplayTag Manager
![QQ20240313114652.png](Wiki%2FQQ20240313114652.png)
我模仿了UE的GAS的Tag管理视图，做了树结构管理。通过选中节点，可以添加子节点，删除节点。要修改父子级关系时，只需要拖动节点即可。
Tag的分类设计需要谨慎，策划（设计师）需要再项目初就规划好Tag的大致分类。如果开发后期出现了Tag的父子级关系大变动，会严重影响原有游戏运作逻辑。

**【注意!!!】  每次编辑完Tag后，一定要点击右上角的【生成TagLib】按钮。GTagLib是包含了当前所有Tag的库类，
便于程序开发使用。GTagLib中Tag变量名会用‘_’代替‘.’ ，比如 State.Buff.PowerUp ->  State_Buff_PowerUp**

### 2.3 Attribute
>Attribute，属性，是GAS中的核心数据单位，用于描述角色的各种属性，如生命值，攻击力，防御力等。

Attribute和AttributeSet（属性集）需要结合起来才能作为唯一标识，简单点说AttributeSet是姓氏，Attribute是名字。
不同的AttributeSet可以有相同名字的Attribute，但是同一组的AttributeSet不可以有相同名字的Attribute。
常见的情况，如下：
> AttributeSet 人物: 生命值, 法力, 攻击力, 防御力
> 
> AttributeSet 武器: 生命值（耐久度）, 攻击力, 防御力
> 
> 而这两组AttributeSet中的生命值，攻击力，防御力，都是不同的属性，他们的意义和作用不同。但他们可以属于同一个单位。
#### 2.3.a Attribute Manager
![QQ20240313115953.png](Wiki%2FQQ20240313115953.png)
Attribute Manager的作用很简单，提供属性名字的管理。然后作为AttributeSet的选项使用。

**【注意!!!】  每次编辑完Attribute后，一定要点击【生成AttrLib】按钮。
只有AttrLib生成后，AttributeSet的Attribute选项才会发生改变。**

### 2.4 AttributeSet
>AttributeSet，属性集，是GAS中的核心数据单位集合，用于描述角色的某一类别的属性集合。

在上文的[2.3 Attribute]中，我们提到了AttributeSet是姓氏，Attribute是名字。二者结合起来才能作为唯一标识。
而对于AttributeSet的设计，可以较为随意，大多数情况，大家会更乐意一个单位只有一个AttributeSet。
因为这样便于管理和分类，不同类别的单位直接使用不同的AttributeSet。但实际上一个单位是可以拥有复数AttributeSet。
我其实比较认同一个单位只有一个AttributeSet的设计，因为这对程序开发也是好事，逻辑处理会更简单直白。

配置时的注意项：
- AttributeSet的名字禁止重复或空。这是因为AttributeSet的名字会作为类名。
- AttributeSet内的Attribute禁止重复。

#### 2.4.a AttributeSet Manager
![QQ20240313121300.png](Wiki%2FQQ20240313121300.png)
AttributeSet Manager统筹属性集的命名和属性管理。

**【注意!!!】  每次编辑完后，一定要点击【生成AttrSetLib】按钮。AttrSetLib会在AbilitySystemComponent的预设配置中用到。
AttrSetLib.gen.cs脚本中会包含所有的AttributeSet类（详见下文API章节），AttributeSet对应的类名是：“AS_名字”。
比如AttributeSet名字是Fight，那它对应的类名是AS_Fight。**

### 2.5 ModifierMagnitudeCalculation
>ModifierMagnitudeCalculation，修改器，负责GAS中Attribute的数值计算逻辑。

MMC(下文开始会使用缩写指代ModifierMagnitudeCalculation)唯一的使用场景是在GameplayEffect中。
GAS中，体系内运作的情况下，只有GameplayEffect才能修改Attribute的数值。而GameplayEffect就是通过MMC修改Attribute的数值。

MMC具有以下特点：
- 【与Attribute集成】： MMC 与 GAS 中的Attribute系统一起使用。这意味着计算效果幅度时可以考虑角色的属性，使得效果的强度与角色的属性值相关联。
- 【实时性】： MMC 用于在运行时动态计算 Attribute 的修改幅度。这样可以根据角色的状态、属性或其他因素，实时地调整效果的强度。
- 【自定义】： 通过继承 MMC的基类，开发者可以实现自定义的计算逻辑。这允许在计算效果幅度时考虑复杂的游戏逻辑、属性关系或其他条件。
- 【复用性】： 由于 MMC 是一个独立的类(Scriptable Object)，开发者可以在多个 GameplayEffect 中重复使用相同的计算逻辑。这样可以确保在整个游戏中保持一致的效果计算。
- 【灵活性】： 使用 MMC 提高了系统的灵活性，使得效果的强度不再是固定的数值，而可以根据需要在运行时进行调整，适应不同的游戏情境和需求。

MMC在GameplayEffect中的运作逻辑，结合GameplayEffect配置中MMC界面来解释。如下图：

![QQ20240313145154.png](Wiki%2FQQ20240313145154.png)

MMC被存储在Modifier中，Modifier是GameplayEffect的一部分。
Modifier包含了修改的属性，幅值（Magnitude），操作类型和MMC。
- 修改的属性：指的是GameplayEffect作用对象被修改的属性。可以看到属性名是“AS_Fight.POSTURE”，这对应了上文的提到的属性识别是AttrSet和Attr组合而成的。
- 幅值Magnitude：修改器的基础数值。这个数值如何使用由MMC的运行逻辑决定。
- 操作类型：是对属性的操作类型，有3种：
  - Add ： 加法（取值为负便是减法）
  - Multiply： 乘法（除法取倒数即可）
  - Override：覆写属性值
- MMC：计算单位，Modifier的核心，是一个ScriptableObject。MMC的类别如下：
  - ScalableFloatModCalculation：可缩放浮点数计算
    - 该类型是根据Magnitude计算Modifier模值的，计算公式为：`ModifierMagnitude * k + b`
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
    - 通过对GameplayEffectSpec注册数值来实现设置值。
    - 设置数值映射有2种：
      - 自定义键值：通过GameplayEffectSpec的RegisterValue(string key, float value)
      - GameplayTag：通过GameplayEffectSpec的RegisterValue(GameplayTag tag, float value)
  - CustomCalculation：自定义计算（必须继承自抽象基类ModifierMagnitudeCalculation）
    - 上述3种类型显然不够方便且全面的满足游戏开发者的所有需求，所以提供了自定义计算类的功能。
    - 允许开发者自由发挥给出各种各样的计算逻辑。

### 2.6 GameplayCue
>目前EX-GAS的GameplayCue功能还未完善。功能相对简陋。
- 【GameplayCue的作用】：GameplayCue是一个用于播放游戏提示的类，它的作用是在游戏运行时播放游戏效果，比如播放一个特效、播放一个音效等。
- 【GameplayCue的原则】： Cue是游戏提示，他必须遵守以下原则：
  - _**Cue不应该对游戏的数值体系产生影响，比如不应该对游戏的属性进行修改，不应该对游戏的Buff进行修改等。**_
  - _**Cue不应该对游戏玩法产生实际影响，比如即时战斗类的游戏，Cue不应该影响角色的位移、攻击等。**_

>第一条原则是所有类型游戏必须遵守的。
>
>而第二条原则就见仁见智了，因为游戏类型和玩法决定了cue的影响范围。
比如即时战斗类游戏，cue对角色位移有操作显然就是干涉了战斗，但如果是回合制游戏，cue对角色位移的操作就可以被当成是动画表现。
（甚至，即便是即时战斗类游戏，cue对角色位移的操作也可以被当成是动画表现，只要游戏开发人员认为cue的位移操作不影响游戏的战斗结果即可。）
- 【GameplayCue的类型】 GameplayCue的类型分两大类：
  - GameplayCueInstant：瞬时性的Cue，比如播放动画，伤害UI提示等
  - GameplayCueDurational：持续性的Cue，比如持续性的特效、持续性的音效等
>GameplayCueInstant和GameplayCueDurational都是抽象类，它们的子类才是真正的可使用Cue类。
Cue是需要程序开发人员大量实现的，毕竟游戏不同导致游戏提示千变万化。

- 【关于Cue的子类实现】： Cue的完整组成为GameplayCue和GameplayCueSpec：
  - GameplayCue< T >（抽象基类,T为对应的Spec类）：Cue的数据实类，是一个可编辑类，开发人员可以在编辑器中设置Cue的各种参数。该类只可以被视作数据类。
    - 必须实现CreateSpec方法：用于创建对应的Spec类
  - GameplayCueSpec（抽象基类）：Cue的规格类，是Runtime下Cue的真正实例，Cue的具体逻辑在该类中实现。
    - | Spec类|需要实现的方法| 方法触发时机   |
      |---|---|----------|
      |GameplayCueInstantSpec<br/>瞬时性Cue的规格类|Trigger()| 执行时触发|
      |GameplayCueDurationalSpec<br/>持续性Cue的规格类|OnAdd()| Cue被添加时触发|
      |GameplayCueDurationalSpec|OnRemove()| Cue被移除时触发|
      |GameplayCueDurationalSpec|OnGameplayEffectActivated()| Cue所属的GameplayEffect被激活时触发|
      |GameplayCueDurationalSpec|OnGameplayEffectDeactivated()| Cue所属的GameplayEffect被移除时触发|
      |GameplayCueDurationalSpec|OnTick()| Cue的每帧更新逻辑|

- 【关于Cue的参数传递】： 目前EX-GAS的Cue参数传递非常简陋，依赖于结构体GameplayCueParameters，成员如下：
  - GameplayEffectSpec sourceGameplayEffectSpec：Cue所属的GameplayEffect实例（如果是GE触发）
  - AbilitySpec sourceAbilitySpec：Cue所属的Ability实例（如果是Ability触发）
  - object[] customArguments：自定义参数，不同于GameplayCue中的数据。
    customArguments是供程序开发人员在业务逻辑内自由传递参数的载体。
>注意：customArguments是一个object数组，开发人员需要自己保证传递的参数类型正确，否则会导致运行时错误。
customArguments是最暴力的设计，往后EX-GAS的Cue参数传递设计还会进行优化。
- 【GameplayCue的使用】：
GameplayCue的使用手段很多，最基础的是在GameplayEffect中使用，Cue最开始的设计基础也是依附于GameplayEffect。Ability也可以对Cue进行操作。
除此之外，Cue的使用不限制于EX-GAS的体系内。开发者可以在任何地方使用Cue，只要能获取到GameplayCue的资源实例并且遵守Cue的原则即可。
  - 在GameplayEffect中使用Cue 
    - GameplayEffect中使用Cue会根据GameplayEffect执行策略产生变化。
      - | GameplayEffect类型 |Cue名称|Cue类别| 执行时机                    |
        |------------------|---|---|-------------------------|
        | Instant          |CueOnExecute|Instant| GameplayEffect执行时触发     |
        | Durational       |CueDurational|Durational| 生命周期完全和GameplayEffect同步 |
        | Durational       |CueOnAdd|Instant| GameplayEffect添加时触发     |
        | Durational       |CueOnRemove|Instant| GameplayEffect移除时触发     |
        | Durational       |CueOnActivate|Instant| GameplayEffect激活时触发     |
        | Durational       |CueOnDeactivate|Instant| GameplayEffect失活时触发     |

  - 在Ability中使用Cue
    - Ability种Cue的使用完全依赖于Ability自身的业务逻辑，因此程序开发者在AbilitySpec中实现Cue逻辑时一定要保证合理性。
      特别是对于Durational类型的Cue，一定要保证Cue生命周期的合理性，切记不要出现遗漏销毁Cue的情况。
    
### 2.7 GameplayEffect
>GameplayEffect是EX-GAS的核心之一，一切的游戏数值体系交互基于GameplayEffect。

GameplayEffect掌握了游戏内元素的属性控制权。理论上，只有它可以对游戏内元素的属性进行修改
（这里指的是修改，数值的初始化不算是修改）。当然，实际情况下，游戏开发人员当然可以手动直接修改属性值。
但是还是希望游戏开发者尽可能的不要打破EX-GAS的数值体系逻辑，因为过多的额外操作可能会导致游戏的数值体系变得混乱，难以追踪数值变化等等。

另外GameplayEffect还可以触发Cue（游戏提示）完成游戏效果的表现，以及控制获取额外的能力等。

- GameplayEffect的使用
![QQ20240313152015.png](Wiki%2FQQ20240313152015.png)

GameplayEffect的配置界面如图，接下来逐一解释各个参数的含义。
  - Name
    - GameplayEffect的名称，纯粹用于显示，不会影响游戏逻辑。方便编辑者区分GameplayEffect。
  - Description
    - GameplayEffect的描述，纯粹用于显示，不会影响游戏逻辑。方便编辑者阅读理解GameplayEffect。
  - DurationPolicy：GameplayEffect的执行策略，有以下几种：
    - | 策略类型 | 执行逻辑                                                                                                | 
        |---|-----------------------------------------------------------------------------------------------------|
        | Instant | 即时执行，GameplayEffect被添加时立即执行，执行完毕后销毁自身。                                                              |
        | Duration | 持续执行，GameplayEffect被添加时立即执行，持续时间结束后移除自身。                                                            |
        | Infinite | 无限执行，GameplayEffect被添加时立即执行，执行完毕后不会移除，需要手动移除。                                                       |
  - Duration
    - 持续时间，只有DurationPolicy为Duration时有效。
  - Every(Period)
    - 周期，只有DurationPolicy为Duration或者Infinite时有效。每隔Period时间执行一次PeriodExecution。
  - PeriodExecution
    - 周期执行的GameplayEffect，只有DurationPolicy为Duration或者Infinite，且Period>0时有效。每隔Period时间执行一次PeriodExecution。
      _**PeriodExecution禁止为空!!**_PeriodExecution原则上只允许是Instant类型的GameplayEffect。但如果根据开发者需求，也可以使用其他类型的GameplayEffect。
  - GrantedAbilities
    - 授予的能力，只有DurationPolicy为Duration或者Infinite时有效。在GameplayEffect生命周期内，GameplayEffect的持有者会被授予这些能力。
        GameplayEffect被移除时，这些能力也会被移除。具体详见[GrantedAbility](#28c-granted-ability-from-gameplayeffect-来自游戏效果授予的能力)
  - Modifiers: 属性修改器。详见[MMC](#25-modifiermagnitudecalculation)
- Tags：标签。Tag具有非常重要的作用，合适的tag可以处理GameplayEffect之间复杂的关系。
  - | Tag类型 | 作用                                                                                   |
    |---|----------------------------------------------------------------------------------------|
    | AssetTags | 描述性质的标签，用来描述GameplayEffect的特性表现，比如伤害、治疗、控制等。 |
    | GrantedTags | GameplayEffect的持有者会获得这些标签，GameplayEffect被移除时，这些标签也会被移除。Instant类型的GameplayEffect的GrantedTags是无效的。 |
    | ApplicationRequiredTags | GameplayEffect的目标单位必须拥有【所有】这些标签，否则GameplayEffect无法被施加到目标身上。 |
    | OngoingRequiredTags | GameplayEffect的目标单位必须拥有【所有】这些标签，否则GameplayEffect不会被激活。 （施加和激活是两个概念，如果已经被施加的GameplayEffect持续过程中，目标的tag变化了，不满足，效果就会失活；满足了，就会被激活）。Instant类型的GameplayEffect的OngoingRequiredTags是无效的。|
    | RemoveGameplayEffectsWithTags | GameplayEffect的目标单位当前持有的所有GameplayEffect中，拥有【任意】这些标签的GameplayEffect会被移除。 |
    | Application Immunity Tags | GameplayEffect的目标单位拥有【任意】这些标签，就对该GameplayEffect免疫。 |
  - DurationPolicy为Instant时
    - CueOnExecute（Instant）：GameplayEffect执行时触发。
  - DurationPolicy为Duration或者Infinite时
    - CueDurational（Durational）：生命周期完全和GameplayEffect同步
    - CueOnAdd（Instant）：GameplayEffect添加时触发
    - CueOnRemove（Instant）：GameplayEffect移除时触发
    - CueOnActivate（Instant）：GameplayEffect激活时触发。
    - CueOnDeactivate（Instant）：GameplayEffect失活时触发。
- Stacking:堆叠。该系列参数是为了处理常见的叠层类型Buff。比如《黑帝斯》中酒神，爱神，冬神的叠攻buff。stacking的参数基本囊括了绝大多数的叠层型buff的设计。
  - 生效的GE类型：只有非Instant类型（持续型）的GameplayEffect，可以产生叠加（stacking）。
  - stackingCodeName: 堆叠GE的唯一标识码，用于可堆叠GE的识别。
    - 本身是字符串，但是runtime实际使用的是其对应的HashCode。如果为空，则视为不可堆叠
    - stackingCodeName除了基础的堆叠类GE识别功能外，另一个作用是用于支持不同GE的共同堆叠。举个例子：有一个团队性质的增伤buff【元素增伤】，团队所有成员对同一个目标都可以叠加【元素增伤】，至多10层，增伤
      随层数增加而增加。但是增伤是指定第一个施加buff成员的元素，比如第一层打的是【火增伤】，那么之后不管是【水增伤】，【雷增伤】，都是【火增伤】buff往上叠加。遇到这种特殊情况，就可以把【水增伤】，【雷增伤】，【火增伤】
      的stackingCodeName设置为同一个值，这样就可以实现【元素增伤】的共同堆叠。
  - stackingType：GameplayEffect的叠加类型，有三种：
    -  | stacking类型 | 作用                                                                                                                                                               |
       |---|------------------------------------------------------------------------------------------------------------------------------------------------------------------|
       | None | 不叠加                                                                                                                                                              |
       | AggregateBySource | 基于GE来源（ASC）的叠加计数，所有释放单位各自管理一个叠加计数的GE。 举例：BUFF【聚能】效果是单位被叠加三次该buff（来自同一单位）后触发爆炸。 小怪被A玩家叠了2次【聚能】，然后B玩家又对小怪施加了1次【聚能】，但是不会触发爆炸。因为叠加计数是按来源单位各自计数，需要A再叠1次或者B叠2次，小怪才会爆炸。 |
       | AggregateByTarget | 基于GE目标（ASC）的叠加计数，所有释放单位共享一个叠加计数的GE。举例：BUFF【诅咒】效果是单位被叠加3次该buff（无关来源单位）后触发即死效果。经典魂游的咒蛙攻击buff。玩家被数只咒蛙围攻，只要被咒蛙打到3次就死亡。在场所有咒蛙的【诅咒】都会叠加在玩家身上一个计数器上。                    |
  - limitCount：叠加上限。
    - 需要注意一点，叠加溢出的效果触发是在叠加计数【大于】limitCount时触发。举个例子，如果某个buff叠加3层后触发爆炸伤害，那limitCount应该是2。
  - DurationRefreshPolicy：持续时间刷新策略。GE叠加成功后，GE的持续时间的刷新策略。
    - | DurationRefreshPolicy | 作用                                    |
      |-----------------------|---------------------------------------|
      | NeverRefresh                  | 从不刷新持续时间。即叠加的BUFF持续时间从第一层生效后计时就不再受影响。 |
      | RefreshOnSuccessfulApplication | 每次Effect叠加apply成功后刷新Effect的持续时间。      |
  - PeriodResetPolicy：周期重置策略。GE叠加成功后，GE的周期（Period）的刷新策略。
    -  | PeriodResetPolicy | 作用                       |
       |-----------------------|--------------------------|
       | NeverReset                  | 从不重置周期。                  |
       | ResetOnSuccessfulApplication | 每次apply成功后重置Effect的周期计时。 |
  - ExpirationPolicy：过期策略（持续时间结束时逻辑处理）。GE叠加成功后，GE的过期时间（Expiration）的刷新策略。
    - | ExpirationPolicy | 作用                                      |
      |-----------------------|-----------------------------------------|
      | ClearEntireStack                  | 持续时间结束时,清楚所有层数                          |
      | RemoveSingleStackAndRefreshDuration | 持续时间结束时减少一层，然后重新经历一个Duration，一直持续到层数减为0 | 
      | RefreshDuration | 持续时间结束时,再次刷新Duration，这相当于无限Duration。    | 
  - denyOverflowApplication：布尔类型。是否允许溢出的GE叠加生效。
    - 对应于DurationRefreshPolicy = RefreshOnSuccessfulApplication时，如果为true则多余的Apply不会刷新Duration
  - clearStackOnOverflow: 布尔类型。是否溢出时清空所有层数，移除GE。
    - 当DenyOverflowApplication为True是才有效，当Overflow时是否直接删除所有层数，移除GE。
  - overflowEffects:GameplayEffect的数组，溢出时施加的游戏效果。当Stack计数溢出时，对生效单位执行这些GE。

> GameplayEffect的施加（Apply）和激活（Activate）
>   - GameplayEffect的施加（Apply）和激活（Activate）是两个概念，施加是指GameplayEffect被添加到目标身上，激活是指GameplayEffect实际生效。
>      - 为什么做区分？
>      - 举个例子：固有被动技能（Ability）是持续回血，被动技能的逻辑显然是永久激活的状态，而持续回血的效果（GameplayEffect）
>        来源于被动技能，那如果单位受到了外部的debuff禁止所有的回血效果，那么是不是被动技能被禁止？显然不是，被动技能还是会持续激活的。
>        那应该是移除回血效果吗？显然也不是，被动技能整个过程是不做任何变化，如果移除回血效果，那debuff一旦消失，谁再把回血效果加回来？
>        所以，这里需要区分施加和激活，被动技能的持续回血效果被施加到单位身上，而debuff做的是让回血效果失活，而不是移除回血效果，一旦debuff结束，
>        回血效果又被激活，而这个激活的操作可以理解为回血效果自己激活的（依赖于Tag系统）。

### 2.8 Ability
> Ability是EX-GAS的核心类之一，它是游戏中的所有能力基础。
>
> 同时Ability也是程序开发人员最常接触的类，Ability的完整逻辑都是由程序开发人员实现的。

在EX-GAS内，Ability是游戏中可以触发的一切行为和技能。多个Ability可以在同一时刻激活, 例如移动和持盾防御。
Ability作为EX-GAS的核心类之一，他起到了Do（做）的功能。

Ability的业务逻辑取决于游戏类型和玩法。所以不存在一个通用的Ability模板，当然可以针对游戏类型制作一些通用的ability。
Ability的逻辑并非自由，如果胡乱的实现Ability逻辑，可能会导致游戏逻辑混乱，所以需要遵循一些规则。

Ability的具体实现需要策划和程序配合。
这并不是废话，而是在EX-GAS的Ability制作流程中，确确实实的把策划和程序的工作分开了：
- 策划的工作：配置AbilityAsset
- 程序的工作：编写Ability（AbilityAsset,Ability,AbilitySpec）类
- | 类                        | 功能                                                                                |
  |--------------------------|-----------------------------------------------------------------------------------|
  | AbilityAsset  | Ability的配置文件，同一类的Ability可以通过配置不同的AbilityAsset参数，实现复数Ability，比如跳跃段数不同，可以实现普通跳，二段跳。 |
  | Ability  | Ability的Runtime数据类，通常数据还是依赖AbilityAsset。同时也允许运行时不依赖AbilityAsset生成Ability          |
  | AbilitySpec   | Ability的运行实例，Ability的游戏内的表现逻辑，就在该类中实现。                                            |
>建议 AbilitySpec和Ability在同一个脚本中编辑，因为二者本身就是成对出现。AbilityAsset单独一个脚本，因为它是Scriptable Object，应该遵从脚本名和类一致的原则。

Ability运作逻辑的组成可以拆成两部分：
- GAS系统内的运作逻辑：所有Ability通用的数据字段，被保存在AbstractAbility这个抽象基类中。所有的Ability都是继承自AbstractAbility。
- 具体游戏内的表现逻辑：每个Ability都有自己的表现逻辑，这部分逻辑是由程序开发人员自行实现的。

接下来结合Ability的配置界面来解释Ability的数据和运作逻辑。
#### 2.8.a Ability编辑界面
![QQ20240313162642.png](Wiki%2FQQ20240313162642.png)
>图中A的部分就是GAS系统内的运作逻辑的参数，B的部分就是具体游戏内的表现逻辑的参数。

注意到最上方显示了Ability Class，这是Ability的类名，与之成对的AbilitySpec决定了Ability游戏内的表现逻辑。
- GAS系统内的运作逻辑参数
  - U-Name: Ability的名称，唯一标识符，禁止重复或空。U-Name会用于AbilityLib的Ability生成和查找。
  - Description: Ability的描述，纯粹用于显示，不会影响游戏逻辑。方便编辑者区分Ability。
  - 消耗Cost：Ability的消耗GameplayEffect
  - 冷却CD：Ability的冷却GameplayEffect
  - CD时间：冷却时间，它会覆盖冷却GameplayEffect的持续时间。
  - Tags: Ability的标签,与GameplayEffect的Tag些许类似。具体Tag功能如下

| Tag                      | 功能                                                                |
|--------------------------|-------------------------------------------------------------------|
| Asset Tag                | 描述性质的标签，用来描述Ability的特性表现，比如伤害、治疗、控制等                              |
| CancelAbility With Tags  | Ability激活时，Ability持有者当前持有的所有Ability中，拥有**【任意】**这些标签的Ability会被取消   |
| BlockAbility With Tags       | Ability激活时，Ability持有者当前持有的所有Ability中，拥有**【任意】**这些标签的Ability会被阻塞激活 |
| Activation Owned Tags    | Ability激活时，持有者会获得这些标签，Ability被失活时，这些标签也会被移除                       |
| Activation Required Tags | Ability只有在其拥有者拥有**【所有】**这些标签时才可激活                                 |
| Activation Blocked Tags  | Ability在其拥有者拥有**【任意】**这些标签时不能被激活                                       |
   
- 具体游戏内的表现逻辑参数
  - 这部分的参数面板是由程序开发人员自行实现的，这部分参数的含义和作用不固定。
  - 建议使用Odin的特性编辑，一是为了规范，二是美观。 
  - 不同类的Ability，这部分的面板显示和含义不同。如下图：
  ![QQ20240313165819.png](Wiki%2FQQ20240313165819.png)


- **【！！注意！！】**
  - **当创建或修改AbilityAsset的U-Name后，一定要去Ability汇总界面点击【生成AbilityLib】按钮。
  AbilityLib.gen.cs脚本中会包含所有的Ability信息。
  游戏运行时生成Ability依赖于AbilityLib，如果没有保持同步，会导致游戏运行时找不到Ability。**
  - Ability汇总界面入口：在菜单栏EX-GAS -> Asset Aggregator -> 左侧菜单列点击 C-Ability
![QQ20240313175247.png](Wiki%2FQQ20240313175247.png)
  
#### 2.8.b TimelineAbility 通用性Ability（W.I.P TimelineAbility还在完善中）
在实际的开发过程中，我发现，许多的Ability都有顺序和时限两个特点。
每次都新写一个Ability类来实现某个指定技能让我十分烦躁，于是我制作了TimelineAbility，一个极具通用性的顺序，时限Ability。
![TimelineAbilityAsset.png](Wiki%2FTimelineAbilityAsset.png)
>这是TimelineAbilityAsset的面板。

TimelineAbilityAsset的大多数表现逻辑参数在AbilityAsset面板都是隐藏的（HideInInspector）。
转而都是在TimelineAbilityEditor面板中可视化编辑。
唯一在AbilityAsset面板中可见的参数是【手动结束能力】的bool值选项。这个选项决定Ability是手动结束还是播放完成后自动结束。

通过点击【查看/编辑能力时间轴】按钮，可以打开TimelineAbilityEditor面板。
![TimelineAbilityEditor.png](Wiki%2FTimelineAbilityEditor.png)
>这是TimelineAbilityEditor的面板。

接下来详细介绍TimelineAbilityEditor的面板参数含义及操作逻辑。
- 顶部菜单栏
    ![QQ20240315133711.png](Wiki%2FQQ20240315133711.png)
    - Ability配置 ： 当前编辑的TimelineAbilityAsset
    - 查看能力基本信息：点击按钮后，右侧的子Inspector会显示TimelineAbilityAsset的面板信息，和Asset Aggregator中的AbilityAsset面板一致。
    - 预览实例：场景内的GameObject. 选取后，TimelineAbility会以该GameObject为参照物，预览TimelineAbility的表现逻辑。
      用到的常见预览有Animation，特效，物体挂载，伤害碰撞盒等等。
    - 预览场景：点击该按钮后会，进入空场景。目前还没有自定义LookDev场景，所以只是一个临时创建空场景。
    - 返回旧场景：点击该按钮后会，返回到原来的场景。
    - 显示子Inspector：点击该按钮后会，刷新显示右侧的子Inspector的布局。
- 时间轴编辑部分
  - 左侧播放栏
    ![QQ20240315143604.png](Wiki%2FQQ20240315143604.png)
    - 【<】:上一帧，只有当预览实例不为空时，才会生效。
    - 【>】:下一帧，只有当预览实例不为空时，才会生效。
    - 【▶】：播放/暂停 ，只有当预览实例不为空时，才会生效。
    - 左侧帧数：当前预览帧数，只有当预览实例不为空时，才会生效。
    - 右侧帧数：Ability执行总帧数
  - 左侧轨道菜单栏
    - ![QQ20240315144012.png](Wiki%2FQQ20240315144012.png)
    - TimelineAbility基础轨道有6种。【添加轨道只需点击右侧的‘+’，删除轨道只需右键对应轨道选择Delete Track即可】
      1. Instant Cue 【即时Cue轨道】
         - 轨道Item类型：Mark ![QQ20240315151141.png](Wiki%2FQQ20240315151141.png)
         - 一个Mark可以挂多个Cue。理论上，一个TimelineAbility只需要一条Instant Cue轨道。
         ![QQ20240315152528.png](Wiki%2FQQ20240315152528.png)
         - 扩展：详见上文中提到的Instant Cue自定义实现
      2. Release Effect【GameplayEffect释放轨道】
         - 轨道Item类型：Mark
         - 一个Mark持有一个TargetCatcher和数个GameplayEffectAsset
           ![QQ20240315153247.png](Wiki%2FQQ20240315153247.png)
           -  TargetCatcher：GameplayEffect释放需要对象，而TargetCatcher的作用就是找到这些对象。
              TargetCatcher固有初始化会获取Owner（ASC），核心是方法CatchTargets()。 基类如下：
           ```
             public abstract class TargetCatcherBase
             {
                public AbilitySystemComponent Owner;
                public TargetCatcherBase()
                {
                }
                public virtual void Init(AbilitySystemComponent owner)
                {
                    Owner = owner;
                }
                // mainTarget为TimelineAbility的指向性目标单位，为可选参数。具体在API中会介绍。
                public abstract List<AbilitySystemComponent> CatchTargets(AbilitySystemComponent mainTarget);
             }
           ```
             我提供了几个基础TargetCatcher（后续会陆续添加常用的Catcher）
         
           - 
            | Catcher名          | 作用                 |
            |-------------------|--------------------|
            | CatchSelf         | 捕捉自己               |
            | CatchTarget       | 捕捉指向性目标            |
            | CatchAreaBox2D    | 捕捉2d矩形内的目标（适用2D游戏） |
            | CatchAreaCircle2D | 捕捉2d圆形内的目标（适用2D游戏） |
           - TargetCatcher的UI面板绘制：
            自定义TargetCatcher的UI面板需要继承自TargetCatcherInspector<T> T为TargetCatcher类
            **（必须直接继承，不可以多级继承，因为Inspector的Type查找规则依赖第一泛型类做匹配）**
         - Release Effect的执行逻辑：先调用TargetCatcher的CatchTargets()方法，然后对捕获的目标单位施加所有指定GameplayEffect。
      3. Instant Task 【即时Task轨道】
         - 轨道Item类型：Mark
         - 一个Mark可以挂载复数的Instant Task。 关于Ability Task的详细介绍见[下文](#375-abilitytaskwip)。
           ![QQ20240315170234.png](Wiki%2FQQ20240315170234.png)
         - Task是自定义事件，可以是任何游戏逻辑，纯粹由开发者决定。
         - Instant Task的面板绘制：自定义Task的UI面板需要继承自InstantTaskInspector<T> T为InstantAbilityTask类
           **（必须直接继承，不可以多级继承，因为Inspector的Type查找规则依赖第一泛型类做匹配）**
      4. Durational Cue【持续GameplayCue轨道】
         - 轨道Item类型：Clip ![QQ20240315164448.png](Wiki%2FQQ20240315164448.png)
         - 每段Clip只含一个Duration Cue ![QQ20240315164632.png](Wiki%2FQQ20240315164632.png)
         - **注意！！ TimelineAbility下的持续性Cue，
           只会执行OnAdd（Cue播放的第一帧），OnRemove（Cue播放的最后一帧），OnTick，和GameplayEffect相关的方法不会被执行**
      5. Buff【Buff轨道】
         - 轨道Item类型：Clip
         - 每段Clip只含一个Buff（GameplayEffect），且Buff的作用对象只会是Ability的持有者自己。
           ![QQ20240315165106.png](Wiki%2FQQ20240315165106.png)
         - **【注意！！】请确保设置的GameplayEffect类型为Durational或Infinite。
            非持续类型的GameplayEffect不会生效。且GameplayEffect执行时会设置为Infinite执行策略，
            生命周期由Clip长度（Duration）决定。**
      6. Ongoing Task【持续Task轨道】
         - 轨道Item类型：Clip
         - 每段Clip只含一个Ongoing Task。 关于Ability Task的详细介绍见[下文](#)。
           ![QQ20240315170648.png](Wiki%2FQQ20240315170648.png)
         - Ongoing Task的面板绘制：自定义Task的UI面板需要继承自OngoingTaskInspector<T> T为OngoingAbilityTask类
           **（必须直接继承，不可以多级继承，因为Inspector的Type查找规则依赖第一泛型类做匹配）**
      
  - 右侧Inspector
    - 所有Track，TrackItem参数在点击后都会显示在Inspector上

TimelineAbility的执行逻辑很直观，就是沿着时间轴从左往右执行，每个轨道的Item都会在对应的时间点执行。
所有事件的执行，都遵照时间轴的顺序，以及Mark内参数排列顺序。如果存在前后逻辑关系，那么配置的时候请务必注意顺序。
最大帧数决定了Ability的执行时间，如果【手动结束能力】设置为false，那么在播放完TimelineAbility后，会自动调用TryEndAbility()，
反之，需要开发人员在代码中决定调用TryEndAbility()的时机。

TimelineAbility的配置可能还满足不了一些设计时，程序开发人员可以对TimelineAbility进行继承，拓展功能需求。
> 特别是在TargetCatcher，AbilityTask的自定义上，还是很可能遇到这个问题。
> 因为TargetCatcher和AbilityTask的持续化存储是以JsonData的格式，ScriptableObject类型参数的Json存储是存在GUID不匹配问题的。
> 所以，TargetCatcher和AbilityTask的参数中，不建议出现ScriptableObject类型的参数。

#### 2.8.c Granted Ability From GameplayEffect 来自游戏效果授予的能力
能力不仅仅可以由AbilitySystemComponent直接授予，还可以通过GE来授予,甚至是GE来全权控制。

我们为了更通俗的去理解BUFF的概念，就必须允许GE可以实现自由的逻辑自定义。
但是GAS本身的GE仅仅是遵循体系内固定逻辑，不存在开发者自定义GE逻辑。
GAS为GE提供了Granted Ability的解决方案。
> 为了更好理解这种情况，举个例子：
> 
> 在一个RPG游戏中，有一个名为“亡灵收割”的BUFF。
> “亡灵收割”效果为：BUFF持有者，在x米范围内，每当有单位死亡便获得y点生命值。
> 
> 一般的做法可能是把“亡灵收割”视作一个被动技能（Ability）,然后根据设计需求动态的添加/移除/激活/失活“亡灵收割”效果。
> 可能有些设计者为了使之更符合BUFF的逻辑，会通过GE的Add/Remove/Active/Deactive回调，来关联“亡灵收割”添加/移除/激活/失活。

上述例子的这个做法当然是没问题的，而且十分合理。 
而在EX-GAS中，我为了减少Ability管理的事件注册这个繁琐步骤。我对GE的逻辑进行了优化，兼并了这一设计方案。
做法就是在GE中，添加了GrantedAbility变量。
GrantedAbility有5个参数：
- Ability：授予的能力（数据）
- AbilityLevel：授予的能力等级
- ActivationPolicy: 授予的能力的激活策略，类型如下：
  - | 激活策略              | 作用                       |
    |-------------------|--------------------------
    | None | 无激活逻辑, 需要用户自己调用ASC能力激活接口 |
    | WhenAdded | 能力添加时激活（GE添加时激活）         |
    | SyncWithEffect | 同步GE，GE激活时激活             |
- DeactivationPolicy: 授予的能力的取消激活策略，类型如下：
    - | 取消激活策略              | 作用                           |
      |-------------------|------------------------------|
      | None | 无相关取消激活逻辑, 需要用户调用ASC能力取消激活接口 |
      | SyncWithEffect | 同步GE，GE失活时取消激活               |
- RemovePolicy: 授予的能力的移除策略，类型如下：
    - | 移除策略              | 作用                       |
      |-------------------|--------------------------
      | None | 不移除 |
      | SyncWithEffect | 同步GE，GE移除时移除           |
      |WhenEnd| 能力结束时自己移除|
      |WhenCancel|  能力取消时自己移除|
      |WhenCancelOrEnd|  能力结束或取消时自己移除|

到这里Granted Ability的逻辑就清晰了。我们提前将Ability的生命周期通过参数，来确定哪些阶段交给GE来管理。
> 有一点需要注意，Granted Ability的激活不会传任何参数，请保证Ability执行逻辑中依赖的参数，都可以通过Owner（ASC）直接或间接获取。

Granted Ability只是EX-GAS给出的一个现成设计方案，依然可以通过各个事件监听/回调，来实现同样的效果。

---
### 2.9 AbilitySystemComponent
> AbilitySystemComponent是EX-GAS的核心之一，它是GAS的基本运行单位。

ASC(之后都使用缩写指代AbilitySystemComponent),持有Tag，Ability，AttributeSet，GameplayEffect等数据。
其主要职责如下：
- 管理能力（Abilities）： ASC 负责管理角色的所有能力。它允许角色获得、激活、取消和执行各种不同类型的能力，如攻击、防御、技能等。
- 处理效果（GameplayEffects）： ASC 负责处理与能力相关的效果，包括伤害、治疗、状态效果等。它能够跟踪和应用这些效果，并在需要时触发相应的回调或事件。
- 处理标签（Tags）： ASC 负责管理角色身上的标签。标签用于标识角色的状态、属性或其他特征，以便在能力和效果中进行条件检查和过滤。
- 处理属性（Attributes）： ASC 负责管理角色的属性。属性通常表示角色的状态，如生命值、能量值等。ASC 能够增减、修改和监听这些属性的变化。

整个GAS的运作都是围绕着ASC的，所有的Tag，GameplayEffect的作用对象最后都是ASC。而Ability也必须依赖ASC来执行。
为了直观的理解ASC，接下来参考GAS Watcher的监视器界面：
![QQ20240313180923.png](Wiki%2FQQ20240313180923.png)

- ID Mark:这部分可以忽略，只是Unity运行时的Gameobject的标识符相关
- Ability： 图中显示了这个单位有6个Ability。Ability表示了单位的持有能力，但不代表所有的单位能力都要由Ability来做。
    虽然我把Move作为Ability了，但是在FPS,MOBA等类型的游戏中，像Move移动这种强Input关联的行为是不建议用Ability来做的。
- Attribute：单位持有的属性。
  ASC的属性是以AttributeSet为集合，通常ASC的属性集，只加不减。
  因为删除属性集存在很大的隐患，在GAS体系运作时，属性相当于串联GAS运行的线，线上挂着许多GameplayEffect，贸然删除属性集可能会导致线断裂，GameplayEffect运行逻辑失效甚至崩溃。
- GameplayEffects：单位当前持有的GameplayEffect。这里可以直接将GameplayEffect理解为Buff。
  GAS体系内，ASC之间是通过GameplayEffect来交互。
   比如A单位对B单位施法，那么A单位会通过施法（Ability）对B施加一个效果（GameplayEffect），效果如果是持续性的就会被留在B身上（Buff）。
    当然，GAS体系外时，可以通过接口对ASC单位添加/移除GameplayEffect。通常这种情况下，Effect的来源和目标都是指定的那个ASC。
- Tags：单位当前持有的Tag。标签的价值只有在被挂载到ASC时才会体现出来。所有Tag的比较逻辑都直接或间接依赖于ASC。
  - Fixed Tag：固有标签，这些标签是ASC固有的。GAS体系内是不会对FixedTag做任何增减的。
    只有GAS体系外的才会对FixedTag有影响。FixedTag通常是在ASC初始化的时候设置好，之后就尽可能不动。
  - Dynamic Tag：动态标签，这些标签是ASC动态增减的。GAS体系只会对DynamicTag做增减，且只有GAS体系可以管理。
    GAS体系外只能通过GameplayEffect的一些指定接口，间接对DynamicTag进行操作。

ASC是GAS中最复杂，且操作空间最多的组件。对ASC的良好管理和操作就是程序开发人员的重任了。
GAS本身是被动的，而让推动和改变GAS的是ASC。换言之，Runtime下开发者其实是在操作ASC，而不是GAS。
增删管理ASC，调用ASC的Ability执行，以及ASC的体系外Tag，Effect管理才是Runtime下开发者的主要工作。
这之外的GAS配置和拓展，应该由策划承担大部分工作。（但实际上对于中小型团队，程序开发人员还是在做GAS配置的维护工作。）

#### 2.9.a AbilitySystemComponent Preset
AbilitySystemComponent Preset是ASC的预设，用于方便初始化ASC的数据。
![QQ20240315172608.png](Wiki%2FQQ20240315172608.png)
ASC预设是为了可视化角色（单位）的参数。
- 基本信息：ASC的基本信息，仅用于显示，方便配置人员阅读，Runtime不会用到这些参数。
- 属性集：上文提到过，ASC的属性集设计建议只有一个属性集。不建议多个。
- 固有Tag：ASC的基础Tag，通常会把描述性的Tag作为固有Tag，
  比如种族（Race.Human,Race.Monster ）,职业（Job.Wizard,Job.Archer）,阵营（Camp.Good,Camp.Evil）等等。
  当然Tag本身是不做任何限制的，但从Gameplay设计的角度上，状态性质的Tag不建议作为固有Tag。就算设计一个绝对无敌的
  角色，那也应该是把无敌的Tag放在一个永久GameplayEffect上，然后挂到ASC上。而不是把无敌Tag直接当作固有Tag。
- 固有能力：Abilities，单位的基础能力。通常会把单位的基础能力放在这里，比如攻击，防御，跳跃等等。

>如何使用ASC预设？
> 
> 1.AbilitySystemComponent组件自带了序列化的ASC预设字段，可以通过预制体添加，也可以实例化添加。
> 2.依赖ASC预设的初始化，通过AbilitySystemComponentExtension中的静态扩展方法InitWithPreset即可。
> 
> InitWithPreset的参数：
>  - AbilitySystemComponent asc：初始化的ASC，
>  - int level：初始等级
>  - AbilitySystemComponentPreset preset：初始化用的ASC预设
>  - 
> 示例： asc.InitWithPreset(1,ascPreset); // 如果预制体已经设置了参数，那么可以不传ascPreset。

---
## 3.API && Source Code Documentation
本章节会在介绍API和源码的同时，从代码的角度来理解GAS的运作逻辑。
![GAS_IMG_Intro.png](Wiki%2FGAS_IMG_Intro.png)
> 该图简单的解释了GAS的运作逻辑。GAS其实简单的只干一件事：ASC使用Ability对指定的（可以包括自己）ASC释放GameplayEffect。
>
> GAS的推进和运行，就是在不断的重复这件事。
> 体系外的脚本不断的拨动ASC的Ability，而GAS内部会对Ability的运行结果自行消化。

### 3.1 Core 

#### 3.1.1 GameplayAbilitySystem
GameplayAbilitySystem作为核心类，他的作用有2个：管理ASC，控制GAS的运行与否。
- ` static GameplayAbilitySystem GAS`
  - GAS的单例，所有的GAS操作都是通过GAS单例来进行的。
- ` List<AbilitySystemComponent> AbilitySystemComponents { get; } `
  - GAS当前运行的所有AbilitySystemComponent的集合。
- `void Register(AbilitySystemComponent abilitySystemComponent)`
  -  注册ASC到GAS中。
- `void UnRegister(AbilitySystemComponent abilitySystemComponent)`
  -  从GAS中注销ASC。
- `bool IsPaused`
  - GAS是否暂停运行。
- ` void Pause()`
  - 暂停GAS运行。 
- ` void Unpause()`
  - 恢复GAS运行。 
#### 3.1.2 GASTimer
GASTimer是GAS的计时器，它是GAS的时间基准。
- `static long Timestamp()`
  - GAS当前时间戳（毫秒）
- `static long TimestampSeconds()`
  - GAS当前时间戳（秒）
- `static int CurrentFrameCount`
  - GAS当前运行帧数
- `static long StartTimestamp`
  - GAS启动时间戳
- `static void InitStartTimestamp()`
  - GAS初始化启动时间戳
- `static void Pause()`
  - 暂停GASTimer计时 
- `static void Unpause()`
  - 恢复GASTimer计时 
- `static int FrameRate`
  - GAS帧率
#### 3.1.3 GasHost
GasHost是GAS的宿主，它是GAS的运行机器和环境，GasHost没有API可以从外部干涉。

---
### 3.2 AbilitySystemComponent
#### 3.2.1 AbilitySystemComponent
AbilitySystemComponent是GAS的基本运行单位，它是GAS的核心类。
ASC的public方法和属性就是外部干涉GAS的唯一手段。
- `AbilitySystemComponentPreset Preset`
  - ASC的预设。外部读取用，修改preset需要通过SetPreset方法
- `void SetPreset(AbilitySystemComponentPreset preset)`
  - 修改ASC的预设。 
- `int Level { get; protected set; }`
  - ASC的等级
- `GameplayEffectContainer GameplayEffectContainer { get; private set; } `
  - ASC当前所有GameplayEffect的容器，可以通过GameplayEffectContainer对GameplayEffect进行一定的外部干涉。
- `GameplayTagAggregator GameplayTagAggregator { get; private set;} `
  - ASC的GameplayTag聚合器，单位的Tag全部都由聚合器管理，外部可以通过聚合器对Tag进行一定的外部干涉。
- `AbilityContainer AbilityContainer { get; private set;}`
  - ASC的Ability容器，可以通过AbilityContainer对Ability进行一定的外部干涉。 
- `AttributeSetContainer AttributeSetContainer { get; private set;}`
  - ASC的AttributeSet容器，可以通过AttributeSetContainer对AttributeSet进行一定的外部干涉。
- `void Init(GameplayTag[] baseTags, Type[] attrSetTypes,AbilityAsset[] baseAbilities,int level)`
  - 初始化ASC
  - baseTags：ASC的基础Tag
  - attrSetTypes：ASC的初始化AttributeSet类型
  - baseAbilities：ASC的初始化Ability
  - level：ASC的初始化等级
- `void SetLevel(int level)`
  - 设置ASC的等级
- `bool HasTag(GameplayTag gameplayTag)`
  - 判断ASC是否持有指定Tag
  - gameplayTag：指定Tag
  - 返回值：是否持有
- `bool HasAllTags(GameplayTagSet tags)`
  - 判断ASC是否持有指定Tag集合中的所有Tag
  - tags：指定Tag集合
  - 返回值：是否持有
- `bool HasAnyTags(GameplayTagSet tags)`
  - 判断ASC是否持有指定Tag集合中的任意一个Tag
  - tags：指定Tag集合
  - 返回值：是否持有
- `void AddFixedTags(GameplayTagSet tags)`
  - 添加固有Tag
  - tags：添加的Tag集合
- `void RemoveFixedTags(GameplayTagSet tags)`
  - 移除固有Tag
  - tags：移除的Tag集合
- `void AddFixedTag(GameplayTag tag)`
  - 添加固有Tag
  - tag：添加的Tag
-  `void RemoveFixedTag(GameplayTag tag)`
  - 移除固有Tag
  - tag：移除的Tag
- `void RemoveGameplayEffect(GameplayEffectSpec spec)`
  - 移除指定的GameplayEffect
  - spec：指定的GameplayEffect的规格类实例
- `GameplayEffectSpec ApplyGameplayEffectTo(GameplayEffect gameplayEffect, AbilitySystemComponent target)`
  - 对指定的ASC施加指定的GameplayEffect
  - gameplayEffect：指定的GameplayEffect
  - target：目标ASC
  - 返回值：施加的GameplayEffect的规格类实例
- `GameplayEffectSpec ApplyGameplayEffectToSelf(GameplayEffect gameplayEffect)`
  - 对自己施加指定的GameplayEffect
  - gameplayEffect：指定的GameplayEffect
  - 返回值：施加的GameplayEffect的规格类实例
- `void GrantAbility(AbstractAbility ability)`
  - 获得指定的Ability
  - ability：指定的Ability
- `void RemoveAbility(string abilityName)`
  - 移除指定的Ability
  - abilityName：指定的Ability的U-Name
- `float? GetAttributeCurrentValue(string setName, string attributeShortName)`
  - 获取指定Attribute的当前值 
  - setName：AttributeSet的名字
  - attributeShortName：Attribute的短名
  - 返回值：Attribute的当前值
- `float? GetAttributeBaseValue(string setName, string attributeShortName)`
  - 获取指定Attribute的基础值 
  - setName：AttributeSet的名字
  - attributeShortName：Attribute的短名
  - 返回值：Attribute的基础值
- `Dictionary<string, float> DataSnapshot()`
  - 获取ASC的数据快照
  - 返回值：ASC的数据快照
- ` bool TryActivateAbility(string abilityName, params object[] args)`
  - 尝试激活指定的Ability
  - abilityName：指定的Ability的U-Name
  - args：激活Ability的参数
  - 返回值：是否激活成功
- `void TryEndAbility(string abilityName)`
  - 尝试结束指定的Ability
  - abilityName：指定的Ability的U-Name
- `void TryCancelAbility(string abilityName)`
  - 尝试取消指定的Ability
  - abilityName：指定的Ability的U-Name
- `void ApplyModFromInstantGameplayEffect(GameplayEffectSpec spec)`
  - 从Instant GameplayEffect中应用Mod
  - spec：Instant GameplayEffect的规格类实例
- `CooldownTimer CheckCooldownFromTags(GameplayTagSet tags)`
  - 通过Tag检查冷却时间
  - tags：指定的Tag集合
  - 返回值：冷却计时器
- `T AttrSet<T>() where T : AttributeSet`
  - 获取指定类的AttributeSet
  - 返回值：指定类的AttributeSet
- `void ClearGameplayEffect()`
  - 清空ASC的所有GameplayEffect
 
#### 3.2.2 AbilitySystemComponentPreset
AbilitySystemComponentPreset是ASC的预设，用于方便初始化ASC的数据。
- `string[] AttributeSets`
  - ASC的初始化AttributeSet类型 
- `GameplayTag[] BaseTags` 
  - ASC的基础Tag
- `AbilityAsset[] BaseAbilities`
  - ASC的初始化Ability
  
#### 3.2.3 AbilitySystemComponentExtension
AbilitySystemComponentExtension是ASC的扩展方法类，用于方便ASC的初始化和操作。
AbilitySystemComponentExtension不是EX-GAS框架内脚本的，需要EX-GAS框架基础配置完成后，通过生成脚本生成。
- `static Type[] PresetAttributeSetTypes(this AbilitySystemComponent asc)`
  - 获取ASC的预设AttributeSet类型
  - 返回值：ASC的预设AttributeSet类型
- `static GameplayTag[] PresetBaseTags(this AbilitySystemComponent asc)`
  - 获取ASC的预设基础Tag
  - 返回值：ASC的预设基础Tag
- `static void InitWithPreset(this AbilitySystemComponent asc,int level, AbilitySystemComponentPreset preset = null)`
  - 通过预设初始化ASC
  - level：ASC的初始化等级
  - preset：ASC的预设

### 3.3 GameplayTag
#### 3.3.1 GameplayTag
GameplayTag是GAS的标签类，它是GAS的核心类。Tag的设计结构虽然简单，但是在实际应用中十分高效有用。
- `int HashCode => _hashCode;`
  - Tag的HashCode
- `string[] AncestorNames => _ancestorNames;`
  - Tag的父级名 
- `int[] AncestorHashCodes => _ancestorHashCodes;`
  - Tag的父级HashCode集合
- `bool Root => _ancestorHashCodes.Length == 0;`
  - Tag是否是根Tag 
- `bool IsDescendantOf(GameplayTag other)`
  - Tag是否是指定Tag的子Tag
  - other：指定Tag
  - 返回值：是否是子Tag
- `bool HasTag(GameplayTag tag)`
  - Tag是否持有指定Tag,比如‘Buff.Burning’ 持有 ‘Buff’
  - tag：指定Tag
  - 返回值：是否持有
#### 3.3.2 GameplayTagSet
GameplayTagSet是Tag集合类之一。GameplayTagSet适用于稳定不会改变的Tag集合。通常数据类的Tag集合都用GameplayTagSet。
- `readonly GameplayTag[] Tags`
  - Tag数据
- `bool Empty => Tags.Length == 0;`
  - Tag集合是否为空
- `bool HasTag(GameplayTag tag)`
  - TagSet是否持有指定Tag
  - tag：指定Tag
  - 返回值：是否持有
- `bool HasAllTags(GameplayTagSet other) / bool HasAllTags(params GameplayTag[] tags)`
  - TagSet是否持有指定Tag集合中的所有Tag
  - other：指定Tag集合
  - 返回值：是否持有
- `bool HasAnyTags(GameplayTagSet other) / bool HasAnyTags(params GameplayTag[] tags)`
  - TagSet是否持有指定Tag集合中的任意一个Tag
  - other：指定Tag集合
  - 返回值：是否持有
- `bool HasNoneTags(GameplayTagSet other) / bool HasNoneTags(params GameplayTag[] tags)`
  - TagSet是否不持有指定Tag集合中的所有Tag
  - other：指定Tag集合
  - 返回值：是否不持有

#### 3.3.3 GameplayTagContainer
GameplayTagContainer是Tag集合类之一。GameplayTagContainer适用于经常改变的Tag集合。
- `List<GameplayTag> Tags { get; }`
  - Tag数据
- `void AddTag(GameplayTag tag)`
  - 添加Tag
  - tag：指定Tag
- `void AddTag(GameplayTagSet tagSet)`
  - 添加Tag集合
  - tagSet：要添加的Tag集合
- `void RemoveTag(GameplayTag tag)` 
  - 移除Tag
  - tag：指定Tag
- `void RemoveTag(GameplayTagSet tagSet)`
  - 移除Tag集合
  - tagSet：要移除的Tag集合
- `bool HasTag(GameplayTag tag)`
  - TagContainer是否持有指定Tag
  - tag：指定Tag
- `bool HasAllTags(GameplayTagSet other) / bool HasAllTags(params GameplayTag[] tags)`
  - TagContainer是否持有指定Tag集合中的所有Tag
  - other：指定Tag集合
  - 返回值：是否持有
- `bool HasAnyTags(GameplayTagSet other) / bool HasAnyTags(params GameplayTag[] tags)`
  - TagContainer是否持有指定Tag集合中的任意一个Tag
  - other：指定Tag集合
  - 返回值：是否持有
- `bool HasNoneTags(GameplayTagSet other) / bool HasNoneTags(params GameplayTag[] tags)`
  - TagContainer是否不持有指定Tag集合中的所有Tag
  - other：指定Tag集合
  - 返回值：是否不持有

#### 3.3.4 GameplayTagAggregator
GameplayTagAggregator是专门针对ASC的Tag管理类，会针对固有Tag和动态Tag做不同的处理。
- `void Init(GameplayTag[] tags)`
  - 初始化
  - tags：初始化的固有Tag
- `void AddFixedTag(GameplayTag tag)`
  - 添加固有Tag
  - tag：添加的Tag
- `void AddFixedTag(GameplayTagSet tagSet)`
  - 添加固有Tag集合
  - tagSet：添加的Tag集合
- `void RemoveFixedTag(GameplayTag tag)`
  - 移除固有Tag
  - tag：移除的Tag
- `void RemoveFixedTag(GameplayTagSet tagSet)`
  - 移除固有Tag集合
  - tagSet：移除的Tag集合
- `void ApplyGameplayEffectDynamicTag(GameplayEffectSpec source)`
  - 从GameplayEffect中应用动态Tag（Granted Tags）
  - source：GameplayEffect的规格类实例
- `void ApplyGameplayAbilityDynamicTag(AbilitySpec source)`
  - 从Ability中应用动态Tag（Activation Owned Tags）
  - source：Ability的规格类实例
- `RestoreGameplayEffectDynamicTags(GameplayEffectSpec effectSpec)`
  - 从GameplayEffect中恢复动态Tag（Granted Tags）
  - effectSpec：GameplayEffect的规格类实例
- `RestoreGameplayAbilityDynamicTags(AbilitySpec abilitySpec)`
  - 从Ability中恢复动态Tag（Activation Owned Tags）
  - abilitySpec：Ability的规格类实例
- `bool HasTag(GameplayTag tag)` 
  - TagAggregator是否持有指定Tag
  - tag：指定Tag
  - 返回值：是否持有
- `bool HasAllTags(GameplayTagSet other) / bool HasAllTags(params GameplayTag[] tags)`
  - TagAggregator是否持有指定Tag集合中的所有Tag
  - other：指定Tag集合
  - 返回值：是否持有
- `bool HasAnyTags(GameplayTagSet other) / bool HasAnyTags(params GameplayTag[] tags)`
  - TagAggregator是否持有指定Tag集合中的任意一个Tag
  - other：指定Tag集合
  - 返回值：是否持有
- `bool HasNoneTags(GameplayTagSet other) / bool HasNoneTags(params GameplayTag[] tags)`
  - TagAggregator是否不持有指定Tag集合中的所有Tag
  - other：指定Tag集合
  - 返回值：是否不持有 
#### 3.3.5 GTagLib(Script-Generated Code)
GTagLib是GAS的标签库，它是GAS的标签管理类。
GTagLib不是EX-GAS框架内脚本的，需要EX-GAS框架Tag配置改动后，通过生成脚本生成。
- `public static GameplayTag XXX { get;} = new GameplayTag("XXX");`
- `public static GameplayTag XXX_YYY { get;} = new GameplayTag("XXX.YYY");`
  - GTagLib会把所有的Tag都生成为静态字段，方便外部调用。格式如上所示。
  - A.B.C的Tag会生成为A_B_C的静态字段。
- `public static Dictionary<string, GameplayTag> TagMap = new Dictionary<string, GameplayTag>
  {
  ["A"] = A,
  ["A.B"] = A_B,
  ["A.C"] = A_C,
  };`
  - GTagLib还包含了一个TagMap，方便外部通过Tag的字符串名来获取Tag。

---
### 3.4 Attribute
#### 3.4.1 AttributeValue
AttributeValue是一个数据结构体。是实际存储Attribute的值的单位。
- `float BaseValue => _baseValue;`
    - Attribute的基础值，是属性，只读。修改baseValue需要通过AttributeBase的SetBaseValue方法
- `float CurrentValue => _currentValue;`
    - Attribute的当前值，是属性，只读。修改currentValue需要通过AttributeBase的SetCurrentValue方法
- `void SetBaseValue(float value)`
    - 设置Attribute的基础值
    - value：指定的值
- `void SetCurrentValue(float value)`
    - 设置Attribute的当前值
    - value：指定的值
#### 3.4.2 AttributeBase
AttributeBase是GAS的属性基类，它是GAS的核心类之一。
负责管理AttributeValue的值变化，已经Attribute相关回调处理。
- `readonly string Name`
  - Attribute的名字(完整)
- `readonly string ShortName`
  - Attribute的短名
- `readonly string SetName`
  - Attribute所属的AttributeSet的名字
- `AttributeValue Value => _value;`
  - Attribute的值类，数据类
- `float BaseValue => _value.BaseValue;`
  - Attribute的基础值
- `float CurrentValue => _value.CurrentValue;`
  - Attribute的当前值
- `void SetCurrentValue(float value)`
  - 设置Attribute的当前值,会触发_onPreCurrentValueChange和_onPostCurrentValueChange回调
  - value：指定的值
- `void SetBaseValue(float value)`
  - 设置Attribute的基础值,会触发_onPreBaseValueChange和_onPostBaseValueChange回调
  - value：指定的值
- `void SetCurrentValueWithoutEvent(float value)`
  - 设置Attribute的当前值,但不会触发_onPreCurrentValueChange和_onPostCurrentValueChange回调 
  - value：指定的值
- `void SetBaseValueWithoutEvent(float value)`
  - 设置Attribute的基础值,但不会触发_onPreBaseValueChange和_onPostBaseValueChange回调
  - value：指定的值
- `void RegisterPreBaseValueChange(Func<AttributeBase, float,float> func)`
  - 注册Attribute的基础值变化前回调
  - func：回调函数
    - AttributeBase：AttributeBase实例
    - float：变化前的值
    - float：准备变化的值
    - 返回值：回调处理完的变化值
- `void RegisterPostBaseValueChange(Action<AttributeBase, float, float> action)`
  - 注册Attribute的基础值变化后回调
  - action：回调函数
    - AttributeBase：AttributeBase实例
    - float：变化前的值
    - float：变化后的实际值
- `void RegisterPreCurrentValueChange(Func<AttributeBase, float, float> func)`
  - 注册Attribute的当前值变化前回调
  - func：回调函数
    - AttributeBase：AttributeBase实例
    - float：变化前的值
    - float：准备变化的值
    - 返回值：回调处理完的变化值
- `void RegisterPostCurrentValueChange(Action<AttributeBase, float, float> action)`
  - 注册Attribute的当前值变化后回调
  - action：回调函数
    - AttributeBase：AttributeBase实例
    - float：变化前的值
    - float：变化后的实际值
- `void UnregisterPreBaseValueChange(Func<AttributeBase, float,float> func)`
  - 注销Attribute的基础值变化前回调
  - func：注销的回调函数
- `void UnregisterPostBaseValueChange(Action<AttributeBase, float, float> action)`
  - 注销Attribute的基础值变化后回调
  - action：注销的回调函数
- `void UnregisterPreCurrentValueChange(Func<AttributeBase, float, float> func)`
  - 注销Attribute的当前值变化前回调
  - func：注销的回调函数
- `void UnregisterPostCurrentValueChange(Action<AttributeBase, float, float> action)`
  - 注销Attribute的当前值变化后回调
  - action：注销的回调函数

#### 3.4.3 AttributeAggregator
AttributeAggregator是Attribute的单位性质的聚合器，每个AttributeBase会对应一个AttributeAggregator。
AttributeAggregator是完全闭合独立运作，除了构造函数外不提供任何对外方法。
每当AttributeBase的BaseValue变化时，AttributeAggregator会自动更新自己的CurrentValue。

#### 3.4.4 DerivedAttribute(W.I.P)
推导性质的Attribute，理论上不是一个类，而是一个Attribute的设计策略。

---
### 3.5 AttributeSet
#### 3.5.1 AttributeSet
AttributeSet是一个抽象基类。
- `public abstract AttributeBase this[int index] { get; }`
  - 通过AttributeBase的短名作为索引获取AttributeBase
- `public abstract string[] AttributeNames { get; }`
  - AttributeSet的所有Attribute的短名 
- `public void ChangeAttributeBase(string attributeShortName, float value)`
    - 修改AttributeBase的基础值
    - attributeShortName：Attribute的短名
    - value：指定的值
##### 3.5.1.a GAttrSetLib.gen( Script-Generated Code)
GAttrSetLib.gen是便于读取，管理AttributeSet工具脚本。
GAttrSetLib.gen不是EX-GAS框架内脚本的，需要EX-GAS框架AttributeSet配置改动后，通过生成脚本生成。
- 脚本内包含如下静态工具类
- ```
  public static class GAttrSetLib
  {
     public static readonly Dictionary<string,Type> AttrSetTypeDict = new Dictionary<string, Type>()
     {
        {"Fight",typeof(AS_Fight)},
     };
  
     public static List<string> AttributeFullNames=new List<string>()
     {
       "AS_Fight.HP",
       "AS_Fight.MP",
       "AS_Fight.STAMINA",
       "AS_Fight.POSTURE",
       "AS_Fight.ATK",
       "AS_Fight.SPEED",
     };
  }
  ```
- AttrSetTypeDict：AttributeSet的类型字典，方便外部通过字符串名获取AttributeSet的类型。
- AttributeFullNames：所有AttributeSet的所有Attribute的完整名

- 举例:由脚本生成的AttributeSet类
```
public class AS_XXX:AttributeSet
{
    private AttributeBase _A = new AttributeBase("AS_XXX","A");
    public AttributeBase A => _A;
    public void InitA(float value)
    {
        _A.SetBaseValue(value);
        _A.SetCurrentValue(value);
    }
      public void SetCurrentA(float value)
    {
        _A.SetCurrentValue(value);
    }
      public void SetBaseA(float value)
    {
        _A.SetBaseValue(value);
    }
    
      public override AttributeBase this[string key]
      {
          get
          {
              switch (key)
              {
                 case "A":
                    return _A;
              }
              return null;
          }
      }

      public override string[] AttributeNames { get; } =
      {
          "A",
      };
}
``` 
- 配置的AttributeSet名为XXX，包含一个Attribute名为A。

#### 3.5.2 AttributeSetContainer
AttributeSetContainer是AttributeSet的容器类，用于ASC管理AttributeSet。
- `Dictionary<string,AttributeSet> Sets => _attributeSets;` :AttributeSet的集合,为属性，只读。
- `void AddAttributeSet<T>() where T : AttributeSet`:添加AttributeSet
  - `T`：指定的AttributeSet类
- `void AddAttributeSet(Type attrSetType)`:添加AttributeSet
  - `attrSetType`：指定的AttributeSet类型
- `bool TryGetAttributeSet<T>(out T attributeSet) where T : AttributeSet` :尝试获取AttributeSet
  - `attributeSet`：获取的AttributeSet
  - 返回值：是否获取成功
- `float? GetAttributeBaseValue(string attrSetName,string attrShortName)`
  - 获取指定Attribute的基础值
  - attrSetName：AttributeSet的名字
  - attrShortName：Attribute的短名
  - 返回值：Attribute的基础值
- `float? GetAttributeCurrentValue(string attrSetName,string attrShortName)`
  - 获取指定Attribute的当前值
  - attrSetName：AttributeSet的名字
  - attrShortName：Attribute的短名
  - 返回值：Attribute的当前值
- `Dictionary<string, float> Snapshot()`
  - 获取AttributeSetContainer的数据快照
  - 返回值：数据快照
#### 3.5.3 CustomAttrSet
CustomAttrSet是AttributeSet的自定义类，适用于Runtime时动态生成AttributeSet。
- `void AddAttribute(AttributeBase attribute)`
  - 添加Attribute
  - attribute：添加的Attribute
- `void RemoveAttribute(string attributeName)`
  - 移除Attribute
  - attributeName：移除的Attribute的短名

---
### 3.6 GameplayEffect
#### 3.6.1 GameplayEffectAsset
GameplayEffectAsset是GAS的游戏效配置类，是预设用ScriptableObject。
- `EffectsDurationPolicy DurationPolicy;` :GameplayEffect的持续时间策略
- `float Duration` :GameplayEffect的持续时间 
- `float Period` : GameplayEffect的周期
- `GameplayEffectAsset PeriodExecution` :GameplayEffect的周期执行的GameplayEffect
- `GameplayEffectModifier[] Modifiers`:GameplayEffect修改器
-  `GameplayTag[] AssetTags` :GameplayEffect的描述标签
- `GameplayTag[] GrantedTags` :GameplayEffect的授予标签，GameplayEffect生效时会授予目标ASC这些标签，失效时会移除这些标签
- `GameplayTag[] ApplicationRequiredTags`:GameplayEffect的应用要求标签，只有目标ASC持有【所有】这些标签时，GameplayEffect才会生效 
- `GameplayTag[] OngoingRequiredTags`: GameplayEffect的持续要求标签，只有目标ASC持有【所有】这些标签时，GameplayEffect才会持续生效
- `GameplayTag[] RemoveGameplayEffectsWithTags` :GameplayEffect的移除标签，只要目标ASC的GameplayEffect持有【任意】这些标签时，这些GameplayEffect就会被移除
- `GameplayTag[] ApplicationImmunityTags`:GameplayEffect的免疫标签，只要目标ASC持有【任意】这些标签时，这个GameplayEffect就不会生效
- `GameplayCueInstant[] CueOnExecute;` :GameplayEffect执行时触发的GameplayCue
- `GameplayCueDurational[] CueDurational` :GameplayEffect持续时触发的GameplayCue
- `GameplayCueInstant[] CueOnAdd`:GameplayEffect添加时触发的GameplayCue
- `GameplayCueInstant[] CueOnRemove`:GameplayEffect移除时触发的GameplayCue
- `GameplayCueInstant[] CueOnActivate`:GameplayEffect激活时触发的GameplayCue
- `GameplayCueInstant[] CueOnDeactivate`:GameplayEffect失效时触发的GameplayCue

#### 3.6.2 GameplayEffect
GameplayEffect是GAS的Runtime的游戏效果数据类.运行游戏运行时动态生成GameplayEffect。
- GameplayEffect的数据结构与GameplayEffectAsset几乎一致。这里就不再多赘述数据变量了。
- 
#### 3.6.3 GameplayEffectSpec
- `void Apply()`：应用游戏效果。
- `void DisApply()`：取消游戏效果的应用。
- `void Activate()`：激活游戏效果。
- `void Deactivate()`：停用游戏效果。
- `bool CanRunning()`：检查游戏效果是否可以运行。
- `void Tick()`：更新游戏效果的周期性行为。
- `void TriggerOnExecute()`：触发游戏效果执行时的事件。
- `void TriggerOnAdd()`：触发游戏效果添加时的事件。
- `void TriggerOnRemove()`：触发游戏效果移除时的事件。
- `void TriggerOnTick()`：触发游戏效果进行周期性更新时的事件。
- `void TriggerOnImmunity()`：触发游戏效果免疫时的事件。
- `void RemoveSelf()`：移除游戏效果自身。
- `void RegisterValue(GameplayTag tag, float value)`：注册与游戏标签关联的值。
  - `tag`：游戏标签。
  - `value`：与游戏标签关联的值。
- `void RegisterValue(string name, float value)`：注册与名称关联的值。
  - `name`：名称。
  - `value`：与名称关联的值。
- `bool UnregisterValue(GameplayTag tag)`：取消注册与游戏标签关联的值。
    - `tag`：游戏标签。
    - 返回值：如果成功取消注册，则返回 `true`，否则返回 `false`。
- `bool UnregisterValue(string name)`：取消注册与名称关联的值。
    - `name`：名称。
    - 返回值：如果成功取消注册，则返回 `true`，否则返回 `false`。
- `float? GetMapValue(GameplayTag tag)`：获取与游戏标签关联的值。
    - `tag`：游戏标签。
    - 返回值：如果找到与指定游戏标签关联的值，则返回该值；否则返回 `null`。
- `float? GetMapValue(string name)`：获取与名称关联的值。
    - `name`：名称。
    - 返回值：如果找到与指定名称关联的值，则返回该值；否则返回 `null`。
  
#### 3.6.4 GameplayEffectContainer
GameplayEffectContainer是GameplayEffect的容器类，用于ASC管理GameplayEffect。
- `List<GameplayEffectSpec> GetActiveGameplayEffects()`：获取当前生效的游戏效果列表。
- `void Tick()`：处理所有生效游戏效果的周期性更新。
- `void RegisterOnGameplayEffectContainerIsDirty(Action action)`：注册效果容器变为脏状态时的回调函数。
  - `action`：回调函数。 
- `void UnregisterOnGameplayEffectContainerIsDirty(Action action)`：取消注册效果容器变为脏状态时的回调函数。
  - `action`：回调函数。 
- `void RemoveGameplayEffectWithAnyTags(GameplayTagSet tags)`：移除具有指定标签的游戏效果。
  - `tags`：指定的标签。 
- `bool AddGameplayEffectSpec(GameplayEffectSpec spec)`：添加一个游戏效果实例。
  - `spec`：指定的游戏效果规范。 
- `void RemoveGameplayEffectSpec(GameplayEffectSpec spec)`：移除指定的游戏效果实例。
  - `spec`：指定的游戏效果规范。 
- `void RefreshGameplayEffectState()`：刷新游戏效果的状态，包括激活新效果和移除已停用的效果。
- `CooldownTimer CheckCooldownFromTags(GameplayTagSet tags)`：检查指定标签的冷却状态。
  - `tags`：指定的标签。 
  - 返回值：冷却计时器。
- `void ClearGameplayEffect()`：清除所有游戏效果，包括移除已应用的效果和停用的效果。

#### 3.6.5 CooldownTimer
CooldownTimer是冷却计时结构体，用于保存冷却时间数据。
- `public float TimeRemaining;` : 剩余时间
- `public float Duration;` : 总时间
#### 3.6.6 GameplayEffectModifier
GameplayEffectModifier是游戏效果修改器类，用于实现对Attribute的修改。
- `string AttributeName`：属性名称，用于标识游戏效果修改器所影响的属性。
- `float ModiferMagnitude`：修改器的幅度值，用于指定属性修改的具体数值。
- `GEOperation Operation`：修改器的操作类型，指定属性修改的方式，如增加、减少等。
- `ModifierMagnitudeCalculation MMC`：修改器的计算方式，用于指定如何计算修改的幅度值。
- `void SetModiferMagnitude(float value)`：设置修改器的幅度值。
    - `value`：修改器的新幅度值。
- `void OnAttributeChanged()`：当属性名称发生变化时调用的方法，用于更新相关字段的值。
- `static void SetAttributeChoices()`：设置属性选择列表。
- `string AttributeSetName`：属性集名称，用于标识游戏效果修改器所影响的属性集。
- `string AttributeShortName`：属性短名称，用于标识游戏效果修改器所影响的属性的简短版本。

##### 3.6.6.0 ModifierMagnitudeCalculation
ModifierMagnitudeCalculation是一个抽象基类，所有MMC必须继承自他。
- `public abstract float CalculateMagnitude(GameplayEffectSpec spec, AttributeBase attribute, float value)`：计算修改器的幅度值方法是MMC的根本。
    - `spec`：游戏效果规范。
    - `attribute`：属性基类。
    - `value`：指定的值。
    - 返回值：修改器的幅度值。
##### 3.6.6.1 ScalableFloatModCalculation
ScalableFloatModCalculation是一个MMC的实现类，用于实现可缩放的浮点数修改器。
```
    public class ScalableFloatModCalculation:ModifierMagnitudeCalculation
    {
        [SerializeField] private float k;
        [SerializeField] private float b;

        public override float CalculateMagnitude(GameplayEffectSpec spec,float input)
        {
            return input * k + b;
        }
    }
```
- `float k`：缩放系数。
- `float b`：偏移量。
- 执行逻辑：`input * k + b`。线性缩放。
##### 3.6.6.2 AttributeBasedModCalculation
AttributeBasedModCalculation是一个MMC的实现类，用于实现基于属性的修改器。
```
    public class AttributeBasedModCalculation : ModifierMagnitudeCalculation
    {
        public enum AttributeFrom
        {
            Source,
            Target
        }

        public enum GEAttributeCaptureType
        {
            SnapShot,
            Track
        }

        public string attributeName;
        public string attributeSetName;
        public string attributeShortName;
        public AttributeFrom attributeFromType;
        public GEAttributeCaptureType captureType;
        public float k = 1;
        public float b = 0;

        public override float CalculateMagnitude(GameplayEffectSpec spec, float modifierMagnitude)
        {
            if (attributeFromType == AttributeFrom.Source)
            {
                if (captureType == GEAttributeCaptureType.SnapShot)
                {
                    var snapShot = spec.Source.DataSnapshot();
                    var attribute = snapShot[attributeName];
                    return attribute * k + b;
                }
                else
                {
                    var attribute = spec.Source.GetAttributeCurrentValue(attributeSetName, attributeShortName);
                    return (attribute ?? 1) * k + b;
                }
            }

            if (captureType == GEAttributeCaptureType.SnapShot)
            {
                var attribute = spec.Owner.DataSnapshot()[attributeName];
                return attribute * k + b;
            }
            else
            {
                var attribute = spec.Owner.GetAttributeCurrentValue(attributeSetName, attributeShortName);
                return (attribute ?? 1) * k + b;
            }
        }
    }
```
- `string attributeName`：属性名称。
- `string attributeSetName`：属性集名称。
- `string attributeShortName`：属性短名称。
- `AttributeFrom attributeFromType`：属性来源类型。
- `GEAttributeCaptureType captureType`：游戏效果属性捕获类型。
- `float k`：缩放系数。
- `float b`：偏移量。
- 执行逻辑：根据属性来源类型和游戏效果属性捕获类型，获取属性的当前值或快照值，并进行线性缩放。

##### 3.6.6.3 SetByCallerFromNameModCalculation
SetByCallerFromNameModCalculation是一个MMC的实现类，用于实现根据名称设置的修改器。
```
    public class SetByCallerFromNameModCalculation : ModifierMagnitudeCalculation
    {
        [SerializeField] private string valueName;
        public override float CalculateMagnitude(GameplayEffectSpec spec,float input)
        {
            var value = spec.GetMapValue(valueName);
            return value ?? 0;
        }
    }
```
- `string valueName`：键值值名称。
- 执行逻辑：根据值名称获取与名称关联的值。
##### 3.6.6.4 SetByCallerFromTagModCalculation
SetByCallerFromTagModCalculation是一个MMC的实现类，用于实现根据标签设置的修改器。
```
public class SetByCallerFromTagModCalculation:ModifierMagnitudeCalculation
    {
        [SerializeField] private GameplayTag _tag;
        public override float CalculateMagnitude(GameplayEffectSpec spec  ,float input)
        {
            var value = spec.GetMapValue(_tag);
            return value ?? 0;
        }
    }
```
- `GameplayTag _tag`：键值标签。
- 执行逻辑：根据游戏标签获取与游戏标签关联的值。

---
### 3.7 Ability
#### 3.7.1 AbilityAsset
AbilityAsset是GAS的游戏能力配置类，是预设用ScriptableObject。他本身是一个抽象基类，所有的AbilityAsset都必须继承自他。
- `abstract Type AbilityType()`：能力的类型。用于把AbilityAsset和Ability类一一匹配。
    - 返回值：能力的类型。
- `string UniqueName`：唯一名称，用于标识该能力。
- `GameplayEffectAsset Cost`：花费效果，该能力的消耗效果。
- `GameplayEffectAsset Cooldown`：冷却效果，该能力的冷却效果。如果为空，冷却时间也不会生效。
- `float CooldownTime`：冷却时间，该能力的冷却时间长度。
- `GameplayTag[] AssetTag`：资产标签，该能力的标签。
- `GameplayTag[] CancelAbilityTags`：取消能力标签，用于取消该能力的标签。
- `GameplayTag[] BlockAbilityTags`：阻止能力标签，用于阻止该能力的标签。
- `GameplayTag[] ActivationOwnedTag`：激活所需标签，该能力激活所需的标签。
- `GameplayTag[] ActivationRequiredTags`：激活要求标签，该能力激活所需的标签。
- `GameplayTag[] ActivationBlockedTags`：激活阻止标签，用于阻止该能力的激活标签。

#### 3.7.2 AbstractAbility
AbstractAbility是GAS的游戏能力数据基类，他本身是一个抽象基类，所有的Ability都必须继承自他。
- `string Name`：名称，表示能力的名称。
- `AbilityAsset DataReference`：数据引用，指向与该能力相关联的能力资产。
- `AbilityTagContainer Tag`：标签，该能力的标签容器。
- `GameplayEffect Cooldown`：冷却效果，该能力的冷却效果。
- `float CooldownTime`：冷却时间，该能力的冷却时间长度。
- `GameplayEffect Cost`：花费效果，该能力的消耗效果。
- `AbstractAbility(AbilityAsset abilityAsset)`：抽象能力构造函数，初始化抽象能力实例。
  - `abilityAsset`：能力资产，与该能力相关联的能力资产。
- `abstract AbilitySpec CreateSpec(AbilitySystemComponent owner)`：创建能力规格的抽象方法，用于生成能力的规格实例。
  - `owner`：所有者，拥有该能力的实体。
- `void SetCooldown(GameplayEffect coolDown)`：设置冷却效果的方法。
  - `coolDown`：冷却效果，要设置的冷却效果。
- `void SetCost(GameplayEffect cost)`：设置花费效果的方法。
  - `cost`：花费效果，要设置的花费效果。
#### 3.7.2.a AbstractAbility<T> :AbstractAbility where T : AbilityAsset
AbstractAbility<T>是AbstractAbility的泛型子类，用于实现AbstractAbility的泛型版本。
通常Ability都继承自他。方便对应的AbilityAsset和Ability一一匹配。
#### 3.7.3 AbilitySpec
AbilitySpec是GAS的游戏能力规格类，用于实现对Ability的实例化。本身是一个抽象基类，所有的AbilitySpec都必须继承自他。
AbilitySpec是用于实现Ability游戏内实际的表现逻辑。
- `AbstractAbility Ability`：能力，与该能力规格类相关联的能力实例。
- `AbilitySystemComponent Owner`：所有者，拥有该能力规格的单位。
- `float Level`：等级，该能力的等级。
- `bool IsActive`：是否激活，表示该能力当前是否处于激活状态。
- `int ActiveCount`：激活计数，记录该能力被激活的次数。
- ` void RegisterActivateResult(Action<AbilityActivateResult> onActivateResult)`：注册激活结果的方法，用于注册激活结果的回调函数。
- ` void UnregisterActivateResult(Action<AbilityActivateResult> onActivateResult)`：注销激活结果的方法，用于注销激活结果的回调函数。
- ` void RegisterEndAbility(Action onEndAbility)`：注册结束能力的方法，用于注册结束能力的回调函数。
- ` void UnregisterEndAbility(Action onEndAbility)`：注销结束能力的方法，用于注销结束能力的回调函数。
- ` void RegisterCancelAbility(Action onCancelAbility)`：注册取消能力的方法，用于注册取消能力的回调函数。
- ` void UnregisterCancelAbility(Action onCancelAbility)`：注销取消能力的方法，用于注销取消能力的回调函数。
- ` virtual AbilityActivateResult CanActivate()`：检查能力规格是否可以被激活。
  - 返回值：激活结果：
    - Success：成功
    - FailHasActivated：失败，已经激活
    - FailTagRequirement：失败，Tag要求不满足
    - FailCost： 失败，消耗不足
    - FailCooldown： 失败，还在冷却
    - FailOtherReason： 失败，其他原因
- ` void DoCost()`：执行花费的方法，用于执行激活该能力规格的花费操作。
- ` virtual bool TryActivateAbility(params object[] args)`：尝试激活能力
- ` virtual void TryEndAbility()`：尝试结束能力
- ` virtual void TryCancelAbility()`：尝试取消能力
- ` void Tick()`：处理能力的帧更新。
- ` abstract void ActivateAbility(params object[] args)`：激活能力的抽象方法，用于执行激活该能力的操作。
- ` abstract void CancelAbility()`：取消能力的抽象方法，用于执行取消该能力的操作。
- ` abstract void EndAbility()`：结束能力的抽象方法，用于执行结束该能力的操作。
#### 3.7.4 AbilityContainer
能力容器，是ASC的间接管理能力的对象。
- `void Tick()`：处理的方法，用于处理能力容器中所有能力的Tick逻辑。
- ` void GrantAbility(AbstractAbility ability)`：授予能力的方法，用于向能力容器中添加新的能力。
  - `ability`：能力，要添加的新能力实例。
- `void RemoveAbility(AbstractAbility ability)`：移除能力的方法，根据能力实例从能力容器中移除能力规格。
  - `ability`：能力，要移除的能力实例。
- `public void RemoveAbility(string abilityName)`：移除能力的方法，根据能力名称从能力容器中移除能力规格。
  - `abilityName`：能力名称，要移除的能力名称。
- `bool TryActivateAbility(string abilityName, params object[] args)`：尝试激活能力的方法
  - `abilityName`：能力名称，要激活的能力名称。
  - `args`：参数，激活能力所需的额外参数。
  - 返回值：布尔值，表示能否成功激活能力。
- `void EndAbility(string abilityName)`：结束能力
  - `abilityName`：能力名称，要结束的能力名称。
- `void CancelAbility(string abilityName)`：取消能力
  - `abilityName`：能力名称，要取消的能力名称。
- `Dictionary<string, AbilitySpec> AbilitySpecs()`：获取容器内所有能力字典
  - 返回值：包含所有能力规格的能力名称与对应的能力规格实例。
#### 3.7.5 AbilityTask(W.I.P)
Ability我们只能控制他的激活，结束等，并且这些接口都是功能性的即时方法，不存在异步，持续管理的说法。

但是Ability不可能都是瞬时逻辑，因此在Ability的逻辑实现中需要开发者对Tick处理，或者使用异步自行实现逻辑。
而在UE的GAS中，为了解决这个问题，设计团队创造了AbilityTask的概念，他们让AbilityTask来承载实现Ability
逻辑的任务。在UE版本的GAS中，AbilityTask的种类很多，他们能实现即时/异步/持续/等待的逻辑处理。功能非常强大。

因此，我也试着模仿了这个概念，但目前的版本来说，AbilityTask的功能和目的性还很弱。在之后的版本迭代中，我会慢慢完善
AbilityTask，以此来强化GAS中的Ability的逻辑处理能力和可编辑性。
- AbilityTaskBase:基类，Task是依附于Ability的存在，因此他的初始化必须依赖于AbilitySpec。
  - ```
    public abstract class AbilityTaskBase
    {
        protected AbilitySpec _spec;
        public AbilitySpec Spec => _spec;
        public virtual void Init(AbilitySpec spec)
        {
            _spec = spec;
        }
    }
    ```
- InstantAbilityTask: 即时类型的Task，最为常见的Task之一。
  - ```
    public abstract class InstantAbilityTask:AbilityTaskBase
    {
        #if UNITY_EDITOR
        /// <summary>
        ///  编辑器预览用
        ///  【注意】 覆写时，记得用UNITY_EDITOR宏包裹，这是预览表现用的函数，不该被编译。
        /// </summary>
        public virtual void OnEditorPreview()
        {
        }
        #endif
        public abstract void OnExecute();
    }
    ``` 
- OngoingAbilityTask: 持续类型的Task，目前这类Task和TimelineAbility强关联，往后的设计里会抽象出来，让Task更加灵活。
  - ```
    public abstract class OngoingAbilityTask:AbilityTaskBase
    {
        #if UNITY_EDITOR
        /// <summary>
        /// 编辑器预览用
        /// 【注意】 覆写时，记得用UNITY_EDITOR宏包裹，这是预览表现用的函数，不该被编译。
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="startFrame"></param>
        /// <param name="endFrame"></param>
        public virtual void OnEditorPreview(int frame, int startFrame, int endFrame)
        {
        }
        #endif
        public abstract void OnStart(int startFrame);

        public abstract void OnEnd(int endFrame);

        public abstract void OnTick(int frameIndex,int startFrame,int endFrame);
    }
    ```     
---

[//]: # (### 3.7.EX Timeline Ability（W.I.P）)

[//]: # (#### 3.7.EX.1 TimelineAbilityAsset)

[//]: # (#### 3.7.EX.2 TimelineAbility)

[//]: # (#### 3.7.EX.3 TimelineAbilitySpec)

[//]: # (#### 3.7.EX.4 TimelineAbilityPlayer)

[//]: # (#### 3.7.EX.5 Target Catcher)

[//]: # (##### 3.7.EX.5.1 TargetCatcherBase)

[//]: # (##### 3.7.EX.5.2 CatchSelf)

[//]: # (##### 3.7.EX.5.3 CatchTarget)

[//]: # (##### 3.7.EX.5.4 CatchAreaBox2D)

[//]: # (##### 3.7.EX.5.5 CatchAreaCircle2D)

---
### 3.8 GameplayCue
#### 3.8.1 GameplayCue
GameplayCue是GAS的游戏提示配置类，用于实现对游戏效果的提示。他本身是一个抽象基类，所有的GameplayCue都必须继承自他。
- `GameplayTag[] RequiredTags;` :GameplayCue的要求标签,持有【所有】RequiredTags才可触发
- `GameplayTag[] ImmunityTags;` :GameplayCue的免疫标签,持有【任意】ImmunityTags不可触发
##### 3.8.1.a public abstract class GameplayCue<T> : GameplayCue where T : GameplayCueSpec
这个泛型类是为了方便对应的GameplayCueSpec和GameplayCue一一匹配，方便使用。
#### 3.8.2 GameplayCueSpec
GameplayCueSpec是GAS的游戏提示规格类，用于实现对GameplayCue的实例化。本身是一个抽象基类，所有的GameplayCueSpec都必须继承自他。
GameplayCueSpec内实现GameplayCue游戏内实际的表现逻辑。

-  
```
        public virtual bool Triggerable()
        {
            return _cue.Triggerable(Owner);
        }
``` 
- Triggerable()：检查是否可以触发游戏提示的方法。

#### 3.8.3 GameplayCueParameters
GameplayCueParameters是GAS的游戏提示参数结构体，用于实现对GameplayCue的参数化。
目前逻辑简单粗暴，存在拆装箱过程。
```
    public struct GameplayCueParameters
    {
        public GameplayEffectSpec sourceGameplayEffectSpec; 
        public AbilitySpec sourceAbilitySpec;
        public object[] customArguments;
    }
```
#### 3.8.4 GameplayCueInstant
GameplayCueInstant是GAS的GameplayCue中的一大类,属于OneShot类型的Cue。
##### 3.8.4.a GameplayCueInstant
- `InstantCueApplyTarget applyTarget`：立即提示应用目标，指示立即提示的应用目标类型。
- `virtual void ApplyFrom(GameplayEffectSpec gameplayEffectSpec)`：从GameplayEffectSpec应用InstantCue。
  - `gameplayEffectSpec`：游戏效果规格，触发立即提示的游戏效果规格实例。
- `virtual void ApplyFrom(AbilitySpec abilitySpec, params object[] customArguments)`：从AbilitySpec应用InstantCue。
  - `abilitySpec`：能力规格，触发立即提示的能力规格实例。
  - `customArguments`：自定义参数，自定义参数数组。

##### 3.8.4.b GameplayCueInstantSpec
GameplayCueInstantSpec必须覆写Trigger()方法，用于实现对GameplayCueInstant触发。
```
public abstract class GameplayCueInstantSpec : GameplayCueSpec
    {
        public GameplayCueInstantSpec(GameplayCueInstant cue, GameplayCueParameters parameters) : base(cue,
            parameters)
        {
        }
        
        public abstract void Trigger();
    }
```
#### 3.8.5 GameplayCueDuration
GameplayCueDuration是GAS的GameplayCue中的一大类,属于持续类型的Cue。
##### 3.8.5.a GameplayCueDurational
- `public GameplayCueDurationalSpec ApplyFrom(GameplayEffectSpec gameplayEffectSpec)`: 从GameplayEffectSpec应用DurationalCue。
  - `gameplayEffectSpec`：游戏效果规格，触发持续提示的游戏效果规格实例。
- `public GameplayCueDurationalSpec ApplyFrom(AbilitySpec abilitySpec, params object[] customArguments)`: 从AbilitySpec应用DurationalCue。
  - `abilitySpec`：能力规格，触发持续提示的能力规格实例。
  - `customArguments`：自定义参数，自定义参数数组。 
##### 3.8.5.b GameplayCueDurationalSpec
GameplayCueDurationalSpec必须覆写
OnAdd()，
OnRemove()，
OnGameplayEffectActivate()，
OnGameplayEffectDeactivate()，
OnTick()方法，
用于实现对GameplayCueDurational触发和运作。
```
    public abstract class GameplayCueDurationalSpec : GameplayCueSpec
    {
        protected GameplayCueDurationalSpec(GameplayCueDurational cue, GameplayCueParameters parameters) : 
            base(cue, parameters)
        {
        }

        public abstract void OnAdd();
        public abstract void OnRemove();
        public abstract void OnGameplayEffectActivate();
        public abstract void OnGameplayEffectDeactivate();
        public abstract void OnTick();
    }
```

---
## 4.可视化功能
### 1. GAS Setting Manager (GAS基础配置管理器)
![QQ20240313174500.png](Wiki%2FQQ20240313174500.png)
基础配置是与项目工程唯一对应的，所以入口放在了ProjectSetting，另外还有Edit Menu栏入口：EX-GAS -> Setting

- GameplayTag Manager
![QQ20240313114652.png](Wiki%2FQQ20240313114652.png)
- Attribute Manager
![QQ20240313115953.png](Wiki%2FQQ20240313115953.png)
- AttributeSet Manager
![QQ20240313121300.png](Wiki%2FQQ20240313121300.png)


### 2. GAS Asset Aggregator (GAS配置资源聚合器)
![QQ20240313175247.png](Wiki%2FQQ20240313175247.png)
因为GAS使用过程需要大量的配置（各类预设：ASC，游戏能力，游戏效果/buff，游戏提示，MMC），为了方便集中管理，我制作了一个配置资源聚合器。

通过在菜单栏EX-GAS -> Asset Aggregator 可以打开配置资源聚合器。

聚合器支持：分类管理，文件夹树结构显示，搜索栏快速查找，快速创建/删除配置文件（右上角的快捷按钮）
- ASC预设管理
![QQ20240313175513.png](Wiki%2FQQ20240313175513.png)
- 能力配置管理
![QQ20240313175749.png](Wiki%2FQQ20240313175749.png)
- 游戏效果管理
![QQ20240313175829.png](Wiki%2FQQ20240313175829.png)
- 游戏提示 & MMC 管理
![QQ20240313180028.png](Wiki%2FQQ20240313180028.png)
![QQ20240313180054.png](Wiki%2FQQ20240313180054.png)
### 3. GAS Runtime Watcher (GAS运行时监视器)
![QQ20240313180923.png](Wiki%2FQQ20240313180923.png)
__*注意！由于该监视器的监视刷新逻辑过于暴力，因此存在明显的性能问题。监视器只是为了方便调试，所以建议不要一直后台挂着监视器，有需要时再打开。*__

>目前监视器较为简陋，以后可能会优化监视器。

---

## 5.如果...我想...,应该怎么做?(W.I.P)
- Q:我想实现血量（HpMax）上限，随力量(STR)每增加1点，血量上限增加10%，怎么办？
  - A: 有两种常见的方法：
    - 1. 采用Derived Attribute的设计方法，给单位添加一个Infinite的GameplayEffect，
         在修改器参数列表中添加一个修改器：修改属性为HpMax；操作类型为乘法；模值随意；
         MMC为STR属性依赖的MMC，来源为Target，并且Capture必须为Track（只有为Track时才能触发实时重计算），剩下的线性参数为k=0.1，b=1 (Magnitude = 1 + 10% * STR )。
    - 2. 采用监听STR属性的变化事件，手动对HpMax的BaseValue进行修改同步。
---
## 6.暂不支持的功能（可能有遗漏）
- RPC相关的GE复制广播
- GameplayEffect Execution，目前只有Modifier，没有Execution
- Ability的触发判断用的Source/Target Tag目前不生效
- GE过期时，触发的游戏效果

## 7.后续计划
- 修复bug ，性能优化
- 将GAS采用ECS结构来运行
- 补全遗漏的功能 
- 优化Ability的编辑
- 支持RPC的GE复制广播，网络同步 

## 8.特别感谢
本插件全面参考了[UE的GAS解析](https://github.com/tranek/GASDocumentation)，来自github --[@tranek](https://github.com/tranek)

同时还有[中译版本](https://github.com/BillEliot/GASDocumentation_Chinese)，来自github --[@BillEliot](https://github.com/BillEliot)

没有上述二位的文章，本项目的开发会非常痛苦。

另外还要感谢开源项目：[UnityToolchainsTrick](https://github.com/XINCGer/UnityToolchainsTrick)

多亏UnityToolchainsTrick中的大量Editor开发技巧，极大的缩减了项目中编辑器的制作时间，省了很多事儿。非常感谢！

感谢参与EX-GAS开发的朋友们:
- [BBC](https://github.com/kenkinky) :优化了很多编辑器的体验以及bug，还提出了很多问题和反馈。

## 9.插件反馈渠道
QQ群号:616570103

目前该插件是一定有大量bug存在的，因为有非常多的细节没有测试到，虽然有Demo演示，但也只是一部分的功能。所以我希望有人能使用该插件，多多反馈，来完善该插件。

GAS使用门槛高，所以有任何GAS相关使用的疑问，bug或者建议，欢迎来反馈群里交流。我都会尽可能回答的。


