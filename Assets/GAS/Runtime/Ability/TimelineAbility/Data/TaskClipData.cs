using System;
using System.Collections.Generic;
using UnityEngine;

namespace GAS.Runtime
{
    public class TaskClipData
    {
        public string Name;
        public int StartTime;
        public int EndTime;
        public string TaskType { get; set; }
        
        private IExParameterBase _parameter;

        public int Duration => EndTime - StartTime;
        
        
        public void SetParameter(IExParameterBase parameter)
        {
            _parameter = parameter;
        }
        
        public AbilityTaskBase CreateTaskInEditor()
        {
            var type = AbilityHelper.GetAbilityTaskType(TaskType);
            var task = Activator.CreateInstance(type) as AbilityTaskBase;
            if (task != null)
            {
                task.InitParameters(_parameter);
                return task;
            }

            Debug.LogError($"Ability Task Type [{TaskType}] not found");
            return null;
        }
        
        public AbilityTaskBase InstantiateTask(AbilityLogicBase logic)
        {
            // TODO
            return null;
        }
    }
}