using GAS.RuntimeWithECS.Core;
using Unity.Entities;

namespace GAS.RuntimeWithECS.GameplayEffect.Component
{
    public struct ComDuration : IComponentData
    {
        public int duration;
        public TimeUnit timeUnit;
    }
}