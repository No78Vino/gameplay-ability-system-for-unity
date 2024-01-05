using GAS.Runtime.Component;
using UnityEngine;

namespace GAS.Runtime.Cue
{
    public abstract class GameplayCue:ScriptableObject
    {
        protected  AbilitySystemComponent _source;

        protected virtual void Init(AbilitySystemComponent source)
        {
            _source = source;
        }

        public virtual void Trigger(AbilitySystemComponent source)
        {
            Init(source);
        }
    }
}