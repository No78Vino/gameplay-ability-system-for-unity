using UnityEngine;
using UnityEditor;

namespace Cinemachine.Editor
{
    [CustomPropertyDrawer(typeof(TagFieldAttribute))]
    internal sealed class CinemachineTagFieldPropertyDrawer : PropertyDrawer
    {
        const float hSpace = 2;
        private string tagValue = "";
        GUIContent clearText = new GUIContent("Clear", "Set the tag to empty");
        
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            var textDimensions = GUI.skin.button.CalcSize(clearText);

            rect.width -= textDimensions.x + hSpace;
            
            tagValue = property.stringValue;
            EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            tagValue = EditorGUI.TagField(rect, EditorGUI.BeginProperty(rect, label, property), tagValue);
            if (EditorGUI.EndChangeCheck())
                property.stringValue = tagValue;
            EditorGUI.showMixedValue = false;

            rect.x += rect.width + hSpace; rect.width = textDimensions.x; rect.height -=1;
            GUI.enabled = tagValue.Length > 0;
            if (GUI.Button(rect, clearText))
                property.stringValue = string.Empty;
            GUI.enabled = true;
        }
    }
}
