using GAS.Runtime;
using GAS.RuntimeWithECS.Cue;
using GAS.RuntimeWithECS.GameplayEffect;
using Unity.Collections;
using Unity.Entities;
using NotImplementedException = System.NotImplementedException;

namespace GAS.Runtime
{
    public struct CCueOnExecution : IComponentData
    {
        public NativeArray<Entity> cues;
    }
    
    public sealed class ConfCueOnExecution:ConfCueBase
    {
        public override void LoadToGameplayEffectEntity(Entity ge)
        {
            var entities = CreateCueEntityArray(ge);
            EntityHelper.AddComponent<CCueOnExecution>(ge);
            EntityHelper.SetComponent(ge, new CCueOnExecution 
            {
                cues = entities
            });
        }
    }
}