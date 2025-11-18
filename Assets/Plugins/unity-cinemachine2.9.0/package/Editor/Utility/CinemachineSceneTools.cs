#if UNITY_2021_2_OR_NEWER
using System;
using System.Collections.Generic;
using Cinemachine.Utility;
using UnityEditor;
using UnityEngine;

namespace Cinemachine.Editor
{
    /// <summary>
    /// Static class that manages Cinemachine Tools. It knows which tool is active,
    /// and ensures that exclusive tools are not active at the same time.
    /// The tools and editors requiring tools register/unregister themselves here.
    /// </summary>
    static class CinemachineSceneToolUtility
    {
#if UNITY_2022_1_OR_NEWER
        /// <summary>
        /// Checks whether tool is the currently active exclusive tool.
        /// </summary>
        /// <param name="tool">Tool to check.</param>
        /// <returns>True, when the tool is the active exclusive tool. False, otherwise.</returns>
        public static bool IsToolActive(Type tool)
        {
            return s_ActiveExclusiveTool == tool;
        }
        static Type s_ActiveExclusiveTool;

        /// <summary>
        /// Register your Type from the editor script's OnEnable function.
        /// This way CinemachineTools will know which tools to display.
        /// </summary>
        /// <param name="tool">Tool to register</param>
        public static void RegisterTool(Type tool)
        {
            if (s_RequiredTools.ContainsKey(tool))
            {
                s_RequiredTools[tool]++;
            }
            else
            {
                s_RequiredTools.Add(tool, 1);
            }

            s_TriggerRefresh = true;
        }
        
        /// <summary>
        /// Unregister your Type from the editor script's OnDisable function.
        /// This way CinemachineTools will know which tools to display.
        /// </summary>
        /// <param name="tool">Tool to register</param>
        public static void UnregisterTool(Type tool)
        {
            if (s_RequiredTools.ContainsKey(tool))
            {
                s_RequiredTools[tool]--;
                if (s_RequiredTools[tool] <= 0)
                {
                    s_RequiredTools.Remove(tool);
                }
            }

            s_TriggerRefresh = true;
        }

        internal static bool IsToolRequired(Type tool)
        {
            return s_RequiredTools.ContainsKey(tool);
        }
        static Dictionary<Type, int> s_RequiredTools;
        
        internal static void SetTool(bool active, Type tool)
        {
            if (active)
            {
                s_ActiveExclusiveTool = tool;
            }
            else
            {
                s_ActiveExclusiveTool = s_ActiveExclusiveTool == tool ? null : s_ActiveExclusiveTool;
            }
            
            s_TriggerRefresh = true;
        }

        static CinemachineSceneToolUtility()
        {
            s_RequiredTools = new Dictionary<Type, int>();
            EditorApplication.update += RefreshToolbarHack;
        }

        // TODO: remove RefreshToolbarHack hack, when the Tools expose a public API to refresh it!
        static bool s_TriggerRefresh;
        static void RefreshToolbarHack()
        {
            if (s_TriggerRefresh)
            {
                foreach (var scene in SceneView.sceneViews)
                {
                    if (((SceneView)scene).TryGetOverlay("unity-transform-toolbar", out var tools))
                    {
                        if (tools.displayed)
                        {
                            tools.displayed = false;
                            tools.displayed = true;
                            break;
                        }
                    }
                }
                
                s_TriggerRefresh = false;
            }
        }
#else
        /// <summary>
        /// Checks whether tool is the currently active exclusive tool.
        /// </summary>
        /// <param name="tool">Tool to check.</param>
        /// <returns>True, when the tool is the active exclusive tool. False, otherwise.</returns>
        public static bool IsToolActive(Type tool)
        {
            return s_ActiveExclusiveTool == tool;
        }
        static Type s_ActiveExclusiveTool;

        /// <summary>
        /// Register your Type from the editor script's OnEnable function.
        /// This way CinemachineTools will know which tools to display.
        /// </summary>
        /// <param name="tool">Tool to register</param>
        public static void RegisterTool(Type tool)
        {
            if (s_RequiredTools.ContainsKey(tool))
            {
                s_RequiredTools[tool]++;
            }
            else
            {
                s_RequiredTools.Add(tool, 1);
            }
        }
        
        /// <summary>
        /// Unregister your Type from the editor script's OnDisable function.
        /// This way CinemachineTools will know which tools to display.
        /// </summary>
        /// <param name="tool">Tool to register</param>
        public static void UnregisterTool(Type tool)
        {
            if (s_RequiredTools.ContainsKey(tool))
            {
                s_RequiredTools[tool]--;
                if (s_RequiredTools[tool] <= 0)
                {
                    s_RequiredTools.Remove(tool);
                }
            }
        }
        static Dictionary<Type, int> s_RequiredTools;

        public delegate void ToolHandler(bool v);
        struct CinemachineSceneToolDelegates
        {
            public ToolHandler ToggleSetter;
            public ToolHandler IsDisplayedSetter;
        }
        static Dictionary<Type, CinemachineSceneToolDelegates> s_ExclusiveTools; // tools that can't be active at the same time
        static Dictionary<Type, CinemachineSceneToolDelegates> s_Tools; // tools without restrictions
        
        /// <summary>
        /// Use for registering tool handlers for tools that are exclusive with each other,
        /// meaning they cannot be active at the same time.
        /// </summary>
        /// <param name="tool">The tool to register.</param>
        /// <param name="toggleSetter">The tool's toggle value setter.</param>
        /// <param name="isDisplayedSetter">The tool's isDisplayed setter.</param>
        internal static void RegisterExclusiveToolHandlers(Type tool, ToolHandler toggleSetter, ToolHandler isDisplayedSetter)
        {
            RegisterToolHandlers(ref s_ExclusiveTools, tool, toggleSetter, isDisplayedSetter);
        }
        
        /// <summary>
        /// Use for registering tool handlers for tools that can be active anytime
        /// without taking other tools into consideration.
        /// </summary>
        /// <param name="tool">The tool to register.</param>
        /// <param name="toggleSetter">The tool's toggle value setter.</param>
        /// <param name="isDisplayedSetter">The tool's isDisplayed setter.</param>
        internal static void RegisterToolHandlers(Type tool, ToolHandler toggleSetter, ToolHandler isDisplayedSetter)
        {
            RegisterToolHandlers(ref s_Tools, tool, toggleSetter, isDisplayedSetter);
        }

        static void RegisterToolHandlers(ref Dictionary<Type, CinemachineSceneToolDelegates> tools,
            Type tool, ToolHandler toggleSetter, ToolHandler isDisplayedSetter)
        {
            if (tools.ContainsKey(tool))
            {
                tools.Remove(tool);
            }
            
            tools.Add(tool, new CinemachineSceneToolDelegates
            {
                ToggleSetter = toggleSetter,
                IsDisplayedSetter = isDisplayedSetter,
            });
        }
        
        internal delegate bool ToolbarHandler();
        static ToolbarHandler s_ToolBarIsDisplayed;
        internal static void RegisterToolbarIsDisplayedHandler(ToolbarHandler handler)
        {
            s_ToolBarIsDisplayed = handler;
        }

        static bool s_ToolbarTurnOffByMe;
        internal delegate bool ToolbarTurnOnOffHandler(bool on);
        static ToolbarTurnOnOffHandler s_ToolBarDisplay;
        internal static void RegisterToolbarDisplayHandler(ToolbarTurnOnOffHandler handler)
        {
            s_ToolBarDisplay = handler;
        }

        internal static void SetTool(bool active, Type tool)
        {
            if (active)
            {
                s_ActiveExclusiveTool = tool;
                EnsureCinemachineToolsAreExclusiveWithUnityTools();
            }
            else
            {
                s_ActiveExclusiveTool = s_ActiveExclusiveTool == tool ? null : s_ActiveExclusiveTool;
            }
        }

        static void EnsureCinemachineToolsAreExclusiveWithUnityTools()
        {
            foreach (var toolHandle in s_ExclusiveTools)
            {
                toolHandle.Value.ToggleSetter(toolHandle.Key == s_ActiveExclusiveTool);
            }
            if (s_ActiveExclusiveTool != null)
            {
                Tools.current = Tool.None; // Cinemachine tools are exclusive with unity tools
            }
        }
        
        static CinemachineSceneToolUtility()
        {
            s_ExclusiveTools = new Dictionary<Type, CinemachineSceneToolDelegates>();
            s_Tools = new Dictionary<Type, CinemachineSceneToolDelegates>();
            s_RequiredTools = new Dictionary<Type, int>();

            EditorApplication.update += () =>
            {
                if (s_RequiredTools.Count <= 0)
                {
                    s_ToolbarTurnOffByMe |= s_ToolBarDisplay(false);
                }
                else if (s_ToolbarTurnOffByMe)
                {
                    s_ToolBarDisplay(true);
                    s_ToolbarTurnOffByMe = false;
                }
                
                var cmToolbarIsHidden = !s_ToolBarIsDisplayed();
                // if a unity tool is selected or cmToolbar is hidden, unselect our tools.
                if (s_ActiveExclusiveTool != null && (Tools.current != Tool.None || cmToolbarIsHidden))
                {
                    SetTool(true, null);
                }

                if (!cmToolbarIsHidden)
                {
                    // only display cm tools that are relevant for the current selection
                    foreach (var toolHandle in s_ExclusiveTools)
                    {
                        toolHandle.Value.IsDisplayedSetter(s_RequiredTools.ContainsKey(toolHandle.Key));
                    }
                    foreach (var toolHandle in s_Tools)
                    {
                        toolHandle.Value.IsDisplayedSetter(s_RequiredTools.ContainsKey(toolHandle.Key));
                    }
                }
            };
        }
#endif
    }
    
    static class CinemachineSceneToolHelpers
    {
        public const float LineThickness = 2f;
        public static readonly Color HelperLineDefaultColor = new Color(255, 255, 255, 25);
        const float k_DottedLineSpacing = 4f;

        static GUIStyle s_LabelStyle = new GUIStyle 
        { 
            normal =
            {
                background = AssetDatabase.LoadAssetAtPath<Texture2D>(ScriptableObjectUtility.
                    CinemachineRealativeInstallPath + "/Editor/EditorResources/SceneToolsLabelBackground.png"),
                textColor = Handles.selectedColor,
            },
            fontStyle = FontStyle.Bold,
            padding = new RectOffset(5, 0, 5, 0)
        };
        

        public static float SliderHandleDelta(Vector3 newPos, Vector3 oldPos, Vector3 forward)
        {
            var delta = newPos - oldPos;
            return Mathf.Sign(Vector3.Dot(delta, forward)) * delta.magnitude;
        }

        /// <summary>
        /// Calculate delta and discard imprecision.
        /// </summary>
        public static Vector3 PositionHandleDelta(Quaternion rot, Vector3 newPos, Vector3 oldPos)
        {
            var delta =
                Quaternion.Inverse(rot) * (newPos - oldPos);
            delta = new Vector3(
                Mathf.Abs(delta.x) < UnityVectorExtensions.Epsilon ? 0 : delta.x,
                Mathf.Abs(delta.y) < UnityVectorExtensions.Epsilon ? 0 : delta.y,
                Mathf.Abs(delta.z) < UnityVectorExtensions.Epsilon ? 0 : delta.z);
            return delta;
        }
        
        public static void DrawLabel(Vector3 position, string text)
        {
            var labelOffset = HandleUtility.GetHandleSize(position) / 5f;
            Handles.Label(position + new Vector3(0, -labelOffset, 0), text, s_LabelStyle);
        }

        public static float CubeHandleCapSize(Vector3 position) => HandleUtility.GetHandleSize(position) / 10f;

        static int s_ScaleSliderHash = "ScaleSliderHash".GetHashCode();
        static float s_FOVAfterLastToolModification;

        public static void FovToolHandle(CinemachineVirtualCameraBase vcam, SerializedProperty lensProperty,
            in LensSettings lens, bool isLensHorizontal)
        {
            var orthographic = lens.Orthographic;
            if (GUIUtility.hotControl == 0)
            {
                s_FOVAfterLastToolModification = orthographic ? lens.OrthographicSize : lens.FieldOfView;
            }
            var originalColor = Handles.color;
            Handles.color = Handles.preselectionColor;
            
            var camPos = vcam.State.FinalPosition;
            var camRot = vcam.State.FinalOrientation;
            var camForward = camRot * Vector3.forward;
                
            EditorGUI.BeginChangeCheck();
            var fovHandleId = GUIUtility.GetControlID(s_ScaleSliderHash, FocusType.Passive) + 1; // TODO: KGB workaround until id is exposed
            var newFov = Handles.ScaleSlider(
                s_FOVAfterLastToolModification, 
                camPos, camForward, camRot, HandleUtility.GetHandleSize(camPos), 0.1f);
            if (EditorGUI.EndChangeCheck())
            {
                if (orthographic)
                {
                    lensProperty.FindPropertyRelative("OrthographicSize").floatValue += 
                        (s_FOVAfterLastToolModification - newFov);
                }
                else
                {
                    lensProperty.FindPropertyRelative("FieldOfView").floatValue += 
                        (s_FOVAfterLastToolModification - newFov);
                    lensProperty.FindPropertyRelative("FieldOfView").floatValue = 
                        Mathf.Clamp(lensProperty.FindPropertyRelative("FieldOfView").floatValue, 1f, 179f);
                }
                lensProperty.serializedObject.ApplyModifiedProperties();
            }
            s_FOVAfterLastToolModification = newFov;

            var fovHandleDraggedOrHovered = 
                GUIUtility.hotControl == fovHandleId || HandleUtility.nearestControl == fovHandleId;
            if (fovHandleDraggedOrHovered)
            {
                var labelPos = camPos + camForward * HandleUtility.GetHandleSize(camPos);
                if (lens.IsPhysicalCamera)
                {
                    DrawLabel(labelPos, "Focal Length (" + 
                        Camera.FieldOfViewToFocalLength(lens.FieldOfView, lens.SensorSize.y).ToString("F1") + ")");
                }
                else if (orthographic)
                {
                    DrawLabel(labelPos, "Orthographic Size (" + 
                        lens.OrthographicSize.ToString("F1") + ")");
                }
                else if (isLensHorizontal)
                {
                    DrawLabel(labelPos, "Horizontal FOV (" +
                        Camera.VerticalToHorizontalFieldOfView(lens.FieldOfView, lens.Aspect).ToString("F1") + ")");
                }
                else
                {
                    DrawLabel(labelPos, "Vertical FOV (" + 
                        lens.FieldOfView.ToString("F1") + ")");
                }
            }
            
            Handles.color = fovHandleDraggedOrHovered ? Handles.selectedColor : HelperLineDefaultColor;
            var vcamLocalToWorld = Matrix4x4.TRS(camPos, camRot, Vector3.one);
            DrawFrustum(vcamLocalToWorld, lens);
                
            SoloOnDrag(GUIUtility.hotControl == fovHandleId, vcam, fovHandleId);

            Handles.color = originalColor;
        }

        public static void NearFarClipHandle(CinemachineVirtualCameraBase vcam, SerializedProperty lens)
        {
            var originalColor = Handles.color;
            Handles.color = Handles.preselectionColor;
            
            var vcamState = vcam.State;
            var camPos = vcamState.FinalPosition;
            var camRot = vcamState.FinalOrientation;
            var camForward = camRot * Vector3.forward;
            var nearClipPlane = lens.FindPropertyRelative("NearClipPlane");
            var farClipPlane = lens.FindPropertyRelative("FarClipPlane");
            var nearClipPos = camPos + camForward * nearClipPlane.floatValue;
            var farClipPos = camPos + camForward * farClipPlane.floatValue;
            var vcamLens = vcamState.Lens;
            
            EditorGUI.BeginChangeCheck();
            var ncHandleId = GUIUtility.GetControlID(FocusType.Passive);
            var newNearClipPos = Handles.Slider(ncHandleId, nearClipPos, camForward, 
                CubeHandleCapSize(nearClipPos), Handles.CubeHandleCap, 0.5f);
            var fcHandleId = GUIUtility.GetControlID(FocusType.Passive);
            var newFarClipPos = Handles.Slider(fcHandleId, farClipPos, camForward, 
                CubeHandleCapSize(farClipPos), Handles.CubeHandleCap, 0.5f);
            if (EditorGUI.EndChangeCheck())
            {
                nearClipPlane.floatValue += 
                    SliderHandleDelta(newNearClipPos, nearClipPos, camForward);
                if (!vcamLens.Orthographic)
                {
                    nearClipPlane.floatValue = Mathf.Max(0.01f, nearClipPlane.floatValue);
                }
                farClipPlane.floatValue += 
                    SliderHandleDelta(newFarClipPos, farClipPos, camForward);
                lens.serializedObject.ApplyModifiedProperties();
            }
            
            var vcamLocalToWorld = Matrix4x4.TRS(camPos, camRot, Vector3.one);
            Handles.color = HelperLineDefaultColor;
            DrawFrustum(vcamLocalToWorld, vcamLens);
            if (GUIUtility.hotControl == ncHandleId || HandleUtility.nearestControl == ncHandleId)
            {
                DrawPreFrustum(vcamLocalToWorld, vcamLens);
                DrawLabel(nearClipPos, "Near Clip Plane (" + nearClipPlane.floatValue.ToString("F1") + ")");
            }
            if (GUIUtility.hotControl == fcHandleId || HandleUtility.nearestControl == fcHandleId)
            {
                DrawPreFrustum(vcamLocalToWorld, vcamLens);
                DrawLabel(farClipPos, "Far Clip Plane (" + farClipPlane.floatValue.ToString("F1") + ")");
            }
            
            SoloOnDrag(GUIUtility.hotControl == ncHandleId || GUIUtility.hotControl == fcHandleId, 
                vcam, Mathf.Min(ncHandleId, fcHandleId));

            Handles.color = originalColor;
        }

        static void DrawPreFrustum(Matrix4x4 transform, LensSettings lens)
        {
            if (!lens.Orthographic && lens.NearClipPlane >= 0)
            {
                DrawPerspectiveFrustum(transform, lens.FieldOfView, 
                    lens.NearClipPlane, 0, lens.Aspect, true);
            }
        }

        static void DrawFrustum(Matrix4x4 transform, LensSettings lens)
        {
            if (lens.Orthographic)
            {
                DrawOrthographicFrustum(transform, lens.OrthographicSize,
                    lens.FarClipPlane, lens.NearClipPlane, lens.Aspect);
            }
            else
            {
                DrawPerspectiveFrustum(transform, lens.FieldOfView, 
                    lens.FarClipPlane, lens.NearClipPlane, lens.Aspect, false);
            }
        }

        static void DrawOrthographicFrustum(Matrix4x4 transform, 
            float orthographicSize, float farClipPlane, float nearClipRange, float aspect)
        {
            var originalMatrix = Handles.matrix;
            Handles.matrix = transform;
            
            var size = new Vector3(aspect * orthographicSize * 2, 
                orthographicSize * 2, farClipPlane - nearClipRange);
            Handles.DrawWireCube(new Vector3(0, 0, (size.z / 2) + nearClipRange), size);
            
            Handles.matrix = originalMatrix;
        }
        
        static void DrawPerspectiveFrustum(Matrix4x4 transform, 
            float fov, float farClipPlane, float nearClipRange, float aspect, bool dottedLine)
        {
            var originalMatrix = Handles.matrix;
            Handles.matrix = transform;
            
            fov = fov * 0.5f * Mathf.Deg2Rad;
            var tanfov = Mathf.Tan(fov);
            var farEnd = new Vector3(0, 0, farClipPlane);
            var endSizeX = new Vector3(farClipPlane * tanfov * aspect, 0, 0);
            var endSizeY = new Vector3(0, farClipPlane * tanfov, 0);

            Vector3 s1, s2, s3, s4;
            var e1 = farEnd + endSizeX + endSizeY;
            var e2 = farEnd - endSizeX + endSizeY;
            var e3 = farEnd - endSizeX - endSizeY;
            var e4 = farEnd + endSizeX - endSizeY;
            if (nearClipRange <= 0.0f)
            {
                s1 = s2 = s3 = s4 = Vector3.zero;
            }
            else
            {
                var startSizeX = new Vector3(nearClipRange * tanfov * aspect, 0, 0);
                var startSizeY = new Vector3(0, nearClipRange * tanfov, 0);
                var startPoint = new Vector3(0, 0, nearClipRange);
                s1 = startPoint + startSizeX + startSizeY;
                s2 = startPoint - startSizeX + startSizeY;
                s3 = startPoint - startSizeX - startSizeY;
                s4 = startPoint + startSizeX - startSizeY;

                if (dottedLine)
                {
                    Handles.DrawDottedLine(s1, s2, k_DottedLineSpacing);
                    Handles.DrawDottedLine(s2, s3, k_DottedLineSpacing);
                    Handles.DrawDottedLine(s3, s4, k_DottedLineSpacing);
                    Handles.DrawDottedLine(s4, s1, k_DottedLineSpacing);
                }
                else
                {
                    Handles.DrawLine(s1, s2);
                    Handles.DrawLine(s2, s3);
                    Handles.DrawLine(s3, s4);
                    Handles.DrawLine(s4, s1);
                }
            }

            if (dottedLine)
            {
                Handles.DrawDottedLine(e1, e2, k_DottedLineSpacing);
                Handles.DrawDottedLine(e2, e3, k_DottedLineSpacing);
                Handles.DrawDottedLine(e3, e4, k_DottedLineSpacing);
                Handles.DrawDottedLine(e4, e1, k_DottedLineSpacing);

                Handles.DrawDottedLine(s1, e1, k_DottedLineSpacing);
                Handles.DrawDottedLine(s2, e2, k_DottedLineSpacing);
                Handles.DrawDottedLine(s3, e3, k_DottedLineSpacing);
                Handles.DrawDottedLine(s4, e4, k_DottedLineSpacing);
            }
            else
            {
                Handles.DrawLine(e1, e2);
                Handles.DrawLine(e2, e3);
                Handles.DrawLine(e3, e4);
                Handles.DrawLine(e4, e1);

                Handles.DrawLine(s1, e1);
                Handles.DrawLine(s2, e2);
                Handles.DrawLine(s3, e3);
                Handles.DrawLine(s4, e4);
            }

            Handles.matrix = originalMatrix;
        }

        public static void TrackedObjectOffsetTool(
            CinemachineComponentBase cmComponent, SerializedProperty trackedObjectOffset)
        {
            var originalColor = Handles.color;
            
            var lookAtPos = cmComponent.LookAtTargetPosition;
            var lookAtRot = cmComponent.LookAtTargetRotation;
            var trackedObjectPos = lookAtPos + lookAtRot * trackedObjectOffset.vector3Value;

            EditorGUI.BeginChangeCheck();
            var tooHandleMinId = GUIUtility.GetControlID(FocusType.Passive); // TODO: KGB workaround until id is exposed
            var newTrackedObjectPos = Handles.PositionHandle(trackedObjectPos, lookAtRot);
            var tooHandleMaxId = GUIUtility.GetControlID(FocusType.Passive); // TODO: KGB workaround until id is exposed
            if (EditorGUI.EndChangeCheck())
            {
                trackedObjectOffset.vector3Value += 
                    PositionHandleDelta(lookAtRot, newTrackedObjectPos, trackedObjectPos);
                trackedObjectOffset.serializedObject.ApplyModifiedProperties();
            }

            var trackedObjectOffsetHandleIsDragged = 
                tooHandleMinId < GUIUtility.hotControl && GUIUtility.hotControl < tooHandleMaxId;
            var trackedObjectOffsetHandleIsUsedOrHovered = trackedObjectOffsetHandleIsDragged || 
                tooHandleMinId < HandleUtility.nearestControl && HandleUtility.nearestControl < tooHandleMaxId;
            if (trackedObjectOffsetHandleIsUsedOrHovered)
            {
                DrawLabel(trackedObjectPos, "(" + cmComponent.Stage + ") Tracked Object Offset " 
                    + trackedObjectOffset.vector3Value.ToString("F1"));
            }
            
            Handles.color = trackedObjectOffsetHandleIsUsedOrHovered ? 
                Handles.selectedColor : HelperLineDefaultColor;
            Handles.DrawDottedLine(lookAtPos, trackedObjectPos, k_DottedLineSpacing);
            Handles.DrawLine(trackedObjectPos, cmComponent.VcamState.FinalPosition);

            SoloOnDrag(trackedObjectOffsetHandleIsDragged, cmComponent.VirtualCamera, tooHandleMaxId);
            
            Handles.color = originalColor;
        }

        public static void TransposerFollowOffsetTool(CinemachineTransposer cmComponent)
        {
            var originalColor = Handles.color;
            
            var brain = CinemachineCore.Instance.FindPotentialTargetBrain(cmComponent.VirtualCamera);
            var up = brain != null ? brain.DefaultWorldUp : Vector3.up;
            var camPos = cmComponent.GetTargetCameraPosition(up);
            var camRot = cmComponent.GetReferenceOrientation(up);

            EditorGUI.BeginChangeCheck();
            var foHandleMinId = GUIUtility.GetControlID(FocusType.Passive); // TODO: KGB workaround until id is exposed
            var newPos = Handles.PositionHandle(camPos, camRot);
            var foHandleMaxId = GUIUtility.GetControlID(FocusType.Passive); // TODO: KGB workaround until id is exposed
            if (EditorGUI.EndChangeCheck())
            {
                var so = new SerializedObject(cmComponent);
                var followOffset = so.FindProperty(() => cmComponent.m_FollowOffset);
                followOffset.vector3Value += PositionHandleDelta(camRot, newPos, camPos);
                so.ApplyModifiedProperties();
                followOffset.vector3Value = cmComponent.EffectiveOffset;
                so.ApplyModifiedProperties();
            }

            var followOffsetHandleIsDragged = 
                foHandleMinId < GUIUtility.hotControl && GUIUtility.hotControl < foHandleMaxId;
            var followOffsetHandleIsDraggedOrHovered = followOffsetHandleIsDragged || 
                foHandleMinId < HandleUtility.nearestControl && HandleUtility.nearestControl < foHandleMaxId;
            if (followOffsetHandleIsDraggedOrHovered)
            {
                DrawLabel(camPos, "Follow offset " + cmComponent.m_FollowOffset.ToString("F1"));
            }

            Handles.color = followOffsetHandleIsDraggedOrHovered ? Handles.selectedColor : HelperLineDefaultColor;
            Handles.DrawDottedLine(cmComponent.FollowTargetPosition, camPos, k_DottedLineSpacing);
            
            SoloOnDrag(followOffsetHandleIsDragged, cmComponent.VirtualCamera, foHandleMaxId);
            
            Handles.color = originalColor;
        }
        
        /// <summary>
        /// Draws Orbit handles (e.g. for freelook)
        /// </summary>
        /// <returns>Index of the rig being edited, or -1 if none</returns>
        public static int OrbitControlHandle(
            CinemachineVirtualCameraBase vcam, SerializedProperty orbits)
        {
            var originalColor = Handles.color;
            var followPos = vcam.Follow.position;
            var draggedRig = -1;
            var minIndex = 1;
            for (var rigIndex = 0; rigIndex < orbits.arraySize; ++rigIndex)
            {
                var orbit = orbits.GetArrayElementAtIndex(rigIndex);
                var orbitHeight = orbit.FindPropertyRelative("m_Height");
                var orbitRadius = orbit.FindPropertyRelative("m_Radius");
                Handles.color = Handles.preselectionColor;
                EditorGUI.BeginChangeCheck();
            
                var heightHandleId = GUIUtility.GetControlID(FocusType.Passive);
                var heightHandlePos = followPos + Vector3.up * orbitHeight.floatValue;
                var newHeightHandlePos = Handles.Slider(heightHandleId, heightHandlePos, Vector3.up, 
                    CubeHandleCapSize(heightHandlePos), Handles.CubeHandleCap, 0.5f);
                
                var radiusHandleOffset = Vector3.right;
                var radiusHandleId = GUIUtility.GetControlID(FocusType.Passive);
                var radiusHandlePos = followPos + Vector3.up * orbitHeight.floatValue
                    + radiusHandleOffset * orbitRadius.floatValue;
                var newRadiusHandlePos = Handles.Slider(radiusHandleId, radiusHandlePos, radiusHandleOffset, 
                    CubeHandleCapSize(radiusHandlePos), Handles.CubeHandleCap, 0.5f);

                if (EditorGUI.EndChangeCheck())
                {
                    orbitHeight.floatValue += 
                        SliderHandleDelta(newHeightHandlePos, heightHandlePos, Vector3.up);
                    orbitRadius.floatValue += 
                        SliderHandleDelta(newRadiusHandlePos, radiusHandlePos, radiusHandleOffset);
                    orbits.serializedObject.ApplyModifiedProperties();
                }

                var isDragged = GUIUtility.hotControl == heightHandleId || GUIUtility.hotControl == radiusHandleId;
                Handles.color = isDragged || HandleUtility.nearestControl == heightHandleId ||
                    HandleUtility.nearestControl == radiusHandleId ? Handles.selectedColor : HelperLineDefaultColor;
                if (GUIUtility.hotControl == heightHandleId || HandleUtility.nearestControl == heightHandleId)
                {
                    DrawLabel(heightHandlePos, "Height: " + orbitHeight.floatValue);
                }
                if (GUIUtility.hotControl == radiusHandleId || HandleUtility.nearestControl == radiusHandleId)
                {
                    DrawLabel(radiusHandlePos, "Radius: " + orbitRadius.floatValue);
                }

                Handles.DrawWireDisc(newHeightHandlePos, Vector3.up, orbitRadius.floatValue);
                if (isDragged)
                {
                    draggedRig = rigIndex;
                    minIndex = Mathf.Min(Mathf.Min(heightHandleId), radiusHandleId);
                }
            }
            SoloOnDrag(draggedRig != -1, vcam, minIndex);

            Handles.color = originalColor;
            return draggedRig;
        }
        
        static bool s_IsDragging;
        static ICinemachineCamera s_UserSolo;
        public static void SoloOnDrag(bool isDragged, ICinemachineCamera vcam, int handleMaxId)
        {
            if (isDragged)
            {
                if (!s_IsDragging)
                {
                    s_UserSolo = CinemachineBrain.SoloCamera;
                    s_IsDragging = true;
                }
                CinemachineBrain.SoloCamera = vcam;
            }
            else if (s_IsDragging && handleMaxId != -1) // Handles sometimes return -1 as id, ignore those frames
            {
                CinemachineBrain.SoloCamera = s_UserSolo;
                InspectorUtility.RepaintGameView();
                s_IsDragging = false;
                s_UserSolo = null;
            }
        }
    } 
}
#endif