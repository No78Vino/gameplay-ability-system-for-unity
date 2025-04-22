using GAS.RuntimeWithECS.Attribute;
using GAS.RuntimeWithECS.AttributeSet.Component;
using GAS.Runtime;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.GameplayEffect
{
    [UpdateInGroup(typeof(SysGroupInstantEffect))]
    [UpdateAfter(typeof(SInstantEffectModifyBaseValue))]
    [UpdateBefore(typeof(SInstantEffectOver))]
    public partial struct SRecalculateAttrCurrentValueWithBaseValueDirty : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CAttributeIsDirty>();
        }

        //[BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
        //     var ecb = new EntityCommandBuffer(Allocator.Temp);
        //     foreach (var (_, attrSets, asc) in SystemAPI
        //                  .Query<RefRO<CAttributeIsDirty>, DynamicBuffer<BEAttributeSet>>()
        //                  .WithEntityAccess())
        //     {
        //         for (var index = 0; index < attrSets.Length; index++)
        //         {
        //             var attrSet = attrSets[index];
        //             for (var i = 0; i < attrSet.Attributes.Length; i++)
        //             {
        //                 var attr = attrSet.Attributes[i];
        //                 if (!attr.Dirty) continue;
        //
        //                 attr.Dirty = false;
        //
        //                 var oldValue = attr.CurrentValue;
        //                 var newValue = AttributeTool.RecalculateCurrentValue(asc, attrSet.Code, attr.Code);
        //                 attr.CurrentValue = newValue;
        //                 
        //                 attrSet.Attributes[i] = attr;
        //                 var attributeSetBufferElements = attrSets;
        //                 attributeSetBufferElements[index] = attrSet;
        //                 
        //                 // OnChangeAfter
        //                 if (newValue != oldValue)
        //                     GASEventCenter.InvokeOnCurrentValueChangeAfter(asc, attrSet.Code, attr.Code, oldValue,
        //                         newValue);
        //             }
        //         }
        //
        //         ecb.RemoveComponent<CAttributeIsDirty>(asc);
        //     }
        //
        //     ecb.Playback(state.EntityManager);
        //     ecb.Dispose();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}