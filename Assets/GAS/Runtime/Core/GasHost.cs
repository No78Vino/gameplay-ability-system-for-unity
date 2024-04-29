using GAS.General;
using UnityEngine;

namespace GAS
{
    public class GasHost : MonoBehaviour
    {
        private GameplayAbilitySystem _gas => GameplayAbilitySystem.GAS;

        private void Update()
        {
            GASTimer.UpdateCurrentFrameCount();
            _gas.Tick();
        }

        private void OnDestroy()
        {
            _gas.ClearComponents();
        }
    }
}