using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
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
                hasAllTags = ASCHelper.HasAllTags(owner, abilityActivationRequiredTags.tags);
            }

            // 2. 检查激活被阻止的标签
            var notHasAnyTags = true;
            var abilityHasActivationBlockedTags = _entityManager.HasComponent<CAbilityActivationBlockedTags>(ability);
            if (abilityHasActivationBlockedTags)
            {
                var abilityActivationBlockedTags =
                    _entityManager.GetComponentData<CAbilityActivationBlockedTags>(ability);
                notHasAnyTags = !ASCHelper.HasAnyTags(owner, abilityActivationBlockedTags.tags);
            }

            // 3. 检查是否被其他能力阻止,遍历宿主其它能力,检查是否有阻止激活的标签
            var notBlockedByOtherAbility = true;
            var ownerAbilities = _entityManager.GetBuffer<BAbility>(owner);
            foreach (var ownerAbility in ownerAbilities)
            {
                var ownerAbilityEntity = ownerAbility.Ability;
                if (ownerAbilityEntity == ability) continue;
                if (!_entityManager.HasComponent<CAbilityActive>(ownerAbilityEntity)) continue;
                
                var ownerAbilityHasBlockAbilitiesWithTags =
                    _entityManager.HasComponent<CBlockAbilityWithTags>(ownerAbilityEntity);
                if (!ownerAbilityHasBlockAbilitiesWithTags) continue;
                
                var ownerAbilityBlockAbilitiesWithTags =
                    _entityManager.GetComponentData<CBlockAbilityWithTags>(ownerAbilityEntity);
                if (!HasAnyTags(ability, ownerAbilityBlockAbilitiesWithTags.tags)) continue;
                
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
            
            var mcModifiers = _entityManager.GetComponentData<MCModifiers>(costComponent.ProtoGameplayEffectCost);
            var owner = _entityManager.GetComponentData<CAbilityBaseInfo>(ability).Owner;
            var attrSets = _entityManager.GetBuffer<BEAttrSet>(owner);
            foreach (var modifier in mcModifiers.Modifiers)
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
                var resultValue = MmcHelper.Calculate(costComponent.ProtoGameplayEffectCost, modifier, attr.CurrentValue);
                return resultValue >= 0;
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
            EffectUtil.ApplyGameplayEffectImmediate(costComponent.ProtoGameplayEffectCost, owner, owner);
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
            if (!hasAssetTag) return false;

            var assetTags = _entityManager.GetComponentData<CAbilityAssetTags>(ability);
            foreach (var tag in tags)
            foreach (var assetTag in assetTags.tags)
                if (TagHelper.HasTag(assetTag, tag))
                    return true;
            return false;
        }
        
        /// <summary>  
        ///     激活能力时，根据 CancelAbilityWithTags 取消 Owner 上匹配的已激活能力  
        /// </summary>  
        public static void CancelAbilitiesWithTags(Entity ability)  
        {  
            if (!_entityManager.HasComponent<CCancelAbilityWithTags>(ability)) return;  
  
            var cancelTags = _entityManager.GetComponentData<CCancelAbilityWithTags>(ability);  
            var owner = _entityManager.GetComponentData<CAbilityBaseInfo>(ability).Owner;  
            var ownerAbilities = _entityManager.GetBuffer<BAbility>(owner);  
  
            foreach (var ownerAbility in ownerAbilities)  
            {  
                var otherAbility = ownerAbility.Ability;  
                if (otherAbility == ability) continue;  
  
                // 只取消已激活的能力  
                if (!_entityManager.HasComponent<CAbilityActive>(otherAbility)) continue;  
  
                // 已经在取消流程中的跳过  
                if (_entityManager.HasComponent<CAbilityInTryCancel>(otherAbility)) continue;  
  
                // 检查其他能力的 AssetTags 是否匹配 CancelAbilityTags 中的任意一个  
                if (HasAnyTags(otherAbility, cancelTags.tags))  
                {  
                    EntityHelper.AddComponent<CAbilityInTryCancel>(otherAbility);  
                }  
            }  
        }
    }
}