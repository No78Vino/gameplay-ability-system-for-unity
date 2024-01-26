# EX-GAS Wiki
> 本页为菜单引导页，另外还会总体的概括EX-GAS的设计实现思路。
> 
## 简介
>EX-GAS是对UnrealEngine的GAS（Gameplay Ability System）的模仿和实现。

GAS 是 "Gameplay Ability System" 的缩写，是一套游戏能力系统。
这个系统的目的是为开发者提供一种灵活而强大的框架，用于实现和管理游戏中的各种角色能力、技能和效果。

如果把EX-GAS高度概括为一句话，那就是：**WHO DO WHAT**。
- Who：AbilitySystemComponent（ASC）,EX-GAS的实例对象，是体系运转的基础单位
- Do：Ability，是游戏中可以触发的一切行为和技能
- What：GameplayEffect(GE)，掌握了游戏内元素的属性实际控制权，GameplayEffect本身应该理解为结果

GAS本质是一套属性数值的管理系统，GameplayCue我个人理解为是附加价值（虽然这个附加价值很有分量）。
纵使GAS的Tag体系解决复杂的GameplayEffect和Ability的逻辑，但最终的结果目的也只是掌握属性数值变化。
而属性的最底层修改权力交由了GameplayEffect。所以我把GE理解为结果。

UE的GAS的使用门槛很高，这一点在我构筑完EX-GAS雏形后更是深有体会。
所以在EX-GAS的设计上，我尽可能的做简化，优化，来降低了使用门槛。
我制作了几个关键的编辑器，来帮助开发者快速的使用EX-GAS。
但即便如此，GAS本身的繁多参数依然让编辑器的界面看上去十分臃肿，这很难简化，没有哪个参数是可以被删除的。
甚至，雏形阶段的EX-GAS还有很多功能还未实现，也就是说还有更多的参数是没有被编辑器暴露出来的。

_**GAS的使用者必须至少有一名程序开发人员，因为GAS的使用需要编写大量自定义业务逻辑。
Ability，Cue，MMC等都是必须根据游戏类型和内容玩法而定的。
非程序开发人员则需要完全理解EX-GAS的运作逻辑，才能更好的快速配置出各种各样的技能和玩法表现。**_

## Wiki:EX-GAS的核心组成
- [GameplayTag](#GameplayTag)
- [Attribute](#Attribute)
- [AttributeSet](#AttributeSet)
- [ModifierMagnitudeCalculation](#ModifierMagnitudeCalculation)
- [GameplayEffect](#GameplayEffect)
- [GameplayCue](#GameplayCue)
- [Ability](#Ability)
- [AbilitySystemComponent](#AbilitySystemComponent)

## Wiki:EX-GAS辅助编辑器的使用
- [Setting Aggregator](#SettingAggregator)
  - [Gameplay Tag Manager](#AttributeSetEditor)
  - [Attribute Manager](#AttributeSetEditor)
  - [AttributeSet Manager](#AttributeSetEditor)
- [GAS Asset Aggregator]() 
  - [Gameplay Effect](#GameplayEffectManager) 
  - [Ability](#AbilityAssetManager)
  - [AbilitySystemComponent](#GameplayCueManager)