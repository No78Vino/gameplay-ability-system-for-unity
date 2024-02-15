namespace BehaviorDesigner.Runtime.Tasks
{
    // The entry task is a task that is used for display purposes within Behavior Designer to indicate the root of the tree. It is not a real task and
    // cannot be used within the behavior tree.
    [TaskIcon("{SkinColor}EntryIcon.png")]
    public class EntryTask : ParentTask
    {
        public override int MaxChildren()
        {
            return 1;
        }
    }
}