using System;
using GAS.Runtime.Component;
using UnityEngine;

namespace GAS.Runtime.Ability.TargetCatcher
{
    [Serializable]
    public abstract class CatchAreaBase : TargetCatcherBase
    {
        public LayerMask checkLayer;

        public void Init(AbilitySystemComponent owner, LayerMask checkLayer) 
        {
            base.Init(owner);
            this.checkLayer = checkLayer;
        }
    }
}