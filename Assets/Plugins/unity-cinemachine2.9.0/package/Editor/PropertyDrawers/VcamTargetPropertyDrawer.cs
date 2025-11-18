using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Cinemachine.Editor
{
    [CustomPropertyDrawer(typeof(VcamTargetPropertyAttribute))]
    internal sealed class VcamTargetPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            const float hSpace = 2;
            float iconSize = rect.height + 4;
            rect.width -= iconSize + hSpace;
            EditorGUI.PropertyField(rect, property, EditorGUI.BeginProperty(rect, label, property));
            rect.x += rect.width + hSpace; rect.width = iconSize;

            var oldEnabled = GUI.enabled;
            var target = property.objectReferenceValue as Transform;
            if (target == null || target.GetComponent<CinemachineTargetGroup>() != null)
                GUI.enabled = false;
            if (GUI.Button(rect, EditorGUIUtility.IconContent("_Popup"), GUI.skin.label))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Convert to TargetGroup"), false, () =>
                {
                    GameObject go = ObjectFactory.CreateGameObject("Target Group", typeof(CinemachineTargetGroup));
                    var group = go.GetComponent<CinemachineTargetGroup>();
                   
                    group.m_RotationMode = CinemachineTargetGroup.RotationMode.GroupAverage;
                    group.AddMember(target, 1, 1);
                    property.objectReferenceValue = group.Transform;
                    property.serializedObject.ApplyModifiedProperties();
                });
                menu.ShowAsContext();
            }
            GUI.enabled = oldEnabled;
        }
    }
}
