using UnityEngine;
using UnityEditor;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.ObjectDrawers;

namespace BehaviorDesigner.Editor.ObjectDrawers
{
    [CustomObjectDrawer(typeof(IntSliderAttribute))]
    public class IntSliderDrawer : ObjectDrawer
    {
        public override void OnGUI(GUIContent label)
        {
            var intSliderAttribute = (IntSliderAttribute)attribute;
            if (value is SharedInt) {
                var sharedFloat = value as SharedInt;
                sharedFloat.Value = EditorGUILayout.IntSlider(label, sharedFloat.Value, intSliderAttribute.min, intSliderAttribute.max);
            } else {
                value = EditorGUILayout.IntSlider(label, (int)value, intSliderAttribute.min, intSliderAttribute.max);
            }
        }
    }
}