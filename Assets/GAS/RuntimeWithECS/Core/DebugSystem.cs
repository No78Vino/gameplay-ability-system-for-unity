using Unity.Entities;
using UnityEngine;

namespace GAS.RuntimeWithECS.Core
{
    [UpdateAfter(typeof(GASTimerSystem))]
    public partial class DebugSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            // GASManager.TurnController.NextTurn();
            // var turn = GASManager.TurnController.CurrentTurn;
            // Debug.Log("OnUpdate TURN = "+turn);
        }
    }
}