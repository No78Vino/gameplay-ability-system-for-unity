using UnityEngine;
using UnityEditor;

namespace RootMotion.FinalIK {

	/*
	 * Custom inspector and scene view helpers for the InteractionTarget.
	 * */
	[CustomEditor(typeof(InteractionTarget))]
	public class InteractionTargetInspector : Editor {

		private InteractionTarget script { get { return target as InteractionTarget; }}

		private const string twistAxisLabel = " Twist Axis";
		private const float size = 0.005f;
		private static Color targetColor = new Color(0.2f, 1f, 0.5f);
		private static Color pivotColor = new Color(0.2f, 0.5f, 1f);

		void OnSceneGUI() {
			Handles.color = targetColor;

			Inspector.SphereCap(0, script.transform.position, Quaternion.identity, size);

			DrawChildrenRecursive(script.transform);

			if (script.pivot != null) {
				Handles.color = pivotColor;
				GUI.color = pivotColor;

				Inspector.SphereCap(0, script.pivot.position, Quaternion.identity, size);

                if (script.rotationMode == InteractionTarget.RotationMode.TwoDOF)
                {
                    Vector3 twistAxisWorld = script.pivot.rotation * script.twistAxis.normalized * size * 40;
                    Handles.DrawLine(script.pivot.position, script.pivot.position + twistAxisWorld);
                    Inspector.SphereCap(0, script.pivot.position + twistAxisWorld, Quaternion.identity, size);

                    Inspector.CircleCap(0, script.pivot.position, Quaternion.LookRotation(twistAxisWorld), size * 20);
                    Handles.Label(script.pivot.position + twistAxisWorld, twistAxisLabel);
                }
			}

			Handles.color = Color.white;
			GUI.color = Color.white;
		}

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("effectorType"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("multipliers"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("interactionSpeedMlp"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("pivot"));

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("rotationMode"));

            int rotationMode = serializedObject.FindProperty("rotationMode").enumValueIndex;
            if (rotationMode == 0)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("twistAxis"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("twistWeight"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("swingWeight"));
            } else if (rotationMode == 1)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("threeDOFWeight"));
            }
            EditorGUILayout.PropertyField(serializedObject.FindProperty("rotateOnce"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("usePoser"));

            if (serializedObject.FindProperty("usePoser").boolValue)
            {
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
                            bones.GetArrayElementAtIndex(i - 1).objectReferenceValue = children[i];
                        }
                    }
                }
            }

            if (serializedObject.ApplyModifiedProperties())
            {
                EditorUtility.SetDirty(script);
            }
        }

        private void DrawChildrenRecursive(Transform t) {
			for (int i = 0; i < t.childCount; i++) {

				Handles.DrawLine(t.position, t.GetChild(i).position);
				Inspector.SphereCap(0, t.GetChild(i).position, Quaternion.identity, size);

				DrawChildrenRecursive(t.GetChild(i));
			}
		}
	}
}
