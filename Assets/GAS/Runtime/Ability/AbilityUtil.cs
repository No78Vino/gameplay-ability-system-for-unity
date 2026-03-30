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
    public static class AbilityUtil
    {
        private static EntityManager _em => GASManager.EntityManager;

        private static bool EvaluateAscTagRequirement(Entity asc, in TagRequirementData requirement)
        {
            bool passAll = !requirement.all.IsCreated || requirement.all.Length == 0 || ASCHelper.HasAllTags(asc, requirement.all);
            bool passAny = !requirement.any.IsCreated || requirement.any.Length == 0 || ASCHelper.HasAnyTags(asc, requirement.any);
            bool passNone = !requirement.none.IsCreated || requirement.none.Length == 0 || !ASCHelper.HasAnyTags(asc, requirement.none);
            return passAll && passAny && passNone;
        }

        /// <summary>
        ///     检查能力是否可以激活
        /// </summary>
        /// <param name="ability"></param>
        /// <returns>   </returns>
        public static AbilityActivationResult CanActivateAbility(Entity ability)
        {
            if (_em.HasComponent<CAbilityActive>(ability))
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
            var owner = _em.GetComponentData<CAbilityBaseInfo>(ability).Owner;
            // 1. 检查激活所需标签
            var hasAllTags = true;
            var abilityHasActivationRequiredTags = _em.HasComponent<CAbilityActivationRequiredTags>(ability);
            if (abilityHasActivationRequiredTags)
            {
                var abilityActivationRequiredTags =
                    _em.GetComponentData<CAbilityActivationRequiredTags>(ability);
                hasAllTags = EvaluateAscTagRequirement(owner, abilityActivationRequiredTags.requirement);
            }

            // 2. 检查激活被阻止的标签
            var notHasAnyTags = true;
            var abilityHasActivationBlockedTags = _em.HasComponent<CAbilityActivationBlockedTags>(ability);
            if (abilityHasActivationBlockedTags)
            {
                var abilityActivationBlockedTags =
                    _em.GetComponentData<CAbilityActivationBlockedTags>(ability);
                notHasAnyTags = EvaluateAscTagRequirement(owner, abilityActivationBlockedTags.requirement);
            }

            // 3. 检查是否被其他能力阻止,遍历宿主其它能力,检查是否有阻止激活的标签
            var notBlockedByOtherAbility = true;
            var ownerAbilities = _em.GetBuffer<BAbility>(owner);
            foreach (var ownerAbility in ownerAbilities)
            {
                var ownerAbilityEntity = ownerAbility.Ability;
                if (ownerAbilityEntity == ability) continue;
                if (!_em.HasComponent<CAbilityActive>(ownerAbilityEntity)) continue;
                
                var ownerAbilityHasBlockAbilitiesWithTags =
                    _em.HasComponent<CBlockAbilityWithTags>(ownerAbilityEntity);
                if (!ownerAbilityHasBlockAbilitiesWithTags) continue;
                
                var ownerAbilityBlockAbilitiesWithTags =
                    _em.GetComponentData<CBlockAbilityWithTags>(ownerAbilityEntity);
                if (!HasAnyTags(ability, ownerAbilityBlockAbilitiesWithTags.tags)) continue;
                
                notBlockedByOtherAbility = false;
                break;
            }
            return hasAllTags && notHasAnyTags && notBlockedByOtherAbility;
        }

        public static bool CheckCost(Entity ability)
        {
            bool hasCostComponent = _em.HasComponent<CAbilityCost>(ability);
            if (!hasCostComponent) return true;
            
            var costComponent = _em.GetComponentData<CAbilityCost>(ability);
            bool isInstantEffect = !_em.HasComponent<CDuration>(costComponent.ProtoGameplayEffectCost);
            if (!isInstantEffect) return true;
            
            var mcModifiers = _em.GetComponentData<MCModifiers>(costComponent.ProtoGameplayEffectCost);
            var owner = _em.GetComponentData<CAbilityBaseInfo>(ability).Owner;
            var attrSets = _em.GetBuffer<BEAttrSet>(owner);
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
                var resultValue = MmcHelper.Calculate(costComponent.ProtoGameplayEffectCost, modifier, attr.CurrentValue, owner, owner);
                if (resultValue < 0) return false;  // 任一资源不足，立即失败  
            }

            return true;
        }

        public static bool CheckCooldownReady(Entity ability)  
        {  
            bool hasCooldownComponent = _em.HasComponent<CAbilityCooldown>(ability);  
            if (!hasCooldownComponent) return true;  
  
            CAbilityCooldown cooldown = _em.GetComponentData<CAbilityCooldown>(ability);  
            // 没有配置 CooldownTags，则始终就绪  
            if (!cooldown.CooldownTags.IsCreated || cooldown.CooldownTags.Length == 0) return true;  
  
            var owner = _em.GetComponentData<CAbilityBaseInfo>(ability).Owner;  
            // ASC 上有任意一个 CooldownTag → 冷却中 → 不可激活  
            return !ASCHelper.HasAnyTags(owner, cooldown.CooldownTags);  
        }

        public static void DoCost(Entity ability)
        {
            if (!_em.HasComponent<CAbilityCost>(ability)) return;
            
            var costComponent = _em.GetComponentData<CAbilityCost>(ability);
            var owner = _em.GetComponentData<CAbilityBaseInfo>(ability).Owner;
            GameplayEffectHelper.ApplyGameplayEffectImmediate(costComponent.ProtoGameplayEffectCost, owner, owner);
        }

        /// <summary>  
        ///     执行冷却：从原型克隆CD GE实例，覆写Duration，通过ECS管线应用到Owner  
        /// </summary>  
        public static void DoCooldown(Entity ability)  
        {  
            if (!_em.HasComponent<CAbilityCooldown>(ability)) return;  
  
            var cooldown = _em.GetComponentData<CAbilityCooldown>(ability);  
            var owner = _em.GetComponentData<CAbilityBaseInfo>(ability).Owner;  
  
            // 1. 从原型克隆GE实例  
            var instanceGe = _em.Instantiate(cooldown.ProtoGameplayEffectCooldown);  
  
            // 2. 用Ability配置的Cooldown值覆写GE的Duration  
            if (cooldown.Cooldown > 0 && _em.HasComponent<CDuration>(instanceGe))  
            {  
                var duration = _em.GetComponentData<CDuration>(instanceGe);  
                duration.duration = cooldown.Cooldown;  
                _em.SetComponentData(instanceGe, duration);  
            }  
  
            // 3. 通过ECS GE管线应用到Owner（走完整的 Instantiate→CheckApply→Apply→Activate 流程）  
            GameplayEffectHelper.ApplyGameplayEffectTo(instanceGe, owner, owner);  
        }
        
        /// <summary>
        ///     检查是否有指定标签，能力的tag校验只校验AssetTag
        /// </summary>
        /// <param name="ability"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        public static bool HasAnyTags(Entity ability, NativeArray<int> tags)
        {
            var hasAssetTag = _em.HasComponent<CAbilityAssetTags>(ability);
            if (!hasAssetTag) return false;

            var assetTags = _em.GetComponentData<CAbilityAssetTags>(ability);
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
            if (!_em.HasComponent<CCancelAbilityWithTags>(ability)) return;  
  
            var cancelTags = _em.GetComponentData<CCancelAbilityWithTags>(ability);  
            var owner = _em.GetComponentData<CAbilityBaseInfo>(ability).Owner;  
            var ownerAbilities = _em.GetBuffer<BAbility>(owner);  
  
            foreach (var ownerAbility in ownerAbilities)  
            {  
                var otherAbility = ownerAbility.Ability;  
                if (otherAbility == ability) continue;  
  
                // 只取消已激活的能力  
                if (!_em.HasComponent<CAbilityActive>(otherAbility)) continue;  
  
                // 已经在取消流程中的跳过  
                if (_em.HasComponent<CAbilityInTryCancel>(otherAbility)) continue;  
  
                // 检查其他能力的 AssetTags 是否匹配 CancelAbilityTags 中的任意一个  
                if (HasAnyTags(otherAbility, cancelTags.tags))  
                {  
                    EntityHelper.AddComponent<CAbilityInTryCancel>(otherAbility);  
                }  
            }  
        }
    }
}
