using UnityEngine;
using System;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityString
{
    [TaskCategory("Unity/String")]
    [TaskDescription("Stores a string with the specified format.")]
    public class Format : Action
    {
        [Tooltip("The format of the string")]
        public SharedString format;
        [Tooltip("Any variables to appear in the string")]
        public SharedGenericVariable[] variables;
        [Tooltip("The result of the format")]
        [RequiredField]
        public SharedString storeResult;

        private object[] variableValues;

        public override void OnAwake()
        {
            variableValues = new object[variables.Length];
        }

        public override TaskStatus OnUpdate()
        {
            for (int i = 0; i < variableValues.Length; ++i) {
                variableValues[i] = variables[i].Value.value.GetValue();
            }

            try {
                storeResult.Value = string.Format(format.Value, variableValues);
            } catch (Exception e) {
                Debug.LogError(e.Message);
                return TaskStatus.Failure;
            }
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            format = "";
            variables = null;
            storeResult = null;
        }
    }
}