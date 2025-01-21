using Unity.Entities;

namespace GAS.RuntimeWithECS.Ability.Component.Static
{
    public struct CAbilityCooldown : IComponentData
    {
        public float Cooldown;
        public Entity CooldownGameplayEffect;
    }
}