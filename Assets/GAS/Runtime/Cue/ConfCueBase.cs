using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    public abstract class ConfCueBase: GameplayEffectComponentConfig
    {
        public GameplayCueConfig[] cues;

        public NativeArray<Entity> CreateCueEntityArray(Entity ge)
        {
            var entities = new Entity[cues.Length];
            for (var i = 0; i < cues.Length; i++)
            {
                entities[i] = GASManager.EntityManager.CreateEntity();
                var c = cues[i];
                // Cue是否可播放组件
                EntityHelper.AddComponent<ECCuePlayable>(entities[i]);
                EntityHelper.SetComponentEnabled<ECCuePlayable>(entities[i],false);
                
                // Cue是否播放中组件
                EntityHelper.AddComponent<ECCuePlaying>(entities[i]);
                EntityHelper.SetComponentEnabled<ECCuePlaying>(entities[i],false);
                
                // Cue是否死亡组件
                EntityHelper.AddComponent<ECKillCue>(entities[i]);
                EntityHelper.SetComponentEnabled<ECKillCue>(entities[i],false);
                
                // Cue逻辑
                EntityHelper.AddManagedComponent<MCCue>(entities[i]);
                var instantCue = CueHelper.InitInstantCueFromGameplayEffect(new MCCue(c.CreateCue()),entities[i],ge);
                EntityHelper.SetManagedComponent(entities[i], instantCue);
                
                // cue播放免疫tag
                if (c.ImmunityTags.Length > 0)
                {
                    EntityHelper.AddComponent<CPlayImmunitedTags>(entities[i]);
                    EntityHelper.SetComponent(entities[i], new CPlayImmunitedTags
                    {
                        tags = new NativeArray<int>(c.ImmunityTags, Allocator.Persistent)
                    });
                }

                // cue播放需求tag
                if (c.RequiredTags.Length > 0)
                {
                    EntityHelper.AddComponent<CPlayRequiredTags>(entities[i]);
                    EntityHelper.SetComponent(entities[i], new CPlayRequiredTags
                    {
                        tags = new NativeArray<int>(c.RequiredTags, Allocator.Persistent)
                    });
                }
            }

            return new NativeArray<Entity>(entities, Allocator.Persistent);
        }
    }
}