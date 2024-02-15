namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Perform the actual interruption. This will immediately stop the specified tasks from running and will return success or failure depending on the value of interrupt success.")]
    [TaskIcon("{SkinColor}PerformInterruptionIcon.png")]
    public class PerformInterruption : Action
    {
        [Tooltip("The list of tasks to interrupt. Can be any number of tasks")]
        public Interrupt[] interruptTasks;
        [Tooltip("When we interrupt the task should we return a task status of success?")]
        public SharedBool interruptSuccess;

        public override TaskStatus OnUpdate()
        {
            // Loop through all of the tasks and fire an interruption. Once complete return success.
            for (int i = 0; i < interruptTasks.Length; ++i) {
                interruptTasks[i].DoInterrupt(interruptSuccess.Value ? TaskStatus.Success : TaskStatus.Failure);
            }
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            // Reset the properties back to their original values.
            interruptTasks = null;
            interruptSuccess = false;
        }
    }
}