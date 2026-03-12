using System;  
using Unity.Collections;  
using Unity.Entities;  
  
namespace GAS.Runtime  
{  
    /// <summary>  
    /// AbilitySpec 是 Ability Entity 的 OOP 包装层。  
    /// 所有 public/protected 成员的形参和返回值不出现 ECS/Entities 类型。  
    /// 内部实现自由使用 ECS API。  
    /// </summary>  
    public class AbilitySpec  
    {  
        // ======== 内部字段（黑箱）========  
        private Entity _abilityEntity;  
        private static EntityManager _em => GASManager.EntityManager;  
  
        // ======== internal 访问（框架内部用）========  
        internal Entity Entity => _abilityEntity;  
  

        public AbilitySpec(Entity abilityEntity)  
        {  
            _abilityEntity = abilityEntity;  
        }  
  
        /// <summary>Ability Entity 是否仍然有效</summary>  
        public bool IsValid => _abilityEntity != Entity.Null && _em.Exists(_abilityEntity);  
  

        #region BaseInfo  
  
        /// <summary>能力 Code（配置 ID）</summary>  
        public int Code  
        {  
            get  
            {  
                if (!IsValid) return 0;  
                return _em.GetComponentData<CAbilityBaseInfo>(_abilityEntity).Code;  
            }  
        }  
  
        /// <summary>能力等级</summary>  
        public int Level  
        {  
            get  
            {  
                if (!IsValid) return 0;  
                return _em.GetComponentData<CAbilityBaseInfo>(_abilityEntity).Level;  
            }  
        }  
  
        /// <summary>设置能力等级</summary>  
        public void SetLevel(int level)  
        {  
            if (!IsValid) return;  
            var info = _em.GetComponentData<CAbilityBaseInfo>(_abilityEntity);  
            info.Level = level;  
            _em.SetComponentData(_abilityEntity, info);  
        }  
  
        /// <summary>获取 Owner ASC（动态读取，不缓存）</summary>  
        public AbilitySystemCell Owner  
        {  
            get  
            {  
                if (!IsValid) return null;  
                var ownerEntity = _em.GetComponentData<CAbilityBaseInfo>(_abilityEntity).Owner;  
                if (ownerEntity == Entity.Null) return null;  
                return GASManager.GetAscFromEntity(ownerEntity);  
            }  
        }  
  
        #endregion  
  

        #region RuntimeState  
  
        /// <summary>是否正在激活</summary>  
        public bool IsActive => IsValid && _em.HasComponent<CAbilityActive>(_abilityEntity);  
  
        /// <summary>综合检查是否可以激活（Tag、Cost、CD 全部通过）</summary>  
        public bool CanActivate => IsValid && GAUtil.CanActivateAbility(_abilityEntity) == AbilityActivationResult.Success;  
  
        /// <summary>详细的激活检查，返回具体失败原因</summary>  
        public AbilityActivationResult CheckActivation()  
        {  
            if (!IsValid) return AbilityActivationResult.FailOtherReason;  
            return GAUtil.CanActivateAbility(_abilityEntity);  
        }  
  
        /// <summary>Tag 条件是否满足激活</summary>  
        public bool IsTagRequirementMet => IsValid && GAUtil.CheckGameplayTagsValidTpActivate(_abilityEntity);  
  
        /// <summary>Cost 是否足够</summary>  
        public bool CanAffordCost => IsValid && GAUtil.CheckCost(_abilityEntity);  
  
        /// <summary>冷却是否就绪</summary>  
        public bool IsCooldownReady => IsValid && GAUtil.CheckCooldownReady(_abilityEntity);  
  
        #endregion  
  

        #region Operations  
  
        /// <summary>尝试激活（添加 CAbilityInTryActivate 标记，下一帧由 System 处理）</summary>  
        public void TryActivate()  
        {  
            if (!IsValid) return;  
            EntityHelper.AddComponent<CAbilityInTryActivate>(_abilityEntity);  
        }  
  
        /// <summary>尝试结束（添加 CAbilityInTryEnd 标记，下一帧由 System 处理）</summary>  
        public void TryEnd()  
        {  
            if (!IsValid) return;  
            EntityHelper.AddComponent<CAbilityInTryEnd>(_abilityEntity);  
        }  
  
        /// <summary>尝试取消（添加 CAbilityInTryCancel 标记，下一帧由 System 处理）</summary>  
        public void TryCancel()  
        {  
            if (!IsValid) return;  
            EntityHelper.AddComponent<CAbilityInTryCancel>(_abilityEntity);  
        }  
  
        /// <summary>手动触发 CD</summary>  
        public void DoCooldown()  
        {  
            if (!IsValid) return;  
            GAUtil.DoCooldown(_abilityEntity);  
        }  
  
        /// <summary>手动触发 Cost</summary>  
        public void DoCost()  
        {  
            if (!IsValid) return;  
            GAUtil.DoCost(_abilityEntity);  
        }  
  
        #endregion  
  
        // =====================================================================================  
        //  AbilityLogic 组件 (MCAbilityLogic) — 必定存在  
        // =====================================================================================  
  
        #region AbilityLogic  
  
        /// <summary>获取 AbilityLogicBase 实例</summary>  
        public AbilityLogicBase GetLogic()  
        {  
            if (!IsValid) return null;  
            return _em.GetComponentData<MCAbilityLogic>(_abilityEntity)?.Logic;  
        }  
  
        /// <summary>获取强类型的 AbilityLogicBase 实例</summary>  
        public T GetLogic<T>() where T : AbilityLogicBase  
        {  
            return GetLogic() as T;  
        }  
  
        /// <summary>设置 Ability 参数（传递给 AbilityLogicBase）</summary>  
        public void SetParam(XParam param)  
        {  
            if (!IsValid) return;  
            var logic = _em.GetComponentData<MCAbilityLogic>(_abilityEntity);  
            logic?.Logic?.SetParam(param);  
        }  
  
        /// <summary>获取 Ability 的原始参数</summary>  
        public XParam GetParamRaw()  
        {  
            var logic = GetLogic();  
            if (logic == null) return null;  
            // AbilityLogicBase._paramRaw is protected,   
            // 需要通过 Logic 暴露或通过反射获取  
            // 这里建议在 AbilityLogicBase 上新增 public XParam ParamRaw => _paramRaw;  
            return null; // 需要 AbilityLogicBase 配合暴露  
        }  
  
        #endregion  
        
        private bool CheckTagComponentExist<T>() where T : unmanaged, IComponentData  
            => IsValid && _em.HasComponent<T>(_abilityEntity);  
  
        private int[] GetTagsInternal<T>(Func<T, NativeArray<int>> getter) where T : unmanaged, IComponentData  
        {  
            if (!CheckTagComponentExist<T>()) return Array.Empty<int>();  
            var com = _em.GetComponentData<T>(_abilityEntity);  
            var nativeArray = getter(com);  
            return nativeArray.IsCreated ? nativeArray.ToArray() : Array.Empty<int>();  
        }  
  
        private void SetTagsInternal<T>(int[] tags, Func<T, NativeArray<int>> getter, Func<NativeArray<int>, T> factory)  
            where T : unmanaged, IComponentData  
        {  
            if (!CheckTagComponentExist<T>()) return;  
            var com = _em.GetComponentData<T>(_abilityEntity);  
            var old = getter(com);  
            if (old.IsCreated) old.Dispose();  
            _em.SetComponentData(_abilityEntity, factory(new NativeArray<int>(tags, Allocator.Persistent)));  
        }  
  
        private void AddTagComponentInternal<T>(int[] tags, Func<NativeArray<int>, T> factory)  
            where T : unmanaged, IComponentData  
        {  
            if (!IsValid || CheckTagComponentExist<T>()) return;  
            EntityHelper.AddComponent<T>(_abilityEntity);  
            EntityHelper.SetComponent(_abilityEntity, factory(new NativeArray<int>(tags, Allocator.Persistent)));  
        }  
  
        private void RemoveTagComponentInternal<T>(Func<T, NativeArray<int>> getter)  
            where T : unmanaged, IComponentData  
        {  
            if (!CheckTagComponentExist<T>()) return;  
            var com = _em.GetComponentData<T>(_abilityEntity);  
            var arr = getter(com);  
            if (arr.IsCreated) arr.Dispose();  
            _em.RemoveComponent<T>(_abilityEntity);  
        }  
  
        
        #region AssetTags  
  
        public bool CheckAssetTagsExist() => CheckTagComponentExist<CAbilityAssetTags>();  
  
        public int[] GetAssetTags() => GetTagsInternal<CAbilityAssetTags>(c => c.tags);  
  
        public void SetAssetTags(int[] tags) => SetTagsInternal<CAbilityAssetTags>(tags,  
            c => c.tags, arr => new CAbilityAssetTags { tags = arr });  
  
        public void AddAssetTags(int[] tags) => AddTagComponentInternal<CAbilityAssetTags>(tags,  
            arr => new CAbilityAssetTags { tags = arr });  
  
        public void RemoveAssetTags() => RemoveTagComponentInternal<CAbilityAssetTags>(c => c.tags);  
  
        #endregion  
  
         
        #region ActivationOwnedTags  
  
        public bool CheckActivationOwnedTagsExist() => CheckTagComponentExist<CAbilityActivationOwnedTags>();  
  
        public int[] GetActivationOwnedTags() => GetTagsInternal<CAbilityActivationOwnedTags>(c => c.tags);  
  
        public void SetActivationOwnedTags(int[] tags) => SetTagsInternal<CAbilityActivationOwnedTags>(tags,  
            c => c.tags, arr => new CAbilityActivationOwnedTags { tags = arr });  
  
        public void AddActivationOwnedTags(int[] tags) => AddTagComponentInternal<CAbilityActivationOwnedTags>(tags,  
            arr => new CAbilityActivationOwnedTags { tags = arr });  
  
        public void RemoveActivationOwnedTags() => RemoveTagComponentInternal<CAbilityActivationOwnedTags>(c => c.tags);  
  
        #endregion  
  
          
        #region ActivationRequiredTags  
  
        public bool CheckActivationRequiredTagsExist() => CheckTagComponentExist<CAbilityActivationRequiredTags>();  
  
        public int[] GetActivationRequiredTags() => GetTagsInternal<CAbilityActivationRequiredTags>(c => c.tags);  
  
        public void SetActivationRequiredTags(int[] tags) => SetTagsInternal<CAbilityActivationRequiredTags>(tags,  
            c => c.tags, arr => new CAbilityActivationRequiredTags { tags = arr });  
  
        public void AddActivationRequiredTags(int[] tags) => AddTagComponentInternal<CAbilityActivationRequiredTags>(tags,  
            arr => new CAbilityActivationRequiredTags { tags = arr });  
  
        public void RemoveActivationRequiredTags() => RemoveTagComponentInternal<CAbilityActivationRequiredTags>(c => c.tags);  
  
        #endregion  
  
          
        #region ActivationBlockedTags  
  
        public bool CheckActivationBlockedTagsExist() => CheckTagComponentExist<CAbilityActivationBlockedTags>();  
  
        public int[] GetActivationBlockedTags() => GetTagsInternal<CAbilityActivationBlockedTags>(c => c.tags);  
  
        public void SetActivationBlockedTags(int[] tags) => SetTagsInternal<CAbilityActivationBlockedTags>(tags,  
            c => c.tags, arr => new CAbilityActivationBlockedTags { tags = arr });  
  
        public void AddActivationBlockedTags(int[] tags) => AddTagComponentInternal<CAbilityActivationBlockedTags>(tags,  
            arr => new CAbilityActivationBlockedTags { tags = arr });  
  
        public void RemoveActivationBlockedTags() => RemoveTagComponentInternal<CAbilityActivationBlockedTags>(c => c.tags);  
  
        #endregion  
  
        
        #region CancelAbilityTags  
  
        public bool CheckCancelAbilityTagsExist() => CheckTagComponentExist<CCancelAbilityWithTags>();  
  
        public int[] GetCancelAbilityTags() => GetTagsInternal<CCancelAbilityWithTags>(c => c.tags);  
  
        public void SetCancelAbilityTags(int[] tags) => SetTagsInternal<CCancelAbilityWithTags>(tags,  
            c => c.tags, arr => new CCancelAbilityWithTags { tags = arr });  
  
        public void AddCancelAbilityTags(int[] tags) => AddTagComponentInternal<CCancelAbilityWithTags>(tags,  
            arr => new CCancelAbilityWithTags { tags = arr });  
  
        public void RemoveCancelAbilityTags() => RemoveTagComponentInternal<CCancelAbilityWithTags>(c => c.tags);  
  
        #endregion  

        #region BlockAbilityTags  
  
        public bool CheckBlockAbilityTagsExist() => CheckTagComponentExist<CBlockAbilityWithTags>();  
  
        public int[] GetBlockAbilityTags() => GetTagsInternal<CBlockAbilityWithTags>(c => c.tags);  
  
        public void SetBlockAbilityTags(int[] tags) => SetTagsInternal<CBlockAbilityWithTags>(tags,  
            c => c.tags, arr => new CBlockAbilityWithTags { tags = arr });  
  
        public void AddBlockAbilityTags(int[] tags) => AddTagComponentInternal<CBlockAbilityWithTags>(tags,  
            arr => new CBlockAbilityWithTags { tags = arr });  
  
        public void RemoveBlockAbilityTags() => RemoveTagComponentInternal<CBlockAbilityWithTags>(c => c.tags);  
  
        #endregion  
  

        #region Cooldown  
  
        public bool CheckCooldownExist() => IsValid && _em.HasComponent<CAbilityCooldown>(_abilityEntity);  
  
        /// <summary>获取冷却时长（帧）</summary>  
        public int GetCooldown()  
        {  
            if (!CheckCooldownExist()) return 0;  
            return _em.GetComponentData<CAbilityCooldown>(_abilityEntity).Cooldown;  
        }  
  
        /// <summary>设置冷却时长（帧），会覆写原型GE的Duration</summary>  
        public void SetCooldown(int cooldown)  
        {  
            if (!CheckCooldownExist()) return;  
            var com = _em.GetComponentData<CAbilityCooldown>(_abilityEntity);  
            com.Cooldown = cooldown;  
            _em.SetComponentData(_abilityEntity, com);  
        }  
  
        /// <summary>获取冷却Tag列表（从原型GE的GrantedTags拷贝而来）</summary>  
        public int[] GetCooldownTags()  
        {  
            if (!CheckCooldownExist()) return Array.Empty<int>();  
            var com = _em.GetComponentData<CAbilityCooldown>(_abilityEntity);  
            if (!com.CooldownTags.IsCreated || com.CooldownTags.Length == 0)   
                return Array.Empty<int>();  
            return com.CooldownTags.ToArray();  
        }  
  
        /// <summary>获取冷却原型GE的Spec包装（可进一步修改原型GE属性）</summary>  
        public GameplayEffectSpec GetCooldownProtoGE()  
        {  
            if (!CheckCooldownExist()) return null;  
            var com = _em.GetComponentData<CAbilityCooldown>(_abilityEntity);  
            return new GameplayEffectSpec(com.ProtoGameplayEffectCooldown);  
        }  
  
        #endregion
        
        
        #region Cost  
  
        public bool CheckCostExist() => IsValid && _em.HasComponent<CAbilityCost>(_abilityEntity);  
  
        /// <summary>获取消耗 GE 原型的 Spec 包装（可进一步读取/修改消耗 GE 的 Modifier）</summary>  
        public GameplayEffectSpec GetCostEffectProto()  
        {  
            if (!CheckCostExist()) return null;  
            var com = _em.GetComponentData<CAbilityCost>(_abilityEntity);  
            if (com.ProtoGameplayEffectCost == Entity.Null) return null;  
            return new GameplayEffectSpec(com.ProtoGameplayEffectCost);  
        }  
  
        /// <summary>  
        /// 添加 Cost 组件。  
        /// <para>[Warning] 建议仅在 Ability 首次 Activate 之前调用。</para>  
        /// </summary>  
        /// <param name="costEffectConfigID">消耗 GE 的配置 ID</param>  
        public void AddCost(int costEffectConfigID)  
        {  
            if (!IsValid || CheckCostExist()) return;  
            var effectCfg = GameplayEffectHelper.GetConfigByID(costEffectConfigID);  
            EntityHelper.AddComponent<CAbilityCost>(_abilityEntity);  
            EntityHelper.SetComponent(_abilityEntity, new CAbilityCost  
            {  
                ProtoGameplayEffectCost = EffectUtil.CreateGameplayEffectEntity(effectCfg.ComponentConfigs),  
            });  
        }  
  
        /// <summary>移除 Cost 组件</summary>  
        public void RemoveCost()  
        {  
            if (!CheckCostExist()) return;  
            var com = _em.GetComponentData<CAbilityCost>(_abilityEntity);  
            if (com.ProtoGameplayEffectCost != Entity.Null && _em.Exists(com.ProtoGameplayEffectCost))  
                _em.DestroyEntity(com.ProtoGameplayEffectCost);  
            _em.RemoveComponent<CAbilityCost>(_abilityEntity);  
        }  
  
        #endregion  
  

        #region Events  
  
        /// <summary>注册激活结果回调</summary>  
        public void RegisterOnActivateResult(Action<AbilityActivationResult> action)  
        {  
            if (!IsValid) return;  
            GASEventCenter.RegisterOnActivateResult(_abilityEntity, action);  
        }  
  
        /// <summary>注销激活结果回调</summary>  
        public void UnRegisterOnActivateResult(Action<AbilityActivationResult> action)  
        {  
            if (!IsValid) return;  
            GASEventCenter.UnRegisterOnActivateResult(_abilityEntity, action);  
        }  
  
        /// <summary>注册结束回调</summary>  
        public void RegisterOnEndAbility(Action action)  
        {  
            if (!IsValid) return;  
            GASEventCenter.RegisterOnEndAbility(_abilityEntity, action);  
        }  
  
        /// <summary>注销结束回调</summary>  
        public void UnRegisterOnEndAbility(Action action)  
        {  
            if (!IsValid) return;  
            GASEventCenter.UnRegisterOnEndAbility(_abilityEntity, action);  
        }  
  
        /// <summary>注册取消回调</summary>  
        public void RegisterOnCancelAbility(Action action)  
        {  
            if (!IsValid) return;  
            GASEventCenter.RegisterOnCancelAbility(_abilityEntity, action);  
        }  
  
        /// <summary>注销取消回调</summary>  
        public void UnRegisterOnCancelAbility(Action action)  
        {  
            if (!IsValid) return;  
            GASEventCenter.UnRegisterOnCancelAbility(_abilityEntity, action);  
        }  
  
        #endregion  
    }  
}