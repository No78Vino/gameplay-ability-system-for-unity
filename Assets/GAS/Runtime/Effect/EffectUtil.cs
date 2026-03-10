using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace GAS.Runtime
{
    public static class EffectUtil
    {
        private static EntityManager _entityManager => GASManager.EntityManager;

        /// <summary>  
        /// 立即应用 Instant 类型的 GameplayEffect（直接修改 BaseValue）  
        /// 用于 Cost、或其他需要绕过 ECS GE 管线立即生效的场景。  
        /// 该方法不会走 ECS GE 生命周期管线，不会产生 GE 实例 Entity。  
        /// </summary>  
        /// <param name="gameplayEffect">GE 的 Prototype Entity（需包含 MCModifiers）</param>  
        /// <param name="target">目标 ASC Entity</param>  
        /// <param name="source">来源 ASC Entity</param>  
        public static void ApplyGameplayEffectImmediate(Entity gameplayEffect, Entity target, Entity source)
        {
            if (!_entityManager.HasComponent<MCModifiers>(gameplayEffect)) return;

            var modifiers = _entityManager.GetComponentData<MCModifiers>(gameplayEffect);
            var attrSets = _entityManager.GetBuffer<BEAttrSet>(target);
            bool change = false;

            foreach (var modifier in modifiers.Modifiers)
            {
                var attrSetIndex = attrSets.IndexOfAttrSetCode(modifier.AttrSetCode);
                if (attrSetIndex == -1) continue;

                var attrSet = attrSets[attrSetIndex];
                var attributes = attrSet.Attributes;

                var attrIndex = attributes.IndexOfAttrCode(modifier.AttrCode);
                if (attrIndex == -1) continue;

                var data = attributes[attrIndex];
                var oldValue = data.BaseValue;
                // 使用显式 Source/Target 的 Calculate 重载，避免原型 Entity 缺少 CEffectInUsage 的问题  
                var newValue = MmcHelper.Calculate(gameplayEffect, modifier, data.BaseValue, source, target);

                // 钳制计算处理  
                if (data.IsClampMin) newValue = Unity.Mathematics.math.max(newValue, data.MinValue);
                if (data.IsClampMax) newValue = Unity.Mathematics.math.min(newValue, data.MaxValue);

                // OnChangeBefore  
                newValue = GASEventCenter.InvokeOnBaseValueChangeBefore(target, modifier.AttrSetCode,
                    modifier.AttrCode, newValue);

                data.BaseValue = newValue;

                // OnChangeAfter  
                if (newValue != oldValue)
                {
                    data.Dirty = true;
                    change = true;
                    GASEventCenter.InvokeOnBaseValueChangeAfter(target, modifier.AttrSetCode,
                        modifier.AttrCode, oldValue, newValue);
                }

                attrSet.Attributes[attrIndex] = data;
                attrSets[attrSetIndex] = attrSet;
            }

            // 标记属性需要重计算 CurrentValue  
            if (change)
                EntityHelper.AddComponent<CAttributeIsDirty>(target);
        }

































        public static bool HasAnyTags(Entity ge, NativeArray<int> tags)
        {
            // 1.判断AssetTags
            if (_entityManager.HasComponent<CEffectAssetTags>(ge))
            {
                var assetTags = _entityManager.GetComponentData<CEffectAssetTags>(ge).tags;

                foreach (var assetTag in assetTags)
                foreach (var tag in tags)
                    if (TagHelper.HasTag(assetTag, tag))
                        return true;
            }

            //2.判断GrantedTags
            if (_entityManager.HasComponent<CEffectGrantedTags>(ge))
            {
                var grantedTags = _entityManager.GetComponentData<CEffectGrantedTags>(ge).tags;
                foreach (var grantedTag in grantedTags)
                foreach (var tag in tags)
                    if (TagHelper.HasTag(grantedTag, tag))
                        return true;
            }

            return false;
        }

        public static Entity CreateGameplayEffectEntity(GameplayEffectComponentConfig[] componentAssets)
        {
            var entity = _entityManager.CreateEntity();
            EntityHelper.SetName(entity,$"GE_V{entity.Version}_{entity.Index}");
            foreach (var config in componentAssets)
                config.LoadToGameplayEffectEntity(entity);
            return entity;
        }
        
        public static GameplayEffectSpec CreateGameplayEffectSpec(GameplayEffectComponentConfig[] componentAssets)
        {
            return new GameplayEffectSpec(componentAssets);
        }

        public static void ApplyGameplayEffectTo(Entity gameplayEffect, Entity target, Entity source)
        {
            EntityHelper.AddComponent<CEffectInUsage>(gameplayEffect);
            EntityHelper.AddComponent<WipInstantiateEffect>(gameplayEffect);
            EntityHelper.SetComponent(gameplayEffect, new CEffectInUsage
            {
                Source = source,
                Target = target
            });
        }
        
        public static void ApplyGameplayEffectTo(Entity gameplayEffect, AbilitySystemCell target, AbilitySystemCell source)
        {
            ApplyGameplayEffectTo(gameplayEffect, target.Entity, source.Entity);
        }
        
        public static void RemoveGameplayEffect(Entity gameplayEffect)
        {
            if (!_entityManager.HasComponent<CEffectInUsage>(gameplayEffect)) return;
            
            EntityHelper.AddComponent<CEffectDestroy>(gameplayEffect);
            EntityHelper.AddComponent<WipDeactivateEffect>(gameplayEffect);
            EntityHelper.AddComponent<WipRemoveEffect>(gameplayEffect);
        }

        public static void RemoveGameplayEffect(Entity gameplayEffect,EntityCommandBuffer ecb)
        {
            //if (!_entityManager.HasComponent<CInUsage>(gameplayEffect)) return;
            ecb.RemoveComponent<CEffectApplied>(gameplayEffect);
            ecb.AddComponent<CEffectDestroy>(gameplayEffect);
            
            // 从ASC容器中移除
            var inUsage = _entityManager.GetComponentData<CEffectInUsage>(gameplayEffect);
            var target = inUsage.Target;
            var gameplayEffects = _entityManager.GetBuffer<BGameplayEffect>(target);
            for (var i = 0; i < gameplayEffects.Length; i++)
            {
                if (gameplayEffects[i].GameplayEffect != gameplayEffect) continue;
                gameplayEffects.RemoveAt(i);
                break;
            }
        }
        
        /// <summary>
        ///     检测应用标签
        /// </summary>
        /// <param name="gameplayEffect"></param>
        /// <param name="asc"></param>
        /// <returns></returns>
        public static bool CheckApplicationRequiredTags(this Entity gameplayEffect, Entity asc)
        {
            if (!_entityManager.HasComponent<CApplicationRequiredTags>(gameplayEffect)) return true;
            var requiredTags = _entityManager.GetComponentData<CApplicationRequiredTags>(gameplayEffect);
            return ASCHelper.HasAllTags(asc, requiredTags.tags);
        }

        /// <summary>
        ///     检测激活标签
        /// </summary>
        /// <param name="gameplayEffect"></param>
        /// <param name="asc"></param>
        /// <returns></returns>
        public static bool CheckOngoingRequiredTags(this Entity gameplayEffect, Entity asc)
        {
            if (!_entityManager.HasComponent<COngoingRequiredTags>(gameplayEffect)) return true;
            var requiredTags = _entityManager.GetComponentData<COngoingRequiredTags>(gameplayEffect);
            return ASCHelper.HasAllTags(asc, requiredTags.tags);

        }

        /// <summary>
        ///     检测免疫标签
        /// </summary>
        /// <param name="gameplayEffect"></param>
        /// <param name="asc"></param>
        /// <returns></returns>
        public static bool CheckImmunityTags(this Entity gameplayEffect, Entity asc)
        {
            if (!_entityManager.HasComponent<CEffectImmunityTags>(gameplayEffect)) return false;
            var immunityTags = _entityManager.GetComponentData<CEffectImmunityTags>(gameplayEffect);
            return ASCHelper.HasAnyTags(asc, immunityTags.tags);
        }

        public static void InitGameplayEffect(this Entity gameplayEffect, Entity source, Entity target, int level)
        {
            if (!_entityManager.HasComponent<CEffectInUsage>(gameplayEffect)) return;

            _entityManager.SetComponentData(gameplayEffect,
                new CEffectInUsage { Source = source, Target = target, Level = level });

            if (_entityManager.HasComponent<CDuration>(gameplayEffect))
                if (_entityManager.HasComponent<CPeriod>(gameplayEffect))
                {
                    var period = _entityManager.GetComponentData<CPeriod>(gameplayEffect);
                    var periodGEs = period.GameplayEffects;
                    foreach (var ge in periodGEs)
                        ge.InitGameplayEffect(source, target, level);
                }
            
            // TODO 
            // SetGrantedAbility(GameplayEffect.GrantedAbilities);
        }

        public static void TriggerOnExecute(this Entity gameplayEffect)
        {
            if (!_entityManager.HasComponent<CEffectInUsage>(gameplayEffect)) return;

            var inUsage = _entityManager.GetComponentData<CEffectInUsage>(gameplayEffect);
            var owner = inUsage.Target;
            // 1.移除GameplayEffectWithAnyTags
            owner.RemoveGameplayEffectWithAnyTags(gameplayEffect);

            // 2。应用Modifiers
            owner.ApplyModFromInstantGameplayEffect(gameplayEffect);

            // TODO
            // 3.触发Cue
            // TriggerCueOnExecute();
        }

        public static bool CheckEffectHasAnyTags(this Entity gameplayEffect, NativeArray<int> tags)
        {
            // 1.判断AssetTags
            if (_entityManager.HasComponent<CEffectAssetTags>(gameplayEffect))
            {
                var assetTags = _entityManager.GetComponentData<CEffectAssetTags>(gameplayEffect).tags;

                foreach (var assetTag in assetTags)
                foreach (var tag in tags)
                    if (TagHelper.HasTag(assetTag, tag))
                        return true;
            }

            //2.判断GrantedTags
            if (_entityManager.HasComponent<CEffectGrantedTags>(gameplayEffect))
            {
                var grantedTags = _entityManager.GetComponentData<CEffectGrantedTags>(gameplayEffect).tags;
                foreach (var grantedTag in grantedTags)
                foreach (var tag in tags)
                    if (TagHelper.HasTag(grantedTag, tag))
                        return true;
            }

            return false;
        }

        public static bool CheckEffectHasAnyTags(this Entity gameplayEffect,  SingletonGameplayTagMap singletonGameplayTagMap ,EntityManager entityManager ,NativeArray<int> tags)
        {
            // 1.判断AssetTags
            if (entityManager.HasComponent<CEffectAssetTags>(gameplayEffect))
            {
                var assetTags = entityManager.GetComponentData<CEffectAssetTags>(gameplayEffect).tags;

                foreach (var assetTag in assetTags)
                foreach (var tag in tags)
                    if (singletonGameplayTagMap.IsTagAIncludeTagB(assetTag, tag))
                        return true;
            }

            //2.判断GrantedTags
            if (entityManager.HasComponent<CEffectGrantedTags>(gameplayEffect))
            {
                var grantedTags = entityManager.GetComponentData<CEffectGrantedTags>(gameplayEffect).tags;
                foreach (var grantedTag in grantedTags)
                foreach (var tag in tags)
                    if (singletonGameplayTagMap.IsTagAIncludeTagB(grantedTag, tag))
                        return true;
            }

            return false;
        }
        
        public static void EffectApply(this Entity gameplayEffect)
        {
            if (_entityManager.HasComponent<CEffectApplied>(gameplayEffect)) return;
            _entityManager.AddComponent<CEffectApplied>(gameplayEffect);
            
            // 校验是否可激活
            var owner = _entityManager.GetComponentData<CEffectInUsage>(gameplayEffect).Target;
            if (gameplayEffect.CheckOngoingRequiredTags(owner))
                gameplayEffect.EffectActivate();
        }
        
        public static void EffectActivate(this Entity gameplayEffect)
        {
            var comDuration = _entityManager.GetComponentData<CDuration>(gameplayEffect);
            if (comDuration.active) return;
            comDuration.active = true;
            
            // 1. 更新激活时间
            var globalFrameTimer = _entityManager.GetComponentData<GlobalTimer>(GASManager.EntityGlobalTimer);
            var currentFrame = globalFrameTimer.Frame;
            var currentTurn = globalFrameTimer.Turn;
            
            if(comDuration.timeUnit == TimeUnit.Frame)
            {
                if (comDuration.activeTime == 0 || comDuration.ResetStartTimeWhenActivated)
                    comDuration.activeTime = currentFrame;
                    
                comDuration.lastActiveTime = currentFrame;
            }
            else
            {
                if (comDuration.activeTime == 0 || comDuration.ResetStartTimeWhenActivated)
                    comDuration.activeTime = currentTurn;
                    
                comDuration.lastActiveTime = currentTurn;
            }
            
            _entityManager.SetComponentData(gameplayEffect,comDuration);
            
            // TODO 触发OnActivation的Cue
            // TriggerOnActivation();
        }
        
        public static void EffectDeactivate(this Entity gameplayEffect)
        {
            
        }

        #region TriggerCue

        public static void TriggerCueOnAdd(this Entity gameplayEffect)
        {
            
        }

        public static void TriggerCueOnRemove(this Entity gameplayEffect)
        {
            
        }

        public static void TriggerCueOnExecute(this Entity gameplayEffect)
        {
            
        }

        public static void TriggerCueOnActivation(this Entity gameplayEffect)
        {
            
        }
        #endregion
    }
}