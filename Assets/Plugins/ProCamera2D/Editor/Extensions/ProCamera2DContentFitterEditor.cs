using System;
using UnityEditor;
using UnityEngine;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    [CustomEditor(typeof(ProCamera2DContentFitter))]
    public class ProCamera2DAspectRatioEditor : Editor
    {
        GUIContent _tooltip;

        MonoScript _script;

        void OnEnable()
        {
            if (target == null)
                return;
            
            _script = MonoScript.FromMonoBehaviour((ProCamera2DContentFitter)target);
        }

        public override void OnInspectorGUI()
        {
            if (target == null)
                return;
            
            var proCamera2DAspectRatio = (ProCamera2DContentFitter)target;
            if (proCamera2DAspectRatio.ProCamera2D == null)
            {
                EditorGUILayout.HelpBox("ProCamera2D is not set.", MessageType.Error, true);
                return;
            }

            serializedObject.Update();

            // Show script link
            GUI.enabled = false;
            _script = EditorGUILayout.ObjectField("Script", _script, typeof(MonoScript), false) as MonoScript;
            GUI.enabled = true;

            // ProCamera2D
            _tooltip = new GUIContent("Pro Camera 2D", "");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_pc2D"), _tooltip);
    
            // Mode
            var changedMode = false;
            EditorGUI.BeginChangeCheck ();
            _tooltip = new GUIContent("Content Fitter Mode", "");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_contentFitterMode"), _tooltip);
            if (EditorGUI.EndChangeCheck())
            {
                changedMode = true;
                proCamera2DAspectRatio.ProCamera2D.GameCamera.ResetProjectionMatrix();
            }
            
            switch (proCamera2DAspectRatio.ContentFitterMode)
            {
                case ContentFitterMode.FixedHeight:
                    _tooltip = new GUIContent("Target Height", "");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_targetHeight"), _tooltip);
                    break;
                    
                case ContentFitterMode.FixedWidth:
                    _tooltip = new GUIContent("Target Width", "");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_targetWidth"), _tooltip);
                    break;
                    
                case ContentFitterMode.AspectRatio:
                    var changedWidth = false;
                    var changedHeight = false;
                    
                    // Target Width
                    EditorGUI.BeginChangeCheck ();
                    _tooltip = new GUIContent("Target Width", "");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_targetWidth"), _tooltip);
                    if (EditorGUI.EndChangeCheck ())
                        changedWidth = true;
                    
                    // Target Height
                    EditorGUI.BeginChangeCheck ();
                    _tooltip = new GUIContent("Target Height", "");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_targetHeight"), _tooltip);
                    if (EditorGUI.EndChangeCheck ())
                        changedHeight = true;
                    
                    // Target Aspect Ratio
                    EditorGUI.BeginChangeCheck ();
                    _tooltip = new GUIContent("Target Aspect Ratio", "");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_targetAspectRatio"), _tooltip);
                    if (EditorGUI.EndChangeCheck ())
                    {
                        changedWidth = true;
                        proCamera2DAspectRatio.TargetWidth =
                            proCamera2DAspectRatio.TargetHeight * proCamera2DAspectRatio.TargetAspectRatio;
                    }
                    
                    // Horizontal Alignment
                    _tooltip = new GUIContent("Horizontal Alignment", "");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("HorizontalAlignment"), _tooltip);
                    
                    // Vertical Alignment
                    _tooltip = new GUIContent("Vertical Alignment", "");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("VerticalAlignment"), _tooltip);
                    
                    // Use letterbox
                    _tooltip = new GUIContent("Use Letter/Pillarboxing", "");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_useLetterOrPillarboxing"), _tooltip);
                    
                    serializedObject.ApplyModifiedProperties();

                    if (changedWidth)
                    {
                        proCamera2DAspectRatio.TargetHeight =
                            proCamera2DAspectRatio.TargetWidth / proCamera2DAspectRatio.TargetAspectRatio;
                    }
                    else if (changedHeight)
                    {
                        proCamera2DAspectRatio.TargetWidth =
                            proCamera2DAspectRatio.TargetHeight * proCamera2DAspectRatio.TargetAspectRatio;
                    }
                    break;
            }
            
            serializedObject.ApplyModifiedProperties();
            
            if(changedMode && proCamera2DAspectRatio.ContentFitterMode == ContentFitterMode.AspectRatio)
                proCamera2DAspectRatio.TargetWidth = proCamera2DAspectRatio.TargetHeight * proCamera2DAspectRatio.TargetAspectRatio;

            if (proCamera2DAspectRatio.TargetWidth < .1f)
                proCamera2DAspectRatio.TargetWidth = .1f;
            if (proCamera2DAspectRatio.TargetHeight < .1f)
                proCamera2DAspectRatio.TargetHeight = .1f;
        }
    }
}