using UnityEditor;
using UnityEngine;
using System;

namespace RootMotion
{
    
    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    public class ShowIfPropertyDrawer : PropertyDrawer
    {
        protected ShowIfAttribute showIfAttribute;
        protected SerializedProperty prop;
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!Show(property) && showIfAttribute.mode == ShowIfMode.Hidden) return -EditorGUIUtility.standardVerticalSpacing;

            return EditorGUI.GetPropertyHeight(property, label);
        }

        protected bool Show(SerializedProperty property)
        {
            showIfAttribute = attribute as ShowIfAttribute;

            var path = property.propertyPath.Contains(".") ? System.IO.Path.ChangeExtension(property.propertyPath, showIfAttribute.propName) : showIfAttribute.propName;

            prop = property.serializedObject.FindProperty(path);
            if (prop == null) return true;
            
            switch(prop.propertyType)
            {
                case SerializedPropertyType.Enum:
                    return prop.enumValueIndex.Equals((int)showIfAttribute.propValue);
                case SerializedPropertyType.Boolean:
                    return prop.boolValue.Equals(showIfAttribute.propValue);
                case SerializedPropertyType.Float:
                    return prop.floatValue > (float)showIfAttribute.propValue && prop.floatValue < (float)showIfAttribute.otherPropValue;
                case SerializedPropertyType.LayerMask:
                    return prop.intValue != 0;
                case SerializedPropertyType.String:
                    return prop.stringValue != string.Empty && prop.stringValue != "";
                case SerializedPropertyType.Vector2:
                    float sqrMag2 = prop.vector2Value.sqrMagnitude;
                    return sqrMag2 > (float)showIfAttribute.propValue && sqrMag2 < (float)showIfAttribute.otherPropValue;
                case SerializedPropertyType.Vector3:
                    float sqrMag3 = prop.vector3Value.sqrMagnitude;
                    return sqrMag3 > (float)showIfAttribute.propValue && sqrMag3 < (float)showIfAttribute.otherPropValue;
                case SerializedPropertyType.Vector4:
                    float sqrMag4 = prop.vector4Value.sqrMagnitude;
                    return sqrMag4 > (float)showIfAttribute.propValue && sqrMag4 < (float)showIfAttribute.otherPropValue;
                case SerializedPropertyType.ObjectReference:
                    return prop.objectReferenceValue != null;
                default:
                    Debug.LogError("Unsupported ShowIf property type: " + prop.propertyType);
                    return true;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            showIfAttribute = attribute as ShowIfAttribute;

            EditorGUI.BeginProperty(position, label, property);

            if (Show(property))
            {
                if (showIfAttribute.indent) EditorGUI.indentLevel++;
                Draw(position, property, attribute, label);
                if (showIfAttribute.indent) EditorGUI.indentLevel--;
            }
            else if (showIfAttribute.mode == ShowIfMode.Disabled)
            {
                if (showIfAttribute.indent) EditorGUI.indentLevel++;
                GUI.enabled = false;
                Draw(position, property, attribute, label);
                GUI.enabled = true;
                if (showIfAttribute.indent) EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        protected virtual void Draw(Rect position, SerializedProperty property, PropertyAttribute attribute, GUIContent label)
        {
            
            EditorGUI.PropertyField(position, property, label, true);   
        }
    }

    [CustomPropertyDrawer(typeof(ShowRangeIfAttribute))]
    public class ShowRangeIfPropertyDrawer : ShowIfPropertyDrawer
    {
        protected override void Draw(Rect position, SerializedProperty property, PropertyAttribute attribute, GUIContent label)
        {
            ShowRangeIfAttribute range = attribute as ShowRangeIfAttribute;

            if (property.propertyType == SerializedPropertyType.Float)
                EditorGUI.Slider(position, property, range.min, range.max, label);
            else if (property.propertyType == SerializedPropertyType.Integer)
                EditorGUI.IntSlider(position, property, Convert.ToInt32(range.min), Convert.ToInt32(range.max), label);
            else
                EditorGUI.LabelField(position, label.text, "Use Range with float or int.");
        }
    }

    // Custom drawer for the LargeHeader attribute
    [CustomPropertyDrawer(typeof(ShowLargeHeaderIf))]
    public class ShowLargeHeaderIfDrawer : ShowIfPropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!Show(property) && showIfAttribute.mode == ShowIfMode.Hidden) return -EditorGUIUtility.standardVerticalSpacing;

            return base.GetPropertyHeight(property, label) * 2f;
        }

        // Override the GUI drawing for this attribute
        protected override void Draw(Rect position, SerializedProperty property, PropertyAttribute attribute, GUIContent label)
        {
            var largeHeader = (ShowLargeHeaderIf)attribute;
            LargeHeaderDrawer.Draw(position, largeHeader.name, largeHeader.color);
        }
    }

    // Custom drawer for the LargeHeader attribute
    [CustomPropertyDrawer(typeof(LargeHeader))]
    public class LargeHeaderDrawer : DecoratorDrawer
    {
        // Get the height of the element
        public override float GetHeight()
        {
            return base.GetHeight() * 2f;
        }

        // Override the GUI drawing for this attribute
        public override void OnGUI(Rect position)
        {
            var largeHeader = (LargeHeader)attribute;
            Draw(position, largeHeader.name, largeHeader.color);
        }

        public static void Draw(Rect position, string name, string color)
        {
            // Get the color the line should be
            Color c = Color.white;
            switch (color.ToString().ToLower())
            {
                case "white": c = Color.white; break;
                case "red": c = Color.red; break;
                case "blue": c = Color.blue; break;
                case "green": c = Color.green; break;
                case "gray": c = Color.gray; break;
                case "grey": c = Color.grey; break;
                case "black": c = Color.black; break;
            }

            c *= 0.7f;

           var style = new GUIStyle(GUI.skin.label);
            style.fontSize = 16;
            style.fontStyle = FontStyle.Normal;
            style.alignment = TextAnchor.LowerLeft;
            GUI.color = c;

            Rect labelRect = position;
            EditorGUI.LabelField(labelRect, name, style);

            GUI.color = Color.white;
        }
    }
}
