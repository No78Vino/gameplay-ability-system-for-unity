using System;
using UnityEngine;

namespace GAS.Runtime
{
    [Serializable]
    public abstract class NewCatchAreaBase : NewTargetCatcherBase
    {
        public LayerMask checkLayer;

        public void Init(AbilitySystemCellMono owner, LayerMask checkLayer) 
        {
            base.Init(owner);
            this.checkLayer = checkLayer;
        }
    }
}