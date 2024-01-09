using GAS.Core;
using UnityEngine;

namespace GAS
{
    public class GasHost : MonoBehaviour
    {
        private GameplayAbilitySystem _gas => GameplayAbilitySystem.GAS;

        private void Update()
        {
            foreach (var abilitySystemComponent in _gas.AbilitySystemComponents) abilitySystemComponent.Tick();
        }
    }
}