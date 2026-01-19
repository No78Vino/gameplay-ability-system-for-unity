using System;

namespace GAS.Runtime
{
    [Serializable]
    public class TaskClipData
    {
        public string Name;
        public int StartTime;
        public int EndTime;
        public string TaskType;
        public IExParameterBase Parameter;

        public int Duration => EndTime - StartTime;
        
        
        public void SetParameter(IExParameterBase parameter)
        {
            Parameter = parameter;
        }
        
        public AbilityTaskBase InstantiateTask(AbilityLogicBase logic)
        {
            return AbilityHelper.TryCreateAbilityTask(TaskType,logic);
        }
    }
}