using GAS.RuntimeWithECS.GameplayEffect.Component;
using Unity.Burst;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.GameplayEffect
{
    public partial struct SysRemoveInvalidEffect : ISystem
    {
        private EntityManager _gasEntityManager;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ComInUsage>();
            state.RequireForUpdate<ComBasicInfo>();
            _gasEntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (vBasicInfo, _) in SystemAPI
                         .Query<RefRW<ComBasicInfo>,  RefRO<ComInUsage>>())
            {
                bool isValid = vBasicInfo.ValueRO.Valid;
                if (!isValid)
                {
                    // TODO 移除不合法GE
                }
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}