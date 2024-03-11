using UnityEditor;
using UnityEngine;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    [CustomEditor(typeof(ProCamera2DRepeater))]
    public class ProCamera2DRepeaterEditor : Editor
    {
        GUIContent _tooltip;

        MonoScript _script;

        void OnEnable()
        {
            if (target == null)
                return;
            
            _script = MonoScript.FromMonoBehaviour((ProCamera2DRepeater)target);
        }

        public override void OnInspectorGUI()
        {
            if (target == null)
                return;
            
            var proCamera2DRepeater = (ProCamera2DRepeater)target;

            serializedObject.Update();

            // Show script link
            GUI.enabled = false;
            _script = EditorGUILayout.ObjectField("Script", _script, typeof(MonoScript), false) as MonoScript;
            GUI.enabled = true;

            // ProCamera2D
            _tooltip = new GUIContent("Pro Camera 2D", "");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_pc2D"), _tooltip);

            if (proCamera2DRepeater.ProCamera2D == null)
                EditorGUILayout.HelpBox("ProCamera2D is not set.", MessageType.Error, true);

            // Object To Repeat
            EditorGUI.BeginChangeCheck();

            _tooltip = new GUIContent("Object To Repeat", "This is the object that you want to repeat. It may be a prefab or not.");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ObjectToRepeat"), _tooltip);

            if (EditorGUI.EndChangeCheck())
            {
                var objectToRepeat = serializedObject.FindProperty("ObjectToRepeat").objectReferenceValue as Transform;
                if (objectToRepeat != null && objectToRepeat.GetComponent<SpriteRenderer>())
                {
                    var sprite = objectToRepeat.GetComponent<SpriteRenderer>();
                    var size = new Vector2(sprite.bounds.max.x - sprite.bounds.min.x, sprite.bounds.max.y - sprite.bounds.min.y);

                    serializedObject.FindProperty("ObjectSize").vector2Value = size;
                    serializedObject.FindProperty("ObjectBottomLeft").vector2Value = -size / 2f;
                }
            }

            if (proCamera2DRepeater.ObjectToRepeat == null)
                EditorGUILayout.HelpBox("No object to repeat set. Please drag one to the field above.", MessageType.Error, true);

            // Object Size
            _tooltip = new GUIContent("Object Size", "The size of the object to repeat (in world units).");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ObjectSize"), _tooltip);

            if (proCamera2DRepeater.ObjectSize.x <= 0)
                proCamera2DRepeater.ObjectSize.x = .01f;

            if (proCamera2DRepeater.ObjectSize.y <= 0)
                proCamera2DRepeater.ObjectSize.y = .01f;

            // Object Bottom Left
            _tooltip = new GUIContent("Object Bottom Left", "The bottom left position of your object in relation to it's pivot (in world units).");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ObjectBottomLeft"), _tooltip);

            // Object On Stage
            _tooltip = new GUIContent("Object On Stage", "Mark as selected if your original object to repeat is already placed on the scene.");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ObjectOnStage"), _tooltip);

            // Repeat horizontal
            _tooltip = new GUIContent("Repeat Horizontal", "If enabled, the object will be repeated horizontally.");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_repeatHorizontal"), _tooltip);

            // Repeat vertical
            _tooltip = new GUIContent("Repeat Vertical", "If enabled, the object will be repeated vertically.");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_repeatVertical"), _tooltip);

            // Camera to use
            if (proCamera2DRepeater.CameraToUse == null)
                proCamera2DRepeater.CameraToUse = proCamera2DRepeater.ProCamera2D.GameCamera;

            var parallax = FindObjectOfType<ProCamera2DParallax>();
            if (parallax != null)
            {
                _tooltip = new GUIContent("Camera To Use", "Choose what camera is rendering the object to repeat.");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraToUse"), _tooltip);
            }

            if (serializedObject.ApplyModifiedProperties())
            {
                if (Application.isPlaying)
                {
                    proCamera2DRepeater.RepeatHorizontal = serializedObject.FindProperty("_repeatHorizontal").boolValue;
                    proCamera2DRepeater.RepeatVertical = serializedObject.FindProperty("_repeatVertical").boolValue;
                }
            }
        }
    }
}