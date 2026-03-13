using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace GAS.Runtime
{
    public static class ASCHelper
    {
        private static EntityManager _em => GASManager.EntityManager;

        public static bool HasAllTags(Entity asc, NativeArray<int> tags)
        {
            var fixedTags = _em.GetBuffer<BFixedTag>(asc);
            var tempTags = _em.GetBuffer<BTemporaryTag>(asc);

            foreach (var tag in tags)
            {
                var hasTag = false;

                foreach (var fixedTag in fixedTags)
                    if (TagHelper.HasTag(fixedTag.tag, tag))
                    {
                        hasTag = true;
                        break;
                    }

                if (!hasTag)
                    foreach (var tempTag in tempTags)
                        if (TagHelper.HasTag(tempTag.tag, tag))
                        {
                            hasTag = true;
                            break;
                        }

                if (!hasTag) return false;
            }

            return true;
        }

        public static bool HasAllTags(Entity asc, int[] tags)
        {
            var nativeTags = new NativeArray<int>(tags, Allocator.Temp);
            var result = HasAllTags(asc, nativeTags);
            nativeTags.Dispose();
            return result;
        }

        public static bool HasAnyTags(Entity asc, NativeArray<int> tags)
        {
            var fixedTags = _em.GetBuffer<BFixedTag>(asc);
            var tempTags = _em.GetBuffer<BTemporaryTag>(asc);

            foreach (var tag in tags)
            {
                foreach (var fixedTag in fixedTags)
                    if (TagHelper.HasTag(fixedTag.tag, tag))
                        return true;

                foreach (var tempTag in tempTags)
                    if (TagHelper.HasTag(tempTag.tag, tag))
                        return true;
            }

            return false;
        }
        
        public static bool HasAnyTags(Entity asc, int[] tags)
        {
            var nativeTags = new NativeArray<int>(tags, Allocator.Temp);
            var result = HasAnyTags(asc, nativeTags);
            nativeTags.Dispose();
            return result;
        }

        public static bool HasGameplayEffect(this Entity asc, Entity gameplayEffect)
        {
            var geBuff = _em.GetBuffer<BGameplayEffect>(asc);
            foreach (var geElem in geBuff)
                if (geElem.GameplayEffect == gameplayEffect)
                    return true;
            return false;
        }
        
        public static bool TryAddGameplayEffect(this Entity asc, Entity gameplayEffect)
        {
            var geBuff = _em.GetBuffer<BGameplayEffect>(asc);
            foreach (var geElem in geBuff)
                if (geElem.GameplayEffect == gameplayEffect)
                    return false;
            geBuff.Add(new BGameplayEffect { GameplayEffect = gameplayEffect });
            return true;
        }


        #region GameplayEffect 相关工具函数
        
        /// <summary>  
        /// 从 GE Entity 获取 Source ASC 的 Entity。  
        /// 通过 CEffectInUsage 组件读取 Source 字段。  
        /// </summary>  
        public static Entity GetSourceAscEntity(Entity geEntity)  
        {
            if (EntityHelper.HasComponent<CEffectInUsage>(geEntity))
                return EntityHelper.GetComponentData<CEffectInUsage>(geEntity).Source;
            Debug.LogWarning("[ASCHelper] GE 上没有 CEffectInUsage 组件，无法获取 Source ASC Entity。");  
            return Entity.Null;
        }  
  
        /// <summary>  
        /// 从 GE Entity 获取 Target ASC 的 Entity。  
        /// 通过 CEffectInUsage 组件读取 Target 字段。  
        /// </summary>  
        public static Entity GetTargetAscEntity(Entity geEntity)  
        {
            if (EntityHelper.HasComponent<CEffectInUsage>(geEntity))
                return EntityHelper.GetComponentData<CEffectInUsage>(geEntity).Target;
            Debug.LogWarning("[ASCHelper] GE 上没有 CEffectInUsage 组件，无法获取 Target ASC Entity。");  
            return Entity.Null;
        }  
  
        /// <summary>  
        /// 从 GE Entity 获取 Source ASC 的 OOP 包装（AbilitySystemCell）。  
        /// 先取 Source Entity，再通过 GASManager 转换为 AbilitySystemCell。  
        /// </summary>  
        public static AbilitySystemCell GetSourceAsc(Entity geEntity)  
        {  
            var sourceEntity = GetSourceAscEntity(geEntity);  
            return sourceEntity == Entity.Null ? null : GASManager.GetAscFromEntity(sourceEntity);
        }
        
        #endregion
        
        #region 重计算AttrSet Current Value相关工具函数

        /// <summary>
        ///     尝试重计算
        /// </summary>
        /// <param name="asc"></param>
        /// <returns></returns>
        public static bool TryRecalculateAttributeCurrentValue(this Entity asc)
        {
            bool isValueChanged = false;
            var attrSets = _em.GetBuffer<BEAttrSet>(asc);
            var effects = _em.GetBuffer<BGameplayEffect>(asc);
            for (var attrSetIndex = 0; attrSetIndex < attrSets.Length; attrSetIndex++)
            {
                var attrSet = attrSets[attrSetIndex];
                for (var attrIndex = 0; attrIndex < attrSet.Attributes.Length; attrIndex++)
                {
                    var attr = attrSet.Attributes[attrIndex];
                    if (!attr.Dirty) continue;


                    var oldValue = attr.CurrentValue;
                    var newValue = oldValue;
                    // 遍历当前asc的GE队列重计算Current Value
                    foreach (var element in effects)
                    {
                        var ge = element.GameplayEffect;
                        var mods = _em.GetComponentData<MCModifiers>(ge);
                        foreach (var modElement in mods.Modifiers)
                            if (modElement.AttrSetCode == attrSet.Code && modElement.AttrCode == attr.Code)
                                newValue = MmcHelper.Calculate(ge, modElement, newValue);
                    }

                    attr.CurrentValue = newValue;
                    // OnChangeAfter
                    if (newValue != oldValue)
                    {
                        GASEventCenter.InvokeOnCurrentValueChangeAfter(
                            asc, attrSet.Code, attr.Code, oldValue, newValue);
                        isValueChanged = true;
                    }

                    // CurrentValue改变完成，取消标记Dirty
                    attr.Dirty = false;

                    attrSet.Attributes[attrIndex] = attr;
                    attrSets[attrSetIndex] = attrSet;
                }
            }

            return isValueChanged;
        }

        #endregion


        #region Tag管理相关工具函数

        public static DynamicBuffer<BFixedTag> GetDynamicBufferFixedTags(Entity asc)
        {
            return _em.GetBuffer<BFixedTag>(asc);
        }
        
        public static DynamicBuffer<BTemporaryTag> GetDynamicBufferTemporaryTags(Entity asc)
        {
            return _em.GetBuffer<BTemporaryTag>(asc);
        }
        
        public static void TryAddDynamicAddedTag(Entity asc, Entity source, int tag)
        {
            TagHelper.AddTemporaryTagTo(asc, source, tag);
        }
        
        public static void TryAddDynamicAddedTags(Entity asc, Entity source, IEnumerable<int> tags)
        {
            foreach (var tag in tags)
                TagHelper.AddTemporaryTagTo(asc, source, tag);
        }
        
        private static bool TryRemoveDynamicAddedTag(Entity asc,Entity source,int tag)
        {
            var dirty = false;
            var tempTags = GetDynamicBufferTemporaryTags(asc);
            int index = -1;
            for (var i = 0; i < tempTags.Length; i++)
            {
                if (tempTags[i].tag != tag) continue;
                if (tempTags[i].source != source) continue;
                index = i;
                break;
            }

            if (index >= 0)
            {
                dirty = true;
                tempTags.RemoveAt(index);
            }
            
            return dirty;
        }
        
        public static void RestoreDynamicTags(Entity asc,Entity source,NativeArray<int> tags)
        {
            foreach (var tag in tags)
                if(TryRemoveDynamicAddedTag(asc,source, tag))
                    GASEventCenter.InvokeOnTagIsDirty(asc,tag,GameplayTagChangeEvent.RemoveTag);
        }
        
        public static void RestoreDynamicTags(Entity asc,Entity source,IEnumerable<int> tags)
        {
            foreach (var tag in tags)
                if( TryRemoveDynamicAddedTag(asc,source, tag))
                    GASEventCenter.InvokeOnTagIsDirty(asc,tag,GameplayTagChangeEvent.RemoveTag);
        }
        
        public static void RestoreDynamicTags(Entity source)
        {
            // 如果是ability
            bool hasAbilityBaseInfo = _em.HasComponent<CAbilityBaseInfo>(source);
            if (hasAbilityBaseInfo)
            {
                bool hasActivationOwnedTags = _em.HasComponent<CAbilityActivationOwnedTags>(source);
                if (hasActivationOwnedTags)
                {
                    var activationOwnedTags = _em.GetComponentData<CAbilityActivationOwnedTags>(source);
                    if (_em.HasComponent<CAbilityBaseInfo>(source))
                    {
                        var abilityBaseInfo = _em.GetComponentData<CAbilityBaseInfo>(source);
                        RestoreDynamicTags(abilityBaseInfo.Owner, source,activationOwnedTags.tags);
                    }
                }
            }
            
            // 如果是durational effect
            bool hasDuration = _em.HasComponent<CDuration>(source);
            if (hasDuration)
            {
                bool hasGrantedTags = _em.HasComponent<CEffectGrantedTags>(source);
                if (hasGrantedTags)
                {
                    var grantedTags = _em.GetComponentData<CEffectGrantedTags>(source);
                    if (_em.HasComponent<CEffectInUsage>(source))
                    {
                        var inUsage = _em.GetComponentData<CEffectInUsage>(source);
                        RestoreDynamicTags(inUsage.Target,source,grantedTags.tags);
                    }
                }
            }
        }

        #endregion
    }
}