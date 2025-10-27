using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    [UpdateInGroup(typeof(SGDurationalEffect))]
    public partial struct SAddEffectToAscBuffList : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CEffectInstance>();
            state.RequireForUpdate<CEffectInUsage>();
            state.RequireForUpdate<WipApplyEffect>();
            state.RequireForUpdate<CDuration>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            foreach (var (_,_, _,inUsage, ge) in SystemAPI
                         .Query<
                             RefRO<CEffectInstance>, 
                             RefRO<WipApplyEffect>, 
                             RefRO<CDuration>, 
                             RefRO<CEffectInUsage>>()
                         .WithEntityAccess())
            {
                if (!SystemAPI.HasComponent<CStacking>(ge))
                {
                    // 处理没有堆叠组件的GameplayEffect
                    AddToAscBuffList(state.EntityManager,ge, inUsage.ValueRO.Target);
                }
                else
                {
                    // 处理有堆叠组件的GameplayEffect
                    var stacking = SystemAPI.GetComponent<CStacking>(ge);
                    var stackGe = stacking.StackType switch
                    {
                        EffectStackType.AggregateBySource => 
                            GameplayEffectHelper.GetStackingEffectBySource(stacking.StackingCode,
                            inUsage.ValueRO.Target, inUsage.ValueRO.Source, state.EntityManager),
                        EffectStackType.AggregateByTarget => 
                            GameplayEffectHelper.GetStackingEffectByTarget(stacking.StackingCode,
                            inUsage.ValueRO.Source, state.EntityManager),
                        _ => Entity.Null
                    };

                    if (stackGe == Entity.Null) AddToAscBuffList(state.EntityManager,ge, inUsage.ValueRO.Target);
                }
            }
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }

        [BurstCompile]
        private void AddToAscBuffList(EntityManager entityManager, Entity ge, Entity asc)
        {
            var geBuff = entityManager.GetBuffer<BGameplayEffect>(asc);
            var alreadyExist = false;
            foreach (var geElem in geBuff)
                if (geElem.GameplayEffect == ge)
                {
                    alreadyExist = true;
                    break;
                }

            if (!alreadyExist) geBuff.Add(new BGameplayEffect { GameplayEffect = ge });
        }
    }
}