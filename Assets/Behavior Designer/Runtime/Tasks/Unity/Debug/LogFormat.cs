using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityDebug
{
    [TaskDescription("LogFormat is analgous to Debug.LogFormat().\n" +
        "It takes format string, substitutes arguments supplied a '{0-4}' and returns success.\n" +
        "Any fields or arguments not supplied are ignored." +
        "It can be used for debugging.")]
    [TaskIcon("{SkinColor}LogIcon.png")]
    public class LogFormat : Action
    {
        [Tooltip("Text format with {0}, {1}, etc")]
        public SharedString textFormat;

        [Tooltip("Is this text an error?")]
        public SharedBool logError;

        public SharedVariable arg0;
        public SharedVariable arg1;
        public SharedVariable arg2;
        public SharedVariable arg3;

        public override TaskStatus OnUpdate()
        {
            var paramsArray = buildParamsArray();
            // Log the text and return success
            if (logError.Value) {
                Debug.LogErrorFormat(textFormat.Value, paramsArray);
            } else {
                Debug.LogFormat(textFormat.Value, paramsArray);
            }
            return TaskStatus.Success;
        }

        private object[] buildParamsArray() {
            object[] paramsArray;
            if (isValid(arg3)) {
                paramsArray = new object[4];
                paramsArray[3] = arg3.GetValue();
                paramsArray[2] = arg2.GetValue();
                paramsArray[1] = arg1.GetValue();
                paramsArray[0] = arg0.GetValue();
            } else if (isValid(arg2)) {
                paramsArray = new object[3];
                paramsArray[2] = arg2.GetValue();
                paramsArray[1] = arg1.GetValue();
                paramsArray[0] = arg0.GetValue();
            } else if (isValid(arg1)) {
                paramsArray = new object[2];
                paramsArray[1] = arg1.GetValue();
                paramsArray[0] = arg0.GetValue();
            } else if (isValid(arg0)) {
                paramsArray = new object[1];
                paramsArray[0] = arg0.GetValue();
            } else {
                return null;
            }
            return paramsArray;
        }

        private bool isValid(SharedVariable sv) {
            return null != sv && !sv.IsNone;
        }

        public override void OnReset()
        {
            // Reset the properties back to their original values
            textFormat = string.Empty;
            logError = false;
            arg0 = null;
            arg1 = null;
            arg2 = null;
            arg3 = null;
        }
    }
}