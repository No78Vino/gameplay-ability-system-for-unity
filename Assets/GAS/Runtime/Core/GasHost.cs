using GAS.Core;
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
            foreach (var abilitySystemComponent in _gas.AbilitySystemComponents) abilitySystemComponent.Tick();
        }
    }
}