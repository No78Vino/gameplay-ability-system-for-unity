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
        public XParam Parameter;

        public int Duration => EndTime - StartTime;
        
        
        public void SetParameter(XParam parameter)
        {
            Parameter = parameter;
        }
        
        public AbilityTaskBase InstantiateTask(AbilityLogicBase logic)
        {
            var task = AbilityHelper.TryCreateAbilityTask(TaskType,logic);
            task.InitParameters(Parameter);
            return task;
        }
    }
}