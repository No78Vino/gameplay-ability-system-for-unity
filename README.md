# EX Gameplay Ability System For Unity
## 前言
该项目为Unreal Engine的Gameplay Ability System的Unity实现，目前实现了部分功能，后续会继续完善。
## ！！提醒！！请注意！！
__*该项目依赖Odin Inspector插件（付费），请自行解决!!!!!!!!*__

```若没有方法解决，可以加反馈qq群（616570103），群内提供帮助 ```

目前EX-GAS没有非常全面的测试，所以存在不可知的大量bug和性能问题。所以对于打算使用该插件的朋友请谨慎考虑，当然我是希望更多人用EX-GAS，毕竟相当于是变相的QA。

但是要讲良心嘛，现在的EX-GAS算不上很稳定的版本，如果你是业余时间开发rpg类的独立游戏，开发时间十分充裕，我当然建议你试试EX-GAS，我会尽可能修复bug，提供使用上的帮助。

如果有好兄弟确实打算用，那么请务必加反馈群（616570103）。我会尽力抽时间修复提出的bug。

>我非常希望EX-GAS能早日稳定，为更多游戏提供支持帮助。

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
- 3.[可视化功能](#3可视化功能)
  - [GAS Base Manager (GAS基础配置管理器)](#1-gas-base-manager-gas基础配置管理器)
  - [GAS Asset Aggregator (GAS配置资源聚合器)](#2-gas-asset-aggregator-gas配置资源聚合器)
  - [GAS Runtime Watcher (GAS运行时监视器)](#3-gas-runtime-watcher-gas运行时监视器)
- 4.[API(W.I.P 施工中)](#4api(W.I.P 施工中))
- 5.[如果...我想...,应该怎么做?(持续补充)](#如果...我想...,应该怎么做?)
- 6.[暂不支持的功能（可能有遗漏）](#6暂不支持的功能可能有遗漏)
- 7.[后续计划](#7后续计划)
- 8.[特别感谢](#8特别感谢)
- 9.[插件反馈渠道](#9插件反馈渠道)
## 1.快速开始
### 安装
1. 使用Unity Package Manager安装
在Unity Package Manager中添加git地址：
2. 使用git clone

---
### 使用
GAS十分复杂，使用门槛较高。因为本项目是对UE的GAS的模仿移植，所以实现逻辑基本一致。建议先粗略了解一下UE版本的GAS整体逻辑，参考项目文档：https://github.com/BillEliot/GASDocumentation_Chinese

`参考使用案例：`

#### *使用流程*
1. 基础设置

在ProjectSetting中（或者Edit Menu栏入口：EX-GAS -> Setting），找到EX Gameplay Ability System的基本设置界面：

![O`~ CLEHDMFE9O@M$5~`$1H](https://github.com/No78Vino/gameplay-ability-system-for-unity/assets/43328860/200ddd3c-e28c-4630-884b-e8fa165e7b5d)

设置好以下两个路径
- Config Asset Path: 这是该项目有关GAS的配置的路径。
- Code Gen Path: 这是GAS的生成脚本路径。GAS的基础配置（Tag，Attribute，AttributeSet）都会有对应的脚本生成。

设置完路径后，点击保存（Save）按钮。


2. 配置Tag

Tag是GAS核心逻辑运作的依赖,非常重要。关于Tag的使用及运作逻辑详见章节([GameplayTag](#GameplayTag))

3. 配置Attribute

Attribute是GAS运行时数据单位。关于Attribute的使用及运作逻辑详见章节([Attribute](#Attribute))

4. 配置AttributeSet

AttributeSet是GAS运行时数据单位集合，合理的AttributeSet设计能够帮助程序解耦，提高开发效率。关于AttributeSet的使用及运作逻辑详见章节([AttributeSet](#AttributeSet))

5. 设计MMC,Cue


6. 设计Gameplay Effect

7. 设计Ability

8. 设计ASC预设（可选）
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
开发者可以通过[GameplayTag Manager](#a.GameplayTag Manager)在项目设置中管理这些标签，无需手动编辑配置文件。
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

### 2.3 Attribute
>Attribute，属性，是GAS中的核心数据单位，用于描述角色的各种属性，如生命值，攻击力，防御力等。


### 2.4 AttributeSet
### 2.5 ModifierMagnitudeCalculation
### 2.6 GameplayCue
### 2.7 GameplayEffect
### 2.8 Ability
### 2.9 AbilitySystemComponent

---
## 3.可视化功能
### 1. GAS Base Manager (GAS基础配置管理器)
![$5C1@A0}R89 %WS33OY6UP0](https://github.com/No78Vino/gameplay-ability-system-for-unity/assets/43328860/85f4b1e2-ab3b-4735-8d71-b6623557bf02)

基础配置是与项目工程唯一对应的，所以入口放在了ProjectSetting，另外还有Edit Menu栏入口：EX-GAS -> Setting

#### a.GameplayTag Manager
![{)7T)P@{U}GWY7T%@ 5$@@W](https://github.com/No78Vino/gameplay-ability-system-for-unity/assets/43328860/d5306afc-82a0-4c3e-a263-280c0088f1ae)

我模仿了UE GAS的Tag管理视图，做了树结构管理。

- b.Attribute 管理器

项目内可操作的属性只可从已配置的Attribute中选取。

- c.AttributeSet 管理器
![HBDG` 0 {{9 G6@AS_I0YF](https://github.com/No78Vino/gameplay-ability-system-for-unity/assets/43328860/bc227c91-1dc1-408b-93e6-c93b5936b232)

属性集可以预设生成，也支持运行时自定义（CustomAttributeSet）。

### 2. GAS Asset Aggregator (GAS配置资源聚合器)
![N~~1W5_AQQ42XY6T`9D)G3F](https://github.com/No78Vino/gameplay-ability-system-for-unity/assets/43328860/8f3ab649-fc80-426a-aa44-3a52a9df19c4)

因为GAS使用过程需要大量的配置（各类预设：ASC，游戏能力，游戏效果/buff，游戏提示，MMC），为了方便集中管理，我制作了一个配置资源聚合器。

通过在菜单栏EX-GAS -> Asset Aggregator 可以打开配置资源聚合器。

聚合器支持：分类管理，文件夹树结构显示，搜索栏快速查找，快速创建/删除配置文件（右上角的快捷按钮）
- a.ASC预设管理
![)@6J1G3%FESLFG$1R0E$WNL](https://github.com/No78Vino/gameplay-ability-system-for-unity/assets/43328860/0cb0b5a9-cfde-44ad-b121-e0077330a02e)
- b.能力配置管理
![RY_~6~7BSVSOA0`C 4F8FD](https://github.com/No78Vino/gameplay-ability-system-for-unity/assets/43328860/acd53826-d032-49f3-a878-a649f98311a1)
- c.游戏效果管理
![X8L)UXB@ ARMV8}{P$04JAY](https://github.com/No78Vino/gameplay-ability-system-for-unity/assets/43328860/3442e5f8-fce4-4784-b393-064c15998401)
- d.游戏提示 & MMC 管理

### 3. GAS Runtime Watcher (GAS运行时监视器)
![UA_XISRD_ F9_W}7}JKII9M](https://github.com/No78Vino/gameplay-ability-system-for-unity/assets/43328860/d1a689a2-5c72-42ec-8d2d-60f005eab899)
通过在菜单栏EX-GAS -> GAS Runtime Watcher 可以打开监视器。监视器只能在Editor下游戏运行时使用，监视器会显示GAS下正在运行的所有ASC（Ability System Component）的基础信息。

__*注意！由于该监视器的监视刷新逻辑过于暴力，因此存在明显的性能问题。监视器只是为了方便调试，所以建议不要一直后台挂着监视器，有需要时再打开。*__

>目前监视器较为简陋，以后可能会优化监视器。

---
## 4.API(W.I.P 施工中)

---
## 5.如果...我想...,应该怎么做?
---
## 6.暂不支持的功能（可能有遗漏）
1. GameplayEffect Stack， 同一游戏效果堆叠（如燃烧效果堆叠，伤害提升）
2. RPC相关的GE复制广播
3. GameplayEffect Execution，目前只有Modifier，没有Execution
4. Ability的触发判断用的Source/Target Tag目前不生效

## 7.后续计划
- 修复bug 
- 补全遗漏的功能 
- 优化Ability的编辑
- 支持RPC的GE复制广播，网络同步 
- 将GAS移交DOTS或采用ECS结构来运行(可能会做)

## 8.特别感谢
本插件全面参考了[UE的GAS解析](https://github.com/tranek/GASDocumentation)，来自github --[@tranek](https://github.com/tranek)

同时还有[中译版本](https://github.com/BillEliot/GASDocumentation_Chinese)，来自github --[@BillEliot](https://github.com/BillEliot)

没有上述二位的文章，本项目的开发会非常痛苦。

另外还要感谢开源项目：[UnityToolchainsTrick](https://github.com/XINCGer/UnityToolchainsTrick)

多亏UnityToolchainsTrick中的大量Editor开发技巧，极大的缩减了项目中编辑器的制作时间，省了很多事儿。非常感谢！
## 9.插件反馈渠道
QQ群号:616570103

目前该插件是一定有大量bug存在的，因为有非常多的细节没有测试到，虽然有Demo演示，但也只是一部分的功能。所以我希望有人能使用该插件，多多反馈，来完善该插件。

GAS使用门槛高，所以有任何GAS相关使用的疑问，bug或者建议，欢迎来反馈群里交流。我都会尽可能回答的。


