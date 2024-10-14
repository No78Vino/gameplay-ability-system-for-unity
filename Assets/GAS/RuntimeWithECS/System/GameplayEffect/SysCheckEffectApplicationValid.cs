using GAS.RuntimeWithECS.GameplayEffect.Component;
using GAS.RuntimeWithECS.Tag;
using GAS.RuntimeWithECS.Tag.Component;
using Unity.Burst;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.GameplayEffect
{
    public partial struct SysCheckEffectApplicationValid : ISystem
    {
        private EntityManager _gasEntityManager;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ComApplicationRequiredTags>();
            state.RequireForUpdate<ComInUsage>();
            state.RequireForUpdate<ComBasicInfo>();
            _gasEntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (vBasicInfo, requiredTags, _) in SystemAPI
                         .Query<RefRW<ComBasicInfo>, RefRO<ComApplicationRequiredTags>, RefRO<ComInUsage>>())
            {
                var asc = vBasicInfo.ValueRO.Target;
                var fixedTags = _gasEntityManager.GetBuffer<BuffElemFixedTag>(asc);
                var tempTags = _gasEntityManager.GetBuffer<BuffElemTemporaryTag>(asc);

                foreach (var tag in requiredTags.ValueRO.tags)
                {
                    var hasTag = false;
                    // 遍历固有Tag
                    foreach (var fixedTag in fixedTags)
                        if (GameplayTagHub.HasTag(fixedTag.tag, tag))
                        {
                            hasTag = true;
                            break;
                        }

                    // 遍历临时Tag
                    if (!hasTag)
                        foreach (var tempTag in tempTags)
                            if (GameplayTagHub.HasTag(tempTag.tag, tag))
                            {
                                hasTag = true;
                                break;
                            }

                    if (!hasTag)
                    {
                        vBasicInfo.ValueRW.Valid = false;
                        break;
                    }
                }
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}