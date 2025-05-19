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
    
    public sealed class ConfCueOnExecution:GameplayEffectComponentConfig
    {
        public GameplayCueBase[] cues;
        
        public override void LoadToGameplayEffectEntity(Entity ge)
        {
            Entity[] entities = new Entity[cues.Length];
            for (int i = 0; i < cues.Length; i++)
            {
                entities[i] = EntityHelper.CreateEntity();
                EntityHelper.AddManagedComponent<MCCue>(entities[i]);
                cues[i].SetSourceEntity(ge,CueSourceType.GameplayEffect);
                EntityHelper.SetManagedComponent(entities[i],new MCCue(cues[i]));
            }
            EntityHelper.AddComponent<CCueOnExecution>(ge);
            EntityHelper.SetComponent(ge, new CCueOnExecution
            {
                cues = new NativeArray<Entity>(entities, Allocator.Persistent)
            });
        }
    }
}