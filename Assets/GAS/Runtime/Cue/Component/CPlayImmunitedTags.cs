using Unity.Entities;

namespace GAS.Runtime
{
    public struct CPlayImmunitedTags : IComponentData
    {
        public TagRequirementData requirement;
    }
}
