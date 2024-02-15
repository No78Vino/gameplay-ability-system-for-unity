using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityPlayerPrefs
{
    [TaskCategory("Unity/PlayerPrefs")]
    [TaskDescription("Deletes all entries from the PlayerPrefs.")]
    public class DeleteAll : Action
    {
        public override TaskStatus OnUpdate()
        {
            PlayerPrefs.DeleteAll();

            return TaskStatus.Success;
        }
    }
}