using GAS.RuntimeWithECS.Cue.Component;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using GAS.RuntimeWithECS.System.SystemGroup;
using Unity.Burst;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.GameplayEffect
{
    [UpdateInGroup(typeof(SysGroupInstantEffect))]
    [UpdateAfter(typeof(SInstantEffectModifyBaseValue))]
    public partial struct STriggerCueOnExecution : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CInApplicationProgress>();
            state.RequireForUpdate<CEffectApplied>();
            state.RequireForUpdate<CEffectInUsage>();
            state.RequireForUpdate<CCueOnExecution>();
        }

        //[BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // foreach (var aspect in SystemAPI.Query<AspCueOnExecution>())
            // {
            //     aspect.Trigger(state.EntityManager);
            // }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }

    public readonly partial struct AspCueOnExecution : IAspect
    {
        public readonly Entity self;
        private readonly RefRO<CEffectInUsage> _inUsage;
        private readonly RefRO<CEffectApplied> _comValidEffect;
        private readonly RefRO<CInApplicationProgress> _inApplicationProgress;
        private readonly RefRO<CCueOnExecution> _cueOnExecution;

        public void Trigger(EntityManager entityManager)
        {
            foreach (var entity in _cueOnExecution.ValueRO.cues)
            {
                var cue = entityManager.GetComponentData<ComInstantCue>(entity);
                cue.cue.TryTrigger();
            }
        }
    }
}