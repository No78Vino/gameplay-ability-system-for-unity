using Unity.Entities;

namespace GAS.RuntimeWithECS.Ability.Component.Static
{
    public struct CAbilityCooldown : IComponentData
    {
        public float Cooldown;
        
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
}