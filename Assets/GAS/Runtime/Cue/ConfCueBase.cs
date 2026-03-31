using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    public abstract class ConfCueBase: GameplayEffectComponentConfig
    {
        public GameplayCueConfig[] cues;

        public NativeArray<Entity> CreateCueEntityArray(Entity ge)
        {
            bool HasTags(int[] tags) => tags != null && tags.Length > 0;

            var entities = new Entity[cues.Length];
            for (var i = 0; i < cues.Length; i++)
            {
                entities[i] = GASManager.EntityManager.CreateEntity();
                var c = cues[i];
                EntityHelper.SetName(entities[i], $"Cue_{c.CueType.Name}_V{entities[i].Version}_{entities[i].Index}");
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
                if (HasTags(c.ImmunityAllTags) || HasTags(c.ImmunityAnyTags) || HasTags(c.ImmunityNoneTags))
                {
                    EntityHelper.AddComponent<CPlayImmunitedTags>(entities[i]);
                    EntityHelper.SetComponent(entities[i], new CPlayImmunitedTags
                    {
                        requirement = new TagRequirementData
                        {
                            all = new NativeArray<int>(c.ImmunityAllTags ?? System.Array.Empty<int>(), Allocator.Persistent),
                            any = new NativeArray<int>(c.ImmunityAnyTags ?? System.Array.Empty<int>(), Allocator.Persistent),
                            none = new NativeArray<int>(c.ImmunityNoneTags ?? System.Array.Empty<int>(), Allocator.Persistent)
                        }
                    });
                }

                // cue播放需求tag
                if (HasTags(c.RequiredAllTags) || HasTags(c.RequiredAnyTags) || HasTags(c.RequiredNoneTags))
                {
                    EntityHelper.AddComponent<CPlayRequiredTags>(entities[i]);
                    EntityHelper.SetComponent(entities[i], new CPlayRequiredTags
                    {
                        requirement = new TagRequirementData
                        {
                            all = new NativeArray<int>(c.RequiredAllTags ?? System.Array.Empty<int>(), Allocator.Persistent),
                            any = new NativeArray<int>(c.RequiredAnyTags ?? System.Array.Empty<int>(), Allocator.Persistent),
                            none = new NativeArray<int>(c.RequiredNoneTags ?? System.Array.Empty<int>(), Allocator.Persistent)
                        }
                    });
                }
            }

            return new NativeArray<Entity>(entities, Allocator.Persistent);
        }
    }
}
