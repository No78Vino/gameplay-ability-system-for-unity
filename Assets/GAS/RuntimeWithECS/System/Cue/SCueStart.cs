using GAS.RuntimeWithECS.Cue.Component;
using Unity.Burst;
using Unity.Entities;

namespace GAS.Runtime
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SysGroupDisplay))]
    public partial struct SCueStart : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ECCuePlayable>();
            state.RequireForUpdate<ECCuePlaying>();
        }

        //[BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (_, cue) in
                     SystemAPI.Query<RefRO<ECCuePlayable>>()
                         .WithDisabled<ECCuePlaying>()
                         .WithEntityAccess())
            {
                SystemAPI.SetComponentEnabled<ECCuePlaying>(cue, true);
                
                var mcCue = state.EntityManager.GetComponentData<MCCue>(cue);
                mcCue.cue.TryTrigger();
                // TODO 触发Cue开始播放事件
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}