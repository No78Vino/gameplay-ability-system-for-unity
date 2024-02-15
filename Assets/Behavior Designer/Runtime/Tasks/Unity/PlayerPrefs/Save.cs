using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityPlayerPrefs
{
    [TaskCategory("Unity/PlayerPrefs")]
    [TaskDescription("Saves the PlayerPrefs.")]
    public class Save : Action
    {
        public override TaskStatus OnUpdate()
        {
            PlayerPrefs.Save();

            return TaskStatus.Success;
        }
    }
}