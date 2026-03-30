﻿using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    public struct SingletonGameplayTagMap : IComponentData
    {
        public NativeHashMap<int, ComGameplayTag> Map;
    }

    public struct ComGameplayTag : IComponentData
    {
        public int Code;
        public NativeArray<int> Parents;
        public NativeArray<int> Children;
    }
    
    
    public static class SingletonGameplayTagMapExtension
    {
        [BurstCompile]
        public static bool IsTagAIncludeTagB(this SingletonGameplayTagMap map,int tagA, int tagB)
        {
            if (map.Map.ContainsKey(tagA) && map.Map.ContainsKey(tagB))
                return map.Map[tagA].HasTag(map.Map[tagB]);
            return false;
        }

        [BurstCompile]
        private static bool HasTag(this ComGameplayTag gTag, ComGameplayTag tag)
        {
            if (gTag.Code == tag.Code) return true;
            foreach (var pTag in gTag.Parents)
                if (pTag == tag.Code)
                    return true;

            return false;
        }

        [BurstCompile]
        public static bool AscEvaluateTagRequirement(this SingletonGameplayTagMap map, EntityManager entityManager, Entity asc,
            in TagRequirementData query)
        {
            bool passAll = !query.all.IsCreated || query.all.Length == 0 || map.AscHasAllTags(entityManager, asc, query.all);
            bool passAny = !query.any.IsCreated || query.any.Length == 0 || map.AscHasAnyTags(entityManager, asc, query.any);
            bool passNone = !query.none.IsCreated || query.none.Length == 0 || !map.AscHasAnyTags(entityManager, asc, query.none);
            return passAll && passAny && passNone;
        }

        [BurstCompile]
        public static bool AscHasAllTags(this SingletonGameplayTagMap map, EntityManager entityManager, Entity asc,
            NativeArray<int> tags)
        {
            if (tags.Length == 0) return true;
            
            var fixedTags = entityManager.GetBuffer<BFixedTag>(asc);
            var tempTags = entityManager.GetBuffer<BTemporaryTag>(asc);

            foreach (var tag in tags)
            {
                var hasTag = false;
                // 遍历固有Tag
                foreach (var fixedTag in fixedTags)
                    if (map.IsTagAIncludeTagB(fixedTag.tag, tag))
                    {
                        hasTag = true;
                        break;
                    }

                // 遍历临时Tag
                if (!hasTag)
                    foreach (var tempTag in tempTags)
                        if (map.IsTagAIncludeTagB(tempTag.tag, tag))
                        {
                            hasTag = true;
                            break;
                        }

                if (!hasTag) return false;
            }

            return true;
        }


        [BurstCompile]
        public static bool AscHasAnyTags(this SingletonGameplayTagMap map, EntityManager entityManager, Entity asc,
            NativeArray<int> tags)
        {
            if (tags.Length == 0) return true;
            
            var fixedTags = entityManager.GetBuffer<BFixedTag>(asc);
            var tempTags = entityManager.GetBuffer<BTemporaryTag>(asc);

            foreach (var tag in tags)
            {
                // 遍历固有Tag
                foreach (var fixedTag in fixedTags)
                    if (map.IsTagAIncludeTagB(fixedTag.tag, tag))
                        return true;

                // 遍历临时Tag
                foreach (var tempTag in tempTags)
                    if (map.IsTagAIncludeTagB(tempTag.tag, tag))
                        return true;
            }

            return false;
        }

        [BurstCompile]
        public static bool EffectEvaluateTagRequirement(this SingletonGameplayTagMap map, EntityManager entityManager, Entity gameplayEffect,
            in TagRequirementData query)
        {
            bool passAll = !query.all.IsCreated || query.all.Length == 0 || map.EffectHasAllTags(entityManager, gameplayEffect, query.all);
            bool passAny = !query.any.IsCreated || query.any.Length == 0 || map.EffectHasAnyTags(entityManager, gameplayEffect, query.any);
            bool passNone = !query.none.IsCreated || query.none.Length == 0 || !map.EffectHasAnyTags(entityManager, gameplayEffect, query.none);
            return passAll && passAny && passNone;
        }

        [BurstCompile]
        public static bool EffectHasAllTags(this SingletonGameplayTagMap map, EntityManager entityManager,
            Entity gameplayEffect,
            NativeArray<int> tags)
        {
            if (tags.Length == 0) return true;

            NativeArray<int> assetTags = default;
            NativeArray<int> grantedTags = default;
            bool hasAssetTags = entityManager.HasComponent<CEffectAssetTags>(gameplayEffect);
            bool hasGrantedTags = entityManager.HasComponent<CEffectGrantedTags>(gameplayEffect);
            if(hasAssetTags)
                assetTags = entityManager.GetComponentData<CEffectAssetTags>(gameplayEffect).tags;
            if(hasGrantedTags)
                grantedTags = entityManager.GetComponentData<CEffectGrantedTags>(gameplayEffect).tags;
            
            foreach (var tag in tags)
            {
                var hasTag = false;
                // 1.判断AssetTags
                if (hasAssetTags)
                    foreach (var assetTag in assetTags)
                        if (map.IsTagAIncludeTagB(assetTag, tag))
                        {
                            hasTag = true;
                            break;
                        }

                //2.判断GrantedTags
                if (!hasTag && hasGrantedTags)
                    foreach (var grantedTag in grantedTags)
                        if (map.IsTagAIncludeTagB(grantedTag, tag))
                        {
                            hasTag = true;
                            break;
                        }

                if (!hasTag) return false;
            }

            return true;
        }

        [BurstCompile]
        public static bool EffectHasAnyTags(this SingletonGameplayTagMap map, EntityManager entityManager,
            Entity gameplayEffect,
            NativeArray<int> tags)
        {
            if (tags.Length == 0) return true;

            // 1.判断AssetTags
            if (entityManager.HasComponent<CEffectAssetTags>(gameplayEffect))
            {
                var assetTags = entityManager.GetComponentData<CEffectAssetTags>(gameplayEffect).tags;
                foreach (var assetTag in assetTags)
                    foreach (var tag in tags)
                        if (map.IsTagAIncludeTagB(assetTag, tag))
                            return true;
            }

            //2.判断GrantedTags
            if (entityManager.HasComponent<CEffectGrantedTags>(gameplayEffect))
            {
                var grantedTags = entityManager.GetComponentData<CEffectGrantedTags>(gameplayEffect).tags;
                foreach (var grantedTag in grantedTags)
                    foreach (var tag in tags)
                        if (map.IsTagAIncludeTagB(grantedTag, tag))
                            return true;
            }

            return false;
        }
    }
}