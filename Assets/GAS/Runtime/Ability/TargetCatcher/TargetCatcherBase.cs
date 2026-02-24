using System;
using System.Collections.Generic;
using UnityEngine;

namespace GAS.Runtime
{
    public abstract class TargetCatcherBase
    {
        public AbilitySystemComponent Owner;

        public virtual void Init(AbilitySystemComponent owner)
        {
            Owner = owner;
        }

        [Obsolete("请使用CatchTargetsNonAlloc方法来避免产生垃圾收集（GC）。")]
        public List<AbilitySystemComponent> CatchTargets(AbilitySystemComponent mainTarget)
        {
            var result = new List<AbilitySystemComponent>();

            CatchTargetsNonAlloc(mainTarget, result);

            return result;
        }

        public void CatchTargetsNonAllocSafe(AbilitySystemComponent mainTarget, ref List<AbilitySystemComponent> results)
        {
            results.Clear();
            CatchTargetsNonAlloc(mainTarget, results);
        }

        protected abstract void CatchTargetsNonAlloc(AbilitySystemComponent mainTarget, List<AbilitySystemComponent> results);

#if UNITY_EDITOR
        public virtual void OnEditorPreview(GameObject obj)
        {
        }
#endif
    }
    
    public abstract class TargetCatcherBase<T> : TargetCatcherBase where T : XParam
    {
        public T Parameter { get; private set; }

        public virtual void InitParameters(XParam parameter)
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