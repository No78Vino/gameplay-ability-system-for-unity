using Unity.Entities;

namespace GAS.RuntimeWithECS.Tag.Component
{
    public struct BTemporaryTag: IBufferElementData
    {
        public int tag;
        public Entity source;
    }
}