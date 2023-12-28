using GAS.Runtime.Component;
using UnityEngine;

namespace GAS.Runtime.Cue
{
    public abstract class GameplayCue:ScriptableObject
    {
        protected  AbilitySystemComponent _source;
        public virtual void Init(AbilitySystemComponent source)
        {
            _source = source;
        }
        
        public abstract void Trigger();
    }
}