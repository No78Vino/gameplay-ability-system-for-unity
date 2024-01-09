#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Demos
{
    using Sirenix.OdinInspector.Editor;
    using Sirenix.OdinInspector.Editor.ActionResolvers;
    using Sirenix.OdinInspector.Editor.ValueResolvers;
    using Sirenix.Utilities.Editor;
    using System;
    using UnityEngine;

#if UNITY_EDITOR

#endif
    // Example demonstrating how reflection can be used to enhance custom drawers.
    [TypeInfoBox(
            "This example demonstrates how resolved strings can be used to extend the behaviour of drawers.\n\n" +
            "In this case, a user can use a resolved string to pass a value to a drawer or specify an action to be invoked from the drawer. Note how little drawer code this needs, compared to the Reflection Example.\n\n" +
            "Resolved strings can be hardcoded (if the resolved type is a string), or member references, or expressions, and are globally extendable, so users can add their own string resolution behaviour.")]
    public class ValueAndActionResolversExample : MonoBehaviour
    {
        [Title("Action resolvers")]
        [OnClickAction("OnClick")]
        public int MethodAction;

        [OnClickAction("@UnityEngine.Debug.Log(DateTime.Now.ToString())")]
        public int ExpressionAction;

        [OnClickAction("Invalid Action String"), InfoBox("The following shows an example of the error message you get if you pass in an invalid resolved string to an action resolver:")]
        public int InvalidActionString;

        [Title("Value resolvers")]
        [DisplayValueAsString("$MemberReferenceValue")]
        public int MemberReferenceValue = 1337;

        [DisplayValueAsString("@Mathf.Sin(Time.realtimeSinceStartup)")]
        public int ExpressionValue;

        [DisplayValueAsString("Invalid Value String"), InfoBox("The following shows an example of the error message you get if you pass in an invalid resolved string to a value resolver:")]
        public int InvalidValueString;

        private void OnClick()
        {
            Debug.Log("On click - this could be a static or an instance method, the code still works");
        }

#if UNITY_EDITOR
        [OnInspectorGUI]
        private void RepaintConstantly() { GUIHelper.RequestRepaint(); }
#endif
    }

    // Attribute with resolved action string.
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class OnClickActionAttribute : Attribute
    {
        public string ActionString { get; private set; }

        public OnClickActionAttribute(string actionString)
        {
            this.ActionString = actionString;
        }
    }

    // Attribute with resolved value string.
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class DisplayValueAsStringAttribute : Attribute
    {
        public string ValueString { get; private set; }

        public DisplayValueAsStringAttribute(string valueString)
        {
            this.ValueString = valueString;
        }
    }


#if UNITY_EDITOR
    public class OnClickActionAttributeDrawer : OdinAttributeDrawer<OnClickActionAttribute>
    {
        private ActionResolver action;

        protected override void Initialize()
        {
            this.action = ActionResolver.Get(this.Property, this.Attribute.ActionString);
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (this.action.HasError)
            {
                this.action.DrawError();
            }

            if (GUILayout.Button("Execute Action '" + this.Attribute.ActionString + "'"))
            {
                // If there is an error, this does nothing
                this.action.DoActionForAllSelectionIndices();
            }

            this.CallNextDrawer(label);
        }
    }

    public class DisplayValueAsStringAttributeDrawer : OdinAttributeDrawer<DisplayValueAsStringAttribute>
    {
        private ValueResolver<object> valueResolver;

        protected override void Initialize()
        {
            this.valueResolver = ValueResolver.Get<object>(this.Property, this.Attribute.ValueString);
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (this.valueResolver.HasError)
            {
                this.valueResolver.DrawError();
            }
            else
            {
                var value = this.valueResolver.GetValue();
                string valueStr = value == null ? "Null" : value.ToString();
                GUILayout.Label("Value of '" + this.Attribute.ValueString + "': " + valueStr);
            }

            this.CallNextDrawer(label);
        }
    }
#endif
}
#endif
