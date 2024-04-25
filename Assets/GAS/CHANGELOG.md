# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).


## [1.0.9] - 2024-04-25

优化type查找，优化GAS的项目级配置文件管理。

### Changed

- 新增TimelineAbilityT, 方便继承和扩展TimelineAbility.（From: BCC @kenkinky）

### Fixed

- 修正TryAddDynamicAddedTag添加不同类型Source时类型转换失败异常（From: BCC @kenkinky）
- 修复了Setting中生成配置目录后，未调用AssetDatabase.Refresh()导致配置文件目录未及时更新的问题。
## [1.0.8] - 2024-04-23

优化了部分GC。

### Fixed

- AttributeSetContainer的TryGetAttributeSet方法中，Type.Name存在GC。
  - 新增了预缓存接口:GasCache.CacheAttributeSetName。
  - 使用方法：在GAS初始化时，调用GasCache.CacheAttributeSetName(GAttrSetLib.TypeToName);
- GameplayTagAggregator的Tag判断相关方法存在GC。GC来源是LINQ表达式的过程匿名方法产生的GC。已经把LINQ表达式改成了普通循环做法。
- 新增了Pool工具类，优化了部分GC。（From: BCC @kenkinky）

## [1.0.7] - 2024-04-17

修复全局配置保存失败问题；修复Editor代码不该编译问题

### Fixed

- 修复全局配置保存失败问题，Tag，Attribute，AttributeSet，Setting的配置文件保存不该使用AssetDataBase。
- 修正无法打包编译异常 #11 （From: BCC @kenkinky）

## [1.0.6] - 2024-04-16

优化type查找，优化GAS的项目级配置文件管理。

### Changed

- 修改了Tag，Attribute，AttributeSet，Setting的配置文件路径，调整至ProjectSettings，并且为单例配置文件。
- 优化了TypeUtil，Editor环境下类型查找范围改为全程序集。

### Fixed

- 修复一个严重bug: 修复AttributeBasedModCalculation不能正确保存的问题, 还有一些小优化.（From: BCC @kenkinky）

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