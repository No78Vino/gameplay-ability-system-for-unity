using Unity.Entities;

namespace GAS.Runtime
{
    public static class GameplayEffectApplicator
    {
        private static EntityManager _entityManager => GASManager.EntityManager;

        
        /// <summary>
        /// 1.判断GE Application Condition是否生效
        /// 2.判断ApplicationRequiredTags
        /// 3.判断ImmunityTags【触发Immunity Cue】
        /// </summary>
        public static Entity TryApplyGameplayEffectTo(Entity gameplayEffect,Entity source,Entity target,int level = 1)
        {
            // 1. GE Application Condition
            if (!IsConditionSatisfied()) return Entity.Null;
            
            // 2. ApplicationRequiredTags
            if(!gameplayEffect.CheckApplicationRequiredTags(target)) return Entity.Null;

            // 3. ImmunityTags
            if (gameplayEffect.CheckImmunityTags(target))
            {
                // TODO 免疫Cue触发
                return Entity.Null;
            }
            
            // 4. 设置source，target
            _entityManager.SetComponentData(gameplayEffect, new CEffectInUsage { Source = source, Target = target });
            
            // 5. 根据ge类型处理
            bool hasDuration = _entityManager.HasComponent<CDuration>(gameplayEffect);
            if (hasDuration)
            {
                // Durational GE
                return ApplyInstantDurationEffect(gameplayEffect,source,target,level);
            }
            else
            {
                // Instant 
                return ApplyInstantGameplayEffect(gameplayEffect,source,target,level);

            }

            return Entity.Null;
        }
        
        private static bool IsConditionSatisfied()
        {
            return true;
        }

        
        private static Entity ApplyInstantGameplayEffect(Entity gameplayEffect,Entity source,Entity target,int level)
        {
            gameplayEffect.InitGameplayEffect(source, target, level);
            gameplayEffect.TriggerOnExecute();
            return Entity.Null;
        }
        
        private static Entity ApplyInstantDurationEffect(Entity gameplayEffect,Entity source,Entity target,int level)
        {
            bool isStacking = _entityManager.HasComponent<CStacking>(gameplayEffect);
            // Check GE Stacking
            if (!isStacking)
                return Operation_AddNewGameplayEffect(gameplayEffect,source,target ,level);
            
            
            //var stacking = _entityManager.GetComponentData<CStacking>(gameplayEffect);
            // TODO
            // 处理GE堆叠
            // 基于Target类型GE堆叠
            // if (stacking.StackingType == EffectStackingType.AggregateByTarget)
            // {
            //     GetStackingEffectSpecByData(effectSpec.GameplayEffect, out var geSpec);
            //     // 新添加GE
            //     if (geSpec == null)
            //         return Operation_AddNewGameplayEffectSpec(source, effectSpec,overwriteEffectLevel,effectLevel);
            //     bool stackCountChange = geSpec.RefreshStack();
            //     if (stackCountChange) OnRefreshStackCountMakeContainerDirty();
            //     return geSpec;
            // }
            //     
            // // 基于Source类型GE堆叠
            // if (stacking.StackingType == EffectStackingType.AggregateBySource)
            // {
            //     GetStackingEffectSpecByDataFrom(effectSpec.GameplayEffect, source, out var geSpec);
            //     if (geSpec == null)
            //         return Operation_AddNewGameplayEffectSpec(source, effectSpec, overwriteEffectLevel,
            //             effectLevel);
            //     bool stackCountChange = geSpec.RefreshStack();
            //     if (stackCountChange) OnRefreshStackCountMakeContainerDirty();
            //     return geSpec;
            // }
            
            return Entity.Null;
        }
        
        private static Entity Operation_AddNewGameplayEffect(Entity gameplayEffect,Entity source,Entity target,int level)
        {
            // 1.初始化
            gameplayEffect.InitGameplayEffect(source, target, level);
            
            // 2.添加GE到target
            var geBuffers = GameplayEffectHelper.GameplayEffectsOf(target);
            geBuffers.Add(new BEGameplayEffect { GameplayEffect = gameplayEffect });
            
            // 3.触发OnAdd的Cue
            gameplayEffect.TriggerCueOnAdd();
            
            // 4.执行GE
            gameplayEffect.EffectApply();

            // If the gameplay effect was removed immediately after being applied, return false
            if (!target.HasGameplayEffect(gameplayEffect))
            {
#if UNITY_EDITOR
                UnityEngine.Debug.LogWarning(
                    $"GameplayEffect {gameplayEffect.ToString()} was removed immediately after being applied. This may indicate a problem with the RemoveGameplayEffectsWithTags.");
#endif
                // No need to trigger OnGameplayEffectContainerIsDirty, it has already been triggered when it was removed.
                return Entity.Null;
            }

            // 触发Effect Is Dirty
            target.GameplayEffectListIsDirty();
            return gameplayEffect;
        }
    }
}