using UnityEngine;
using UnityEditor;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.ObjectDrawers;

namespace BehaviorDesigner.Editor.ObjectDrawers
{
    [CustomObjectDrawer(typeof(FloatSliderAttribute))]
    public class FloatSliderDrawer : ObjectDrawer
    {
        public override void OnGUI(GUIContent label)
        {
            var floatSliderAttribute = (FloatSliderAttribute)attribute;
            if (value is SharedFloat) {
                var sharedFloat = value as SharedFloat;
                sharedFloat.Value = EditorGUILayout.Slider(label, sharedFloat.Value, floatSliderAttribute.min, floatSliderAttribute.max);
            } else {
                value = EditorGUILayout.Slider(label, (float)value, floatSliderAttribute.min, floatSliderAttribute.max);
            }
        }
    }
}