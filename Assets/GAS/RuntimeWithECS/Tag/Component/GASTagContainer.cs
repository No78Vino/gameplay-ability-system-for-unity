using Unity.Entities;

namespace GAS.RuntimeWithECS.Tag.Component
{
    public struct GASTagContainer : IComponentData
    {
        public DynamicBuffer<int> FixedTags;
        public DynamicBuffer<int> DynamicTags;
    }
}