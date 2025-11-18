#if CINEMACHINE_EXPERIMENTAL_VCAM
using UnityEngine;
using UnityEditor;
using Cinemachine.Editor;
using System.Collections.Generic;

namespace Cinemachine
{
    [CustomEditor(typeof(CinemachineNewFreeLook))]
    sealed class CinemachineNewFreeLookEditor
        : CinemachineVirtualCameraBaseEditor<CinemachineNewFreeLook>
    {
        GUIContent[] m_OrbitNames = new GUIContent[]
            { new GUIContent("Top Rig"), new GUIContent("Main Rig"), new GUIContent("Bottom Rig") };

        GUIContent m_CustomizeLabel = new GUIContent(
            "Customize", "Custom settings for this rig.  If unchecked, main rig settins will be used");

        VcamStageEditorPipeline m_PipelineSet = new VcamStageEditorPipeline();

        /// <summary>Get the property names to exclude in the inspector.</summary>
        /// <param name="excluded">Add the names to this list</param>
        protected override void GetExcludedPropertiesInInspector(List<string> excluded)
        {
            base.GetExcludedPropertiesInInspector(excluded);
            excluded.Add(FieldPath(x => x.m_Rigs)); // can't use HideInInspector for this
            excluded.Add(FieldPath(x => x.m_Orbits));
            excluded.Add(FieldPath(x => x.m_SplineCurvature));
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Undo.undoRedoPerformed += ResetTargetOnUndo;
            m_PipelineSet.Initialize(
                // GetComponent
                (stage, result) =>
                {
                    int numNullComponents = 0;
                    foreach (var obj in targets)
                    {
                        var vcam = obj as CinemachineNewVirtualCamera;
                        if (vcam != null)
                        {
                            var c = vcam.GetCinemachineComponent(stage);
                            if (c != null)
                                result.Add(c);
                            else
                                ++numNullComponents;
                        }
                    }
                    return numNullComponents;
                },
                // SetComponent
                (stage, type) => 
                {
                    Undo.SetCurrentGroupName("Cinemachine pipeline change");
                    foreach (var obj in targets)
                    {
                        var vcam = obj as CinemachineNewVirtualCamera;
                        if (vcam != null)
                        {
                            Component c = vcam.GetCinemachineComponent(stage);
                            if (c != null && c.GetType() == type)
                                continue;
                            if (c != null)
                            {
                                Undo.DestroyObjectImmediate(c);
                                vcam.InvalidateComponentCache();
                            }
                            if (type != null)
                            {
                                Undo.AddComponent(vcam.gameObject, type);
                                vcam.InvalidateComponentCache();
                            }
                        }
                    }
                });

            m_PipelineSet.SetStageIsLocked(CinemachineCore.Stage.Body);

            for (int i = 0; i < targets.Length; ++i)
                (targets[i] as CinemachineNewFreeLook).UpdateInputAxisProvider();
            
#if UNITY_2021_2_OR_NEWER
            CinemachineSceneToolUtility.RegisterTool(typeof(FoVTool));
            CinemachineSceneToolUtility.RegisterTool(typeof(FarNearClipTool));
            CinemachineSceneToolUtility.RegisterTool(typeof(FollowOffsetTool));
#endif
        }

        protected override void OnDisable()
        {
            m_PipelineSet.Shutdown();
            base.OnDisable();
            
#if UNITY_2021_2_OR_NEWER
            CinemachineSceneToolUtility.UnregisterTool(typeof(FoVTool));
            CinemachineSceneToolUtility.UnregisterTool(typeof(FarNearClipTool));
            CinemachineSceneToolUtility.UnregisterTool(typeof(FollowOffsetTool));
#endif
        }

        void ResetTargetOnUndo() 
        {
            ResetTarget();
        }

        public override void OnInspectorGUI()
        {
            // Ordinary properties
            BeginInspector();
            DrawHeaderInInspector();
            DrawPropertyInInspector(FindProperty(x => x.m_Priority));
            DrawTargetsInInspector(FindProperty(x => x.m_Follow), FindProperty(x => x.m_LookAt));
            DrawPropertyInInspector(FindProperty(x => x.m_StandbyUpdate));
            DrawLensSettingsInInspector(FindProperty(x => x.m_Lens));
            DrawRemainingPropertiesInInspector();

            // Orbits
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Orbits", EditorStyles.boldLabel);
            SerializedProperty orbits = FindProperty(x => x.m_Orbits);
            EditorGUI.BeginChangeCheck();
            for (int i = 0; i < 3; ++i)
            {
                var o = orbits.GetArrayElementAtIndex(i);
                Rect rect = EditorGUILayout.GetControlRect(true);
                InspectorUtility.MultiPropertyOnLine(
                    rect, m_OrbitNames[i],
                    new [] { o.FindPropertyRelative(() => Target.m_Orbits[i].m_Height),
                            o.FindPropertyRelative(() => Target.m_Orbits[i].m_Radius) },
                    null);
            }
            EditorGUILayout.PropertyField(FindProperty(x => x.m_SplineCurvature));
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            // Pipeline Stages
            EditorGUILayout.Space();
            var selectedRig = Selection.objects.Length == 1 
                ? GUILayout.Toolbar(GetSelectedRig(Target), s_RigNames) : 0;
            SetSelectedRig(Target, selectedRig);
            EditorGUILayout.BeginVertical(GUI.skin.box);
            if (selectedRig == 1)
                m_PipelineSet.OnInspectorGUI(false);
            else
                DrawRigEditor(selectedRig == 0 ? 0 : 1);
            EditorGUILayout.EndVertical();

            // Extensions
            DrawExtensionsWidgetInInspector();
        }

        static GUIContent[] s_RigNames = 
        {
            new GUIContent("Top Rig"), 
            new GUIContent("Main Rig"), 
            new GUIContent("Bottom Rig")
        };

        static int GetSelectedRig(CinemachineNewFreeLook freelook)
        {
            return freelook.m_VerticalAxis.Value < 0.33f ? 2 : (freelook.m_VerticalAxis.Value > 0.66f ? 0 : 1);
        }

        static void SetSelectedRig(CinemachineNewFreeLook freelook, int rigIndex)
        {
            Debug.Assert(rigIndex >= 0 && rigIndex < 3);
            if (GetSelectedRig(freelook) != rigIndex)
            {
                var prop = new SerializedObject(freelook).FindProperty(
                    () => freelook.m_VerticalAxis).FindPropertyRelative(() => freelook.m_VerticalAxis.Value);
                prop.floatValue = rigIndex == 0 ? 1 : (rigIndex == 1 ? 0.5f : 0);
                prop.serializedObject.ApplyModifiedProperties();
            }
        }
        
        void OnSceneGUI()
        {
            m_PipelineSet.OnSceneGUI(); 
            
#if UNITY_2021_2_OR_NEWER
            DrawSceneTools();
#endif
        }
        
#if UNITY_2021_2_OR_NEWER
        void DrawSceneTools()
        {
            var newFreelook = Target;
            if (newFreelook == null || !newFreelook.IsValid)
            {
                return;
            }

            if (CinemachineSceneToolUtility.IsToolActive(typeof(FoVTool)))
            {
                CinemachineSceneToolHelpers.FovToolHandle(newFreelook, 
                    new SerializedObject(newFreelook).FindProperty(() => newFreelook.m_Lens), 
                    newFreelook.m_Lens, IsHorizontalFOVUsed());
            }
            else if (CinemachineSceneToolUtility.IsToolActive(typeof(FarNearClipTool)))
            {
                CinemachineSceneToolHelpers.NearFarClipHandle(newFreelook,
                    new SerializedObject(newFreelook).FindProperty(() => newFreelook.m_Lens));
            }
            else if (newFreelook.Follow != null && CinemachineSceneToolUtility.IsToolActive(typeof(FollowOffsetTool)))
            {
                CinemachineSceneToolHelpers.OrbitControlHandle(newFreelook,
                    new SerializedObject(newFreelook).FindProperty(() => newFreelook.m_Orbits));
            }
        }
#endif

        void DrawRigEditor(int rigIndex)
        {
            const float kBoxMargin = 3;

            SerializedProperty rig = FindProperty(x => x.m_Rigs).GetArrayElementAtIndex(rigIndex);

            CinemachineNewFreeLook.Rig def = new CinemachineNewFreeLook.Rig(); // for properties
            EditorGUIUtility.labelWidth -= kBoxMargin;

            ++EditorGUI.indentLevel;
            var components = Target.ComponentCache;
            if (DrawFoldoutPropertyWithEnabledCheckbox(
                rig.FindPropertyRelative(() => def.m_CustomLens),
                rig.FindPropertyRelative(() => def.m_Lens)))
            {
                Target.m_Rigs[rigIndex].m_Lens = Target.m_Lens;
            }

            int index = (int)CinemachineCore.Stage.Body;
            if (components[index] is CinemachineTransposer)
            {
                if (DrawFoldoutPropertyWithEnabledCheckbox(
                    rig.FindPropertyRelative(() => def.m_CustomBody),
                    rig.FindPropertyRelative(() => def.m_Body)))
                {
                    Target.m_Rigs[rigIndex].m_Body.PullFrom(
                        components[index] as CinemachineTransposer);
                }
            }

            index = (int)CinemachineCore.Stage.Aim;
            if (components[index] is CinemachineComposer)
            {
                if (DrawFoldoutPropertyWithEnabledCheckbox(
                    rig.FindPropertyRelative(() => def.m_CustomAim),
                    rig.FindPropertyRelative(() => def.m_Aim)))
                {
                    Target.m_Rigs[rigIndex].m_Aim.PullFrom(
                        components[index] as CinemachineComposer);
                }
            }

            index = (int)CinemachineCore.Stage.Noise;
            if (components[index] is CinemachineBasicMultiChannelPerlin)
            {
                if (DrawFoldoutPropertyWithEnabledCheckbox(
                    rig.FindPropertyRelative(() => def.m_CustomNoise),
                    rig.FindPropertyRelative(() => def.m_Noise)))
                {
                    Target.m_Rigs[rigIndex].m_Noise.PullFrom(
                        components[index] as CinemachineBasicMultiChannelPerlin);
                }
            }
            --EditorGUI.indentLevel;
            EditorGUIUtility.labelWidth += kBoxMargin;
        }

        // Returns true if default value should be applied
        bool DrawFoldoutPropertyWithEnabledCheckbox(
            SerializedProperty enabledProperty, SerializedProperty property)
        {
            GUIContent label = new GUIContent(property.displayName, property.tooltip);
            Rect rect = EditorGUILayout.GetControlRect(true,
                (enabledProperty.boolValue && property.isExpanded)
                    ? EditorGUI.GetPropertyHeight(property)
                        : EditorGUIUtility.singleLineHeight);
            Rect r = rect; r.height = EditorGUIUtility.singleLineHeight;
            if (!enabledProperty.boolValue)
                EditorGUI.LabelField(r, label);

            float labelWidth = EditorGUIUtility.labelWidth;
            bool newValue = EditorGUI.ToggleLeft(
                new Rect(labelWidth, r.y, r.width - labelWidth, r.height),
                m_CustomizeLabel, enabledProperty.boolValue);
            if (newValue != enabledProperty.boolValue)
            {
                enabledProperty.boolValue = newValue;
                enabledProperty.serializedObject.ApplyModifiedProperties();
                property.isExpanded = newValue;
                return true;
            }
            if (newValue == true)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.PropertyField(rect, property, property.isExpanded);
                if (EditorGUI.EndChangeCheck())
                    enabledProperty.serializedObject.ApplyModifiedProperties();
            }
            return false;
        }

        [DrawGizmo(GizmoType.Active | GizmoType.Selected, typeof(CinemachineNewFreeLook))]
        private static void DrawFreeLookGizmos(CinemachineNewFreeLook vcam, GizmoType selectionType)
        {
            // Standard frustum and logo
            CinemachineBrainEditor.DrawVirtualCameraBaseGizmos(vcam, selectionType);

            Color originalGizmoColour = Gizmos.color;
            bool isActiveVirtualCam = CinemachineCore.Instance.IsLive(vcam);
            Gizmos.color = isActiveVirtualCam
                ? CinemachineSettings.CinemachineCoreSettings.ActiveGizmoColour
                : CinemachineSettings.CinemachineCoreSettings.InactiveGizmoColour;

            if (vcam.Follow != null)
            {
                Vector3 pos = vcam.Follow.position;
                Vector3 up = Vector3.up;
                CinemachineBrain brain = CinemachineCore.Instance.FindPotentialTargetBrain(vcam);
                if (brain != null)
                    up = brain.DefaultWorldUp;

                var middleRig = vcam.GetComponent<CinemachineTransposer>();
                if (middleRig != null)
                {
                    float scale = vcam.m_RadialAxis.Value;
                    Quaternion orient = middleRig.GetReferenceOrientation(up);
                    up = orient * Vector3.up;
                    var orbital = middleRig as CinemachineOrbitalTransposer;
                    if (orbital != null)
                    {
                        float rotation = orbital.m_XAxis.Value + orbital.m_Heading.m_Bias;
                        orient = Quaternion.AngleAxis(rotation, up) * orient;
                    }
                    CinemachineOrbitalTransposerEditor.DrawCircleAtPointWithRadius(
                        pos + up * vcam.m_Orbits[0].m_Height * scale,
                        orient, vcam.m_Orbits[0].m_Radius * scale);
                    CinemachineOrbitalTransposerEditor.DrawCircleAtPointWithRadius(
                        pos + up * vcam.m_Orbits[1].m_Height * scale,
                        orient, vcam.m_Orbits[1].m_Radius * scale);
                    CinemachineOrbitalTransposerEditor.DrawCircleAtPointWithRadius(
                        pos + up * vcam.m_Orbits[2].m_Height * scale,
                        orient, vcam.m_Orbits[2].m_Radius * scale);

                    DrawCameraPath(pos, orient, vcam);
                }
            }
            Gizmos.color = originalGizmoColour;
        }

        private static void DrawCameraPath(
            Vector3 atPos, Quaternion orient, CinemachineNewFreeLook vcam)
        {
            Matrix4x4 prevMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(atPos, orient, Vector3.one);

            const int kNumSteps = 20;
            Vector3 currPos = vcam.GetLocalPositionForCameraFromInput(0f);
            for (int i = 1; i < kNumSteps + 1; ++i)
            {
                float t = (float)i / (float)kNumSteps;
                Vector3 nextPos = vcam.GetLocalPositionForCameraFromInput(t);
                Gizmos.DrawLine(currPos, nextPos);
                currPos = nextPos;
            }
            Gizmos.matrix = prevMatrix;
        }
    }
}
#endif
