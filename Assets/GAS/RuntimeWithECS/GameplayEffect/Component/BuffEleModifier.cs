using GAS.Runtime;
using GAS.RuntimeWithECS.Modifier;
using Unity.Entities;

namespace GAS.RuntimeWithECS.GameplayEffect.Component
{
    public struct BuffEleModifier : IBufferElementData
    {
        public int AttrSetCode;
        public int AttrCode;
        public GEOperation Operation;
        public float Magnitude;
        public MMCSetting MMC;
    }
}