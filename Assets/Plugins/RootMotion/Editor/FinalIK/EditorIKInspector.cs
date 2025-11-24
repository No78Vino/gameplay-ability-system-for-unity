using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace RootMotion.FinalIK
{
    [CustomEditor(typeof(EditorIK))]
    public class EditorIKInspector : Editor
    {
        private EditorIK script { get { return target as EditorIK; } }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (Application.isPlaying) return;
            if (!script.enabled) return;

            EditorGUILayout.Space();

            if (script.defaultPose != null && script.ik != null && !script.ik.GetIKSolver().executedInEditor)
            {
                if (GUILayout.Button("Store Default Pose"))
                {
                    script.StoreDefaultPose();
                    serializedObject.ApplyModifiedProperties();

                    EditorUtility.SetDirty(script.defaultPose);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }

                if (script.defaultPose.poseStored && script.defaultPose.localPositions.Length == script.bones.Length)
                {
                    if (GUILayout.Button("Reset To Default Pose"))
                    {
                        script.defaultPose.Restore(script.bones);

                        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                    }
                }
            }

            EditorGUILayout.Space();

            if (script.defaultPose != null && script.defaultPose.poseStored && script.ik != null)
            {
                if (!script.ik.GetIKSolver().executedInEditor)
                {
                    bool isValid = script.ik.GetIKSolver().IsValid();
                    EditorGUI.BeginDisabledGroup(!isValid);
                    if (GUILayout.Button(isValid? "Start Solver": "'Start Solver' disabled for invalid solver setup"))
                    {
                        bool initiated = script.Initiate();
                        serializedObject.ApplyModifiedProperties();

                        EditorUtility.SetDirty(script.defaultPose);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        var ikS = new SerializedObject(script.ik);
                        ikS.FindProperty("solver").FindPropertyRelative("executedInEditor").boolValue = initiated;
                        ikS.ApplyModifiedProperties();

                        script.Update();

                        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                    }
                    EditorGUI.EndDisabledGroup();
                }

                if (script.ik.GetIKSolver().executedInEditor)
                {
                    if (GUILayout.Button("Stop"))
                    {
                        var ikS = new SerializedObject(script.ik);
                        ikS.FindProperty("solver").FindPropertyRelative("executedInEditor").boolValue = false;
                        ikS.ApplyModifiedProperties();
                    }
                }
            }
        }

    }
}
