using System.Collections.Generic;

namespace GAS.Runtime
{
    public class TaskClipData
    {
        public int StartTime;
        public int EndTime;
        public string TaskType { get; set; }
        public List<string> Parameters { get; set; } = new List<string>();

        public AbilityTaskBase CreateTask(AbilityLogicBase logic)
        {
            // TODO
            return null;
        }
    }
}