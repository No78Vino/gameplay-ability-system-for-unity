using GAS.RuntimeWithECS.Cue;
using UnityEngine;

namespace GAS.Runtime
{
    public abstract class CueDurational : NewGameplayCueBase
    {
        public virtual void OnAdd()
        {
        }

        public virtual void OnRemove()
        {
        }

        public virtual void OnGameplayEffectActivate()
        {
        }

        public virtual void OnGameplayEffectDeactivate()
        {
        }

        public virtual void OnTick()
        {
        }
    }
    
    public abstract class CueDurational<T> : CueDurational where T : ICueParameter
    {
        public T Parameter { get; private set; }
        
        public override void InitParameters(ICueParameter parameter)
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