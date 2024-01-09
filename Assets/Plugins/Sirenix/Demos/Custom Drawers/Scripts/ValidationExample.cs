#if UNITY_EDITOR
[assembly: Sirenix.OdinInspector.Editor.Validation.RegisterValidator(typeof(Sirenix.OdinInspector.Demos.NotOneAttributeValidator))]

namespace Sirenix.OdinInspector.Demos
{
    using System;
    using UnityEngine;

#if UNITY_EDITOR

    using Sirenix.OdinInspector.Editor.Validation;

#endif

    // This example demonstrates how to implement validators that can validate properties,
    // and how to add warnings or errors that will be picked up by Odin Project Validator.
    [TypeInfoBox(
       "This is example demonstrates how to implement a custom validator, that validates the property's value, " +
       "and how to get Odin Project Validator (if installed) to pick up that validation warning or error.")]
    public class ValidationExample : MonoBehaviour
    {
        [NotOne]
        public int NotOne;
    }

    // Attribute used by the NotOneAttributeDrawer
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class NotOneAttribute : Attribute
    {
    }

#if UNITY_EDITOR

    // Place the validator script file in an Editor folder, and remember to include the registration attribute at the top of this file.
    public class NotOneAttributeValidator : AttributeValidator<NotOneAttribute, int>
    {
        protected override void Validate(ValidationResult result)
        {
            if (this.ValueEntry.SmartValue == 1)
            {
                result.Message = "1 is not a valid value.";
                result.ResultType = ValidationResultType.Error;
            }
        }
    }

#endif
}
#endif
