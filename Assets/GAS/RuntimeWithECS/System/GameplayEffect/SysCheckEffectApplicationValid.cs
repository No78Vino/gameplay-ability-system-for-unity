using GAS.RuntimeWithECS.Core;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using GAS.RuntimeWithECS.Tag.Component;
using Unity.Burst;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.GameplayEffect
{
    public partial struct SysCheckEffectApplicationValid : ISystem
    {
        private EntityManager GASEntityManager;
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ComApplicationRequiredTags>();
            state.RequireForUpdate<ComInUsage>();
            state.RequireForUpdate<ComBasicInfo>();
            GASEntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (vBasicInfo, requiredTags, _) in SystemAPI
                         .Query<RefRW<ComBasicInfo>, RefRO<ComApplicationRequiredTags>, RefRO<ComInUsage>>())
            {
                var asc = vBasicInfo.ValueRO.Target;
                var fixedTags = GASEntityManager.GetBuffer<BuffElemFixedTag>(asc);
                
                foreach (var tag in requiredTags.ValueRO.tags)
                {
                    
                }
                vBasicInfo.ValueRW.Valid = true;
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
        
    //     : SystemBase
    // {
    //     protected override void OnCreate()
    //     {
    //         base.OnCreate();
    //         RequireForUpdate<ComApplicationRequiredTags>();
    //         RequireForUpdate<ComInUsage>();
    //         RequireForUpdate<ComBasicInfo>();
    //     }
    //
    //     protected override void OnUpdate()
    //     {
    //         
    //     }
    // }
}