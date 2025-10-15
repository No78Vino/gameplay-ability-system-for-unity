using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    public static class GameplayEffectUtils
    {
        private static EntityManager GasEntityManager => GASManager.EntityManager;

        public static DynamicBuffer<BEGameplayEffect> GameplayEffectsOf(Entity asc)
        {
            return GasEntityManager.GetBuffer<BEGameplayEffect>(asc);
        }

        public static bool CheckAscAttributeDirty(DynamicBuffer<BEAttributeSet> attrSets,MCModifiers modifiers)
        {
            var isDirty = false;
            foreach (var modifier in modifiers.Modifiers)
            {
                int attrSetIndex = attrSets.IndexOfAttrSetCode(modifier.AttrSetCode);
                if(attrSetIndex==-1) continue;
                    
                var attrSet = attrSets[attrSetIndex];
                var attributes = attrSet.Attributes;

                int attrIndex = attributes.IndexOfAttrCode(modifier.AttrCode);
                if(attrIndex==-1) continue;
                
                isDirty = true;
                
                var attr = attributes[attrIndex];
                if(attr.Dirty) continue;
                
                attr.Dirty = true;
                attrSet.Attributes[attrIndex] = attr;
                attrSets[attrSetIndex] = attrSet;
            }
            return isDirty;
        }


        private static bool CheckOngoingRequiredTags(Entity gameplayEffect,Entity targetAsc,EntityManager entityManager)
        {
            if (!entityManager.HasComponent<COngoingRequiredTags>(gameplayEffect)) return true;
            var ongoingRequiredTags = entityManager.GetComponentData<COngoingRequiredTags>(gameplayEffect);
            return ASCUtil.HasAllTags(targetAsc,ongoingRequiredTags.tags);
        }
        
        public static bool ActivateGameplayEffect(Entity gameplayEffect, Entity targetAsc, EntityManager entityManager,
            GlobalTimer globalTimer)
        {
            if (!CheckOngoingRequiredTags(gameplayEffect, targetAsc, entityManager)) return false;

            var duration = entityManager.GetComponentData<CDuration>(gameplayEffect);
            if (!duration.active)
            {
                duration.active = true;
                duration.activeTime = duration.timeUnit == TimeUnit.Frame
                    ? globalTimer.Frame
                    : globalTimer.Turn;
                entityManager.SetComponentData(gameplayEffect, duration);

                if (entityManager.HasComponent<CEffectGrantedTags>(gameplayEffect))
                {
                    var grantedTags = entityManager.GetComponentData<CEffectGrantedTags>(gameplayEffect);
                    ASCUtil.TryAddDynamicAddedTags(targetAsc, gameplayEffect, grantedTags.tags.ToArray());
                }

                if (entityManager.HasComponent<CCueOnActivate>(gameplayEffect))
                {
                    var cCue = entityManager.GetComponentData<CCueOnActivate>(gameplayEffect);
                    cCue.runtimeCues = GetTriggerCues(gameplayEffect, targetAsc, entityManager, cCue.runtimeCues,
                        cCue.cues);
                    entityManager.SetComponentData(gameplayEffect, cCue);
                }
            }

            return true;
        }
        
        public static NativeArray<Entity> GetTriggerCues(Entity gameplayEffect,Entity targetAsc,EntityManager entityManager,
            NativeArray<Entity> lastCueInstances,NativeArray<Entity> prefabCues)
        {
            // 0.先清楚已实例化的cue
            foreach (var cueInstance in lastCueInstances)
            {
                if (!entityManager.Exists(cueInstance)) continue;
                var mcCue = entityManager.GetComponentData<MCCue>(cueInstance);
                mcCue.cue.KillSelf();
            }
            // 1.实例化Cue
            var prefabCue = prefabCues;
            var cueEntities = new NativeArray<Entity>(prefabCue.Length, Allocator.Persistent);
            for (var i = 0; i < prefabCue.Length; i++)
            {
                var prefabEntity = prefabCue[i];
                // 1.先判断tag是否可以播放cue
                bool hasRequiredTags = entityManager.HasComponent<CPlayRequiredTags>(prefabEntity);
                if (hasRequiredTags)
                {
                    var requiredTags = entityManager.GetComponentData<CPlayRequiredTags>(prefabEntity);
                    if(!ASCUtil.HasAllTags(targetAsc,requiredTags.tags)) continue;
                }
                bool hasImmunitedTags = entityManager.HasComponent<CPlayImmunitedTags>(prefabEntity);
                if (hasImmunitedTags)
                {
                    var immunitedTags = entityManager.GetComponentData<CPlayImmunitedTags>(prefabEntity);
                    if(ASCUtil.HasAnyTags(targetAsc,immunitedTags.tags)) continue;
                }

                // 2.创建运行cue实例
                cueEntities[i] = entityManager.CreateEntity(); 
                entityManager.SetName(cueEntities[i], $"RuntimeCue_{cueEntities[i].Version}_{cueEntities[i].Index}");;
                // 2.1 复制RequiredTags
                if (hasRequiredTags)
                {
                    EntityHelper.AddComponent<CPlayRequiredTags>(cueEntities[i]);
                    var requiredTags = entityManager.GetComponentData<CPlayRequiredTags>(prefabEntity);
                    EntityHelper.SetComponent(cueEntities[i], new CPlayRequiredTags
                    {
                        tags = new NativeArray<int>(requiredTags.tags.ToArray(), Allocator.Persistent)
                    });
                }
                // 2.2 复制ImmunitedTags
                if (hasImmunitedTags)
                {
                    EntityHelper.AddComponent<CPlayImmunitedTags>(cueEntities[i]);
                    var immunitedTags = entityManager.GetComponentData<CPlayImmunitedTags>(prefabEntity);
                    EntityHelper.SetComponent(cueEntities[i], new CPlayImmunitedTags
                    {
                        tags = new NativeArray<int>(immunitedTags.tags.ToArray(), Allocator.Persistent)
                    });
                }
                // 2.3 复制 ECCuePlayable,ECCuePlaying,ECKillCue
                EntityHelper.AddComponent<ECCuePlaying>(cueEntities[i]);
                EntityHelper.AddComponent<ECCuePlayable>(cueEntities[i]);
                EntityHelper.AddComponent<ECKillCue>(cueEntities[i]);
                
                // 2.4 复制Cue逻辑
                var cueLogic = entityManager.GetComponentData<MCCue>(prefabEntity);
                EntityHelper.AddManagedComponent<MCCue>(cueEntities[i]);
                var cloneCue = CueHelper.CopyCueComponent(cueLogic);
                cloneCue = CueHelper.InitInstantCueFromGameplayEffect(cloneCue, cueEntities[i], gameplayEffect);
                cloneCue.cue.AddToTargetAsc(targetAsc);
                cloneCue.cue.Play(true);
                EntityHelper.SetManagedComponent(cueEntities[i],cloneCue);
            }

            return cueEntities;
        }
    }
}