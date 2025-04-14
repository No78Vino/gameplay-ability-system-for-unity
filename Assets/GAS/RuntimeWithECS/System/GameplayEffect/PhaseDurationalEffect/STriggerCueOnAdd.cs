using GAS.RuntimeWithECS.Cue.Component;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using GAS.RuntimeWithECS.System.SystemGroup;
using Unity.Burst;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.GameplayEffect.PhaseDurationalEffect
{
    [UpdateInGroup(typeof(SysGroupDurationalEffect))]
    [UpdateAfter(typeof(SInitDuartionalEffect))]
    public partial struct STriggerCueOnAdd : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CInUsage>();
            state.RequireForUpdate<CInApplicationProgress>();
            state.RequireForUpdate<CValidEffect>();
            state.RequireForUpdate<CCueOnAdd>();
        }

        //[BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var aspect in SystemAPI.Query<AspCueOnAdd>())
            {
                aspect.Trigger(state.EntityManager);
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
    
    public readonly partial struct AspCueOnAdd : IAspect
    {
        public readonly Entity self;
        private readonly RefRO<CInUsage> _inUsage;
        private readonly RefRO<CValidEffect> _comValidEffect;
        private readonly RefRO<CInApplicationProgress> _inApplicationProgress;
        private readonly RefRO<CCueOnAdd> _cueOnAdd;

        public void Trigger(EntityManager entityManager)
        {
            foreach (var entity in _cueOnAdd.ValueRO.cues)
            {
                var cue = entityManager.GetComponentData<ComInstantCue>(entity);
                cue.cue.TryTrigger();
            }
        }
    }
}