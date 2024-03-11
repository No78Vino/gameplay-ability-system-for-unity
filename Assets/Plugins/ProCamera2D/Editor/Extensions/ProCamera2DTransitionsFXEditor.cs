using UnityEditor;
using UnityEngine;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    [CustomEditor(typeof(ProCamera2DTransitionsFX))]
    public class ProCamera2DTransitionsFXEditor : Editor
    {
        GUIContent _tooltip;

        MonoScript _script;

        void OnEnable()
        {
            if (target == null)
                return;
            
            _script = MonoScript.FromMonoBehaviour((ProCamera2DTransitionsFX)target);
        }

        public override void OnInspectorGUI()
        {
            if (target == null)
                return;
            
            var proCamera2DTransitionsFX = (ProCamera2DTransitionsFX)target;

            serializedObject.Update();

            // Show script link
            GUI.enabled = false;
            _script = EditorGUILayout.ObjectField("Script", _script, typeof(MonoScript), false) as MonoScript;
            GUI.enabled = true;

            // ProCamera2D
            _tooltip = new GUIContent("Pro Camera 2D", "");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_pc2D"), _tooltip);

            if (proCamera2DTransitionsFX.ProCamera2D == null)
                EditorGUILayout.HelpBox("ProCamera2D is not set.", MessageType.Error, true);

            EditorGUILayout.Space();

            // Enter
            _tooltip = new GUIContent("TransitionFX Enter", "The FX for the enter animation");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("TransitionShaderEnter"), _tooltip);

            EditorGUI.indentLevel = 1;

            _tooltip = new GUIContent("Duration", "The duration of the animation");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("DurationEnter"), _tooltip);

            _tooltip = new GUIContent("Delay", "A delay for the start of the animation");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("DelayEnter"), _tooltip);

            _tooltip = new GUIContent("Ease Type", "The ease type of the animation");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("EaseTypeEnter"), _tooltip);

            _tooltip = new GUIContent("Background Color", "The background color of the animation");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("BackgroundColorEnter"), _tooltip);

            if (proCamera2DTransitionsFX.TransitionShaderEnter == TransitionsFXShaders.Wipe ||
                proCamera2DTransitionsFX.TransitionShaderEnter == TransitionsFXShaders.Blinds)
            {
                _tooltip = new GUIContent("Side", "The side of the animation");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("SideEnter"), _tooltip);

                if (proCamera2DTransitionsFX.TransitionShaderEnter == TransitionsFXShaders.Blinds)
                {
                    _tooltip = new GUIContent("Blinds", "The number of blinds");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("BlindsEnter"), _tooltip);
                }
            }
            else if (proCamera2DTransitionsFX.TransitionShaderEnter == TransitionsFXShaders.Shutters)
            {
                _tooltip = new GUIContent("Direction", "The direction of the animation");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("DirectionEnter"), _tooltip);
            }
            else if (proCamera2DTransitionsFX.TransitionShaderEnter == TransitionsFXShaders.Texture)
            {
                _tooltip = new GUIContent("Texture", "The texture to use as a mask. Should be a black and white texture.");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("TextureEnter"), _tooltip);

                _tooltip = new GUIContent("Texture Smoothing", "The amount of fade smoothing to apply on the texture.");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("TextureSmoothingEnter"), _tooltip);
            }
            EditorGUI.indentLevel = 0;

            EditorGUILayout.Space();

            // Exit
            _tooltip = new GUIContent("TransitionFX Exit", "The FX for the exit animation");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("TransitionShaderExit"), _tooltip);

            EditorGUI.indentLevel = 1;

            _tooltip = new GUIContent("Duration", "The duration of the animation");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("DurationExit"), _tooltip);

            _tooltip = new GUIContent("Delay", "A delay for the start of the animation");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("DelayExit"), _tooltip);

            _tooltip = new GUIContent("Ease Type", "The ease type of the animation");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("EaseTypeExit"), _tooltip);

            _tooltip = new GUIContent("Background Color", "The background color of the animation");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("BackgroundColorExit"), _tooltip);

            if (proCamera2DTransitionsFX.TransitionShaderExit == TransitionsFXShaders.Wipe ||
                proCamera2DTransitionsFX.TransitionShaderExit == TransitionsFXShaders.Blinds)
            {
                _tooltip = new GUIContent("Side", "The side of the animation");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("SideExit"), _tooltip);

                if (proCamera2DTransitionsFX.TransitionShaderExit == TransitionsFXShaders.Blinds)
                {
                    _tooltip = new GUIContent("Blinds", "The number of blinds");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("BlindsExit"), _tooltip);
                }
            }
            else if (proCamera2DTransitionsFX.TransitionShaderExit == TransitionsFXShaders.Shutters)
            {
                _tooltip = new GUIContent("Direction", "The direction of the animation");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("DirectionExit"), _tooltip);
            }
            else if (proCamera2DTransitionsFX.TransitionShaderExit == TransitionsFXShaders.Texture)
            {
                _tooltip = new GUIContent("Texture", "The texture to use as a mask. Should be a black and white texture.");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("TextureExit"), _tooltip);

                _tooltip = new GUIContent("Texture Smoothing", "The amount of fade smoothing to apply on the texture.");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("TextureSmoothingExit"), _tooltip);
            }
            EditorGUI.indentLevel = 0;

            // Start scene on enter state
            EditorGUILayout.Space();
            _tooltip = new GUIContent("Start Scene On Enter State", "If selected, on scene start the Enter FX will be loaded.");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("StartSceneOnEnterState"), _tooltip);


            // Limit values
            if (proCamera2DTransitionsFX.DurationEnter < 0)
                proCamera2DTransitionsFX.DurationEnter = 0;
            if (proCamera2DTransitionsFX.DurationExit < 0)
                proCamera2DTransitionsFX.DurationExit = 0;

            if (proCamera2DTransitionsFX.DelayEnter < 0)
                proCamera2DTransitionsFX.DelayEnter = 0;
            if (proCamera2DTransitionsFX.DelayExit < 0)
                proCamera2DTransitionsFX.DelayExit = 0;

            // Apply properties
            serializedObject.ApplyModifiedProperties();


            EditorGUILayout.Space();
            EditorGUILayout.Space();

            // Detect changes
            if (GUI.changed && Application.isPlaying)
            {
                proCamera2DTransitionsFX.UpdateTransitionsShaders();
                proCamera2DTransitionsFX.UpdateTransitionsProperties();
                proCamera2DTransitionsFX.UpdateTransitionsColor();
            }


            // Test buttons
            GUI.enabled = Application.isPlaying;
            if (GUILayout.Button("Transition Enter"))
            {
                proCamera2DTransitionsFX.TransitionEnter();
            }

            if (GUILayout.Button("Transition Exit"))
            {
                proCamera2DTransitionsFX.TransitionExit();
            }
            GUI.enabled = true;
        }
    }
}