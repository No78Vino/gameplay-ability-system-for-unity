using GAS.RuntimeWithECS.Attribute.Component;
using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.AttributeSet.Component
{
    public struct AttributeSetComponent : IComponentData
    {
        public int CodeValue;
    }
}