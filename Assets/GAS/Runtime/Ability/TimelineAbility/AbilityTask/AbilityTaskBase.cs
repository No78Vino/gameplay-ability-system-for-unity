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
        [BoxGroup]
        private string TextTaskType =>
            $"<color=white><b>TaskClass: ({(this is InstantAbilityTask ? "Instant" : "Ongoing")}) {GetType().FullName} </b></color>";
#endif
        protected AbilitySpec _spec;
        public virtual void Init(AbilitySpec spec)
        {
            _spec = spec;
        }
    }
}