using Unity.Burst;
using Unity.Entities;

namespace GAS.Runtime
{
    [UpdateInGroup(typeof(SGDeactivateEffect))]
    [UpdateBefore(typeof(SDeactivateEnd))]
    public partial struct SRemoveModifiers : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WipDeactivateEffect>();
            state.RequireForUpdate<CEffectInstance>();
            state.RequireForUpdate<MCModifiers>();
            state.RequireForUpdate<CEffectInUsage>();
        }

        //[BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var (entity, _, inUsage, modifiersComp, ge) in
                     SystemAPI.Query<
                         RefRO<CEffectInstance>,
                         RefRO<WipDeactivateEffect>,
                         RefRO<CEffectInUsage>,
                         MCModifiers>().WithEntityAccess())
            {
                var modifiers = modifiersComp.Modifiers;
                var targetAsc = inUsage.ValueRO.Target;
                var attrSets = SystemAPI.GetBuffer<BEAttrSet>(targetAsc);
                bool isAttrDirty = false;
                foreach (var modifier in modifiers)
                {
                    modifier.MMC?.OnRemoveMmc();
                    
                    var attrSetIndex = attrSets.IndexOfAttrSetCode(modifier.AttrSetCode);
                    if (attrSetIndex == -1) continue;
                    
                    var attrSet = attrSets[attrSetIndex];
                    var attributes = attrSet.Attributes;
                    
                    var attrIndex = attributes.IndexOfAttrCode(modifier.AttrCode);
                    if (attrIndex == -1) continue;
                    
                    var data = attributes[attrIndex];
                    data.Dirty = true;
                    attributes[attrIndex] = data;
                    attrSet.Attributes = attributes;
                    attrSets[attrSetIndex] = attrSet;
                    
                    isAttrDirty = true;
                }
                if(isAttrDirty) ecb.AddComponent<CAttributeIsDirty>(targetAsc);
            }
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}