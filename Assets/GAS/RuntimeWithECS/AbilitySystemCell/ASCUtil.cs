using System;
using GAS.Runtime;
using GAS.RuntimeWithECS.Ability.Component.Static;
using GAS.RuntimeWithECS.Attribute.Component;
using GAS.RuntimeWithECS.AttributeSet.Component;
using GAS.RuntimeWithECS.Core;
using GAS.RuntimeWithECS.GameplayEffect;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using GAS.RuntimeWithECS.Modifier;
using GAS.RuntimeWithECS.Tag;
using GAS.RuntimeWithECS.Tag.Component;
using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.AbilitySystemCell
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
            var modifiers = _entityManager.GetBuffer<BEModifier>(gameplayEffect);
            foreach (var mod in modifiers)
            {
                var magnitude = MmcHub.Calculate(gameplayEffect, mod);

                var attrSetIndex = attrSets.IndexOfAttrSetCode(mod.AttrSetCode);
                if (attrSetIndex == -1) continue;

                var attrSet = attrSets[attrSetIndex];
                var attributes = attrSet.Attributes;

                var attrIndex = attributes.IndexOfAttrCode(mod.AttrCode);
                if (attrIndex == -1) continue;

                var attr = attributes[attrIndex];
                var oldValue = attr.BaseValue;
                var newValue = oldValue;
                switch (mod.Operation)
                {
                    case GEOperation.Add:
                        newValue += magnitude;
                        break;
                    case GEOperation.Minus:
                        newValue -= magnitude;
                        break;
                    case GEOperation.Multiply:
                        newValue *= magnitude;
                        break;
                    case GEOperation.Divide:
                        newValue /= magnitude;
                        break;
                    case GEOperation.Override:
                        newValue = magnitude;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

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

        public static bool HasGameplayEffect(this Entity asc, Entity gameplayEffect)
        {
            var geBuff = _entityManager.GetBuffer<BEGameplayEffect>(asc);
            foreach (var geElem in geBuff)
                if (geElem.GameplayEffect == gameplayEffect)
                    return true;
            return false;
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
                        var mods = _entityManager.GetBuffer<BEModifier>(ge);
                        foreach (var modElement in mods)
                            if (modElement.AttrSetCode == attrSet.Code && modElement.AttrCode == attr.Code)
                                newValue = MmcHub.Calculate(ge, modElement, newValue);
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

        private static bool TryRemoveDynamicAddedTag(Entity asc,Entity source,int tag)
        {
            var dirty = false;

            // if (source is GameplayEffectSpec || source is AbilitySpec)
            // {
            //     var hasValue = dynamicTag.TryGetValue(tag, out var tagList);
            //     if (hasValue)
            //     {
            //         tagList.Remove(source);
            //
            //         dirty = tagList.Count == 0;
            //         if (dirty)
            //         {
            //             _pool.Return(tagList);
            //             
            //             dynamicTag.Remove(tag); // 有 GC
            //         }
            //     }
            // }
            
            return dirty;
        }
        
        // TODO
        public static void RestoreDynamicTags(Entity asc,Entity source,NativeArray<int> tags)
        {
            var tagIsDirty = false;
            foreach (var tag in tags)
            {
                var dirty = TryRemoveDynamicAddedTag(asc,source, tag);
                tagIsDirty = tagIsDirty || dirty;
            }

            if (tagIsDirty)
            {
                if (tags.Length > 0)
                {
                    //GASEventCenter.InvokeOnTagIsDirty(asc,tag,eventType);
                }
            }
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
                bool hasGrantedTags = _entityManager.HasComponent<CGrantedTags>(source);
                if (hasGrantedTags)
                {
                    var grantedTags = _entityManager.GetComponentData<CGrantedTags>(source);
                    if (_entityManager.HasComponent<CInUsage>(source))
                    {
                        var inUsage = _entityManager.GetComponentData<CInUsage>(source);
                        RestoreDynamicTags(inUsage.Target,source,grantedTags.tags);
                    }
                }
            }
        }

        #endregion
    }
}