# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.5] - 2024-04-12

修复了部分bug；优化编辑器操作。

### Added

- 优化编辑器操作。（From: BCC @kenkinky）

### Fixed

- 修复了TryActivateAbility的返回值逻辑错误。


## [1.0.4] - 2024-04-11

修复了部分bug；测试通过了推导属性设计；优化了GE容器的管理，增强代码可读性。

### Added

- 添加了GAS内部的子Event系统，为方便之后用上事件系统做准备。

### Fixed

- 推导属性的实时更新错误。补上了AttributeBasedMMC的Track类修改器属性变化监听。
- 修复GASHost销毁时的错误逻辑，Host的静态单例改为饿汉式，同步GAS的初始化只会执行一次。

### Changed

- 优化GameplayEffectContainer结构，现在只维护一个GameplayEffect列表

### Removed

- 移除DerivedAttribute和MetaAttribute脚本，弃用。这两个属性式设计方式，而不是实际存在的类。

## [1.0.3] - 2024-04-09

删除SetByCallerModCalculation,弃用。

### Removed

- 删除SetByCallerModCalculation,弃用。

## [1.0.2] - 2024-04-08

优化Editor使用体验（From: BCC @kenkinky）

### Changed

- 优化Editor使用体验（From: BCC @kenkinky）


## [1.0.1] - 2024-03-29

删除Instant类型GameplayCue的Apply Target参数。

### Removed

- Instant类型GameplayCue的Apply Target弃用。

## [1.0.0] - 2024-03-13

EX-GAS 1.0.0 发布

### Added


### Fixed

- none

### Changed

- none

### Removed

- none