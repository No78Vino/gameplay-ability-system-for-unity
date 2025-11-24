using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RootMotion.FinalIK
{

    [CustomEditor(typeof(UniversalPoser))]
    public class UniversalPoserInspector : Editor
    {
        private UniversalPoser script { get { return target as UniversalPoser; } }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("fixTransforms"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("poseRoot"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("weight"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("localRotationWeight"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("localPositionWeight"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("targetAxis1"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("targetAxis2"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("axis1"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("axis2"));
            
            var bones = serializedObject.FindProperty("bones");
            EditorGUILayout.PropertyField(bones);

            if (GUILayout.Button("Auto-Assign Bones"))
            {
                var children = script.GetComponentsInChildren<Transform>();
                if (children.Length > 1)
                {
                    bones.ClearArray();
                    for (int i = 1; i < children.Length; i++)
                    {
                        bones.InsertArrayElementAtIndex(i - 1);
                        bones.GetArrayElementAtIndex(i - 1).FindPropertyRelative("bone").objectReferenceValue = children[i];
                    }
                }
            }


            if (serializedObject.ApplyModifiedProperties())
            {
                EditorUtility.SetDirty(script);
            }
        }
    }
}
