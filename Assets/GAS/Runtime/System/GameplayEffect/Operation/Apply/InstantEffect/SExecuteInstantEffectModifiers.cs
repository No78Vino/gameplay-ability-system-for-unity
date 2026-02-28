using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace GAS.Runtime
{
    [UpdateInGroup(typeof(SGInstantEffect))]
    public partial struct SExecuteInstantEffectModifiers : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CEffectInstance>();
            state.RequireForUpdate<MCModifiers>();
            state.RequireForUpdate<CEffectInUsage>();
            state.RequireForUpdate<WipApplyEffect>();
        }


        public void OnUpdate(ref SystemState state)
        {
            // Instant Effect = 没有CDuration组件的Effect

            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (_,_, modifiers, inUsage, ge) in
                     SystemAPI.Query<
                         RefRO<CEffectInstance>,
                         RefRO<WipApplyEffect>,
                         MCModifiers,
                         RefRO<CEffectInUsage>
                     >().WithNone<CDuration>().WithEntityAccess())
            {
                var change = false;
                var asc = inUsage.ValueRO.Target;
                var attrSets = SystemAPI.GetBuffer<BEAttrSet>(asc);
                foreach (var modifier in modifiers.Modifiers)
                {
                    var attrSetIndex = attrSets.IndexOfAttrSetCode(modifier.AttrSetCode);
                    if (attrSetIndex == -1) continue;

                    var attrSet = attrSets[attrSetIndex];
                    var attributes = attrSet.Attributes;

                    var attrIndex = attributes.IndexOfAttrCode(modifier.AttrCode);
                    if (attrIndex == -1) continue;

                    var data = attributes[attrIndex];
                    var oldValue = data.BaseValue;
                    var newValue = MmcHelper.Calculate(ge, modifier, data.BaseValue);
                    // 钳制计算处理
                    if (data.IsClampMin) newValue = math.max(newValue, data.MinValue);
                    if (data.IsClampMax) newValue = math.min(newValue, data.MaxValue);
                    
                    // OnChangeBefore
                    newValue = GASEventCenter.InvokeOnBaseValueChangeBefore(asc, modifier.AttrSetCode,
                        modifier.AttrCode,
                        newValue);

                    data.BaseValue = newValue;

                    // OnChangeAfter
                    if (newValue != oldValue)
                    {
                        // BaseValue 改变，需要标记Dirty
                        data.Dirty = true;
                        change = true;
                        GASEventCenter.InvokeOnBaseValueChangeAfter(
                            asc, 
                            modifier.AttrSetCode, 
                            modifier.AttrCode,
                            oldValue,
                            newValue);
                    }

                    attrSet.Attributes[attrIndex] = data;
                    attrSets[attrSetIndex] = attrSet;
                }

                // 触发刷新CurrentValue的事件
                if (change) ecb.AddComponent<CAttributeIsDirty>(asc);
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