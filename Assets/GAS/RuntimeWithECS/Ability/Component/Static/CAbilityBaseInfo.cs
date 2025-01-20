using Unity.Entities;

namespace GAS.RuntimeWithECS.Ability.Component.Static
{
    public struct CAbilityBaseInfo : IComponentData
    {
        public int Code;
        public int Level;
        public Entity Owner;
    }
}