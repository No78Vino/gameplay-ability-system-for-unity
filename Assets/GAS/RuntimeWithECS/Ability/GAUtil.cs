using GAS.Runtime;
using GAS.RuntimeWithECS.Ability.Component.Dynamic;
using GAS.RuntimeWithECS.Ability.Component.Static;
using GAS.RuntimeWithECS.AbilitySystemCell;
using GAS.RuntimeWithECS.Attribute.Component;
using GAS.RuntimeWithECS.AttributeSet.Component;
using GAS.RuntimeWithECS.Core;
using GAS.RuntimeWithECS.GameplayEffect;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using GAS.RuntimeWithECS.Modifier;
using GAS.RuntimeWithECS.Tag;
using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.Ability
{
    public enum AbilityActivationResult
    {
        Success,
        FailHasActivated,
        FailTagRequirement,
        FailCost,
        FailCooldown,
        FailOtherReason
    }

    /// <summary>
    ///     Gameplay Ability Utility
    ///     游戏能力工具类,对应原本的AbilitySpec
    ///     原本Mono版本里的所有ability自身的功能性方法全部转为静态方法
    ///     Entity + Util方式代替Mono + AbilitySpec方式
    /// </summary>
    public static class GAUtil
    {
        private static EntityManager _entityManager => GASManager.EntityManager;

        /// <summary>
        ///     检查能力是否可以激活
        /// </summary>
        /// <param name="ability"></param>
        /// <returns>   </returns>
        public static AbilityActivationResult CanActivateAbility(Entity ability)
        {
            if (_entityManager.HasComponent<CAbilityActive>(ability))
                return AbilityActivationResult.FailHasActivated;

            if (!CheckGameplayTagsValidTpActivate(ability))
                return AbilityActivationResult.FailTagRequirement;

            if (!CheckCost(ability))
                return AbilityActivationResult.FailCost;

            if (!CheckCooldownReady(ability))
                return AbilityActivationResult.FailCooldown;

            return AbilityActivationResult.Success;
        }

        public static bool CheckGameplayTagsValidTpActivate(Entity ability)
        {
            var owner = _entityManager.GetComponentData<CAbilityBaseInfo>(ability).Owner;
            // 1. 检查激活所需标签
            var hasAllTags = true;
            var abilityHasActivationRequiredTags = _entityManager.HasComponent<CAbilityActivationRequiredTags>(ability);
            if (abilityHasActivationRequiredTags)
            {
                var abilityActivationRequiredTags =
                    _entityManager.GetComponentData<CAbilityActivationRequiredTags>(ability);
                hasAllTags = ASCUtil.HasAllTags(owner, abilityActivationRequiredTags.tags);
            }

            // 2. 检查激活被阻止的标签
            var notHasAnyTags = true;
            var abilityHasActivationBlockedTags = _entityManager.HasComponent<CAbilityActivationBlockedTags>(ability);
            if (abilityHasActivationBlockedTags)
            {
                var abilityActivationBlockedTags =
                    _entityManager.GetComponentData<CAbilityActivationBlockedTags>(ability);
                notHasAnyTags = !ASCUtil.HasAnyTags(owner, abilityActivationBlockedTags.tags);
            }

            // 3. 检查是否被其他能力阻止,遍历宿主其它能力,检查是否有阻止激活的标签
            var notBlockedByOtherAbility = true;
            var ownerAbilities = _entityManager.GetBuffer<BEAbility>(owner);
            foreach (var ownerAbility in ownerAbilities)
            {
                var ownerAbilityEntity = ownerAbility.Ability;
                if (ownerAbilityEntity == ability) continue;
                
                var ownerAbilityHasBlockAbilitiesWithTags =
                    _entityManager.HasComponent<CBlockAbilityTags>(ownerAbilityEntity);
                if (!ownerAbilityHasBlockAbilitiesWithTags) continue;
                
                var ownerAbilityBlockAbilitiesWithTags =
                    _entityManager.GetComponentData<CBlockAbilityTags>(ownerAbilityEntity);
                if (!HasAnyTags(ownerAbilityEntity, ownerAbilityBlockAbilitiesWithTags.tags)) continue;
                
                notBlockedByOtherAbility = false;
                break;
            }
            return hasAllTags && notHasAnyTags && notBlockedByOtherAbility;
        }

        public static bool CheckCost(Entity ability)
        {
            bool hasCostComponent = _entityManager.HasComponent<CAbilityCost>(ability);
            if (!hasCostComponent) return true;
            
            var costComponent = _entityManager.GetComponentData<CAbilityCost>(ability);
            bool isInstantEffect = !_entityManager.HasComponent<CDuration>(costComponent.ProtoGameplayEffectCost);
            if (!isInstantEffect) return true;
            
            var modifierBuffer = _entityManager.GetBuffer<BEModifier>(costComponent.ProtoGameplayEffectCost);
            var owner = _entityManager.GetComponentData<CAbilityBaseInfo>(ability).Owner;
            var attrSets = _entityManager.GetBuffer<BEAttributeSet>(owner);
            foreach (var modifier in modifierBuffer)
            {
                var opt = modifier.Operation;
                if (opt != GEOperation.Add && opt != GEOperation.Minus) continue;
                
                var attrSetIndex = attrSets.IndexOfAttrSetCode(modifier.AttrSetCode);
                if (attrSetIndex == -1) continue;

                var attrSet = attrSets[attrSetIndex];
                var attributes = attrSet.Attributes;

                var attrIndex = attributes.IndexOfAttrCode(modifier.AttrCode);
                if (attrIndex == -1) continue;
                
                var attr = attributes[attrIndex];
                var costValue = MmcHub.Calculate(costComponent.ProtoGameplayEffectCost, modifier);
                var attributeCurrentValue = attr.CurrentValue;
                switch (modifier.Operation)
                {
                    case GEOperation.Add when attributeCurrentValue + costValue < 0:
                    case GEOperation.Minus when attributeCurrentValue - costValue < 0:
                        return false;
                }
            }

            return true;
        }

        public static bool CheckCooldownReady(Entity ability)
        {
            bool hasCooldownComponent = _entityManager.HasComponent<CAbilityCooldown>(ability);
            if (!hasCooldownComponent) return true;
            
            CAbilityCooldown cooldown = _entityManager.GetComponentData<CAbilityCooldown>(ability);
            // 没有激活的实例,说明冷却已经结束
            return cooldown.CooldownGameplayEffectInstance == Entity.Null;
        }

        public static void DoCost(Entity ability)
        {
            if (!_entityManager.HasComponent<CAbilityCost>(ability)) return;
            
            var costComponent = _entityManager.GetComponentData<CAbilityCost>(ability);
            var owner = _entityManager.GetComponentData<CAbilityBaseInfo>(ability).Owner;
            GEUtil.ApplyGameplayEffectImmediate(costComponent.ProtoGameplayEffectCost, owner, owner);
        }

        public static AbilityActivationResult TryActivateAbility(Entity ability)
        {
            var result = CanActivateAbility(ability);
            if (result == AbilityActivationResult.Success)
            {
                var owner = _entityManager.GetComponentData<CAbilityBaseInfo>(ability).Owner;
                var abilityActivationOwnedTags = _entityManager.GetComponentData<CAbilityActivationOwnedTags>(ability);
                foreach (var tag in abilityActivationOwnedTags.tags)
                    GTagUtil.AddTemporaryTagTo(owner, ability, tag);
                
                _entityManager.AddComponentData(ability, new CAbilityActive());
                
                var abilityLogic = _entityManager.GetComponentData<MCAbilityLogic>(ability);
                abilityLogic.Logic.ActivateAbility();
            }
            GASEventCenter.InvokeOnActivateResult(ability, result);
            return result;
        }
        
        public static bool TryEndAbility(Entity ability)
        {
            bool result = _entityManager.HasComponent<CAbilityActive>(ability);
            if (result)
            {
                _entityManager.RemoveComponent<CAbilityActive>(ability);
                ASCUtil.RestoreDynamicTags(ability);
                var abilityLogic = _entityManager.GetComponentData<MCAbilityLogic>(ability);
                abilityLogic.Logic.EndAbility();
                GASEventCenter.InvokeOnEndAbility(ability);
            }
            
            return result;
        }
        
        public static bool TryCancelAbility(Entity ability)
        {
            bool result = _entityManager.HasComponent<CAbilityActive>(ability);
            if (result)
            {
                _entityManager.RemoveComponent<CAbilityActive>(ability);
                ASCUtil.RestoreDynamicTags(ability);
                var abilityLogic = _entityManager.GetComponentData<MCAbilityLogic>(ability);
                abilityLogic.Logic.CancelAbility();
                GASEventCenter.InvokeOnCancelAbility(ability);
            }
            return true;
        }

        /// <summary>
        ///     检查是否有指定标签，能力的tag校验只校验AssetTag
        /// </summary>
        /// <param name="ability"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        public static bool HasAnyTags(Entity ability, NativeArray<int> tags)
        {
            var hasAssetTag = _entityManager.HasComponent<CAbilityAssetTags>(ability);
            if (!hasAssetTag) return true;

            var assetTags = _entityManager.GetComponentData<CAbilityAssetTags>(ability);
            foreach (var tag in tags)
            foreach (var assetTag in assetTags.tags)
                if (GTagUtil.HasTag(assetTag, tag))
                    return true;
            return false;
        }
    }
}