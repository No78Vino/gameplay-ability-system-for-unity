using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    public static class ASCUtil
    {
        private static EntityManager _entityManager => GASManager.EntityManager;

        public static void RemoveGameplayEffectWithAnyTags(this Entity asc, Entity gameplayEffect)
        {
            if (!_entityManager.HasComponent<CRemoveEffectWithTags>(gameplayEffect)) return;

            var comRemoveEffectWithTags = _entityManager.GetComponentData<CRemoveEffectWithTags>(gameplayEffect);
            var removeEffectWithTags = comRemoveEffectWithTags.tags;
            if (removeEffectWithTags.Length == 0) return;

            var geBuff = _entityManager.GetBuffer<BEGameplayEffect>(asc);
            for (var i = geBuff.Length - 1; i >= 0; i--)
            {
                var ge = geBuff[i].GameplayEffect;
                var hasRemoveTag = ge.CheckEffectHasAnyTags(removeEffectWithTags);
                if (!hasRemoveTag) continue;
                geBuff.RemoveAt(i);
                // 含有子实例的组件也要清理
                if (_entityManager.HasComponent<CPeriod>(ge))
                {
                    var period = _entityManager.GetComponentData<CPeriod>(ge);
                    foreach (var sonGe in period.GameplayEffects)
                        _entityManager.DestroyEntity(sonGe);
                }

                _entityManager.DestroyEntity(ge);
            }
        }

        public static void ApplyModFromInstantGameplayEffect(this Entity asc, Entity gameplayEffect)
        {
            var attrSets = _entityManager.GetBuffer<BEAttributeSet>(asc);
            var modifiers = _entityManager.GetComponentData<MCModifiers>(gameplayEffect);
            foreach (var mod in modifiers.Modifiers)
            {
                var attrSetIndex = attrSets.IndexOfAttrSetCode(mod.AttrSetCode);
                if (attrSetIndex == -1) continue;

                var attrSet = attrSets[attrSetIndex];
                var attributes = attrSet.Attributes;

                var attrIndex = attributes.IndexOfAttrCode(mod.AttrCode);
                if (attrIndex == -1) continue;

                var attr = attributes[attrIndex];
                var oldValue = attr.BaseValue;
                var newValue = MmcHelper.Calculate(gameplayEffect, mod, attr.BaseValue);
                    
                // OnChangeBefore
                // BaseValue 不做钳制，因为Max，Min是只针对Current Value
                newValue = GASEventCenter.InvokeOnBaseValueChangeBefore(asc, mod.AttrSetCode, mod.AttrCode, newValue);

                attr.BaseValue = newValue;

                // OnChangeAfter
                if (newValue != oldValue)
                {
                    // BaseValue 改变，需要标记Dirty
                    attr.Dirty = true;
                    GASManager.EntityManager.AddComponent<CAttributeIsDirty>(asc);
                    GASEventCenter.InvokeOnBaseValueChangeAfter(asc, mod.AttrSetCode, mod.AttrCode, oldValue, newValue);
                }

                attrSet.Attributes[attrIndex] = attr;
                attrSets[attrSetIndex] = attrSet;
            }
        }

        public static bool HasAllTags(Entity asc, NativeArray<int> tags)
        {
            var fixedTags = _entityManager.GetBuffer<BFixedTag>(asc);
            var tempTags = _entityManager.GetBuffer<BTemporaryTag>(asc);

            foreach (var tag in tags)
            {
                var hasTag = false;

                foreach (var fixedTag in fixedTags)
                    if (GTagUtil.HasTag(fixedTag.tag, tag))
                    {
                        hasTag = true;
                        break;
                    }

                if (!hasTag)
                    foreach (var tempTag in tempTags)
                        if (GTagUtil.HasTag(tempTag.tag, tag))
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
            var fixedTags = _entityManager.GetBuffer<BFixedTag>(asc);
            var tempTags = _entityManager.GetBuffer<BTemporaryTag>(asc);

            foreach (var tag in tags)
            {
                foreach (var fixedTag in fixedTags)
                    if (GTagUtil.HasTag(fixedTag.tag, tag))
                        return true;

                foreach (var tempTag in tempTags)
                    if (GTagUtil.HasTag(tempTag.tag, tag))
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
            var geBuff = _entityManager.GetBuffer<BEGameplayEffect>(asc);
            foreach (var geElem in geBuff)
                if (geElem.GameplayEffect == gameplayEffect)
                    return true;
            return false;
        }
        
        public static bool TryAddGameplayEffect(this Entity asc, Entity gameplayEffect)
        {
            var geBuff = _entityManager.GetBuffer<BEGameplayEffect>(asc);
            foreach (var geElem in geBuff)
                if (geElem.GameplayEffect == gameplayEffect)
                    return false;
            geBuff.Add(new BEGameplayEffect { GameplayEffect = gameplayEffect });
            return true;
        }

        /// <summary>
        ///     GE列表为脏，需要重新计算Attribute的Current Value
        /// </summary>
        /// <param name="asc"></param>
        public static void GameplayEffectListIsDirty(this Entity asc)
        {
            // 1.尝试更新自身的Attribute的Current Value
            asc.TryRecalculateAttributeCurrentValue();

            // TODO 2.触发 OnGameplayEffectListIsDirty 注册的事件
        }

        #region 重计算AttrSet Current Value相关工具函数

        /// <summary>
        ///     尝试重计算
        /// </summary>
        /// <param name="asc"></param>
        /// <returns></returns>
        public static bool TryRecalculateAttributeCurrentValue(this Entity asc)
        {
            bool isValueChanged = false;
            var attrSets = _entityManager.GetBuffer<BEAttributeSet>(asc);
            var effects = _entityManager.GetBuffer<BEGameplayEffect>(asc);
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
                        var mods = _entityManager.GetComponentData<MCModifiers>(ge);
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
            return _entityManager.GetBuffer<BFixedTag>(asc);
        }
        
        public static DynamicBuffer<BTemporaryTag> GetDynamicBufferTemporaryTags(Entity asc)
        {
            return _entityManager.GetBuffer<BTemporaryTag>(asc);
        }
        
        public static void TryAddDynamicAddedTag(Entity asc, Entity source, int tag)
        {
            GTagUtil.AddTemporaryTagTo(asc, source, tag);
        }
        
        public static void TryAddDynamicAddedTags(Entity asc, Entity source, int[] tags)
        {
            foreach (var tag in tags)
                GTagUtil.AddTemporaryTagTo(asc, source, tag);
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
                if( TryRemoveDynamicAddedTag(asc,source, tag))
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
            bool hasAbilityBaseInfo = _entityManager.HasComponent<CAbilityBaseInfo>(source);
            if (hasAbilityBaseInfo)
            {
                bool hasActivationOwnedTags = _entityManager.HasComponent<CAbilityActivationOwnedTags>(source);
                if (hasActivationOwnedTags)
                {
                    var activationOwnedTags = _entityManager.GetComponentData<CAbilityActivationOwnedTags>(source);
                    if (_entityManager.HasComponent<CAbilityBaseInfo>(source))
                    {
                        var abilityBaseInfo = _entityManager.GetComponentData<CAbilityBaseInfo>(source);
                        RestoreDynamicTags(abilityBaseInfo.Owner, source,activationOwnedTags.tags);
                    }
                }
            }
            
            // 如果是durational effect
            bool hasDuration = _entityManager.HasComponent<CDuration>(source);
            if (hasDuration)
            {
                bool hasGrantedTags = _entityManager.HasComponent<CEffectGrantedTags>(source);
                if (hasGrantedTags)
                {
                    var grantedTags = _entityManager.GetComponentData<CEffectGrantedTags>(source);
                    if (_entityManager.HasComponent<CEffectInUsage>(source))
                    {
                        var inUsage = _entityManager.GetComponentData<CEffectInUsage>(source);
                        RestoreDynamicTags(inUsage.Target,source,grantedTags.tags);
                    }
                }
            }
        }

        #endregion
    }
}