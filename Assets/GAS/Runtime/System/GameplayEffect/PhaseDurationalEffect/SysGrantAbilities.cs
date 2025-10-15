// using GAS.RuntimeWithECS.GameplayEffect.Component;
// using GAS.RuntimeWithECS.System.SystemGroup;
// using Unity.Burst;
// using Unity.Entities;
//
// namespace GAS.RuntimeWithECS.System.GameplayEffect.PhaseDurationalEffect
// {
//     [UpdateInGroup( typeof(SysGroupDurationalEffect))]
//     [UpdateAfter( typeof( SysInitDuartionalEffect ) ) ]
//     public partial struct SysGrantAbilities : ISystem
//     {
//         [BurstCompile]
//         public void OnCreate(ref SystemState state)
//         {
//             state.RequireForUpdate<ComInUsage>();
//             state.RequireForUpdate<ComInApplicationProgress>();
//             state.RequireForUpdate<ComValidEffect>();
//             state.RequireForUpdate<BuffEleGrantedAbility>();
//         }
//
//         [BurstCompile]
//         public void OnUpdate(ref SystemState state)
//         {
//             // foreach (var (inUsage,grantedAbilities,_,_,_,ge) in 
//             //          SystemAPI.Query<RefRW<ComInUsage>,DynamicBuffer<BuffEleGrantedAbility>,RefRO<ComInApplicationProgress>,RefRO<ComValidEffect>,RefRO<ComDuration>>().WithEntityAccess())
//             // {
//             //     var owner = inUsage.ValueRO.Target;
//             //     // TODO 给owner添加ability
//             // }
//         }
//
//         [BurstCompile]
//         public void OnDestroy(ref SystemState state)
//         {
//
//         }
//     }
// }