using System;
using System.Collections.Generic;
using UnityEngine;

namespace GAS.Runtime
{
    public abstract class TargetCatcherBase
    {
        public AbilitySystemCellMono Owner;

        protected TargetCatcherBase()
        {
        }

        public virtual void Init(AbilitySystemCellMono owner)
        {
            Owner = owner;
        }

        [Obsolete("请使用CatchTargetsNonAlloc方法来避免产生垃圾收集（GC）。")]
        public List<AbilitySystemCellMono> CatchTargets(AbilitySystemCellMono mainTarget)
        {
            var result = new List<AbilitySystemCellMono>();

            CatchTargetsNonAlloc(mainTarget, result);

            return result;
        }

        public void CatchTargetsNonAllocSafe(AbilitySystemCellMono mainTarget, List<AbilitySystemCellMono> results)
        {
            results.Clear();

            CatchTargetsNonAlloc(mainTarget, results);
        }

        protected abstract void CatchTargetsNonAlloc(AbilitySystemCellMono mainTarget, List<AbilitySystemCellMono> results);

#if UNITY_EDITOR
        public virtual void OnEditorPreview(GameObject obj)
        {
        }
#endif
    }
}