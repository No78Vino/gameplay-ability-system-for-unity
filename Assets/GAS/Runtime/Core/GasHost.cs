using GAS.Core;
using UnityEngine;

namespace GAS
{
    public class GasHost : MonoBehaviour
    {
        private GameplayAbilitySystem _gas => GameplayAbilitySystem.GAS;

        private void Update()
        {
            Debug.Log($"Time.fixedTime(ms) = {Time.fixedTime * 1000}, " +
                      $"Time.fixedDeltaTime(ms) = {Time.fixedDeltaTime * 1000}");
            foreach (var abilitySystemComponent in _gas.AbilitySystemComponents) abilitySystemComponent.Tick();
        }
    }
}