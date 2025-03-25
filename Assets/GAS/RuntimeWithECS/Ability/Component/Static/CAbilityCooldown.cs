using GAS.RuntimeWithECS.Ability.ComponentConfig;
using GAS.RuntimeWithECS.GameplayEffect;
using Unity.Entities;

namespace GAS.RuntimeWithECS.Ability.Component.Static
{
    public struct CAbilityCooldown : IComponentData
    {
        public int Cooldown;
        
        /// <summary>
        ///  冷却GE的原型
        /// </summary>
        public Entity ProtoGameplayEffectCooldown;
        
        // -------------------------------------以下是RUNTIME数据，不需要初始化---------------------------------------//
        
        /// <summary>
        ///  冷却GE的实例
        /// </summary>
        public Entity CooldownGameplayEffectInstance;
    }
    
    public sealed class ConfAbilityCooldown:GameplayAbilityComponentConfig
    {
        public int Cooldown;
        public GameplayEffectComponentConfig[] CooldownComponentConfigs;
        
        public override void LoadToGameplayAbilityEntity(Entity ability)
        {
            
            _entityManager.AddComponentData(ability, new CAbilityCooldown
            {
                Cooldown = Cooldown,
                ProtoGameplayEffectCooldown = GEUtil.CreateGameplayEffectEntity(CooldownComponentConfigs),
            });
        }
    }
}