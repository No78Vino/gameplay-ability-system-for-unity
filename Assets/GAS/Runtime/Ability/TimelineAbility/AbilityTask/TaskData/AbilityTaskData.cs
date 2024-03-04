using System;
using GAS.General;
using UnityEngine;

namespace GAS.Runtime.Ability
{
    [Serializable]
    public abstract class AbilityTaskData
    {
        public JsonData TaskData;
        
        public AbilityTaskBase TaskBase { get;private set; }
        
        public virtual void Cache(AbilitySpec abilitySpec)
        {
            TaskBase = Load();
            TaskBase.Init(abilitySpec);
        }
        
        public void Save(AbilityTaskBase task)
        {
            var jsonData = JsonUtility.ToJson(task);
            var dataType = task.GetType().FullName;
            TaskData = new JsonData
            {
                Type = dataType,
                Data = jsonData
            };
        }

        public abstract AbilityTaskBase Load();
    }
}