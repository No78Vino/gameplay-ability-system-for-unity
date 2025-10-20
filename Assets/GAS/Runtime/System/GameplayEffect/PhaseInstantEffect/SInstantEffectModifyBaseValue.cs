using GAS.Runtime;
using Unity.Burst;
using Unity.Entities;

namespace GAS.Runtime.System.GameplayEffect
{
    [UpdateInGroup(typeof(SysGrpInstantEffect))]
    [UpdateAfter(typeof(SRemoveEffectWithTags))]
    public partial struct SInstantEffectModifyBaseValue : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MCModifiers>();
            state.RequireForUpdate<CInApplicationProgress>();
            state.RequireForUpdate<CEffectApplied>();
            state.RequireForUpdate<CEffectInUsage>();
        }

        public void OnUpdate(ref SystemState state)
        {
            // var ecb = new EntityCommandBuffer(Allocator.Temp);
            // foreach (var (inUsage,_,_,mcModifiers,ge) in 
            //          SystemAPI.Query<RefRO<CInUsage>,RefRO<CValidEffect>,RefRO<CInApplicationProgress>,MCModifiers>()
            //              .WithEntityAccess())
            // {
            //     bool change = false;
            //     // 排除掉Durational的GE类型
            //     var isDurational = GASManager.EntityManager.HasComponent<CDuration>(ge);
            //     if (!isDurational)
            //     {
            //         var asc = inUsage.ValueRO.Target;
            //         var attrSets = GASManager.EntityManager.GetBuffer<BEAttributeSet>(asc);
            //         foreach (var mod in mcModifiers.Modifiers)
            //         {
            //             var attrSetIndex = attrSets.IndexOfAttrSetCode(mod.AttrSetCode);
            //             if (attrSetIndex == -1) continue;
            //
            //             var attrSet = attrSets[attrSetIndex];
            //             var attributes = attrSet.Attributes;
            //
            //             var attrIndex = attributes.IndexOfAttrCode(mod.AttrCode);
            //             if (attrIndex == -1) continue;
            //
            //             var data = attributes[attrIndex];
            //             var oldValue = data.BaseValue;
            //             var newValue = MmcHelper.Calculate(ge, mod, data.BaseValue);
            //
            //             // OnChangeBefore
            //             // BaseValue 不做钳制，因为Max，Min是只针对Current Value
            //             newValue = GASEventCenter.InvokeOnBaseValueChangeBefore(asc, mod.AttrSetCode, mod.AttrCode,
            //                 newValue);
            //
            //             data.BaseValue = newValue;
            //
            //             // OnChangeAfter
            //             if (newValue != oldValue)
            //             {
            //                 // BaseValue 改变，需要标记Dirty
            //                 data.Dirty = true;
            //                 change = true;
            //                 GASEventCenter.InvokeOnBaseValueChangeAfter(asc, mod.AttrSetCode, mod.AttrCode, oldValue,
            //                     newValue);
            //             }
            //
            //             attrSet.Attributes[attrIndex] = data;
            //             attrSets[attrSetIndex] = attrSet;
            //         }
            //     }
            //
            //     if (change) ecb.AddComponent<CAttributeIsDirty>(inUsage.ValueRO.Target);
            // }
            // ecb.Playback(state.EntityManager);
            // ecb.Dispose();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}