using Unity.Entities;

namespace GAS.Runtime
{
    public struct CAbilityCost : IComponentData
    {
        public Entity ProtoGameplayEffectCost;
    }
    
    public sealed class ConfAbilityCost:AbilityComponentConfig
    {
        public GameplayEffectComponentConfig[] CostComponentConfigs;
        
        public override void LoadToGameplayAbilityEntity(Entity ability)
        {
            _entityManager.AddComponentData(ability, new CAbilityCost
            {
                ProtoGameplayEffectCost = GameplayEffectHelper.CreateGameplayEffectEntity(CostComponentConfigs),
            });
        }
    }
}