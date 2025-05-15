using GAS.Runtime;
using GAS.RuntimeWithECS.Cue;
using GAS.RuntimeWithECS.Cue.Component;
using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.GameplayEffect.Component
{
    public struct CCueOnAdd : IComponentData
    {
        /// <summary>
        ///     cue entity
        /// </summary>
        public NativeArray<Entity> cues;
    }

    public sealed class ConfCueOnAdd : GameplayEffectComponentConfig
    {
        public InstantCueSetting[] cues;

        public override void LoadToGameplayEffectEntity(Entity ge)
        {
            var entities = new Entity[cues.Length];
            for (var i = 0; i < cues.Length; i++)
            {
                entities[i] = GASManager.EntityManager.CreateEntity();
                var c = cues[i];
                // Cue是否播放组件
                GASManager.EntityManager.AddComponent<ECCuePlayable>(entities[i]);
                GASManager.EntityManager.SetComponentEnabled<ECCuePlayable>(entities[i],false);
                
                // Cue是否播放组件
                GASManager.EntityManager.AddComponent<ECCuePlaying>(entities[i]);
                GASManager.EntityManager.SetComponentEnabled<ECCuePlaying>(entities[i],false);
                
                // Cue逻辑
                GASManager.EntityManager.AddComponent<MCCue>(entities[i]);
                var instantCue = CueHelper.InitInstantCueFromGameplayEffect(new MCCue(c.cue.CreateCue()),entities[i],ge);
                GASManager.EntityManager.SetComponentData(entities[i], instantCue);
                
                // cue播放免疫tag
                if (c.immunityTags.Count > 0)
                {
                    GASManager.EntityManager.AddComponent<CPlayImmunitedTags>(entities[i]);
                    GASManager.EntityManager.SetComponentData(entities[i], new CPlayImmunitedTags
                    {
                        tags = new NativeArray<int>(c.immunityTags.ToArray(), Allocator.Persistent)
                    });
                }

                // cue播放需求tag
                if (c.requiredTags.Count > 0)
                {
                    GASManager.EntityManager.AddComponent<CPlayRequiredTags>(entities[i]);
                    GASManager.EntityManager.SetComponentData(entities[i], new CPlayRequiredTags
                    {
                        tags = new NativeArray<int>(c.requiredTags.ToArray(), Allocator.Persistent)
                    });
                }
            }

            _entityManager.AddComponentData(ge, new CCueOnAdd
            {
                cues = new NativeArray<Entity>(entities, Allocator.Persistent)
            });
        }

        public override void LoadToGameplayEffectEntity(Entity ge, EntityCommandBuffer ecb)
        {
            var entities = new Entity[cues.Length];
            for (var i = 0; i < cues.Length; i++)
            {
                entities[i] = GASManager.EntityManager.CreateEntity();
                var c = cues[i];
                ecb.AddComponent<MCCue>(entities[i]);
                
                // Cue是否播放组件
                ecb.AddComponent<ECCuePlayable>(entities[i]);
                ecb.SetComponentEnabled<ECCuePlayable>(entities[i],false);
                
                // Cue是否播放组件
                ecb.AddComponent<ECCuePlaying>(entities[i]);
                ecb.SetComponentEnabled<ECCuePlaying>(entities[i],false);
                
                // Cue逻辑
                ecb.AddComponent<MCCue>(entities[i]);
                var instantCue = CueHelper.InitInstantCueFromGameplayEffect(new MCCue(c.cue.CreateCue()),entities[i],ge);
                ecb.SetComponent(entities[i], instantCue);
                
                if (c.immunityTags.Count > 0)
                {
                    ecb.AddComponent<CPlayImmunitedTags>(entities[i]);
                    ecb.SetComponent(entities[i], new CPlayImmunitedTags
                    {
                        tags = new NativeArray<int>(c.immunityTags.ToArray(), Allocator.Persistent)
                    });
                }

                if (c.requiredTags.Count > 0)
                {
                    ecb.AddComponent<CPlayRequiredTags>(entities[i]);
                    ecb.SetComponent(entities[i], new CPlayRequiredTags
                    {
                        tags = new NativeArray<int>(c.requiredTags.ToArray(), Allocator.Persistent)
                    });
                }
            }

            ecb.AddComponent(ge, new CCueOnAdd
            {
                cues = new NativeArray<Entity>(entities, Allocator.Persistent)
            });
        }
    }
}