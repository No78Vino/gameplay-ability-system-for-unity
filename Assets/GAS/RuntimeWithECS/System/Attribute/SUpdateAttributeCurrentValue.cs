using GAS.RuntimeWithECS.Attribute.Component;
using GAS.RuntimeWithECS.AttributeSet.Component;
using GAS.RuntimeWithECS.Core;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using GAS.RuntimeWithECS.System.GameplayEffect;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace GAS.RuntimeWithECS.System.Attribute
{
    [UpdateAfter(typeof(SUpdateAttributeBaseValue))]
    public partial struct SUpdateAttributeCurrentValue : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            
        }

        // [BurstCompile]
        // public void OnUpdate(ref SystemState state)
        // {
        //     var ecb = new EntityCommandBuffer(Allocator.Temp);
        //     
        //     // 1，先更新由BaseValue和GE更新引起的CurrentValue更新
        //     foreach (var (_,attrSets,asc) in SystemAPI.Query<RefRO<AttributeIsDirty>,DynamicBuffer<AttributeSetBufferElement>>().WithEntityAccess())
        //     {
        //         var effects = SystemAPI.GetBuffer<GameplayEffectBufferElement>(asc);
        //         foreach (var attrSet in attrSets)
        //         {
        //             foreach (var attr in attrSet.Attributes)
        //             {
        //                 if(!attr.Dirty) continue;
        //
        //                 foreach (var geElem in effects)
        //                 {
        //                     var ge = geElem.GameplayEffect;
        //                     var modifiers = SystemAPI.GetBuffer<BuffEleModifier>(ge);
        //                     foreach (var modifier in modifiers)
        //                     {
        //                         if (modifier.AttrSetCode == attrSet.Code && modifier.AttrCode == attr.Code)
        //                         {
        //                             // TODO
        //                         }
        //                     }
        //                 }
        //             }
        //         }
        //         
        //         ecb.RemoveComponent<AttributeIsDirty>(asc);
        //     }
        //     
        //     // 2。再更新由AttrBasedMMC引起的CurrentValue连锁更新
        //     // TODO
        //     
        //     ecb.Playback(state.EntityManager);
        //     ecb.Dispose();
        //     // var baseValueUpdateInfos = GasQueueCenter.BaseValueUpdateInfos();
        //     //
        //     // foreach (var updateInfo in baseValueUpdateInfos)
        //     // {
        //     //     var asc = updateInfo.ASC;
        //     //     var attrSets = SystemAPI.GetBuffer<AttributeSetBufferElement>(asc);
        //     //     
        //     //     int attrSetIndex = attrSets.IndexOfAttrSetCode(updateInfo.AttrSetCode);
        //     //     if(attrSetIndex==-1) continue;
        //     //         
        //     //     var attrSet = attrSets[attrSetIndex];
        //     //     var attributes = attrSet.Attributes;
        //     //
        //     //     int attrIndex = attributes.IndexOfAttrCode(updateInfo.AttrCode);
        //     //     if(attrIndex==-1) continue;
        //     //         
        //     //     var attr = attributes[attrIndex];
        //     //
        //     //     
        //     //     
        //     //     float oldValue = attr.BaseValue;
        //     //     float newValue = updateInfo.Value;
        //     //     // OnChangeBefore
        //     //     // BaseValue 不做钳制，因为Max，Min是只针对Current Value
        //     //     newValue = GASEventCenter.InvokeOnBaseValueChangeBefore(updateInfo.ASC,updateInfo.AttrSetCode,updateInfo.AttrCode,newValue);
        //     //     
        //     //     attr.BaseValue = newValue;
        //     //     
        //     //     // OnChangeAfter
        //     //     if (newValue != oldValue)
        //     //     {
        //     //         attr.TriggerCueEvent = true;
        //     //         attr.Dirty = true;
        //     //         GASManager.EntityManager.AddComponent<ComAttributeDirty>(asc);
        //     //         GASEventCenter.InvokeOnBaseValueChangeAfter(updateInfo.ASC,updateInfo.AttrSetCode,updateInfo.AttrCode,oldValue,newValue);
        //     //     }
        //     //     
        //     //     attrSet.Attributes[attrIndex] = attr;
        //     //     attrSets[attrSetIndex] = attrSet;
        //     // }
        //     //
        //     // GasQueueCenter.ClearBaseValueUpdateInfos();
        // }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}