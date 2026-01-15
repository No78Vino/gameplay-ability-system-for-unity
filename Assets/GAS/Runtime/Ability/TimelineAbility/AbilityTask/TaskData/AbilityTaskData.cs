using System;
using GAS.General;
using UnityEngine;

namespace GAS.Runtime
{
    [Serializable]
    public abstract class AbilityTaskData
    {
        public virtual AbilityTaskBase Create(AbilityLogicBase abilityLogic)
        {
            var task = Load();
            //task.Init(abilityLogic);
            return task;
        }

        public abstract AbilityTaskBase Load();
    }
}