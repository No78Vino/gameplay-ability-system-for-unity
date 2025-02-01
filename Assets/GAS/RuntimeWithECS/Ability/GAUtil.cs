using GAS.RuntimeWithECS.Ability.Component.Dynamic;
using GAS.RuntimeWithECS.Ability.Component.Static;
using GAS.RuntimeWithECS.AbilitySystemCell;
using GAS.RuntimeWithECS.Core;
using GAS.RuntimeWithECS.GameplayEffect.Component;
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
            // TODO
            bool hasCostComponent = _entityManager.HasComponent<CAbilityCostGameplayEffect>(ability);
            if (!hasCostComponent) return true;
            
            var costComponent = _entityManager.GetComponentData<CAbilityCostGameplayEffect>(ability);
            bool isInstantEffect = !_entityManager.HasComponent<CDuration>(costComponent.CostGameplayEffect);
            if (!isInstantEffect) return true;
            
            var modifierBuffer = _entityManager.GetBuffer<BEGameplayEffect>(costComponent.CostGameplayEffect);
            foreach (var modifier in modifierBuffer)
            {
                // var gameplayEffect = modifier.GameplayEffect;
                // var gameplayEffectSpec = _entityManager.GetComponentData<CGameplayEffectSpec>(gameplayEffect);
                // var costSpec = _entityManager.GetComponentData<CGameplayEffectSpec>(gameplayEffectSpec.CostSpec);
                // var costValue = _entityManager.GetComponentData<CGameplayEffectMagnitude>(costSpec.Magnitude);
                // var attributeCurrentValue = _entityManager.GetComponentData<CAttributeData>(
                //     _entityManager.GetComponentData<CGameplayEffectAttribute>(costSpec.Attribute).Attribute).Value;
                // if (costValue.Value < 0)
                // {
                //     if (attributeCurrentValue + costValue.Value < 0) return false;
                // }
                // else
                // {
                //     if (attributeCurrentValue - costValue.Value < 0) return false;
                // }
            }
            // foreach (var modifier in Ability.Cost.Modifiers)
            // {
            //     // 常规来说消耗是减法, 但是加一个负数也应该被视为减法
            //     if (modifier.Operation != GEOperation.Add && modifier.Operation != GEOperation.Minus) continue;
            //
            //     var costValue = modifier.CalculateMagnitude(costSpec, modifier.ModiferMagnitude);
            //     var attributeCurrentValue =
            //         Owner.GetAttributeCurrentValue(modifier.AttributeSetName, modifier.AttributeShortName);
            //     
            //     if(modifier.Operation == GEOperation.Add)
            //         if (attributeCurrentValue + costValue < 0) return false;
            //     
            //     if(modifier.Operation == GEOperation.Minus)
            //         if (attributeCurrentValue - costValue < 0) return false;
            // }

            return true;
        }

        public static bool CheckCooldownReady(Entity ability)
        {
            // TODO
            return true;
        }

        public static void DoCost(Entity ability)
        {
            // TODO
        }

        public static bool TryActivateAbility(Entity ability)
        {
            // var result = CanActivateAbility(ability);
            // if (result != AbilityActivationResult.Success)
            //     return false;
            //
            // DoCost(ability);
            // _entityManager.AddComponentData(ability, new CAbilityActive());
            return true;
        }

        public static bool TryEndAbility(Entity ability)
        {
            // if(!_entityManager.HasComponent<CAbilityActive>(ability))
            //     return false;
            //
            // _entityManager.RemoveComponent<CAbilityActive>(ability);
            return true;
        }

        public static bool TryCancelAbility(Entity ability)
        {
            // if(!_entityManager.HasComponent<CAbilityActive>(ability))
            //     return false;
            //
            // _entityManager.RemoveComponent<CAbilityActive>(ability);
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