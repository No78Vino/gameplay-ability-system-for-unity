using GAS.RuntimeWithECS.Ability.Component.Dynamic;
using GAS.RuntimeWithECS.Core;
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
    ///  Gameplay Ability Utility
    ///  游戏能力工具类,对应原本的AbilitySpec
    ///  原本Mono版本里的所有ability自身的功能性方法全部转为静态方法
    ///  Entity + Util方式代替Mono + AbilitySpec方式
    /// </summary>
    public static class GAUtil
    {
        private static EntityManager _entityManager => GASManager.EntityManager;
        
        /// <summary>
        ///  检查能力是否可以激活
        /// </summary>
        /// <param name="ability"></param>
        /// <returns>   </returns>
        public static AbilityActivationResult CanActivateAbility(Entity ability)
        {
            if(_entityManager.HasComponent<CAbilityActive>(ability))
                return AbilityActivationResult.FailHasActivated;
            
            if(!CheckGameplayTagsValidTpActivate(ability))
                return AbilityActivationResult.FailTagRequirement;
            
            if (!CheckCost(ability)) 
                return AbilityActivationResult.FailCost;
            
            if (!CheckCooldownReady(ability)) 
                return AbilityActivationResult.FailCooldown;
            
            return AbilityActivationResult.Success;
        }
        
        public static bool CheckGameplayTagsValidTpActivate(Entity ability)
        {
            // TODO
            // var hasAllTags = Owner.HasAllTags(Ability.Tag.ActivationRequiredTags);
            // var notHasAnyTags = !Owner.HasAnyTags(Ability.Tag.ActivationBlockedTags);
            // var notBlockedByOtherAbility = true;
            //
            // foreach (var kv in Owner.AbilityContainer.AbilitySpecs())
            // {
            //     var abilitySpec = kv.Value;
            //     if (abilitySpec.IsActive)
            //         if (Ability.Tag.AssetTag.HasAnyTags(abilitySpec.Ability.Tag.BlockAbilitiesWithTags))
            //         {
            //             notBlockedByOtherAbility = false;
            //             break;
            //         }
            // }
            //
            // return hasAllTags && notHasAnyTags && notBlockedByOtherAbility;
            return true;
        }
        
        public static bool CheckCost(Entity ability)
        {
            // TODO
            return true;
        }
        
        public static bool CheckCooldownReady(Entity ability)
        {
            // TODO
            return true;
        }
    }
}