// https://gist.github.com/frarees/9791517

using UnityEngine;
using UnityEditor;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    [CustomPropertyDrawer(typeof(MinMaxSliderAttribute))]
    class MinMaxSliderDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.Vector2)
            {
                Vector2 range = property.vector2Value;
                float min = range.x;
                float max = range.y;
                var attr = attribute as MinMaxSliderAttribute;
                EditorGUI.BeginChangeCheck();

                EditorGUI.LabelField(position, label.text + " (" + min.ToString("F3") + " - " + max.ToString("F3") + ")");

                EditorGUI.MinMaxSlider(position, label, ref min, ref max, attr.min, attr.max);

                if (EditorGUI.EndChangeCheck())
                {
                    range.x = min;
                    range.y = max;
                    property.vector2Value = range;
                }
            }
            else {
                EditorGUI.LabelField(position, label, "Use only with Vector2");
            }
        }
    }
}