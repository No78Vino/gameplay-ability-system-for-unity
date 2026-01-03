using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    [UpdateInGroup(typeof(SGRemoveEffect))]
    [UpdateBefore(typeof(SEffectRemoveEnd))]
    public partial struct SRemoveEffectFromAscBuffList : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WipRemoveEffect>();
            state.RequireForUpdate<CEffectInstance>();
            state.RequireForUpdate<CEffectInUsage>();
            state.RequireForUpdate<CDuration>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (_, _, _, inUsage, ge) in SystemAPI
                         .Query<
                             RefRO<CEffectInstance>,
                             RefRO<WipRemoveEffect>,
                             RefRO<CDuration>,
                             RefRO<CEffectInUsage>>()
                         .WithEntityAccess())
            {
                var asc = inUsage.ValueRO.Target;
                var geBuff = SystemAPI.GetBuffer<BGameplayEffect>(asc);
                // 从geBuff中移除对应的GameplayEffect
                for (var i = geBuff.Length - 1; i >= 0; i--)
                {
                    if (geBuff[i].GameplayEffect != ge) continue;
                    // 触发属性重计算
                    //CheckEffectAttrDirty(state.EntityManager, ecb, asc, ge);
                    geBuff.RemoveAt(i);
                    break;
                }
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        private void CheckEffectAttrDirty(EntityManager entityManager, EntityCommandBuffer ecb, Entity asc, Entity ge)
        {
            if (!entityManager.HasComponent<MCModifiers>(ge)) return;
            
            var modifiers = entityManager.GetComponentData<MCModifiers>(ge);
            if (modifiers.Modifiers.Length == 0) return;
            
            var attrSets = entityManager.GetBuffer<BEAttrSet>(asc);
            foreach (var modifier in modifiers.Modifiers)
            {
                var attrSetIndex = attrSets.IndexOfAttrSetCode(modifier.AttrSetCode);
                if (attrSetIndex == -1) continue;

                var attrSet = attrSets[attrSetIndex];
                var attributes = attrSet.Attributes;

                var attrIndex = attributes.IndexOfAttrCode(modifier.AttrCode);
                if (attrIndex == -1) continue;

                var data = attributes[attrIndex];
                // 标记Dirty
                data.Dirty = true;
                attrSet.Attributes[attrIndex] = data;
                attrSets[attrSetIndex] = attrSet;
            }
            
            ecb.AddComponent<CAttributeIsDirty>(asc);
        }
    }
}