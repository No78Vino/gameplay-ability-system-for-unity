using Unity.Entities;

namespace GAS.Runtime
{
    public struct BTemporaryTag: IBufferElementData
    {
        public int tag;
        public Entity source;
    }
}