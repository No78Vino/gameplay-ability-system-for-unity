using GAS.Runtime;
using GAS.RuntimeWithECS.Cue;
using Unity.Collections;
using Unity.Entities;
using NotImplementedException = System.NotImplementedException;

namespace GAS.RuntimeWithECS.GameplayEffect.Component
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
                entities[i] = GASManager.EntityManager.CreateEntity();
                GASManager.EntityManager.AddComponent<MCCue>(entities[i]);
                cues[i].SetSourceEntity(ge,CueSourceType.GameplayEffect);
                GASManager.EntityManager.SetComponentData(entities[i],new MCCue(cues[i]));
            }
            _entityManager.AddComponentData(ge, new CCueOnExecution
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
                ecb.AddComponent<MCCue>(entities[i]);
                cues[i].SetSourceEntity(ge,CueSourceType.GameplayEffect);
                ecb.SetComponent(entities[i],new MCCue(cues[i]));
            }
            ecb.AddComponent(ge, new CCueOnExecution
            {
                cues = new NativeArray<Entity>(entities, Allocator.Persistent)
            });
        }
    }
}