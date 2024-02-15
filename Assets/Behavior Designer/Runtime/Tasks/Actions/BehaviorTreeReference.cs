

namespace BehaviorDesigner.Runtime.Tasks
{
    // Wrapper class for the Behavior Reference task. The Behavior Tree Reference task allows you to run another behavior tree within the current behavior tree.
    // One use for this task is if you have an unit that plays a series of tasks to attack. You may want the unit to attack at different points within
    // the behavior tree, and you want that attack to always be the same. Instead of copying and pasting the same tasks over and over you can just use
    // an external behavior and then the tasks are always guaranteed to be the same. This example is demonstrated in the RTS sample project located at
    // https://www.opsive.com/downloads/?pid=803.
    [TaskDescription("Behavior Tree Reference allows you to run another behavior tree within the current behavior tree.")]
    [HelpURL("https://www.opsive.com/support/documentation/behavior-designer/external-behavior-trees/")]
    [TaskIcon("BehaviorTreeReferenceIcon.png")]
    public class BehaviorTreeReference : BehaviorReference
    {
        // intentionally left blank - subclass of BehaviorReference
    }
}