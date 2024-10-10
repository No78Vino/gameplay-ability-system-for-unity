using Unity.Entities;

namespace GAS.RuntimeWithECS.Tag.Component
{
    public struct BuffElemTemporaryTag: IBufferElementData
    {
        public int tag;
        public Entity source;
        public GameplayTagSourceType sourceType;
    }
}