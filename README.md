# EX Gameplay Ability System For Unity
## 前言
该项目为Unreal Engine的Gameplay Ability System的Unity实现，目前实现了部分功能，后续会继续完善。
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

### 2. GAS Asset Aggregator (GAS配置资源聚合器)
#### a.ASC预设管理
![_MX$4_XP`OZP{NC}A%OI 7N](https://github.com/No78Vino/gameplay-ability-system-for-unity/assets/43328860/24c97754-c788-4409-96aa-81a6f50176e6)
#### b.能力配置管理
![B4B{UAPNCOA 7CE6MQF7878](https://github.com/No78Vino/gameplay-ability-system-for-unity/assets/43328860/c29c6cf9-6c8b-4ae7-8c24-4e8d445e3d46)
#### c.游戏效果管理


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
