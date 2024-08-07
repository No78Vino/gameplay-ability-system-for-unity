using Unity.Entities;
using UnityEngine;

namespace GAS.RuntimeWithECS.Core
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(GASTimerSystem))]
    public partial class DebugSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<GlobalFrameTimer>();
        }
        protected override void OnUpdate()
        {
            var timer = SystemAPI.GetSingleton<GlobalFrameTimer>();
            // GASManager.TurnController.NextTurn();
            // var turn = GASManager.TurnController.CurrentTurn;
            Debug.Log("OnUpdate timer = "+timer.FrameCount);
        }
    }
}