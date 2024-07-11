using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.Runtime
{
    [CreateAssetMenu(fileName = "SetByCallerFromTag", menuName = "GAS/MMC/SetByCallerFromTagModCalculation")]
    public class SetByCallerFromTagModCalculation : ModifierMagnitudeCalculation
    {
        [SerializeField]
        [ValueDropdown("@ValueDropdownHelper.GameplayTagChoices", HideChildProperties = true)]
        private GameplayTag _tag;

        public override float CalculateMagnitude(GameplayEffectSpec spec, float input)
        {
            var value = spec.GetMapValue(_tag);
#if UNITY_EDITOR
            if (value == null)
                Debug.LogWarning($"[EX] SetByCallerModCalculation: GE's '{_tag.Name}' value(tag map) is not set");
#endif
            return value ?? 0;
        }
    }
}