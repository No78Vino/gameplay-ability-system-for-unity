using GAS.Runtime;
using GAS.RuntimeWithECS.Cue;
using GAS.RuntimeWithECS.Cue.Component;
using Unity.Collections;
using Unity.Entities;
using NotImplementedException = System.NotImplementedException;

namespace GAS.RuntimeWithECS.GameplayEffect.Component
{
    public struct CCueOnAdd : IComponentData
    {
        public NativeArray<Entity> cues;
    }
    
    public sealed class ConfCueOnAdd:GameplayEffectComponentConfig
    {
        public CueInstant[] cues;
        
        public override void LoadToGameplayEffectEntity(Entity ge)
        {
            Entity[] entities = new Entity[cues.Length];
            for (int i = 0; i < cues.Length; i++)
            {
                entities[i] = GASManager.EntityManager.CreateEntity();
                GASManager.EntityManager.AddComponent<ComInstantCue>(entities[i]);
                GASManager.EntityManager.SetComponentData(entities[i],new ComInstantCue(cues[i]));
            }
            _entityManager.AddComponentData(ge, new CCueOnAdd
            {
                cues = new NativeArray<Entity>(entities, Allocator.Persistent)
            });
        }

        public override void LoadToGameplayEffectEntity(Entity ge, EntityCommandBuffer ecb)
        {
            Entity[] entities = new Entity[cues.Length];
            for (int i = 0; i < cues.Length; i++)
            {
                entities[i] = GASManager.EntityManager.CreateEntity();
                ecb.AddComponent<ComInstantCue>(entities[i]);
                ecb.SetComponent(entities[i],new ComInstantCue(cues[i]));
            }
            ecb.AddComponent(ge, new CCueOnAdd
            {
                cues = new NativeArray<Entity>(entities, Allocator.Persistent)
            });
        }
    }
}