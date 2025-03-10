using GAS.RuntimeWithECS.Ability.ComponentConfig;
using GAS.RuntimeWithECS.GameplayEffect;
using Unity.Entities;

namespace GAS.RuntimeWithECS.Ability.Component.Static
{
    public struct CAbilityCost : IComponentData
    {
        public Entity ProtoGameplayEffectCost;
    }
    
    public sealed class ConfAbilityCost:GameplayAbilityComponentConfig
    {
        public GameplayEffectComponentConfig[] CostComponentConfigs;
        
        public override void LoadToGameplayAbilityEntity(Entity ability)
        {
            _entityManager.AddComponentData(ability, new CAbilityCost
            {
                ProtoGameplayEffectCost = GEUtil.CreateGameplayEffectEntity(CostComponentConfigs),
            });
        }
    }
}