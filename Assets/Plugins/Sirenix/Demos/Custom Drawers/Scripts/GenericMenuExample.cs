#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Demos
{
    using System;
    using UnityEngine;

#if UNITY_EDITOR

    using Sirenix.OdinInspector.Editor;
    using UnityEditor;

#endif

    // Example component demonstating how new generic context menus can be created with drawers.
    [TypeInfoBox(
        "In this example, we have an attribute drawer that adds new options to the generic context menu.\n" +
        "In this case, we're adding options to select a color.")]
    public class GenericMenuExample : MonoBehaviour
    {
        [ColorPicker]
        public Color Color;
    }

    // The Color picker attribute.
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ColorPickerAttribute : Attribute
    {
    }

#if UNITY_EDITOR

    public class ColorPickerAttributeDrawer : OdinAttributeDrawer<ColorPickerAttribute, Color>, IDefinesGenericMenuItems
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            // In this example, we don't want to draw anything manually.
            // So we call the next drawer, so another drawer can draw the actual color field for us.
            this.CallNextDrawer(label);
        }

        // The method defined in IDefinesGenericMenuItems allows us to add our own functions to the context menu.
        // This function is called everytime the context menu is opened, which allows you to modify the the context menu.
        public void PopulateGenericMenu(InspectorProperty property, GenericMenu genericMenu)
        {
            if (genericMenu.GetItemCount() > 0)
            {
                genericMenu.AddSeparator("");
            }

            genericMenu.AddItem(new GUIContent("Colors/Red"), false, () => this.SetColor(Color.red));
            genericMenu.AddItem(new GUIContent("Colors/Green"), false, () => this.SetColor(Color.green));
            genericMenu.AddItem(new GUIContent("Colors/Blue"), false, () => this.SetColor(Color.blue));
            genericMenu.AddItem(new GUIContent("Colors/Yellow"), false, () => this.SetColor(Color.yellow));
            genericMenu.AddItem(new GUIContent("Colors/Cyan"), false, () => this.SetColor(Color.cyan));
            genericMenu.AddItem(new GUIContent("Colors/White"), false, () => this.SetColor(Color.white));
            genericMenu.AddItem(new GUIContent("Colors/Black"), false, () => this.SetColor(Color.black));
            genericMenu.AddDisabledItem(new GUIContent("Colors/Magenta"));
        }

        // Helper function called by the context menu.
        private void SetColor(Color color)
        {
            this.ValueEntry.SmartValue = color;
            this.ValueEntry.ApplyChanges(); // ApplyChanges will be called automatically from the DrawPropertyLayout method, but anywhere else you need to call it manually.
        }
    }

#endif
}
#endif
