using System;
using Unity.Burst;
using Unity.Entities;

namespace GAS.Runtime
{
    public static class GameplayEffectHelper
    {
        private static EntityManager _em => GASManager.EntityManager;
        
        public static Entity GetStackingEffectBySource(int stackingCode, Entity targetAsc, Entity sourceAsc,
            EntityManager entityManager)
        {
            var effects = entityManager.GetBuffer<BGameplayEffect>(targetAsc);

            for (var i = 0; i < effects.Length; i++)
            {
                var effect = effects[i].GameplayEffect;

                var hasStacking = entityManager.HasComponent<CStacking>(effect);
                if (!hasStacking) continue;

                var stacking = entityManager.GetComponentData<CStacking>(effect);
                if (stacking.StackType != EffectStackType.AggregateBySource) continue;

                var source = entityManager.GetComponentData<CEffectInUsage>(effect).Source;
                if (source != sourceAsc) continue;

                if (stacking.StackingCode == stackingCode)
                    return effect;
            }

            return Entity.Null;
        }
        
        public static Entity GetStackingEffectByTarget(int stackingCode, Entity targetAsc, EntityManager entityManager)
        {
            var effects = entityManager.GetBuffer<BGameplayEffect>(targetAsc);

            for (var i = 0; i < effects.Length; i++)
            {
                var effect = effects[i].GameplayEffect;

                var hasStacking = entityManager.HasComponent<CStacking>(effect);
                if (!hasStacking) continue;

                var stacking = entityManager.GetComponentData<CStacking>(effect);
                if (stacking.StackType != EffectStackType.AggregateByTarget) continue;

                if (stacking.StackingCode == stackingCode)
                    return effect;
            }

            return Entity.Null;
        }

        #region GameplayEffectConfig

        private static Func<int, GameplayEffectConfig> _getConfigByID;

        public static void RegisterGetConfigByIDFunc(Func<int, GameplayEffectConfig> func)
        {
            _getConfigByID = func;
        }

        public static GameplayEffectConfig GetConfigByID(int id)
        {
            return _getConfigByID?.Invoke(id);
        }

        #endregion
        
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
            if (!_em.HasComponent<MCModifiers>(gameplayEffect)) return;

            var modifiers = _em.GetComponentData<MCModifiers>(gameplayEffect);
            var attrSets = _em.GetBuffer<BEAttrSet>(target);
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


        public static Entity CreateGameplayEffectEntity(GameplayEffectComponentConfig[] componentAssets)
        {
            var entity = _em.CreateEntity();
            EntityHelper.SetName(entity,$"GE_V{entity.Version}_{entity.Index}");
            foreach (var config in componentAssets)
                config.LoadToGameplayEffectEntity(entity);
            return entity;
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
            if (!_em.HasComponent<CEffectInUsage>(gameplayEffect)) return;
            
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
            var inUsage = _em.GetComponentData<CEffectInUsage>(gameplayEffect);
            var target = inUsage.Target;
            var gameplayEffects = _em.GetBuffer<BGameplayEffect>(target);
            for (var i = 0; i < gameplayEffects.Length; i++)
            {
                if (gameplayEffects[i].GameplayEffect != gameplayEffect) continue;
                gameplayEffects.RemoveAt(i);
                break;
            }
        }
    }
}