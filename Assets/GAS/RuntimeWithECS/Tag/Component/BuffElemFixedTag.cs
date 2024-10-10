using Unity.Entities;

namespace GAS.RuntimeWithECS.Tag.Component
{
    public struct BuffElemFixedTag : IBufferElementData
    {
        public int tag;
    }
}