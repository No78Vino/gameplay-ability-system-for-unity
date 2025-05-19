using GAS.Runtime;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace GAS.Runtime
{
    [DisableAutoCreation]
    public partial struct STestSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        //[BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (inUsage,e) in SystemAPI.Query<RefRW<CDuration>>().WithEntityAccess())
            {
                inUsage.ValueRW.duration -= 1;
                Debug.Log($"{e.ToString()} : duration:{inUsage.ValueRW.duration}");
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}