# EX Gameplay Ability System For Unity
## 前言
该项目为Unreal Engine的Gameplay Ability System的Unity实现，目前实现了部分功能，后续会继续完善。
## 提醒
__*该项目依赖Odin Inspector插件（付费），请自行解决!!!!!!!!*__

```若没有方法解决，可以加文末的使用反馈qq群，群内提供帮助 ```
## 项目结构
```
├───Assets
│   ├───GAS
│   │   ├───Editor
│   │   │   ├───Ability
│   │   │   ├───Attribute
│   │   │   ├───AttributeSet
│   │   │   ├───Cue
│   │   │   ├───Tags
│   │   │   ├───Effect
│   │   │   ├───Component
│   │   │   ├───System
│   │   │   └───General(Utility)
│   │   ├───Runtime
│   │   │   ├───Ability
│   │   │   ├───Attribute
│   │   │   ├───AttributeSet
│   │   │   ├───Component
│   │   │   ├───Effect
│   │   │   ├───Cue
│   │   │   ├───Tags
│   │   │   └───Core(System,Host,Define)
│   │   └───General
│   │ 
│   ├───Demo
│   │ 
│   └───Other(Setting,Plugins, ........) 
```
## 基础功能

## 拓展功能
### 1. GAS Base Manager (GAS基础配置管理器)
![$5C1@A0}R89 %WS33OY6UP0](https://github.com/No78Vino/gameplay-ability-system-for-unity/assets/43328860/85f4b1e2-ab3b-4735-8d71-b6623557bf02)

基础配置是与项目工程唯一对应的，所以入口放在了ProjectSetting，另外还有Edit Menu栏入口：EX-GAS -> Setting

#### a.GameplayTag 管理器
![{)7T)P@{U}GWY7T%@ 5$@@W](https://github.com/No78Vino/gameplay-ability-system-for-unity/assets/43328860/d5306afc-82a0-4c3e-a263-280c0088f1ae)

我模仿了UE GAS的Tag管理视图，做了树结构管理。

#### b.Attribute 管理器

项目内可操作的属性只可从已配置的Attribute中选取。

#### c.AttributeSet 管理器
![HBDG` 0 {{9 G6@AS_I0YF](https://github.com/No78Vino/gameplay-ability-system-for-unity/assets/43328860/bc227c91-1dc1-408b-93e6-c93b5936b232)

属性集可以预设生成，也支持运行时自定义（CustomAttributeSet）。

### 2. GAS Asset Aggregator (GAS配置资源聚合器)
![N~~1W5_AQQ42XY6T`9D)G3F](https://github.com/No78Vino/gameplay-ability-system-for-unity/assets/43328860/8f3ab649-fc80-426a-aa44-3a52a9df19c4)

因为GAS使用过程需要大量的配置（各类预设：ASC，游戏能力，游戏效果/buff，游戏提示，MMC），为了方便集中管理，我制作了一个配置资源聚合器。

通过在菜单栏EX-GAS -> Asset Aggregator 可以打开配置资源聚合器。

聚合器支持：分类管理，文件夹树结构显示，搜索栏快速查找，快速创建/删除配置文件（右上角的快捷按钮）
#### a.ASC预设管理
![)@6J1G3%FESLFG$1R0E$WNL](https://github.com/No78Vino/gameplay-ability-system-for-unity/assets/43328860/0cb0b5a9-cfde-44ad-b121-e0077330a02e)
#### b.能力配置管理
![RY_~6~7BSVSOA0`C 4F8FD](https://github.com/No78Vino/gameplay-ability-system-for-unity/assets/43328860/acd53826-d032-49f3-a878-a649f98311a1)
#### c.游戏效果管理
![X8L)UXB@ ARMV8}{P$04JAY](https://github.com/No78Vino/gameplay-ability-system-for-unity/assets/43328860/3442e5f8-fce4-4784-b393-064c15998401)
#### d.游戏提示 & MMC 管理

### 3. GAS Runtime Watcher (GAS运行时监视器)
![UA_XISRD_ F9_W}7}JKII9M](https://github.com/No78Vino/gameplay-ability-system-for-unity/assets/43328860/d1a689a2-5c72-42ec-8d2d-60f005eab899)
通过在菜单栏EX-GAS -> GAS Runtime Watcher 可以打开监视器。监视器只能在Editor下游戏运行时使用，监视器会显示GAS下正在运行的所有ASC（Ability System Component）的基础信息。

__*注意！由于该监视器的监视刷新逻辑过于暴力，因此存在明显的性能问题。监视器只是为了方便调试，所以建议不要一直后台挂着监视器，有需要时再打开。*__

```目前监视器较为简陋，以后可能会优化监视器。```

## 快速开始
### 1.安装
#### 1.1.使用Unity Package Manager安装
在Unity Package Manager中添加git地址：

#### 1.2.使用git clone

### 2.使用


## 后续计划
### 1. 修复bug
### 2. 补全遗漏的功能
### 3. 支持网络同步
### 4. 将GAS移交DOTS或采用ECS结构来运行

## 特别感谢
## 插件反馈渠道
QQ群号:616570103


