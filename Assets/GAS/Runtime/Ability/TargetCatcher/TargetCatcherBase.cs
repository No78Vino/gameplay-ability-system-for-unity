using System;
using System.Collections.Generic;
using GAS.Runtime;
using UnityEngine;

namespace GAS.Runtime
{
    public abstract class TargetCatcherBase
    {
        public AbilitySystemComponent Owner;

        protected TargetCatcherBase()
        {
        }

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

        public abstract void CatchTargetsNonAlloc(AbilitySystemComponent mainTarget, List<AbilitySystemComponent> results);

#if UNITY_EDITOR
        public virtual void OnEditorPreview(GameObject obj)
        {
        }
#endif
    }
}