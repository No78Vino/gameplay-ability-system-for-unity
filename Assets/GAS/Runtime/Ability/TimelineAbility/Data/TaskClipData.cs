using System;
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
        public IExParameterBase Parameter => _parameter;

        public int Duration => EndTime - StartTime;
        
        
        public void SetParameter(IExParameterBase parameter)
        {
            _parameter = parameter;
        }
        
        public AbilityTaskBase InstantiateTask(AbilityLogicBase logic)
        {
            // TODO
            return null;
        }
    }
}