using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine.Utility;

#if CINEMACHINE_UNITY_INPUTSYSTEM
using UnityEngine.InputSystem;
#endif

#if CINEMACHINE_HDRP || CINEMACHINE_LWRP_7_3_1
    #if CINEMACHINE_HDRP_7_3_1
        using UnityEngine.Rendering.HighDefinition;
    #else
        #if CINEMACHINE_LWRP_7_3_1
            using UnityEngine.Rendering.Universal;
        #else
            using UnityEngine.Experimental.Rendering.HDPipeline;
        #endif
    #endif
#endif

#if UNITY_2019_1_OR_NEWER
    using CameraExtensions = UnityEngine.Camera;
#else
    // Needed only for Unity pre-2019.1 because Camera doesn't have these methods
    static class CameraExtensions
    {
        public static float HorizontalToVerticalFieldOfView(float f, float aspect)
        {
            return Mathf.Rad2Deg * 2 * Mathf.Atan(Mathf.Tan(f * Mathf.Deg2Rad * 0.5f) / aspect);
        }

        public static float VerticalToHorizontalFieldOfView(float f, float aspect)
        {
            return Mathf.Rad2Deg * 2 * Mathf.Atan(Mathf.Tan(f * Mathf.Deg2Rad * 0.5f) * aspect);
        }

        public static float FieldOfViewToFocalLength(float fov, float sensorHeight)
        {
            return sensorHeight * 0.5f / Mathf.Tan(Mathf.Deg2Rad * fov * 0.5f);
        }

        public static float FocalLengthToFieldOfView(float focalLength, float sensorHeight)
        {
            if (focalLength < UnityVectorExtensions.Epsilon)
                return 180f;
            return Mathf.Rad2Deg * 2.0f * Mathf.Atan(sensorHeight * 0.5f / focalLength);
        }
    }
#endif


namespace Cinemachine.Editor
{
    /// <summary>
    /// Base class for virtual camera editors.
    /// Handles drawing the header and the basic properties.
    /// </summary>
    /// <typeparam name="T">The type of CinemachineVirtualCameraBase being edited</typeparam>
    public class CinemachineVirtualCameraBaseEditor<T> : BaseEditor<T> where T : CinemachineVirtualCameraBase
    {    
        /// <summary>A collection of GUIContent for use in the inspector</summary>
        public static class Styles
        {
            /// <summary>GUIContent for Add Extension</summary>
            public static GUIContent addExtensionLabel = new GUIContent("Add Extension");
            /// <summary>GUIContent for no-multi-select message</summary>
            public static GUIContent virtualCameraChildrenInfoMsg 
                = new GUIContent("The Virtual Camera Children field is not available when multiple objects are selected.");
        }
        
        static Type[] sExtensionTypes;  // First entry is null
        static string[] sExtensionNames;
        bool IsPrefabBase { get; set; }

        /// <summary>Obsolete, do not use.  Use the overload, which is more performant</summary>
        /// <returns>List of property names to exclude</returns>
        protected override List<string> GetExcludedPropertiesInInspector() 
            { return base.GetExcludedPropertiesInInspector(); }

        /// <summary>Get the property names to exclude in the inspector.  
        /// Implementation should call the base class implementation</summary>
        /// <param name="excluded">Add the names to this list</param>
        protected override void GetExcludedPropertiesInInspector(List<string> excluded)
        {
            base.GetExcludedPropertiesInInspector(excluded);
            if (Target.m_ExcludedPropertiesInInspector != null)
                excluded.AddRange(Target.m_ExcludedPropertiesInInspector);
        }

        /// <summary>Update state information on undo/redo</summary>
        void UpdateCameraState() => Target.InternalUpdateCameraState(Vector3.up, -1);
        

        /// <summary>Inspector panel is being enabled.  
        /// Implementation should call the base class implementation</summary>
        protected virtual void OnEnable()
        {
            Undo.undoRedoPerformed += UpdateCameraState;
            
            IsPrefabBase = Target.gameObject.scene.name == null; // causes a small GC alloc
            if (sExtensionTypes == null)
            {
                // Populate the extension list
                List<Type> exts = new List<Type>();
                List<string> names = new List<string>();
                exts.Add(null);
                names.Add("(select)");
                var allExtensions
                    = ReflectionHelpers.GetTypesInAllDependentAssemblies(
                            (Type t) => typeof(CinemachineExtension).IsAssignableFrom(t) && !t.IsAbstract);
                foreach (Type t in allExtensions)
                {
                    exts.Add(t);
                    names.Add(t.Name);
                }
                sExtensionTypes = exts.ToArray();
                sExtensionNames = names.ToArray();
            }
        }

        /// <summary>Inspector panel is being disabled.
        /// Implementation should call the base class implementation</summary>
        protected virtual void OnDisable()
        {
            Undo.undoRedoPerformed -= UpdateCameraState;
            
            if (CinemachineBrain.SoloCamera == (ICinemachineCamera)Target)
            {
                CinemachineBrain.SoloCamera = null;
                InspectorUtility.RepaintGameView();
            }
        }

        /// <summary>Create the contents of the inspector panel.
        /// This implementation draws header and Extensions widget, and uses default algorithms 
        /// to draw the properties in the inspector</summary>
        public override void OnInspectorGUI()
        {
            BeginInspector();
            DrawHeaderInInspector();
            DrawRemainingPropertiesInInspector();
            DrawExtensionsWidgetInInspector();
        }

        /// <summary>
        /// Draw the virtual camera header in the inspector.  
        /// This includes Solo button, Live status, and global settings
        /// </summary>
        protected void DrawHeaderInInspector()
        {
            if (!IsPropertyExcluded("Header"))
            {
                DrawCameraStatusInInspector();
                DrawGlobalControlsInInspector();
#if CINEMACHINE_UNITY_INPUTSYSTEM
                DrawInputProviderButtonInInspector();
#endif
                ExcludeProperty("Header");
            }
        }
        
#if CINEMACHINE_UNITY_INPUTSYSTEM
        static GUIContent s_InputProviderAddLabel = new GUIContent("Add Input Provider", 
            "Adds CinemachineInputProvider component to this vcam, "
            + "if it does not have one, enabling the vcam to read input from Input Actions. "
            + "By default, a simple mouse XY input action is added.");

        static InputActionReference s_InputActionReference = null;

        void DrawInputProviderButtonInInspector()
        {
            bool needsButton = false;
            for (int i = 0; !needsButton && i < targets.Length; ++i)
            {
                var vcam = targets[i] as CinemachineVirtualCameraBase;
                if (vcam.RequiresUserInput() && vcam.GetComponent<AxisState.IInputAxisProvider>() == null)
                    needsButton = true;
            }
            if (!needsButton)
                return;

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(
                "The InputSystem package is installed, but it is not used to control this vcam.", 
                MessageType.Info);
            var rect = EditorGUILayout.GetControlRect(true);
            rect.x += EditorGUIUtility.labelWidth; 
            rect.width -= EditorGUIUtility.labelWidth;
            if (GUI.Button(rect, s_InputProviderAddLabel))
            {
                if (s_InputActionReference == null)
                {
                    s_InputActionReference = (InputActionReference)AssetDatabase.LoadAllAssetsAtPath(
                            "Packages/com.unity.inputsystem/InputSystem/Plugins/PlayerInput/DefaultInputActions.inputactions").
                        FirstOrDefault(x => x.name == "Player/Look");
                }
                Undo.SetCurrentGroupName("Add CinemachineInputProvider");
                for (int i = 0; i < targets.Length; ++i)
                {
                    var vcam = targets[i] as CinemachineVirtualCameraBase;
                    if (vcam.GetComponent<AxisState.IInputAxisProvider>() != null)
                        continue;
                    var inputProvider = Undo.AddComponent<CinemachineInputProvider>(vcam.gameObject);
                    inputProvider.XYAxis = s_InputActionReference;
                }
            }
            EditorGUILayout.Space();
        }
#endif

        /// <summary>
        /// Draw the LookAt and Follow targets in the inspector
        /// </summary>
        /// <param name="followTarget">Follow target SerializedProperty</param>
        /// <param name="lookAtTarget">LookAt target SerializedProperty</param>
        protected void DrawTargetsInInspector(
            SerializedProperty followTarget, SerializedProperty lookAtTarget)
        {
            EditorGUI.BeginChangeCheck();
            if (!IsPropertyExcluded(followTarget.name))
            {
                if (Target.ParentCamera == null || Target.ParentCamera.Follow == null)
                    EditorGUILayout.PropertyField(followTarget);
                else
                    EditorGUILayout.PropertyField(followTarget,
                        new GUIContent(followTarget.displayName + " Override"));
                ExcludeProperty(followTarget.name);
            }
            if (!IsPropertyExcluded(lookAtTarget.name))
            {
                if (Target.ParentCamera == null || Target.ParentCamera.LookAt == null)
                    EditorGUILayout.PropertyField(lookAtTarget);
                else
                    EditorGUILayout.PropertyField(lookAtTarget,
                        new GUIContent(lookAtTarget.displayName + " Override"));
                ExcludeProperty(lookAtTarget.name);
            }
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Draw the Extensions dropdown in the inspector
        /// </summary>
        protected void DrawExtensionsWidgetInInspector()
        {
            if (!IsPropertyExcluded("Extensions"))
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Extensions", EditorStyles.boldLabel);
                Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
                rect = EditorGUI.PrefixLabel(rect, Styles.addExtensionLabel);

                int selection = EditorGUI.Popup(rect, 0, sExtensionNames);
                if (selection > 0)
                {
                    Type extType = sExtensionTypes[selection];
                    for (int i = 0; i < targets.Length; i++)
                    {
                        var targetGO = (targets[i] as CinemachineVirtualCameraBase).gameObject;
                        if (targetGO != null && targetGO.GetComponent(extType) == null)
                            Undo.AddComponent(targetGO, extType);
                    }
                }
                ExcludeProperty("Extensions");
            }
        }

        /// <summary>
        /// Draw the Live status in the inspector, and the Solo button
        /// </summary>
        protected void DrawCameraStatusInInspector()
        {
            if (Selection.objects.Length > 1)
                return;
            
            // Is the camera navel-gazing?
            CameraState state = Target.State;
            if (state.HasLookAt && (state.ReferenceLookAt - state.CorrectedPosition).AlmostZero())
                EditorGUILayout.HelpBox(
                    "The camera is positioned on the same point at which it is trying to look.",
                    MessageType.Warning);

            // No status and Solo for prefabs
            if (IsPrefabBase)
                return;

            // Active status and Solo button
            Rect rect = EditorGUILayout.GetControlRect(true);
            Rect rectLabel = new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, rect.height);
            rect.width -= rectLabel.width;
            rect.x += rectLabel.width;

            Color color = GUI.color;
            bool isSolo = (CinemachineBrain.SoloCamera == (ICinemachineCamera)Target);
            if (isSolo)
                GUI.color = CinemachineBrain.GetSoloGUIColor();

            bool isLive = CinemachineCore.Instance.IsLive(Target);
            GUI.enabled = isLive;
            GUI.Label(rectLabel, isLive ? "Status: Live"
                : (Target.isActiveAndEnabled ? "Status: Standby" : "Status: Disabled"));
            GUI.enabled = true;

            float labelWidth = 0;
            GUIContent updateText = GUIContent.none;
            UpdateTracker.UpdateClock updateMode = CinemachineCore.Instance.GetVcamUpdateStatus(Target);
            if (Application.isPlaying)
            {
                updateText = new GUIContent(
                    updateMode == UpdateTracker.UpdateClock.Fixed ? " Fixed Update" : " Late Update");
                var textDimensions = GUI.skin.label.CalcSize(updateText);
                labelWidth = textDimensions.x;
            }
            rect.width -= labelWidth;
            if (GUI.Button(rect, "Solo", "Button"))
            {
                isSolo = !isSolo;
                CinemachineBrain.SoloCamera = isSolo ? Target : null;
                InspectorUtility.RepaintGameView();
            }
            GUI.color = color;
            if (isSolo && !Application.isPlaying)
                InspectorUtility.RepaintGameView();

            if (labelWidth > 0)
            {
                GUI.enabled = false;
                rect.x += rect.width; rect.width = labelWidth;
                GUI.Label(rect, updateText);
                GUI.enabled = true;
            }
        }

        static GUIContent ShowInGameGuidesLabel = new GUIContent(
            "Game Window Guides",
            "Enable the display of overlays in the Game window.  "
                + "You can adjust colours and opacity in Cinemachine Preferences.");

        static GUIContent SaveDuringPlayLabel = new GUIContent(
            "Save During Play",
            "If checked, Virtual Camera settings changes made during Play Mode "
                + "will be propagated back to the scene when Play Mode is exited.");

        /// <summary>
        /// Draw the global settings controls in the inspector
        /// </summary>
        protected void DrawGlobalControlsInInspector()
        {
            CinemachineSettings.CinemachineCoreSettings.ShowInGameGuides
                = EditorGUILayout.Toggle(ShowInGameGuidesLabel,
                    CinemachineSettings.CinemachineCoreSettings.ShowInGameGuides);

            SaveDuringPlay.SaveDuringPlay.Enabled
                = EditorGUILayout.Toggle(SaveDuringPlayLabel, SaveDuringPlay.SaveDuringPlay.Enabled);

            if (Application.isPlaying && SaveDuringPlay.SaveDuringPlay.Enabled)
                EditorGUILayout.HelpBox(
                    " Virtual Camera settings changes made during Play Mode will be "
                        + "propagated back to the scene when Play Mode is exited.",
                    MessageType.Info);
        }

        LensSettingsInspectorHelper m_LensSettingsInspectorHelper;
        internal bool IsHorizontalFOVUsed() => 
            m_LensSettingsInspectorHelper != null && m_LensSettingsInspectorHelper.UseHorizontalFOV;

        /// <summary>
        /// Draw the Lens Settings controls in the inspector
        /// </summary>
        /// <param name="property">The SerializedProperty for the field of type LensSettings field</param>
        protected void DrawLensSettingsInInspector(SerializedProperty property)
        {
            if (IsPropertyExcluded(property.name))
                return;
            if (m_LensSettingsInspectorHelper == null)
                m_LensSettingsInspectorHelper = new LensSettingsInspectorHelper();

            // This should be a global UX setting, but the best we can do now is to query 
            // the associated camera (if any) for the lens FOV display mode
            Camera camera = null;
            var brain = CinemachineCore.Instance.FindPotentialTargetBrain(Target);
            if (brain != null)
                camera = brain.OutputCamera;
            
            m_LensSettingsInspectorHelper.SnapshotCameraShadowValues(property, camera);

            m_LensSettingsInspectorHelper.DrawLensSettingsInInspector(property);
            ExcludeProperty(property.name);
        }
    }

    // Helper for drawing lensSettings in inspector
    class LensSettingsInspectorHelper
    {
        GUIContent[] m_PresetOptions;
        GUIContent[] m_PhysicalPresetOptions;
        LensSettings m_LensSettingsDef = new LensSettings(); // to access name strings

        static readonly GUIContent EditPresetsLabel = new GUIContent("Edit Presets...");
        static readonly GUIContent LensLabel = new GUIContent("Lens", "Lens settings to apply to the camera");
        static readonly GUIContent HFOVLabel = new GUIContent("Horizontal FOV", "Horizontal Field of View");
        static readonly GUIContent VFOVLabel = new GUIContent("Vertical FOV", "Vertical Field of View");
        static readonly GUIContent FocalLengthLabel = new GUIContent("Focal Length", "The length of the lens (in mm)");
        static readonly GUIContent OrthoSizeLabel = new GUIContent("Ortho Size", "When using an orthographic camera, "
            + "this defines the half-height, in world coordinates, of the camera view.");
        static readonly GUIContent SensorSizeLabel = new GUIContent("Sensor Size", 
            "Actual size of the image sensor (in mm), used to "
            + "convert between focal length and field of vue.");
        static readonly GUIContent AdvancedLabel = new GUIContent("Advanced");

        bool IsOrtho;
        bool IsPhysical;
        Vector2 SensorSize;
        internal bool UseHorizontalFOV;
        static bool s_AdvancedExpanded;
        SerializedProperty ModeOverrideProperty;

    #if CINEMACHINE_HDRP
        GUIContent PhysicalPropertiesLabel;
        static bool mPhysicalExapnded;
    #endif

        public LensSettingsInspectorHelper() 
        {
    #if CINEMACHINE_HDRP
            PhysicalPropertiesLabel = new GUIContent("Physical Properties", "Physical properties of the lens");
    #endif

            var options = new List<GUIContent>();
            CinemachineLensPresets presets = CinemachineLensPresets.InstanceIfExists;
            for (int i = 0; presets != null && i < presets.m_Presets.Length; ++i)
                options.Add(new GUIContent(presets.m_Presets[i].m_Name));
            options.Add(EditPresetsLabel);
            m_PresetOptions = options.ToArray();

            options.Clear();
            for (int i = 0; presets != null && i < presets.m_PhysicalPresets.Length; ++i)
                options.Add(new GUIContent(presets.m_PhysicalPresets[i].m_Name));
            options.Add(EditPresetsLabel);
            m_PhysicalPresetOptions = options.ToArray();
        }

        public void SnapshotCameraShadowValues(SerializedProperty property, Camera camera)
        {
            ModeOverrideProperty = property.FindPropertyRelative(() => m_LensSettingsDef.ModeOverride);

            // Assume lens is up-to-date
            UseHorizontalFOV = false;
            var lensObject = SerializedPropertyHelper.GetPropertyValue(property);
            IsOrtho = AccessProperty<bool>(typeof(LensSettings), lensObject, "Orthographic");
            IsPhysical = AccessProperty<bool>(typeof(LensSettings), lensObject, "IsPhysicalCamera");
            SensorSize = AccessProperty<Vector2>(typeof(LensSettings), lensObject, "SensorSize");

            // Then pull from actual camera if appropriate
            if (camera != null)
            {
#if UNITY_2019_1_OR_NEWER
                // This should really be a global setting, but for now there is no better way than this!
                var p = new SerializedObject(camera).FindProperty("m_FOVAxisMode");
                if (p != null && p.intValue == (int)Camera.FieldOfViewAxis.Horizontal)
                    UseHorizontalFOV = true;
#endif
                // It's possible that the lens isn't synched with its camera - fix that here
                if (ModeOverrideProperty.intValue == (int)LensSettings.OverrideModes.None)
                {
                    IsOrtho = camera.orthographic;
                    IsPhysical = camera.usePhysicalProperties;
                    SensorSize = IsPhysical ? camera.sensorSize : new Vector2(camera.aspect, 1f);
                }
            }
            
            var nearClipPlaneProperty = property.FindPropertyRelative("NearClipPlane");
            if (!IsOrtho)
            {
                nearClipPlaneProperty.floatValue = Mathf.Max(nearClipPlaneProperty.floatValue, 0.01f);
                property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        static T AccessProperty<T>(Type type, object obj, string memberName)
        {
            if (string.IsNullOrEmpty(memberName) || (type == null))
                return default(T);

            System.Reflection.BindingFlags bindingFlags = System.Reflection.BindingFlags.Public;
            if (obj != null)
                bindingFlags |= System.Reflection.BindingFlags.Instance;
            else
                bindingFlags |= System.Reflection.BindingFlags.Static;

            System.Reflection.PropertyInfo pi = type.GetProperty(memberName, bindingFlags);
            if ((pi != null) && (pi.PropertyType == typeof(T)))
                return (T)pi.GetValue(obj, null);
            else
                return default(T);
        }
        
        public void DrawLensSettingsInInspector(SerializedProperty property)
        {
            Rect rect = EditorGUILayout.GetControlRect(true);

            var lensLabelWidth = GUI.skin.label.CalcSize(LensLabel).x;
            var foldoutLabelWidth = lensLabelWidth;

            property.isExpanded = EditorGUI.Foldout(
                new Rect(rect.x, rect.y, foldoutLabelWidth, rect.height),
                property.isExpanded, LensLabel, true);

            if (!property.isExpanded)
            {
                // Put the FOV on the same line
                var oldLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth -= foldoutLabelWidth;
                rect.x += foldoutLabelWidth; rect.width -= foldoutLabelWidth; 
                DrawLensFocusInInspector(rect, property);
                EditorGUIUtility.labelWidth = oldLabelWidth;
            }
            else
            {
                ++EditorGUI.indentLevel;

                rect = EditorGUILayout.GetControlRect(true);
                DrawLensFocusInInspector(rect, property);

                EditorGUILayout.PropertyField(property.FindPropertyRelative(() => m_LensSettingsDef.NearClipPlane));
                EditorGUILayout.PropertyField(property.FindPropertyRelative(() => m_LensSettingsDef.FarClipPlane));

                if (IsPhysical)
                {
#if CINEMACHINE_HDRP
                    mPhysicalExapnded = EditorGUILayout.Foldout(mPhysicalExapnded, PhysicalPropertiesLabel, true);
                    if (mPhysicalExapnded)
                    {
                        ++EditorGUI.indentLevel;
                        EditorGUILayout.PropertyField(property.FindPropertyRelative(() => m_LensSettingsDef.Aperture));
                        EditorGUILayout.PropertyField(property.FindPropertyRelative(() => m_LensSettingsDef.Iso));
                        EditorGUILayout.PropertyField(property.FindPropertyRelative(() => m_LensSettingsDef.ShutterSpeed));
                        EditorGUILayout.PropertyField(property.FindPropertyRelative(() => m_LensSettingsDef.BladeCount));

                        rect = EditorGUILayout.GetControlRect(true);
                        var curvature = property.FindPropertyRelative(() => m_LensSettingsDef.Curvature);
                        using (var propertyScope = new EditorGUI.PropertyScope(rect, new GUIContent("Curvature"), curvature))
                        {
                            var v = curvature.vector2Value;

                            // The layout system breaks alignment when mixing inspector fields with custom layout'd
                            // fields as soon as a scrollbar is needed in the inspector, so we'll do the layout
                            // manually instead
                            const int kFloatFieldWidth = 50;
                            const int kSeparatorWidth = 5;
                            float indentOffset = EditorGUI.indentLevel * 15f;
                            var labelRect = new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth - indentOffset, rect.height);
                            var floatFieldLeft = new Rect(labelRect.xMax, rect.y, kFloatFieldWidth + indentOffset, rect.height);
                            var sliderRect = new Rect(floatFieldLeft.xMax + kSeparatorWidth - indentOffset, rect.y, rect.width - labelRect.width - kFloatFieldWidth * 2 - kSeparatorWidth * 2, rect.height);
                            var floatFieldRight = new Rect(sliderRect.xMax + kSeparatorWidth - indentOffset, rect.y, kFloatFieldWidth + indentOffset, rect.height);

                            EditorGUI.PrefixLabel(labelRect, propertyScope.content);
                            v.x = EditorGUI.FloatField(floatFieldLeft, v.x);
                            EditorGUI.MinMaxSlider(sliderRect, ref v.x, ref v.y, HDPhysicalCamera.kMinAperture, HDPhysicalCamera.kMaxAperture);
                            v.y = EditorGUI.FloatField(floatFieldRight, v.y);

                            curvature.vector2Value = v;
                        }

                        EditorGUILayout.PropertyField(property.FindPropertyRelative(() => m_LensSettingsDef.BarrelClipping));
                        EditorGUILayout.PropertyField(property.FindPropertyRelative(() => m_LensSettingsDef.Anamorphism));

                        DrawSensorSizeInInspector(property);
                        EditorGUILayout.PropertyField(property.FindPropertyRelative(() => m_LensSettingsDef.LensShift));
                        if (ModeOverrideProperty.intValue != (int)LensSettings.OverrideModes.None)
                            EditorGUILayout.PropertyField(property.FindPropertyRelative(() => m_LensSettingsDef.GateFit));

                        --EditorGUI.indentLevel;
                    }
#else
                    DrawSensorSizeInInspector(property);
                    EditorGUILayout.PropertyField(property.FindPropertyRelative(() => m_LensSettingsDef.LensShift));
                    if (ModeOverrideProperty.intValue != (int)LensSettings.OverrideModes.None)
                        EditorGUILayout.PropertyField(property.FindPropertyRelative(() => m_LensSettingsDef.GateFit));
#endif
                }
                EditorGUILayout.PropertyField(property.FindPropertyRelative(() => m_LensSettingsDef.Dutch));
                s_AdvancedExpanded = EditorGUILayout.Foldout(s_AdvancedExpanded, AdvancedLabel);
                if (s_AdvancedExpanded)
                {
                    ++EditorGUI.indentLevel;
                    EditorGUILayout.HelpBox("Setting a mode override here implies changes to the Camera component when "
                        + "Cinemachine activates this Virtual Camera, and the changes will remain after the Virtual "
                        + "Camera deactivation. If you set a mode override in any Virtual Camera, you should set "
                        + "one in all Virtual Cameras.", MessageType.Info);
                    EditorGUILayout.PropertyField(ModeOverrideProperty);
                    --EditorGUI.indentLevel;
                }
                --EditorGUI.indentLevel;
            }
            property.serializedObject.ApplyModifiedProperties();
        }

        static float ExtraSpaceHackWTF() { return EditorGUI.indentLevel * (EditorGUIUtility.singleLineHeight - 3); }

        void DrawSensorSizeInInspector(SerializedProperty property)
        {
            if (ModeOverrideProperty.intValue != (int)LensSettings.OverrideModes.None)
            {
                property = property.FindPropertyRelative("m_SensorSize");
                var rect = EditorGUILayout.GetControlRect(true);
                EditorGUI.BeginProperty(rect, SensorSizeLabel, property);
                var v = EditorGUI.Vector2Field(rect, SensorSizeLabel, property.vector2Value);
                v.x = Mathf.Max(v.x, 0.1f);
                v.y = Mathf.Max(v.y, 0.1f);
                property.vector2Value = v;
                EditorGUI.EndProperty();
            }
        }

        void DrawLensFocusInInspector(Rect rect, SerializedProperty property)
        {
            if (IsOrtho)
                EditorGUI.PropertyField(
                    rect, property.FindPropertyRelative(() => m_LensSettingsDef.OrthographicSize), OrthoSizeLabel);
            else if (IsPhysical)
                DrawFocalLengthControl(rect, property, FocalLengthLabel);
            else
                DrawFOVControl(rect, property);
        }

        void DrawFOVControl(Rect rect, SerializedProperty property)
        {
            const float hSpace = 2;
            var label = UseHorizontalFOV ? HFOVLabel : VFOVLabel;

            var FOVProperty = property.FindPropertyRelative(() => m_LensSettingsDef.FieldOfView);
            float aspect = SensorSize.x / SensorSize.y;

            float dropdownWidth = (rect.width - EditorGUIUtility.labelWidth) / 4;
            rect.width -= dropdownWidth + hSpace;

            float f = FOVProperty.floatValue;
            if (UseHorizontalFOV)
                f = CameraExtensions.VerticalToHorizontalFieldOfView(f, aspect);
            EditorGUI.BeginProperty(rect, label, FOVProperty);
            f = EditorGUI.FloatField(rect, label, f);
            if (UseHorizontalFOV)
                f = CameraExtensions.HorizontalToVerticalFieldOfView(Mathf.Clamp(f, 1, 179), aspect);
            if (!Mathf.Approximately(FOVProperty.floatValue, f))
                FOVProperty.floatValue = Mathf.Clamp(f, 1, 179);
            EditorGUI.EndProperty();
            rect.x += rect.width + hSpace; rect.width = dropdownWidth;

            CinemachineLensPresets presets = CinemachineLensPresets.InstanceIfExists;
            int preset = (presets == null) ? -1 : presets.GetMatchingPreset(FOVProperty.floatValue);
            rect.x -= ExtraSpaceHackWTF(); rect.width += ExtraSpaceHackWTF();
            int selection = EditorGUI.Popup(rect, GUIContent.none, preset, m_PresetOptions);
            if (selection == m_PresetOptions.Length-1 && CinemachineLensPresets.Instance != null)
                Selection.activeObject = presets = CinemachineLensPresets.Instance;
            else if (selection >= 0 && selection < m_PresetOptions.Length-1)
            {
                var vfov = presets.m_Presets[selection].m_FieldOfView;
                FOVProperty.floatValue = vfov;
                property.serializedObject.ApplyModifiedProperties();
            }
        }

        void DrawFocalLengthControl(Rect rect, SerializedProperty property, GUIContent label)
        {
            const float hSpace = 2;
            var FOVProperty = property.FindPropertyRelative(() => m_LensSettingsDef.FieldOfView);
            float dropdownWidth = (rect.width - EditorGUIUtility.labelWidth) / 4;
            rect.width -= dropdownWidth + hSpace;

            float f = CameraExtensions.FieldOfViewToFocalLength(FOVProperty.floatValue, SensorSize.y);
            EditorGUI.BeginProperty(rect, label, FOVProperty);
            f = EditorGUI.FloatField(rect, label, f);
            f = CameraExtensions.FocalLengthToFieldOfView(Mathf.Max(f, 0.0001f), SensorSize.y);
            if (!Mathf.Approximately(FOVProperty.floatValue, f))
                FOVProperty.floatValue = Mathf.Clamp(f, 1, 179);
            EditorGUI.EndProperty();

            rect.x += rect.width + hSpace; rect.width = dropdownWidth;

#if CINEMACHINE_HDRP
            CinemachineLensPresets presets = CinemachineLensPresets.InstanceIfExists;
            int preset = -1;
            if (presets != null)
            {
                var focalLength = CameraExtensions.FieldOfViewToFocalLength(FOVProperty.floatValue, SensorSize.y);
                var aperture = property.FindPropertyRelative(() => m_LensSettingsDef.Aperture).floatValue;
                var iso = property.FindPropertyRelative(() => m_LensSettingsDef.Iso).intValue;
                var shutterSpeed = property.FindPropertyRelative(() => m_LensSettingsDef.ShutterSpeed).floatValue;
                var bladeCount = property.FindPropertyRelative(() => m_LensSettingsDef.BladeCount).intValue;
                var curvature = property.FindPropertyRelative(() => m_LensSettingsDef.Curvature).vector2Value;
                var barrelClipping = property.FindPropertyRelative(() => m_LensSettingsDef.BarrelClipping).floatValue;
                var anamprphism = property.FindPropertyRelative(() => m_LensSettingsDef.Anamorphism).floatValue;
                var lensShift = property.FindPropertyRelative(() => m_LensSettingsDef.LensShift).vector2Value;

                preset = presets.GetMatchingPhysicalPreset(
                    focalLength, iso, shutterSpeed, aperture, bladeCount,
                    curvature, barrelClipping, anamprphism, lensShift);
            }
            rect.x -= ExtraSpaceHackWTF(); rect.width += ExtraSpaceHackWTF();
            int selection = EditorGUI.Popup(rect, GUIContent.none, preset, m_PhysicalPresetOptions);
            if (selection == m_PhysicalPresetOptions.Length-1 && CinemachineLensPresets.Instance != null)
                Selection.activeObject = presets = CinemachineLensPresets.Instance;
            else if (selection >= 0 && selection < m_PhysicalPresetOptions.Length-1)
            {
                var v = presets.m_PhysicalPresets[selection];
                FOVProperty.floatValue = CameraExtensions.FocalLengthToFieldOfView(v.m_FocalLength, SensorSize.y);
                property.FindPropertyRelative(() => m_LensSettingsDef.Aperture).floatValue = v.Aperture;
                property.FindPropertyRelative(() => m_LensSettingsDef.Iso).intValue = v.Iso;
                property.FindPropertyRelative(() => m_LensSettingsDef.ShutterSpeed).floatValue = v.ShutterSpeed;
                property.FindPropertyRelative(() => m_LensSettingsDef.BladeCount).intValue = v.BladeCount;
                property.FindPropertyRelative(() => m_LensSettingsDef.Curvature).vector2Value = v.Curvature;
                property.FindPropertyRelative(() => m_LensSettingsDef.BarrelClipping).floatValue = v.BarrelClipping;
                property.FindPropertyRelative(() => m_LensSettingsDef.Anamorphism).floatValue = v.Anamorphism;
                property.FindPropertyRelative(() => m_LensSettingsDef.LensShift).vector2Value = v.LensShift;
                property.serializedObject.ApplyModifiedProperties();
            }
#else
            CinemachineLensPresets presets = CinemachineLensPresets.InstanceIfExists;
            int preset = (presets == null) ? -1 : presets.GetMatchingPhysicalPreset(
                CameraExtensions.FieldOfViewToFocalLength(FOVProperty.floatValue, SensorSize.y));
            rect.x -= ExtraSpaceHackWTF(); rect.width += ExtraSpaceHackWTF();
            int selection = EditorGUI.Popup(rect, GUIContent.none, preset, m_PhysicalPresetOptions);
            if (selection == m_PhysicalPresetOptions.Length-1 && CinemachineLensPresets.Instance != null)
                Selection.activeObject = presets = CinemachineLensPresets.Instance;
            else if (selection >= 0 && selection < m_PhysicalPresetOptions.Length-1)
            {
                FOVProperty.floatValue = CameraExtensions.FocalLengthToFieldOfView(
                    presets.m_PhysicalPresets[selection].m_FocalLength, SensorSize.y);
                property.serializedObject.ApplyModifiedProperties();
            }
#endif
        }
    }
}

