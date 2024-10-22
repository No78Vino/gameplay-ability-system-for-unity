using System;
using GAS.Runtime;
using GAS.RuntimeWithECS.Attribute.Component;
using GAS.RuntimeWithECS.AttributeSet.Component;
using GAS.RuntimeWithECS.Core;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using GAS.RuntimeWithECS.Modifier;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.GameplayEffect
{
    [UpdateAfter(typeof(SysEffectDurationTicker))]
    [UpdateBefore(typeof(SysRemoveInvalidEffect))]
    public partial struct SysInstantEffectModifyBaseValue : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BuffEleModifier>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (modifiers, _, geEntity) in SystemAPI
                         .Query<DynamicBuffer<BuffEleModifier>, RefRO<ComInUsage>>().WithNone<ComDuration>()
                         .WithEntityAccess())
            {
                // 过滤掉已经不合法的Instant GE
                if(!SystemAPI.IsComponentEnabled<ComInUsage>(geEntity)) continue;
                
                var asc = SystemAPI.GetComponentRO<ComInUsage>(geEntity).ValueRO.Target;
                var attrSets = SystemAPI.GetBuffer<AttributeSetBufferElement>(asc);

                foreach (var mod in modifiers)
                {
                    var magnitude = MmcHub.Calculate(geEntity, mod);

                    int attrSetIndex = attrSets.IndexOfAttrSetCode(mod.AttrSetCode);
                    if(attrSetIndex==-1) continue;
                    
                    var attrSet = attrSets[attrSetIndex];
                    var attributes = attrSet.Attributes;

                    int attrIndex = attributes.IndexOfAttrCode(mod.AttrCode);
                    if(attrIndex==-1) continue;
                    
                    var data = attributes[attrIndex];
                    var baseValue = data.BaseValue;
                    switch (mod.Operation)
                    {
                        case GEOperation.Add:
                            baseValue += magnitude;
                            break;
                        case GEOperation.Minus:
                            baseValue -= magnitude;
                            break;
                        case GEOperation.Multiply:
                            baseValue *= magnitude;
                            break;
                        case GEOperation.Divide:
                            baseValue /= magnitude;
                            break;
                        case GEOperation.Override:
                            baseValue = magnitude;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    // 加入base value 更新队列
                    GasQueueCenter.AddBaseValueUpdateInfo(asc,mod.AttrSetCode,mod.AttrCode,baseValue);
                    
                    
                    // data.TriggerCueEvent = true;
                    // data.Dirty = true;
                    // ecb.AddComponent<ComAttributeDirty>(geEntity);
                    //
                    // attrSet.Attributes[attrIndex] = data;
                    // attrSets[attrSetIndex] = attrSet;
                    
                    // 应用完成的Instant GE，使其不合法
                    ecb.SetComponentEnabled<ComInUsage>(geEntity, false);
                }

            }

            ecb.Playback(state.EntityManager);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}