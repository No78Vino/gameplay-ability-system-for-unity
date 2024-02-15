using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Log is a simple task which will output the specified text and return success. It can be used for debugging.")]
    [TaskIcon("{SkinColor}LogIcon.png")]
    public class Log : Action
    {
        [Tooltip("Text to output to the log")]
        public SharedString text;
        [Tooltip("Is this text an error?")]
        public SharedBool logError;
        [Tooltip("Should the time be included in the log message?")]
        public SharedBool logTime;
        
        public override TaskStatus OnUpdate()
        {
            // Log the text and return success
            if (logError.Value) {
                Debug.LogError(logTime.Value ? string.Format("{0}: {1}", Time.time, text) : text);
            } else {
                Debug.Log(logTime.Value ? string.Format("{0}: {1}",Time.time, text) : text);
            }
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            // Reset the properties back to their original values
            text = "";
            logError = false;
            logTime = false;
        }
    }
}