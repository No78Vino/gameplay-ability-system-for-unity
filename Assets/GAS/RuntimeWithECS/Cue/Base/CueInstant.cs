using GAS.Runtime;
using GAS.RuntimeWithECS.Modifier.CommonUsage;
using UnityEngine;

namespace GAS.RuntimeWithECS.Cue
{
    public abstract class CueInstant : NewGameplayCueBase
    {
        public bool TryTrigger()
        {
            var triggerable = Triggerable();
            if (triggerable) Trigger();
            return triggerable;
        }

        protected abstract void Trigger();

        protected CueInstant(NewGameplayCueParametersBase p) : base(p)
        {
        }
    }
    
    public abstract class CueInstant<T> : CueInstant where T : ICueParameter
    {
        public T Parameter { get; private set; }
        
        public override void InitParameters(IMmcParameter parameter)
        {
            if (parameter is T t)
                Parameter = t;
#if UNITY_EDITOR
            else
                Debug.LogError($"Parameter type mismatch: expected {typeof(T)}, but got {parameter.GetType()}");
#endif
        }
    }
}