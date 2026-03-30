using Unity.Entities;

namespace GAS.Runtime
{
    public struct CPlayRequiredTags : IComponentData
    {
        public TagRequirementData requirement;
    }
}
