using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.Runtime.Ability.TimelineAbility.AbilityTask
{
    public abstract class AbilityTaskBase:ScriptableObject
    {
#if UNITY_EDITOR
        [ShowInInspector]
        [DisplayAsString(TextAlignment.Left, true)]
        [HideLabel]
        [BoxGroup(Order = -1)]
        private string TextTaskType =>
            $"<color=white><b>TaskClass: ({(this is InstantAbilityTask ? "Instant" : "Ongoing")}) {GetType().FullName} </b></color>";
#endif
    }
    
    public static class AbilityTaskBaseExtension
    {
        public static T CreateSpec<T>(this AbilityTaskBase abilityTaskBase,AbilitySpec abilitySpec) where T:AbilityTaskSpec,new()
        {
            var spec = new T();
            spec.Init(abilityTaskBase,abilitySpec);
            return spec;
        }
    }
}