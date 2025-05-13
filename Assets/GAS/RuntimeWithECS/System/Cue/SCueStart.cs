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
            foreach (var (playable,playing, cue) in 
                     SystemAPI.Query<RefRO<ECCuePlayable>,RefRO<ECCuePlaying>>()
                         .WithEntityAccess())
            {
                bool isPlayable = SystemAPI.IsComponentEnabled<ECCuePlayable>(cue);
                bool isPlaying = SystemAPI.IsComponentEnabled<ECCuePlaying>(cue);
                if (isPlayable && !isPlaying)
                {
                    SystemAPI.SetComponentEnabled<ECCuePlaying>(cue,true);
                    
                    // Instant cue
                    if (state.EntityManager.HasComponent<MCInstantCue>(cue))
                    {
                        var instantCue = state.EntityManager.GetComponentData<MCInstantCue>(cue);
                        instantCue.cue.TryTrigger();
                    }
                    // TODO Durational cue
                    // TODO 触发Cue开始播放事件
                }
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}