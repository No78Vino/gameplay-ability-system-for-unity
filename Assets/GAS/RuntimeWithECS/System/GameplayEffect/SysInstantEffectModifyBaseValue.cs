using System;
using GAS.Runtime;
using GAS.RuntimeWithECS.Attribute.Component;
using GAS.RuntimeWithECS.AttributeSet.Component;
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

                    int attrSetIndex = IndexOfAttrSetCode(attrSets, mod.AttrSetCode);
                    var attrSet = attrSets[attrSetIndex];
                    var attributes = attrSet.Attributes;
                    int attrIndex = IndexOfAttrCode(attributes, mod.AttrCode);
                    var data = attributes[attrIndex];
                    switch (mod.Operation)
                    {
                        case GEOperation.Add:
                            data.BaseValue += magnitude;
                            break;
                        case GEOperation.Minus:
                            data.BaseValue -= magnitude;
                            break;
                        case GEOperation.Multiply:
                            data.BaseValue *= magnitude;
                            break;
                        case GEOperation.Divide:
                            data.BaseValue /= magnitude;
                            break;
                        case GEOperation.Override:
                            data.BaseValue = magnitude;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    data.TriggerCueEvent = true;
                    data.Dirty = true;
                    ecb.AddComponent<TagAttributeDirty>(geEntity);

                    attrSet.Attributes[attrIndex] = data;
                    attrSets[attrSetIndex] = attrSet;
                    
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
        
        private int IndexOfAttrSetCode(DynamicBuffer<AttributeSetBufferElement> attrSets,int attrSetCode)
        {
            for (int i = 0; i < attrSets.Length; i++)
            {
                if (attrSets[i].Code == attrSetCode) return i;
            }
            return -1;
        }        
        
        private int IndexOfAttrCode(NativeArray<AttributeData> attrs,int attrCode)
        {
            for (int i = 0; i < attrs.Length; i++)
            {
                if (attrs[i].Code == attrCode) return i;
            }
            return -1;
        }
    }
}