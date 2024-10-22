using GAS.RuntimeWithECS.Attribute.Component;
using GAS.RuntimeWithECS.AttributeSet.Component;
using GAS.RuntimeWithECS.Core;
using GAS.RuntimeWithECS.System.GameplayEffect;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace GAS.RuntimeWithECS.System.Attribute
{
    [UpdateAfter(typeof(SysUpdateAttributeBaseValue))]
    public partial struct SysUpdateAttributeCurrentValue : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            
        }

        // [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // var baseValueUpdateInfos = GasQueueCenter.BaseValueUpdateInfos();
            //
            // foreach (var updateInfo in baseValueUpdateInfos)
            // {
            //     var asc = updateInfo.ASC;
            //     var attrSets = SystemAPI.GetBuffer<AttributeSetBufferElement>(asc);
            //     
            //     int attrSetIndex = attrSets.IndexOfAttrSetCode(updateInfo.AttrSetCode);
            //     if(attrSetIndex==-1) continue;
            //         
            //     var attrSet = attrSets[attrSetIndex];
            //     var attributes = attrSet.Attributes;
            //
            //     int attrIndex = attributes.IndexOfAttrCode(updateInfo.AttrCode);
            //     if(attrIndex==-1) continue;
            //         
            //     var attr = attributes[attrIndex];
            //
            //     
            //     
            //     float oldValue = attr.BaseValue;
            //     float newValue = updateInfo.Value;
            //     // OnChangeBefore
            //     // BaseValue 不做钳制，因为Max，Min是只针对Current Value
            //     newValue = GASEventCenter.InvokeOnBaseValueChangeBefore(updateInfo.ASC,updateInfo.AttrSetCode,updateInfo.AttrCode,newValue);
            //     
            //     attr.BaseValue = newValue;
            //     
            //     // OnChangeAfter
            //     if (newValue != oldValue)
            //     {
            //         attr.TriggerCueEvent = true;
            //         attr.Dirty = true;
            //         GASManager.EntityManager.AddComponent<ComAttributeDirty>(asc);
            //         GASEventCenter.InvokeOnBaseValueChangeAfter(updateInfo.ASC,updateInfo.AttrSetCode,updateInfo.AttrCode,oldValue,newValue);
            //     }
            //     
            //     attrSet.Attributes[attrIndex] = attr;
            //     attrSets[attrSetIndex] = attrSet;
            // }
            //
            // GasQueueCenter.ClearBaseValueUpdateInfos();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}