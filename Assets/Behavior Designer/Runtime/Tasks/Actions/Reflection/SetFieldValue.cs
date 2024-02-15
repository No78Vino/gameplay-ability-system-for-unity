using UnityEngine;
using System;
using System.Reflection;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Sets the field to the value specified. Returns success if the field was set.")]
    [TaskCategory("Reflection")]
    [TaskIcon("{SkinColor}ReflectionIcon.png")]
    public class SetFieldValue : Action
    {
        [Tooltip("The GameObject to set the field on")]
        public SharedGameObject targetGameObject;
        [Tooltip("The component to set the field on")]
        public SharedString componentName;
        [Tooltip("The name of the field")]
        public SharedString fieldName;
        [Tooltip("The value to set")]
        public SharedVariable fieldValue;

        public override TaskStatus OnUpdate()
        {
            if (fieldValue == null) {
                Debug.LogWarning("Unable to get field - field value is null");
                return TaskStatus.Failure;
            }
            
            var type = TaskUtility.GetTypeWithinAssembly(componentName.Value);
            if (type == null) {
                Debug.LogWarning("Unable to set field - type is null");
                return TaskStatus.Failure;
            }

            var component = GetDefaultGameObject(targetGameObject.Value).GetComponent(type);
            if (component == null) {
                Debug.LogWarning("Unable to set the field with component " + componentName.Value);
                return TaskStatus.Failure;
            }

            // If you are receiving a compiler error on the Windows Store platform see this topic:
            // https://www.opsive.com/support/documentation/behavior-designer/installation/
            var field = component.GetType().GetField(fieldName.Value);
            field.SetValue(component, fieldValue.GetValue());

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            componentName = null;
            fieldName = null; 
            fieldValue = null;
        }
    }
}