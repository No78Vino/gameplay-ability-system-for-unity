using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityPlayerPrefs
{
    [TaskCategory("Unity/PlayerPrefs")]
    [TaskDescription("Sets the value with the specified key from the PlayerPrefs.")]
    public class SetString : Action
    {
        [Tooltip("The key to store")]
        public SharedString key;
        [Tooltip("The value to set")]
        public SharedString value;

        public override TaskStatus OnUpdate()
        {
            PlayerPrefs.SetString(key.Value, value.Value);

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            key = "";
            value = "";
        }
    }
}