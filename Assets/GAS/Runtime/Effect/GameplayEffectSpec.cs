using System;  
using Unity.Collections;  
using Unity.Entities;

namespace GAS.Runtime
{
    /// <summary>  
    /// GameplayEffect 的 OOP 包装类，面向用户的操作入口。  
    /// 设计准则：门面无 ECS，内部随便用。  
    ///   
    /// 所有 public/protected 成员的形参和返回值中不出现 ECS/Entities 类型  
    /// （Entity, EntityManager, NativeArray, DynamicBuffer, IComponentData 等）。  
    ///   
    /// 【注意】组件的增删（Add/Remove）应在 GE Apply 之前完成。  
    /// GE 被 Apply 后动态增删组件可能导致 ECS System 运行异常。  
    /// 数据修改（Set 系列方法）在任何阶段都是安全的。  
    /// </summary>  
    public class GameplayEffectSpec
    {
        // ==================== 内部字段 ====================  

        internal Entity Entity { get; private set; }

        private static EntityManager _em => GASManager.EntityManager;

        // ==================== 构造函数 ====================  

        public GameplayEffectSpec(GameplayEffectComponentConfig[] componentConfigs)
        {
            Entity = GameplayEffectHelper.CreateGameplayEffectEntity(componentConfigs);
        }

        internal GameplayEffectSpec(Entity geEntity)
        {
            Entity = geEntity;
        }

        // ==================== 基础属性 ====================  

        /// <summary>GE 是否有效（底层 Entity 是否存在）</summary>  
        public bool IsValid => Entity != Entity.Null && _em.Exists(Entity);

        /// <summary>GE 名称（调试用）</summary>  
        public string Name
        {
            get
            {
                if (!IsValid || !_em.HasComponent<CEffectBasicInfo>(Entity)) return string.Empty;
                return _em.GetComponentData<CEffectBasicInfo>(Entity).name.ToString();
            }
        }

        /// <summary>GE 是否已被 Apply（已进入 ECS 管线）</summary>  
        public bool IsApplied => IsValid && _em.HasComponent<CEffectApplied>(Entity);

        /// <summary>GE 是否被标记销毁</summary>  
        public bool IsDestroyed => !IsValid || _em.HasComponent<CEffectDestroy>(Entity);

        /// <summary>GE 是否是实例（从原型 Instantiate 出来的）</summary>  
        public bool IsInstance => IsValid && _em.HasComponent<CEffectInstance>(Entity);

        /// <summary>GE 是否正在被销毁</summary>  
        public bool IsDestroying => IsValid && _em.HasComponent<CEffectDestroy>(Entity);

        // ==================== 来源/目标/等级 ====================  

        /// <summary>施加来源 ASC（GE Apply 后才有值）</summary>  
        public AbilitySystemCell Source
        {
            get
            {
                if (!IsValid || !_em.HasComponent<CEffectInUsage>(Entity)) return null;
                return GASManager.GetAscFromEntity(_em.GetComponentData<CEffectInUsage>(Entity).Source);
            }
        }

        /// <summary>施加目标 ASC（GE Apply 后才有值）</summary>  
        public AbilitySystemCell Target
        {
            get
            {
                if (!IsValid || !_em.HasComponent<CEffectInUsage>(Entity)) return null;
                return GASManager.GetAscFromEntity(_em.GetComponentData<CEffectInUsage>(Entity).Target);
            }
        }

        /// <summary>GE 等级</summary>  
        public int Level
        {
            get
            {
                if (!IsValid || !_em.HasComponent<CEffectInUsage>(Entity)) return 0;
                return _em.GetComponentData<CEffectInUsage>(Entity).Level;
            }
            set
            {
                if (!IsValid || !_em.HasComponent<CEffectInUsage>(Entity)) return;
                var inUsage = _em.GetComponentData<CEffectInUsage>(Entity);
                inUsage.Level = value;
                _em.SetComponentData(Entity, inUsage);
            }
        }

        // ==================== 操作方法 ====================  

        /// <summary>移除此 GE（标记销毁，由 ECS System 执行实际移除）</summary>  
        public void Remove()
        {
            if (!IsValid) return;
            GameplayEffectHelper.RemoveGameplayEffect(Entity);
        }

        /// <summary>对目标施加此 GE</summary>  
        public void ApplyTo(AbilitySystemCell target, AbilitySystemCell source)
        {
            if (!IsValid || target == null || source == null) return;
            GameplayEffectHelper.ApplyGameplayEffectTo(Entity, target.Entity, source.Entity);
        }

        /// <summary>对目标施加此 GE（source = target，即自身施加）</summary>  
        public void ApplyToSelf(AbilitySystemCell target)
        {
            ApplyTo(target, target);
        }

        #region Duration

        /// <summary>检查是否存在 Duration 组件</summary>  
        public bool CheckDurationExist() => IsValid && _em.HasComponent<CDuration>(Entity);

        /// <summary>添加 Duration 组件</summary>  
        /// <param name="duration">持续时间（帧/回合），-1 表示无限</param>  
        /// <param name="timeUnit">计时单位</param>  
        /// <param name="resetStartTimeWhenActivated">激活时是否重置计时起始</param>  
        /// <param name="stopTickWhenDeactivated">失活时是否停止计时</param>  
        public void AddDuration(int duration, TimeUnit timeUnit = TimeUnit.Frame,
            bool resetStartTimeWhenActivated = false, bool stopTickWhenDeactivated = false)
        {
            if (!IsValid || CheckDurationExist()) return;
            EntityHelper.AddComponent<CDuration>(Entity);
            EntityHelper.SetComponent(Entity, new CDuration
            {
                duration = duration,
                timeUnit = timeUnit,
                ResetStartTimeWhenActivated = resetStartTimeWhenActivated,
                StopTickWhenDeactivated = stopTickWhenDeactivated,
                active = false
            });
        }

        /// <summary>移除 Duration 组件</summary>  
        public void RemoveDuration()
        {
            if (!IsValid || !CheckDurationExist()) return;
            EntityHelper.RemoveComponent<CDuration>(Entity);
        }

        /// <summary>获取持续时间配置值</summary>  
        public int GetDuration()
        {
            if (!CheckDurationExist()) return 0;
            return _em.GetComponentData<CDuration>(Entity).duration;
        }

        /// <summary>设置持续时间配置值</summary>  
        public void SetDuration(int duration)
        {
            if (!CheckDurationExist()) return;
            var com = _em.GetComponentData<CDuration>(Entity);
            com.duration = duration;
            _em.SetComponentData(Entity, com);
        }

        /// <summary>获取计时单位</summary>  
        public TimeUnit GetDurationTimeUnit()
        {
            if (!CheckDurationExist()) return TimeUnit.Frame;
            return _em.GetComponentData<CDuration>(Entity).timeUnit;
        }

        /// <summary>设置计时单位</summary>  
        public void SetDurationTimeUnit(TimeUnit timeUnit)
        {
            if (!CheckDurationExist()) return;
            var com = _em.GetComponentData<CDuration>(Entity);
            com.timeUnit = timeUnit;
            _em.SetComponentData(Entity, com);
        }

        /// <summary>Duration 是否激活生效中（运行时状态）</summary>  
        public bool IsDurationActive()
        {
            if (!CheckDurationExist()) return false;
            return _em.GetComponentData<CDuration>(Entity).active;
        }

        /// <summary>获取激活起始时间点（运行时状态）</summary>  
        public int GetDurationActiveTime()
        {
            if (!CheckDurationExist()) return 0;
            return _em.GetComponentData<CDuration>(Entity).activeTime;
        }

        /// <summary>获取剩余持续时间（运行时状态，StopTickWhenDeactivated=true 时有效）</summary>  
        public int GetDurationRemainTime()
        {
            if (!CheckDurationExist()) return 0;
            return _em.GetComponentData<CDuration>(Entity).remianTime;
        }

        /// <summary>获取 ResetStartTimeWhenActivated 配置</summary>  
        public bool GetDurationResetOnActivated()
        {
            if (!CheckDurationExist()) return false;
            return _em.GetComponentData<CDuration>(Entity).ResetStartTimeWhenActivated;
        }

        /// <summary>设置 ResetStartTimeWhenActivated 配置</summary>  
        public void SetDurationResetOnActivated(bool value)
        {
            if (!CheckDurationExist()) return;
            var com = _em.GetComponentData<CDuration>(Entity);
            com.ResetStartTimeWhenActivated = value;
            _em.SetComponentData(Entity, com);
        }

        /// <summary>获取 StopTickWhenDeactivated 配置</summary>  
        public bool GetDurationStopTickWhenDeactivated()
        {
            if (!CheckDurationExist()) return false;
            return _em.GetComponentData<CDuration>(Entity).StopTickWhenDeactivated;
        }

        /// <summary>设置 StopTickWhenDeactivated 配置</summary>  
        public void SetDurationStopTickWhenDeactivated(bool value)
        {
            if (!CheckDurationExist()) return;
            var com = _em.GetComponentData<CDuration>(Entity);
            com.StopTickWhenDeactivated = value;
            _em.SetComponentData(Entity, com);
        }

        #endregion


        #region 内部 Tag 通用方法

        private bool CheckTagComponentExist<T>() where T : unmanaged, IComponentData
            => IsValid && _em.HasComponent<T>(Entity);

        private (int[] all, int[] any, int[] none) GetTagRequirementInternal<T>(Func<T, TagRequirementData> getter)
            where T : unmanaged, IComponentData
        {
            if (!CheckTagComponentExist<T>()) return (Array.Empty<int>(), Array.Empty<int>(), Array.Empty<int>());
            var requirement = getter(_em.GetComponentData<T>(Entity));
            return (requirement.all.IsCreated ? requirement.all.ToArray() : Array.Empty<int>(),
                requirement.any.IsCreated ? requirement.any.ToArray() : Array.Empty<int>(),
                requirement.none.IsCreated ? requirement.none.ToArray() : Array.Empty<int>());
        }

        private int[] GetTagsInternal<T>(Func<T, NativeArray<int>> getter) where T : unmanaged, IComponentData
        {
            if (!CheckTagComponentExist<T>()) return Array.Empty<int>();
            var native = getter(_em.GetComponentData<T>(Entity));
            return native.IsCreated ? native.ToArray() : Array.Empty<int>();
        }

        private void SetTagRequirementInternal<T>(int[] all, int[] any, int[] none,
            Func<T, TagRequirementData> getter, Func<TagRequirementData, T> factory)
            where T : unmanaged, IComponentData
        {
            if (!IsValid) return;
            // 如果组件已存在，先释放旧 NativeArray  
            if (_em.HasComponent<T>(Entity))
            {
                var old = getter(_em.GetComponentData<T>(Entity));
                if (old.all.IsCreated) old.all.Dispose();
                if (old.any.IsCreated) old.any.Dispose();
                if (old.none.IsCreated) old.none.Dispose();
            }
            else
            {
                EntityHelper.AddComponent<T>(Entity);
            }

            EntityHelper.SetComponent(Entity, factory(new TagRequirementData
            {
                all = new NativeArray<int>(all ?? Array.Empty<int>(), Allocator.Persistent),
                any = new NativeArray<int>(any ?? Array.Empty<int>(), Allocator.Persistent),
                none = new NativeArray<int>(none ?? Array.Empty<int>(), Allocator.Persistent)
            }));
        }

        private void SetTagsInternal<T>(int[] tags, Func<T, NativeArray<int>> getter, Func<NativeArray<int>, T> factory)
            where T : unmanaged, IComponentData
        {
            if (!IsValid) return;
            // 如果组件已存在，先释放旧 NativeArray  
            if (_em.HasComponent<T>(Entity))
            {
                var old = getter(_em.GetComponentData<T>(Entity));
                if (old.IsCreated) old.Dispose();
            }
            else
            {
                EntityHelper.AddComponent<T>(Entity);
            }

            EntityHelper.SetComponent(Entity, factory(new NativeArray<int>(tags, Allocator.Persistent)));
        }

        private void AddTagRequirementComponentInternal<T>(int[] all, int[] any, int[] none, Func<TagRequirementData, T> factory)
            where T : unmanaged, IComponentData
        {
            if (!IsValid || _em.HasComponent<T>(Entity)) return;
            EntityHelper.AddComponent<T>(Entity);
            EntityHelper.SetComponent(Entity, factory(new TagRequirementData
            {
                all = new NativeArray<int>(all ?? Array.Empty<int>(), Allocator.Persistent),
                any = new NativeArray<int>(any ?? Array.Empty<int>(), Allocator.Persistent),
                none = new NativeArray<int>(none ?? Array.Empty<int>(), Allocator.Persistent)
            }));
        }

        private void AddTagComponentInternal<T>(int[] tags, Func<NativeArray<int>, T> factory)
            where T : unmanaged, IComponentData
        {
            if (!IsValid || _em.HasComponent<T>(Entity)) return;
            EntityHelper.AddComponent<T>(Entity);
            EntityHelper.SetComponent(Entity, factory(new NativeArray<int>(tags, Allocator.Persistent)));
        }

        private void RemoveTagRequirementComponentInternal<T>(Func<T, TagRequirementData> getter)
            where T : unmanaged, IComponentData
        {
            if (!IsValid || !_em.HasComponent<T>(Entity)) return;
            var old = getter(_em.GetComponentData<T>(Entity));
            if (old.all.IsCreated) old.all.Dispose();
            if (old.any.IsCreated) old.any.Dispose();
            if (old.none.IsCreated) old.none.Dispose();
            EntityHelper.RemoveComponent<T>(Entity);
        }

        private void RemoveTagComponentInternal<T>(Func<T, NativeArray<int>> getter)
            where T : unmanaged, IComponentData
        {
            if (!IsValid || !_em.HasComponent<T>(Entity)) return;
            var old = getter(_em.GetComponentData<T>(Entity));
            if (old.IsCreated) old.Dispose();
            EntityHelper.RemoveComponent<T>(Entity);
        }

        #endregion


        #region AssetTags

        public bool CheckAssetTagsExist() => CheckTagComponentExist<CEffectAssetTags>();

        public int[] GetAssetTags() => GetTagsInternal<CEffectAssetTags>(c => c.tags);

        public void SetAssetTags(int[] tags) => SetTagsInternal<CEffectAssetTags>(tags,
            c => c.tags, arr => new CEffectAssetTags { tags = arr });

        public void AddAssetTags(int[] tags) => AddTagComponentInternal<CEffectAssetTags>(tags,
            arr => new CEffectAssetTags { tags = arr });

        public void RemoveAssetTags() => RemoveTagComponentInternal<CEffectAssetTags>(c => c.tags);

        #endregion


        #region GrantedTags

        public bool CheckGrantedTagsExist() => CheckTagComponentExist<CEffectGrantedTags>();

        public int[] GetGrantedTags() => GetTagsInternal<CEffectGrantedTags>(c => c.tags);

        public void SetGrantedTags(int[] tags) => SetTagsInternal<CEffectGrantedTags>(tags,
            c => c.tags, arr => new CEffectGrantedTags { tags = arr });

        public void AddGrantedTags(int[] tags) => AddTagComponentInternal<CEffectGrantedTags>(tags,
            arr => new CEffectGrantedTags { tags = arr });

        public void RemoveGrantedTags() => RemoveTagComponentInternal<CEffectGrantedTags>(c => c.tags);

        #endregion


        #region ApplicationRequiredTags

        public bool CheckApplicationRequiredTagsExist() => CheckTagComponentExist<CApplicationRequiredTags>();

        public int[] GetApplicationRequiredTags() => GetTagRequirementInternal<CApplicationRequiredTags>(c => c.requirement).all;

        public void SetApplicationRequiredTags(int[] tags) => SetTagRequirementInternal<CApplicationRequiredTags>(tags, Array.Empty<int>(), Array.Empty<int>(),
            c => c.requirement, req => new CApplicationRequiredTags { requirement = req });

        public void AddApplicationRequiredTags(int[] tags) => AddTagRequirementComponentInternal<CApplicationRequiredTags>(tags, Array.Empty<int>(), Array.Empty<int>(),
            req => new CApplicationRequiredTags { requirement = req });

        public void RemoveApplicationRequiredTags() =>
            RemoveTagRequirementComponentInternal<CApplicationRequiredTags>(c => c.requirement);

        #endregion

        #region OngoingRequiredTags

        public bool CheckOngoingRequiredTagsExist() => CheckTagComponentExist<COngoingRequiredTags>();

        public int[] GetOngoingRequiredTags() => GetTagRequirementInternal<COngoingRequiredTags>(c => c.requirement).all;

        public void SetOngoingRequiredTags(int[] tags) => SetTagRequirementInternal<COngoingRequiredTags>(tags, Array.Empty<int>(), Array.Empty<int>(),
            c => c.requirement, req => new COngoingRequiredTags { requirement = req });

        public void AddOngoingRequiredTags(int[] tags) => AddTagRequirementComponentInternal<COngoingRequiredTags>(tags, Array.Empty<int>(), Array.Empty<int>(),
            req => new COngoingRequiredTags { requirement = req });

        public void RemoveOngoingRequiredTags() => RemoveTagRequirementComponentInternal<COngoingRequiredTags>(c => c.requirement);

        #endregion

        #region RemoveEffectWithTags

        public bool CheckRemoveEffectWithTagsExist() => CheckTagComponentExist<CRemoveEffectWithTags>();

        public int[] GetRemoveEffectWithTags() => GetTagRequirementInternal<CRemoveEffectWithTags>(c => c.requirement).any;

        public void SetRemoveEffectWithTags(int[] tags) => SetTagRequirementInternal<CRemoveEffectWithTags>(Array.Empty<int>(), tags, Array.Empty<int>(),
            c => c.requirement, req => new CRemoveEffectWithTags { requirement = req });

        public void AddRemoveEffectWithTags(int[] tags) => AddTagRequirementComponentInternal<CRemoveEffectWithTags>(Array.Empty<int>(), tags, Array.Empty<int>(),
            req => new CRemoveEffectWithTags { requirement = req });

        public void RemoveRemoveEffectWithTags() => RemoveTagRequirementComponentInternal<CRemoveEffectWithTags>(c => c.requirement);

        #endregion

        #region ImmunityTags

        public bool CheckImmunityTagsExist() => CheckTagComponentExist<CEffectImmunityTags>();

        public int[] GetImmunityTags() => GetTagRequirementInternal<CEffectImmunityTags>(c => c.requirement).any;

        public void SetImmunityTags(int[] tags) => SetTagRequirementInternal<CEffectImmunityTags>(Array.Empty<int>(), tags, Array.Empty<int>(),
            c => c.requirement, req => new CEffectImmunityTags { requirement = req });

        public void AddImmunityTags(int[] tags) => AddTagRequirementComponentInternal<CEffectImmunityTags>(Array.Empty<int>(), tags, Array.Empty<int>(),
            req => new CEffectImmunityTags { requirement = req });

        public void RemoveImmunityTags() => RemoveTagRequirementComponentInternal<CEffectImmunityTags>(c => c.requirement);

        #endregion

// =====================================================================================  
//  Period 组件 (CPeriod) — 完整版，含子GE管理  
// =====================================================================================  

        #region Period

// ---- 组件存在性检查 ----  

        public bool CheckPeriodExist() => IsValid && _em.HasComponent<CPeriod>(Entity);

// ---- 基础参数读写 ----  

        /// <summary>获取周期间隔（帧或回合）</summary>  
        public int GetPeriod()
        {
            if (!CheckPeriodExist()) return 0;
            return _em.GetComponentData<CPeriod>(Entity).Period;
        }

        /// <summary>设置周期间隔</summary>  
        public void SetPeriod(int period)
        {
            if (!CheckPeriodExist()) return;
            var com = _em.GetComponentData<CPeriod>(Entity);
            com.Period = period;
            _em.SetComponentData(Entity, com);
        }

        /// <summary>获取周期开始时间（运行时状态）</summary>  
        public int GetPeriodStartTime()
        {
            if (!CheckPeriodExist()) return 0;
            return _em.GetComponentData<CPeriod>(Entity).StartTime;
        }

        /// <summary>获取 ResetTimeCountWhenDeactivated 配置</summary>  
        public bool GetPeriodResetOnDeactivated()
        {
            if (!CheckPeriodExist()) return false;
            return _em.GetComponentData<CPeriod>(Entity).ResetTimeCountWhenDeactivated;
        }

        /// <summary>设置 ResetTimeCountWhenDeactivated 配置</summary>  
        public void SetPeriodResetOnDeactivated(bool value)
        {
            if (!CheckPeriodExist()) return;
            var com = _em.GetComponentData<CPeriod>(Entity);
            com.ResetTimeCountWhenDeactivated = value;
            _em.SetComponentData(Entity, com);
        }

// ---- 子 GE 管理 ----  

        /// <summary>获取周期子 GE 的数量</summary>  
        public int GetPeriodGameplayEffectCount()
        {
            if (!CheckPeriodExist()) return 0;
            var com = _em.GetComponentData<CPeriod>(Entity);
            return com.GameplayEffects.IsCreated ? com.GameplayEffects.Length : 0;
        }

        /// <summary>  
        /// 获取指定索引的周期子 GE 的 Spec 包装。  
        /// 返回的 GameplayEffectSpec 包装的是子 GE 的原型 Entity（非实例）。  
        /// </summary>  
        public GameplayEffectSpec GetPeriodGameplayEffect(int index)
        {
            if (!CheckPeriodExist()) return null;
            var com = _em.GetComponentData<CPeriod>(Entity);
            if (!com.GameplayEffects.IsCreated || index < 0 || index >= com.GameplayEffects.Length)
                return null;
            return new GameplayEffectSpec(com.GameplayEffects[index]);
        }

        /// <summary>  
        /// 获取所有周期子 GE 的 Spec 包装数组。  
        /// 返回的每个 GameplayEffectSpec 包装的是子 GE 的原型 Entity。  
        /// </summary>  
        public GameplayEffectSpec[] GetAllPeriodGameplayEffects()
        {
            if (!CheckPeriodExist()) return Array.Empty<GameplayEffectSpec>();
            var com = _em.GetComponentData<CPeriod>(Entity);
            if (!com.GameplayEffects.IsCreated || com.GameplayEffects.Length == 0)
                return Array.Empty<GameplayEffectSpec>();

            var specs = new GameplayEffectSpec[com.GameplayEffects.Length];
            for (int i = 0; i < com.GameplayEffects.Length; i++)
                specs[i] = new GameplayEffectSpec(com.GameplayEffects[i]);
            return specs;
        }

        /// <summary>  
        /// 通过 GE 配置 ID 添加一个子 GE 到 Period 中。  
        /// 内部通过 GameplayEffectHelper.GetConfigByID() 加载配置，创建子 GE Entity 并追加到 GameplayEffects 数组。  
        /// </summary>  
        /// <returns>新添加的子 GE 的 Spec 包装，失败返回 null</returns>  
        public GameplayEffectSpec AddPeriodGameplayEffectByConfigID(int geConfigID)
        {
            if (!CheckPeriodExist()) return null;

            var config = GameplayEffectHelper.GetConfigByID(geConfigID);
            if (config == null) return null;

            var newGeEntity = GameplayEffectHelper.CreateGameplayEffectEntity(config.ComponentConfigs);
            AddPeriodGameplayEffectInternal(newGeEntity);
            return new GameplayEffectSpec(newGeEntity);
        }

        /// <summary>  
        /// 通过 GameplayEffectComponentConfig[] 添加一个子 GE 到 Period 中。  
        /// </summary>  
        /// <returns>新添加的子 GE 的 Spec 包装</returns>  
        public GameplayEffectSpec AddPeriodGameplayEffect(GameplayEffectComponentConfig[] componentConfigs)
        {
            if (!CheckPeriodExist()) return null;

            var newGeEntity = GameplayEffectHelper.CreateGameplayEffectEntity(componentConfigs);
            AddPeriodGameplayEffectInternal(newGeEntity);
            return new GameplayEffectSpec(newGeEntity);
        }

        /// <summary>  
        /// 移除指定索引的周期子 GE。  
        /// 会销毁对应的子 GE Entity 并重建 NativeArray。  
        /// </summary>  
        public void RemovePeriodGameplayEffectAt(int index)
        {
            if (!CheckPeriodExist()) return;
            var com = _em.GetComponentData<CPeriod>(Entity);
            if (!com.GameplayEffects.IsCreated || index < 0 || index >= com.GameplayEffects.Length)
                return;

            // 销毁被移除的子 GE Entity  
            var removedGe = com.GameplayEffects[index];
            if (_em.Exists(removedGe))
                _em.DestroyEntity(removedGe);

            // 重建 NativeArray（去掉 index 位置）  
            int oldLen = com.GameplayEffects.Length;
            var newArray = new NativeArray<Entity>(oldLen - 1, Allocator.Persistent);
            int writeIndex = 0;
            for (int i = 0; i < oldLen; i++)
            {
                if (i == index) continue;
                newArray[writeIndex++] = com.GameplayEffects[i];
            }

            com.GameplayEffects.Dispose();
            com.GameplayEffects = newArray;
            _em.SetComponentData(Entity, com);
        }

        /// <summary>  
        /// 清空所有周期子 GE。  
        /// 会销毁所有子 GE Entity 并释放 NativeArray。  
        /// </summary>  
        public void ClearPeriodGameplayEffects()
        {
            if (!CheckPeriodExist()) return;
            var com = _em.GetComponentData<CPeriod>(Entity);
            if (!com.GameplayEffects.IsCreated) return;

            foreach (var ge in com.GameplayEffects)
            {
                if (_em.Exists(ge))
                    _em.DestroyEntity(ge);
            }

            com.GameplayEffects.Dispose();
            com.GameplayEffects = new NativeArray<Entity>(0, Allocator.Persistent);
            _em.SetComponentData(Entity, com);
        }

        /// <summary>  
        /// 替换指定索引的周期子 GE（通过 GE 配置 ID）。  
        /// 销毁旧的子 GE Entity，创建新的替换。  
        /// </summary>  
        /// <returns>新子 GE 的 Spec 包装，失败返回 null</returns>  
        public GameplayEffectSpec ReplacePeriodGameplayEffectByConfigID(int index, int geConfigID)
        {
            if (!CheckPeriodExist()) return null;
            var com = _em.GetComponentData<CPeriod>(Entity);
            if (!com.GameplayEffects.IsCreated || index < 0 || index >= com.GameplayEffects.Length)
                return null;

            var config = GameplayEffectHelper.GetConfigByID(geConfigID);
            if (config == null) return null;

            // 销毁旧的  
            var oldGe = com.GameplayEffects[index];
            if (_em.Exists(oldGe))
                _em.DestroyEntity(oldGe);

            // 创建新的  
            var newGeEntity = GameplayEffectHelper.CreateGameplayEffectEntity(config.ComponentConfigs);
            com.GameplayEffects[index] = newGeEntity;
            _em.SetComponentData(Entity, com);
            return new GameplayEffectSpec(newGeEntity);
        }

// ---- 内部帮助方法 ----  

        /// <summary>向 GameplayEffects 追加一个 Entity（扩容 NativeArray）</summary>  
        private void AddPeriodGameplayEffectInternal(Entity newGeEntity)
        {
            var com = _em.GetComponentData<CPeriod>(Entity);
            int oldLen = com.GameplayEffects.IsCreated ? com.GameplayEffects.Length : 0;

            var newArray = new NativeArray<Entity>(oldLen + 1, Allocator.Persistent);
            if (com.GameplayEffects.IsCreated)
            {
                NativeArray<Entity>.Copy(com.GameplayEffects, newArray, oldLen);
                com.GameplayEffects.Dispose();
            }

            newArray[oldLen] = newGeEntity;

            com.GameplayEffects = newArray;
            _em.SetComponentData(Entity, com);
        }

// ---- 组件级增删 ----  

        /// <summary>  
        /// 添加 Period 组件（含子 GE 配置）。  
        /// <para>[Warning] 建议仅在 GE Apply 之前调用。Period 必须配合 Duration 组件使用。</para>  
        /// </summary>  
        /// <param name="period">周期间隔</param>  
        /// <param name="resetOnDeactivated">失活时是否重置计时</param>  
        /// <param name="gameplayEffectConfigIDs">周期触发的子 GE 配置 ID 数组</param>  
        public void AddPeriod(int period, bool resetOnDeactivated, int[] gameplayEffectConfigIDs = null)
        {
            if (!IsValid || CheckPeriodExist()) return;

            // 创建子 GE Entity 数组  
            int count = gameplayEffectConfigIDs?.Length ?? 0;
            var geEntities = new NativeArray<Entity>(count, Allocator.Persistent);
            for (int i = 0; i < count; i++)
            {
                var config = GameplayEffectHelper.GetConfigByID(gameplayEffectConfigIDs[i]);
                geEntities[i] = config != null
                    ? GameplayEffectHelper.CreateGameplayEffectEntity(config.ComponentConfigs)
                    : Entity.Null;
            }

            EntityHelper.AddComponent<CPeriod>(Entity);
            EntityHelper.SetComponent(Entity, new CPeriod
            {
                Period = period,
                ResetTimeCountWhenDeactivated = resetOnDeactivated,
                GameplayEffects = geEntities
            });
        }

        /// <summary>  
        /// 移除 Period 组件。  
        /// 会销毁所有子 GE Entity 并释放 NativeArray。  
        /// <para>[Warning] 建议仅在 GE Apply 之前调用。</para>  
        /// </summary>  
        public void RemovePeriod()
        {
            if (!CheckPeriodExist()) return;
            var com = _em.GetComponentData<CPeriod>(Entity);
            if (com.GameplayEffects.IsCreated)
            {
                foreach (var ge in com.GameplayEffects)
                {
                    if (_em.Exists(ge))
                        _em.DestroyEntity(ge);
                }

                com.GameplayEffects.Dispose();
            }

            _em.RemoveComponent<CPeriod>(Entity);
        }

        #endregion


        #region Stacking

        public bool CheckStackingExist() => IsValid && _em.HasComponent<CStacking>(Entity);

        /// <summary>获取堆叠类型</summary>  
        public EffectStackType GetStackType()
        {
            if (!CheckStackingExist()) return default;
            return _em.GetComponentData<CStacking>(Entity).StackType;
        }

        /// <summary>获取堆叠码</summary>  
        public int GetStackingCode()
        {
            if (!CheckStackingExist()) return 0;
            return _em.GetComponentData<CStacking>(Entity).StackingCode;
        }

        /// <summary>获取堆叠上限</summary>  
        public int GetStackLimitCount()
        {
            if (!CheckStackingExist()) return 0;
            return _em.GetComponentData<CStacking>(Entity).LimitCount;
        }

        /// <summary>获取 Duration 刷新策略</summary>  
        public EffectDurationRefreshPolicy GetDurationRefreshPolicy()
        {
            if (!CheckStackingExist()) return default;
            return _em.GetComponentData<CStacking>(Entity).EffectDurationRefreshPolicy;
        }

        /// <summary>获取 Period 重置策略</summary>  
        public EffectPeriodResetPolicy GetPeriodResetPolicy()
        {
            if (!CheckStackingExist()) return default;
            return _em.GetComponentData<CStacking>(Entity).EffectPeriodResetPolicy;
        }

        /// <summary>获取过期策略</summary>  
        public EffectExpirationPolicy GetExpirationPolicy()
        {
            if (!CheckStackingExist()) return default;
            return _em.GetComponentData<CStacking>(Entity).EffectExpirationPolicy;
        }

        /// <summary>获取 denyOverflowApplication</summary>  
        public bool GetDenyOverflowApplication()
        {
            if (!CheckStackingExist()) return false;
            return _em.GetComponentData<CStacking>(Entity).denyOverflowApplication;
        }

        /// <summary>获取 clearStackOnOverflow</summary>  
        public bool GetClearStackOnOverflow()
        {
            if (!CheckStackingExist()) return false;
            return _em.GetComponentData<CStacking>(Entity).clearStackOnOverflow;
        }

        // ---- 配置修改 ----  

        /// <summary>设置堆叠类型</summary>  
        public void SetStackType(EffectStackType value)
        {
            if (!CheckStackingExist()) return;
            var com = _em.GetComponentData<CStacking>(Entity);
            com.StackType = value;
            _em.SetComponentData(Entity, com);
        }

        /// <summary>设置堆叠上限</summary>  
        public void SetStackLimitCount(int value)
        {
            if (!CheckStackingExist()) return;
            var com = _em.GetComponentData<CStacking>(Entity);
            com.LimitCount = value;
            _em.SetComponentData(Entity, com);
        }

        /// <summary>设置 Duration 刷新策略</summary>  
        public void SetDurationRefreshPolicy(EffectDurationRefreshPolicy value)
        {
            if (!CheckStackingExist()) return;
            var com = _em.GetComponentData<CStacking>(Entity);
            com.EffectDurationRefreshPolicy = value;
            _em.SetComponentData(Entity, com);
        }

        /// <summary>设置 Period 重置策略</summary>  
        public void SetPeriodResetPolicy(EffectPeriodResetPolicy value)
        {
            if (!CheckStackingExist()) return;
            var com = _em.GetComponentData<CStacking>(Entity);
            com.EffectPeriodResetPolicy = value;
            _em.SetComponentData(Entity, com);
        }

        /// <summary>设置过期策略</summary>  
        public void SetExpirationPolicy(EffectExpirationPolicy value)
        {
            if (!CheckStackingExist()) return;
            var com = _em.GetComponentData<CStacking>(Entity);
            com.EffectExpirationPolicy = value;
            _em.SetComponentData(Entity, com);
        }

        /// <summary>设置 denyOverflowApplication</summary>  
        public void SetDenyOverflowApplication(bool value)
        {
            if (!CheckStackingExist()) return;
            var com = _em.GetComponentData<CStacking>(Entity);
            com.denyOverflowApplication = value;
            _em.SetComponentData(Entity, com);
        }

        /// <summary>设置 clearStackOnOverflow</summary>  
        public void SetClearStackOnOverflow(bool value)
        {
            if (!CheckStackingExist()) return;
            var com = _em.GetComponentData<CStacking>(Entity);
            com.clearStackOnOverflow = value;
            _em.SetComponentData(Entity, com);
        }

        // ---- 运行时状态 ----  

        /// <summary>获取当前堆叠层数（运行时状态）</summary>  
        public int GetStackCount()
        {
            if (!CheckStackingExist()) return 0;
            return _em.GetComponentData<CStacking>(Entity).StackCount;
        }

        /// <summary>设置当前堆叠层数（运行时状态）</summary>  
        public void SetStackCount(int count)
        {
            if (!CheckStackingExist()) return;
            var com = _em.GetComponentData<CStacking>(Entity);
            com.StackCount = count;
            _em.SetComponentData(Entity, com);
        }

        // ---- 增删 ----  

        /// <summary>添加 Stacking 组件</summary>  
        public void AddStacking(int stackingCode, EffectStackType stackType, int limitCount)
        {
            if (!IsValid || CheckStackingExist()) return;
            EntityHelper.AddComponent<CStacking>(Entity);
            EntityHelper.SetComponent(Entity, new CStacking
            {
                StackingCode = stackingCode,
                StackType = stackType,
                LimitCount = limitCount,
            });
        }

        /// <summary>移除 Stacking 组件</summary>  
        public void RemoveStacking()
        {
            if (!CheckStackingExist()) return;
            var com = _em.GetComponentData<CStacking>(Entity);
            if (com.overflowEffects.IsCreated) com.overflowEffects.Dispose();
            _em.RemoveComponent<CStacking>(Entity);
        }

        #endregion

        #region Modifiers

        public bool CheckModifiersExist() => IsValid && _em.HasComponent<MCModifiers>(Entity);

        /// <summary>获取 Modifier 数量</summary>  
        public int GetModifierCount()
        {
            if (!CheckModifiersExist()) return 0;
            var mc = _em.GetComponentData<MCModifiers>(Entity);
            return mc.Modifiers?.Length ?? 0;
        }

        /// <summary>  
        /// 获取指定索引的 Modifier 信息（OOP 结构体）  
        /// </summary>  
        public ModifierInfo GetModifier(int index)
        {
            if (!CheckModifiersExist()) return default;
            var mc = _em.GetComponentData<MCModifiers>(Entity);
            if (mc.Modifiers == null || index < 0 || index >= mc.Modifiers.Length) return default;
            var m = mc.Modifiers[index];
            return new ModifierInfo
            {
                AttrSetCode = m.AttrSetCode,
                AttrCode = m.AttrCode,
                Operation = m.Operation,
                Magnitude = m.Magnitude
            };
        }

        /// <summary>获取所有 Modifier 信息（OOP 数组）</summary>  
        public ModifierInfo[] GetAllModifiers()
        {
            if (!CheckModifiersExist()) return Array.Empty<ModifierInfo>();
            var mc = _em.GetComponentData<MCModifiers>(Entity);
            if (mc.Modifiers == null) return Array.Empty<ModifierInfo>();
            var result = new ModifierInfo[mc.Modifiers.Length];
            for (int i = 0; i < mc.Modifiers.Length; i++)
            {
                var m = mc.Modifiers[i];
                result[i] = new ModifierInfo
                {
                    AttrSetCode = m.AttrSetCode,
                    AttrCode = m.AttrCode,
                    Operation = m.Operation,
                    Magnitude = m.Magnitude
                };
            }

            return result;
        }

        /// <summary>设置指定 Modifier 的 Magnitude</summary>  
        public void SetModifierMagnitude(int index, float magnitude)
        {
            if (!CheckModifiersExist()) return;
            var mc = _em.GetComponentData<MCModifiers>(Entity);
            if (mc.Modifiers == null || index < 0 || index >= mc.Modifiers.Length) return;
            mc.Modifiers[index].Magnitude = magnitude;
            // MCModifiers 是 managed component，引用类型，修改直接生效，无需 SetComponentData  
        }

        /// <summary>设置指定 Modifier 的 Operation</summary>  
        public void SetModifierOperation(int index, GEOperation operation)
        {
            if (!CheckModifiersExist()) return;
            var mc = _em.GetComponentData<MCModifiers>(Entity);
            if (mc.Modifiers == null || index < 0 || index >= mc.Modifiers.Length) return;
            mc.Modifiers[index].Operation = operation;
        }

        /// <summary>添加 Modifiers 组件（初始为空数组）</summary>  
        public void AddModifiers()
        {
            if (!IsValid || CheckModifiersExist()) return;
            EntityHelper.AddManagedComponent<MCModifiers>(Entity);
            EntityHelper.SetManagedComponent(Entity, new MCModifiers(Array.Empty<EffectModifier>()));
        }

        /// <summary>移除 Modifiers 组件</summary>  
        public void RemoveModifiers()
        {
            if (!CheckModifiersExist()) return;
            _em.RemoveComponent<MCModifiers>(Entity);
        }

        #endregion


        #region GrantedAbility

        public bool CheckGrantedAbilityExist() => IsValid && _em.HasComponent<MCGrantedAbility>(Entity);

        /// <summary>获取授予的能力数量</summary>  
        public int GetGrantedAbilityCount()
        {
            if (!CheckGrantedAbilityExist()) return 0;
            var mc = _em.GetComponentData<MCGrantedAbility>(Entity);
            return mc.GrantedAbilities?.Length ?? 0;
        }

        /// <summary>  
        /// 获取指定索引的授予能力信息（OOP 结构体）  
        /// </summary>  
        public GrantedAbilityInfo GetGrantedAbility(int index)
        {
            if (!CheckGrantedAbilityExist()) return default;
            var mc = _em.GetComponentData<MCGrantedAbility>(Entity);
            if (mc.GrantedAbilities == null || index < 0 || index >= mc.GrantedAbilities.Length) return default;
            var ga = mc.GrantedAbilities[index];
            return new GrantedAbilityInfo
            {
                Level = ga.Level,
                ActivationPolicy = ga.ActivationPolicy,
                DeactivationPolicy = ga.DeactivationPolicy,
                RemovePolicy = ga.RemovePolicy,
            };
        }

        /// <summary>获取所有授予能力信息（OOP 数组）</summary>  
        public GrantedAbilityInfo[] GetAllGrantedAbilities()
        {
            if (!CheckGrantedAbilityExist()) return Array.Empty<GrantedAbilityInfo>();
            var mc = _em.GetComponentData<MCGrantedAbility>(Entity);
            if (mc.GrantedAbilities == null) return Array.Empty<GrantedAbilityInfo>();
            var result = new GrantedAbilityInfo[mc.GrantedAbilities.Length];
            for (int i = 0; i < mc.GrantedAbilities.Length; i++)
            {
                var ga = mc.GrantedAbilities[i];
                result[i] = new GrantedAbilityInfo
                {
                    Level = ga.Level,
                    ActivationPolicy = ga.ActivationPolicy,
                    DeactivationPolicy = ga.DeactivationPolicy,
                    RemovePolicy = ga.RemovePolicy,
                };
            }

            return result;
        }

        /// <summary>添加 GrantedAbility 组件（初始为空数组）</summary>  
        public void AddGrantedAbility()
        {
            if (!IsValid || CheckGrantedAbilityExist()) return;
            EntityHelper.AddManagedComponent<MCGrantedAbility>(Entity);
            EntityHelper.SetManagedComponent(Entity, new MCGrantedAbility(Array.Empty<GrantedAbility>()));
        }

        /// <summary>移除 GrantedAbility 组件</summary>  
        public void RemoveGrantedAbility()
        {
            if (!CheckGrantedAbilityExist()) return;
            _em.RemoveComponent<MCGrantedAbility>(Entity);
        }

        #endregion


        #region InUsage

        public bool CheckInUsageExist() => IsValid && _em.HasComponent<CEffectInUsage>(Entity);

        /// <summary>获取施加来源 ASC</summary>  
        public AbilitySystemCell GetSource()
        {
            if (!CheckInUsageExist()) return null;
            var inUsage = _em.GetComponentData<CEffectInUsage>(Entity);
            return GASManager.GetAscFromEntity(inUsage.Source);
        }

        /// <summary>获取施加目标 ASC</summary>  
        public AbilitySystemCell GetTarget()
        {
            if (!CheckInUsageExist()) return null;
            var inUsage = _em.GetComponentData<CEffectInUsage>(Entity);
            return GASManager.GetAscFromEntity(inUsage.Target);
        }

        /// <summary>获取 GE 等级</summary>  
        public int GetLevel()
        {
            if (!CheckInUsageExist()) return 0;
            return _em.GetComponentData<CEffectInUsage>(Entity).Level;
        }

        /// <summary>设置 GE 等级</summary>  
        public void SetLevel(int level)
        {
            if (!CheckInUsageExist()) return;
            var com = _em.GetComponentData<CEffectInUsage>(Entity);
            com.Level = level;
            _em.SetComponentData(Entity, com);
        }

        #endregion


        #region Cue

        /// <summary>是否有 CueOnApply 组件</summary>  
        public bool CheckCueOnApplyExist() => IsValid && _em.HasComponent<CCueOnApply>(Entity);

        /// <summary>是否有 CueOnTick 组件</summary>  
        public bool CheckCueOnTickExist() => IsValid && _em.HasComponent<CCueOnTick>(Entity);

        /// <summary>是否有 CueOnAdd 组件</summary>  
        public bool CheckCueOnAddExist() => IsValid && _em.HasComponent<CCueOnAdd>(Entity);

        /// <summary>是否有 CueOnRemove 组件</summary>  
        public bool CheckCueOnRemoveExist() => IsValid && _em.HasComponent<CCueOnRemove>(Entity);

        /// <summary>是否有 CueOnActivate 组件</summary>  
        public bool CheckCueOnActivateExist() => IsValid && _em.HasComponent<CCueOnActivate>(Entity);

        /// <summary>是否有 CueOnDeactivate 组件</summary>  
        public bool CheckCueOnDeactivateExist() => IsValid && _em.HasComponent<CCueOnDeactivate>(Entity);

        /// <summary>移除 CueOnApply 组件</summary>  
        public void RemoveCueOnApply()
        {
            if (!CheckCueOnApplyExist()) return;
            var com = _em.GetComponentData<CCueOnApply>(Entity);
            if (com.cues.IsCreated) com.cues.Dispose();
            _em.RemoveComponent<CCueOnApply>(Entity);
        }

        /// <summary>移除 CueOnTick 组件</summary>  
        public void RemoveCueOnTick()
        {
            if (!CheckCueOnTickExist()) return;
            var com = _em.GetComponentData<CCueOnTick>(Entity);
            if (com.cues.IsCreated) com.cues.Dispose();
            _em.RemoveComponent<CCueOnTick>(Entity);
        }

        /// <summary>移除 CueOnAdd 组件</summary>  
        public void RemoveCueOnAdd()
        {
            if (!CheckCueOnAddExist()) return;
            var com = _em.GetComponentData<CCueOnAdd>(Entity);
            if (com.cues.IsCreated) com.cues.Dispose();
            _em.RemoveComponent<CCueOnAdd>(Entity);
        }

        /// <summary>移除 CueOnRemove 组件</summary>  
        public void RemoveCueOnRemove()
        {
            if (!CheckCueOnRemoveExist()) return;
            var com = _em.GetComponentData<CCueOnRemove>(Entity);
            if (com.cues.IsCreated) com.cues.Dispose();
            _em.RemoveComponent<CCueOnRemove>(Entity);
        }

        /// <summary>移除 CueOnActivate 组件</summary>  
        public void RemoveCueOnActivate()
        {
            if (!CheckCueOnActivateExist()) return;
            var com = _em.GetComponentData<CCueOnActivate>(Entity);
            if (com.cues.IsCreated) com.cues.Dispose();
            _em.RemoveComponent<CCueOnActivate>(Entity);
        }

        /// <summary>移除 CueOnDeactivate 组件</summary>  
        public void RemoveCueOnDeactivate()
        {
            if (!CheckCueOnDeactivateExist()) return;
            var com = _em.GetComponentData<CCueOnDeactivate>(Entity);
            if (com.cues.IsCreated) com.cues.Dispose();
            _em.RemoveComponent<CCueOnDeactivate>(Entity);
        }

        #endregion


        #region ApplicationCondition

        public bool CheckApplicationConditionExist() => IsValid && _em.HasComponent<CApplicationCondition>(Entity);

        /// <summary>获取 ApplicationCondition 数据</summary>  
        public int[] GetApplicationConditions()
        {
            if (!CheckApplicationConditionExist()) return Array.Empty<int>();
            var com = _em.GetComponentData<CApplicationCondition>(Entity);
            return com.conditions.IsCreated ? com.conditions.ToArray() : Array.Empty<int>();
        }

        /// <summary>设置 ApplicationCondition 数据</summary>  
        public void SetApplicationConditions(int[] conditions)
        {
            if (!CheckApplicationConditionExist()) return;
            var com = _em.GetComponentData<CApplicationCondition>(Entity);
            if (com.conditions.IsCreated) com.conditions.Dispose();
            com.conditions = new NativeArray<int>(conditions, Allocator.Persistent);
            _em.SetComponentData(Entity, com);
        }

        /// <summary>添加 ApplicationCondition 组件</summary>  
        public void AddApplicationCondition(int[] conditions)
        {
            if (!IsValid || CheckApplicationConditionExist()) return;
            EntityHelper.AddComponent<CApplicationCondition>(Entity);
            EntityHelper.SetComponent(Entity, new CApplicationCondition
            {
                conditions = new NativeArray<int>(conditions, Allocator.Persistent)
            });
        }

        /// <summary>移除 ApplicationCondition 组件</summary>  
        public void RemoveApplicationCondition()
        {
            if (!CheckApplicationConditionExist()) return;
            var com = _em.GetComponentData<CApplicationCondition>(Entity);
            if (com.conditions.IsCreated) com.conditions.Dispose();
            _em.RemoveComponent<CApplicationCondition>(Entity);
        }

        #endregion
    }

    /// <summary>Modifier 的 OOP 信息结构（不含 MMC 引用）</summary>  
    public struct ModifierInfo
    {
        public int AttrSetCode;
        public int AttrCode;
        public GEOperation Operation;
        public float Magnitude;
    }

    /// <summary>GrantedAbility 的 OOP 信息结构（不含 AbilityConfig 引用）</summary>  
    public struct GrantedAbilityInfo
    {
        public int Level;
        public GrantedAbilityActivationPolicy ActivationPolicy;
        public GrantedAbilityDeactivationPolicy DeactivationPolicy;
        public GrantedAbilityRemovePolicy RemovePolicy;
    }
}
