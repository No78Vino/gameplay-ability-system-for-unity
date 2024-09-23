using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.AttributeSet.Component
{
    public struct AttrSetContainerComponent : IComponentData
    {
        public DynamicBuffer<AttributeSetComponent> attributeSets;
    }
}