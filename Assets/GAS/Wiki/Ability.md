# EX-GAS Wiki -- Ability
> Ability是EX-GAS的核心类之一，它是游戏中的所有能力基础。
> 
> 同时Ability也是程序开发人员最常接触的类，Ability的完整逻辑都是由程序开发人员实现的。
> 
> 【EX-GAS目前没有能力的可视化编辑器，后续计划会制作Ability的编辑器，尝试实现自动化生成Ability脚本】
> 
## 前言
在EX-GAS内，Ability是游戏中可以触发的一切行为和技能。多个Ability可以在同一时刻激活, 例如移动和持盾防御。
Ability作为EX-GAS的核心类之一，他起到了Do（做）的功能。

Ability的业务逻辑取决于游戏类型和玩法。所以不存在一个通用的Ability模板，当然可以针对游戏类型制作一些通用的ability。
Ability的逻辑并非自由，如果胡乱的实现Ability逻辑，可能会导致游戏逻辑混乱，所以需要遵循一些规则。
---
Ability的具体实现需要策划和程序配合。
这并不是废话，而是在EX-GAS的Ability制作流程中，确确实实的把策划和程序的工作分开了：
- 程序的工作：编写Ability（Ability，AbilitySpec）类
- 策划的工作：配置AbilityAsset
---
为了更好的理解，首先我来介绍Ability的Runtime运行逻辑。
## Ability运行逻辑

## Ability子类的编写流程
这一部分是针对程序开发人员的，策划人员可以粗略一览这一部分。

## AbilityAsset的配置流程
这一部分是针对策划人员的，程序开发人员可以粗略一览这一部分。

## Ability的设计原则和一些建议