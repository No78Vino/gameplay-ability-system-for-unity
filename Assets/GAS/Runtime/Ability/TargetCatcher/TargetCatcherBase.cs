using System;
using System.Collections.Generic;
using UnityEngine;

namespace GAS.Runtime
{
    public abstract class TargetCatcherBase  
    {  
        public AbilitySystemCell Owner;  
  
        public virtual void Init(AbilitySystemCell owner)  
        {  
            Owner = owner;  
        }  
  
        [Obsolete("请使用CatchTargetsNonAlloc方法来避免产生垃圾收集（GC）。")]  
        public List<AbilitySystemCell> CatchTargets(AbilitySystemCell mainTarget)  
        {  
            var result = new List<AbilitySystemCell>();  
            CatchTargetsNonAlloc(mainTarget, result);  
            return result;  
        }  
  
        public void CatchTargetsNonAllocSafe(AbilitySystemCell mainTarget, ref List<AbilitySystemCell> results)  
        {  
            results.Clear();  
            CatchTargetsNonAlloc(mainTarget, results);  
        }  
  
        protected abstract void CatchTargetsNonAlloc(AbilitySystemCell mainTarget, List<AbilitySystemCell> results);  
  
        public virtual void InitParameters(XParam parameter) { }  
        
        public virtual void OnEditorPreview(GameObject obj) { }  
    }
    
    public abstract class TargetCatcherBase<T> : TargetCatcherBase where T : XParam
    {
        public T Parameter { get; private set; }

        public override void InitParameters(XParam parameter)
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