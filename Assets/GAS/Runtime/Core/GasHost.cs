using System;
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
            var snapshot = _gas.AbilitySystemComponents.ToArray();
            foreach (var abilitySystemComponent in snapshot) abilitySystemComponent.Tick();
        }

        private void OnDestroy()
        {
            _gas.ClearComponents();
        }
    }
}