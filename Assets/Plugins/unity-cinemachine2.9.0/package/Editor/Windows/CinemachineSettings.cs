#if !UNITY_2019_1_OR_NEWER
#define CINEMACHINE_UGUI
#endif

using UnityEngine;
using UnityEditor;
using System;

namespace Cinemachine.Editor
{
#if !UNITY_2021_2_OR_NEWER
    [InitializeOnLoad]
    internal sealed class CinemachineSettings
#else
    internal sealed class CinemachineSettings : AssetPostprocessor
#endif
    
    {

#if UNITY_2021_2_OR_NEWER
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
        {
            if (didDomainReload)
            {
                EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
            }
        }
#endif

        public static class CinemachineCoreSettings
        {
            private static readonly string hShowInGameGuidesKey = "CNMCN_Core_ShowInGameGuides";
            public static bool ShowInGameGuides
            {
                get { return EditorPrefs.GetBool(hShowInGameGuidesKey, true); }
                set
                {
                    if (ShowInGameGuides != value)
                    {
                        EditorPrefs.SetBool(hShowInGameGuidesKey, value);
                        UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
                    }
                }
            }


            static string kUseTimelneScrubbingCache = "CNMCN_Timeeline_UseTimelineScrubbingCache";
            public static bool UseTimelneScrubbingCache
            {
                get { return EditorPrefs.GetBool(kUseTimelneScrubbingCache, true); }
                set
                {
                    if (UseTimelneScrubbingCache != value)
                        EditorPrefs.SetBool(kUseTimelneScrubbingCache, value);
                }
            }
        
            private static readonly string kCoreActiveGizmoColourKey = "CNMCN_Core_Active_Gizmo_Colour";
            public static readonly Color kDefaultActiveColour = new Color32(255, 0, 0, 100);
            public static Color ActiveGizmoColour
            {
                get
                {
                    string packedColour = EditorPrefs.GetString(kCoreActiveGizmoColourKey, PackColor(kDefaultActiveColour));
                    return UnpackColour(packedColour);
                }

                set
                {
                    if (ActiveGizmoColour != value)
                    {
                        string packedColour = PackColor(value);
                        EditorPrefs.SetString(kCoreActiveGizmoColourKey, packedColour);
                    }
                }
            }

            private static readonly string kCoreInactiveGizmoColourKey = "CNMCN_Core_Inactive_Gizmo_Colour";
            public static readonly Color kDefaultInactiveColour = new Color32(9, 54, 87, 100);
            public static Color InactiveGizmoColour
            {
                get
                {
                    string packedColour = EditorPrefs.GetString(kCoreInactiveGizmoColourKey, PackColor(kDefaultInactiveColour));
                    return UnpackColour(packedColour);
                }

                set
                {
                    if (InactiveGizmoColour != value)
                    {
                        string packedColour = PackColor(value);
                        EditorPrefs.SetString(kCoreInactiveGizmoColourKey, packedColour);
                    }
                }
            }
            
            private static readonly string kCoreBoundaryObjectGizmoColorKey = "CNMCN_Core_BoundaryObject_Gizmo_Colour";
            public static readonly Color kDefaultBoundaryObjectColour = Color.yellow;
            public static Color BoundaryObjectGizmoColour
            {
                get
                {
                    string packedColour = EditorPrefs.GetString(kCoreBoundaryObjectGizmoColorKey, PackColor(kDefaultBoundaryObjectColour));
                    return UnpackColour(packedColour);
                }

                set
                {
                    if (BoundaryObjectGizmoColour != value)
                    {
                        string packedColour = PackColor(value);
                        EditorPrefs.SetString(kCoreBoundaryObjectGizmoColorKey, packedColour);
                    }
                }
            }
        }

        public static class ComposerSettings
        {
            private static readonly string kOverlayOpacityKey           = "CNMCN_Overlay_Opacity";
            private static readonly string kComposerHardBoundsColourKey = "CNMCN_Composer_HardBounds_Colour";
            private static readonly string kComposerSoftBoundsColourKey = "CNMCN_Composer_SoftBounds_Colour";
            private static readonly string kComposerTargetColourKey     = "CNMCN_Composer_Target_Colour";
            private static readonly string kComposerTargetSizeKey       = "CNMCN_Composer_Target_Size";

            public const float kDefaultOverlayOpacity = 0.15f;
            public static readonly Color kDefaultHardBoundsColour = new Color32(255, 0, 72, 255);
            public static readonly Color kDefaultSoftBoundsColour = new Color32(0, 194, 255, 255);
            public static readonly Color kDefaultTargetColour = new Color32(255, 254, 25, 255);

            public static float OverlayOpacity
            {
                get { return EditorPrefs.GetFloat(kOverlayOpacityKey, kDefaultOverlayOpacity); }
                set
                {
                    if (value != OverlayOpacity)
                    {
                        EditorPrefs.SetFloat(kOverlayOpacityKey, value);
                    }
                }
            }

            public static Color HardBoundsOverlayColour
            {
                get
                {
                    string packedColour = EditorPrefs.GetString(kComposerHardBoundsColourKey, PackColor(kDefaultHardBoundsColour));
                    return UnpackColour(packedColour);
                }

                set
                {
                    if (HardBoundsOverlayColour != value)
                    {
                        string packedColour = PackColor(value);
                        EditorPrefs.SetString(kComposerHardBoundsColourKey, packedColour);
                    }
                }
            }

            public static Color SoftBoundsOverlayColour
            {
                get
                {
                    string packedColour = EditorPrefs.GetString(kComposerSoftBoundsColourKey, PackColor(kDefaultSoftBoundsColour));
                    return UnpackColour(packedColour);
                }

                set
                {
                    if (SoftBoundsOverlayColour != value)
                    {
                        string packedColour = PackColor(value);
                        EditorPrefs.SetString(kComposerSoftBoundsColourKey, packedColour);
                    }
                }
            }

            public static Color TargetColour
            {
                get
                {
                    string packedColour = EditorPrefs.GetString(kComposerTargetColourKey, PackColor(kDefaultTargetColour));
                    return UnpackColour(packedColour);
                }

                set
                {
                    if (TargetColour != value)
                    {
                        string packedColour = PackColor(value);
                        EditorPrefs.SetString(kComposerTargetColourKey, packedColour);
                    }
                }
            }

            public static float TargetSize
            {
                get
                {
                    return EditorPrefs.GetFloat(kComposerTargetSizeKey, 5f);
                }

                set
                {
                    if (TargetSize != value)
                    {
                        EditorPrefs.SetFloat(kComposerTargetSizeKey, value);
                    }
                }
            }
        }

        private static bool ShowCoreSettings
        {
            get { return EditorPrefs.GetBool(kCoreSettingsFoldKey, false); }
            set
            {
                if (value != ShowCoreSettings)
                {
                    EditorPrefs.SetBool(kCoreSettingsFoldKey, value);
                }
            }
        }

        private static bool ShowComposerSettings
        {
            get { return EditorPrefs.GetBool(kComposerSettingsFoldKey, false); }
            set
            {
                if (value != ShowComposerSettings)
                {
                    EditorPrefs.SetBool(kComposerSettingsFoldKey, value);
                }
            }
        }

        private static Texture2D sCinemachineLogoTexture = null;
        internal static Texture2D CinemachineLogoTexture
        {
            get
            {
                if (sCinemachineLogoTexture == null)
                    sCinemachineLogoTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(
                        ScriptableObjectUtility.CinemachineRealativeInstallPath
                            + "/Editor/EditorResources/cm_logo_sm.png");
                if (sCinemachineLogoTexture != null)
                    sCinemachineLogoTexture.hideFlags = HideFlags.DontSaveInEditor;
                return sCinemachineLogoTexture;
            }
        }

        private static Texture2D sCinemachineHeader = null;
        internal static Texture2D CinemachineHeader
        {
            get
            {
                if (sCinemachineHeader == null)
                    sCinemachineHeader = AssetDatabase.LoadAssetAtPath<Texture2D>(
                        ScriptableObjectUtility.CinemachineRealativeInstallPath
                            + "/Editor/EditorResources/cinemachine_header.tif");
                ;
                if (sCinemachineHeader != null)
                    sCinemachineHeader.hideFlags = HideFlags.DontSaveInEditor;
                return sCinemachineHeader;
            }
        }

        private static readonly string kCoreSettingsFoldKey     = "CNMCN_Core_Folded";
        private static readonly string kComposerSettingsFoldKey = "CNMCN_Composer_Folded";

        internal static event Action AdditionalCategories = null;

#if !UNITY_2021_2_OR_NEWER
        [InitializeOnLoadMethod]
        /// Ensures that CM Brain logo is added to the Main Camera
        /// after adding a virtual camera to the project for the first time
        static void OnPackageLoadedInEditor()
        {
            if (CinemachineLogoTexture == null) 
            {
                // After adding the CM to a project, we need to wait for one update cycle for the assets to load
                EditorApplication.update += OnPackageLoadedInEditor; 
            }
            else
            {
                EditorApplication.update -= OnPackageLoadedInEditor;
                EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI; // Update hierarchy with CM Brain logo
            }
        }

        static CinemachineSettings()
        {
            if (CinemachineLogoTexture != null)
            {
                EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
            }
        }
#endif

        class Styles {
            //private static readonly GUIContent sCoreShowHiddenObjectsToggle = new GUIContent("Show Hidden Objects", "If checked, Cinemachine hidden objects will be shown in the inspector.  This might be necessary to repair broken script mappings when upgrading from a pre-release version");
            public static readonly GUIContent sCoreActiveGizmosColour = new GUIContent("Active Virtual Camera", "The colour for the active virtual camera's gizmos");
            public static readonly GUIContent sCoreInactiveGizmosColour = new GUIContent("Inactive Virtual Camera", "The colour for all inactive virtual camera gizmos");
            public static readonly GUIContent sCoreBoundaryObjectGizmosColour = new GUIContent("Boundary Object", "The colour to indicate secondary objects like group boundaries and confiner shapes");
            public static readonly GUIContent sComposerOverlayOpacity = new GUIContent("Overlay Opacity", "The alpha of the composer's overlay when a virtual camera is selected with composer module enabled");
            public static readonly GUIContent sComposerHardBoundsOverlay = new GUIContent("Hard Bounds Overlay", "The colour of the composer overlay's hard bounds region");
            public static readonly GUIContent sComposerSoftBoundsOverlay = new GUIContent("Soft Bounds Overlay", "The colour of the composer overlay's soft bounds region");
            public static readonly GUIContent sComposerTargetOverlay = new GUIContent("Composer Target", "The colour of the composer overlay's target");
            public static readonly GUIContent sComposerTargetOverlayPixels = new GUIContent("Target Size (px)", "The size of the composer overlay's target box in pixels");
        }

        private static Vector2 sScrollPosition = Vector2.zero;
        
        static GUIContent sDraggableText = new GUIContent("Draggable Game Window Guides", "If checked, game window " +
            "guides are draggable in play mode. If false, game window guides are only for visualization");
        static GUIContent sGlobalMuteText = new GUIContent("Storyboard Global Mute", "If checked, all storyboards " +
            "are globally muted.");

#if UNITY_2019_1_OR_NEWER
        [SettingsProvider]
        static SettingsProvider CreateProjectSettingsProvider()
        {
            var provider = new SettingsProvider("Preferences/Cinemachine", 
                SettingsScope.User, SettingsProvider.GetSearchKeywordsFromGUIContentProperties<Styles>());
            provider.guiHandler = (sarchContext) => OnGUI();
            return provider;
        }
#else
        [PreferenceItem("Cinemachine")]
#endif
        private static void OnGUI()
        {
            if (CinemachineHeader != null)
            {
                const float kWidth = 350f;
                float aspectRatio = (float)CinemachineHeader.height / (float)CinemachineHeader.width;
                GUILayout.BeginScrollView(Vector2.zero, false, false, GUILayout.Width(kWidth), GUILayout.Height(kWidth * aspectRatio));
                Rect texRect = new Rect(0f, 0f, kWidth, kWidth * aspectRatio);

                GUILayout.BeginArea(texRect);
                GUI.DrawTexture(texRect, CinemachineHeader, ScaleMode.ScaleToFit);
                GUILayout.EndArea();

                GUILayout.EndScrollView();
            }

            // set label width, so text is not cut for Toggles
            float originalLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = GUI.skin.label.CalcSize(
                sGlobalMuteText.text.Length > sDraggableText.text.Length ? sGlobalMuteText : sDraggableText).x;
            {
#if CINEMACHINE_UGUI
                CinemachineStoryboardMute.Enabled = EditorGUILayout.Toggle(
                    sGlobalMuteText,
                    CinemachineStoryboardMute.Enabled);
#endif
                CinemachineScreenComposerGuidesGlobalDraggable.Enabled = EditorGUILayout.Toggle(
                    sDraggableText,
                    CinemachineScreenComposerGuidesGlobalDraggable.Enabled);
            }
            EditorGUIUtility.labelWidth = originalLabelWidth;

            sScrollPosition = GUILayout.BeginScrollView(sScrollPosition);

            //CinemachineCore.sShowHiddenObjects
            //    = EditorGUILayout.Toggle("Show Hidden Objects", CinemachineCore.sShowHiddenObjects);

            ShowCoreSettings = EditorGUILayout.Foldout(ShowCoreSettings, "Runtime Settings", true);
            if (ShowCoreSettings)
            {
                EditorGUI.indentLevel++;
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();
                Color newActiveGizmoColour = EditorGUILayout.ColorField(Styles.sCoreActiveGizmosColour, CinemachineCoreSettings.ActiveGizmoColour);

                if (EditorGUI.EndChangeCheck())
                {
                    CinemachineCoreSettings.ActiveGizmoColour = newActiveGizmoColour;
                    UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
                }

                if (GUILayout.Button("Reset"))
                {
                    CinemachineCoreSettings.ActiveGizmoColour = CinemachineCoreSettings.kDefaultActiveColour;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();
                Color newInactiveGizmoColour = EditorGUILayout.ColorField(Styles.sCoreInactiveGizmosColour, CinemachineCoreSettings.InactiveGizmoColour);

                if (EditorGUI.EndChangeCheck())
                {
                    CinemachineCoreSettings.InactiveGizmoColour = newInactiveGizmoColour;
                    UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
                }

                if (GUILayout.Button("Reset"))
                {
                    CinemachineCoreSettings.InactiveGizmoColour = CinemachineCoreSettings.kDefaultInactiveColour;
                }
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();
                Color newBoundaryObjectColor = EditorGUILayout.ColorField(Styles.sCoreBoundaryObjectGizmosColour, CinemachineCoreSettings.BoundaryObjectGizmoColour);

                if (EditorGUI.EndChangeCheck())
                {
                    CinemachineCoreSettings.BoundaryObjectGizmoColour = newBoundaryObjectColor;
                    UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
                }

                if (GUILayout.Button("Reset"))
                {
                    CinemachineCoreSettings.BoundaryObjectGizmoColour = CinemachineCoreSettings.kDefaultBoundaryObjectColour;
                }
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel--;
            }

            ShowComposerSettings = EditorGUILayout.Foldout(ShowComposerSettings, "Composer Settings", true);
            if (ShowComposerSettings)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();

                float overlayOpacity = EditorGUILayout.Slider(Styles.sComposerOverlayOpacity, ComposerSettings.OverlayOpacity, 0f, 1f);

                if (EditorGUI.EndChangeCheck())
                {
                    ComposerSettings.OverlayOpacity = overlayOpacity;
                }

                if (GUILayout.Button("Reset"))
                {
                    ComposerSettings.OverlayOpacity = ComposerSettings.kDefaultOverlayOpacity;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();
                Color newHardEdgeColor = EditorGUILayout.ColorField(Styles.sComposerHardBoundsOverlay, ComposerSettings.HardBoundsOverlayColour);

                if (EditorGUI.EndChangeCheck())
                {
                    ComposerSettings.HardBoundsOverlayColour = newHardEdgeColor;
                }

                if (GUILayout.Button("Reset"))
                {
                    ComposerSettings.HardBoundsOverlayColour = ComposerSettings.kDefaultHardBoundsColour;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();
                Color newSoftEdgeColor = EditorGUILayout.ColorField(Styles.sComposerSoftBoundsOverlay, ComposerSettings.SoftBoundsOverlayColour);

                if (EditorGUI.EndChangeCheck())
                {
                    ComposerSettings.SoftBoundsOverlayColour = newSoftEdgeColor;
                }

                if (GUILayout.Button("Reset"))
                {
                    ComposerSettings.SoftBoundsOverlayColour = ComposerSettings.kDefaultSoftBoundsColour;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();
                Color newTargetColour = EditorGUILayout.ColorField(Styles.sComposerTargetOverlay, ComposerSettings.TargetColour);

                if (EditorGUI.EndChangeCheck())
                {
                    ComposerSettings.TargetColour = newTargetColour;
                }

                if (GUILayout.Button("Reset"))
                {
                    ComposerSettings.TargetColour = ComposerSettings.kDefaultTargetColour;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUI.BeginChangeCheck();
                float targetSide = EditorGUILayout.FloatField(Styles.sComposerTargetOverlayPixels, ComposerSettings.TargetSize);

                if (EditorGUI.EndChangeCheck())
                {
                    ComposerSettings.TargetSize = targetSide;
                }
                EditorGUI.indentLevel--;
            }

            if (AdditionalCategories != null)
            {
                AdditionalCategories();
            }

            GUILayout.EndScrollView();
        }

        private static void OnHierarchyGUI(int instanceID, Rect selectionRect)
        {
            GameObject instance = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (instance == null)
            {
                // Object in process of being deleted?
                return;
            }

            if (instance.GetComponent<CinemachineBrain>() != null)
            {
                Rect texRect = new Rect(selectionRect.xMax - selectionRect.height, selectionRect.yMin, selectionRect.height, selectionRect.height);
                GUI.DrawTexture(texRect, CinemachineLogoTexture, ScaleMode.ScaleAndCrop);
            }
        }

        internal static Color UnpackColour(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                byte[] bytes = Base64Decode(str);

                if ((bytes != null) && bytes.Length == 16)
                {
                    float r = BitConverter.ToSingle(bytes, 0);
                    float g = BitConverter.ToSingle(bytes, 4);
                    float b = BitConverter.ToSingle(bytes, 8);
                    float a = BitConverter.ToSingle(bytes, 12);

                    return new Color(r, g, b, a);
                }
            }

            return Color.white;
        }

        internal static string PackColor(Color col)
        {
            byte[] bytes = new byte[16];

            byte[] rBytes = BitConverter.GetBytes(col.r);
            byte[] gBytes = BitConverter.GetBytes(col.g);
            byte[] bBytes = BitConverter.GetBytes(col.b);
            byte[] aBytes = BitConverter.GetBytes(col.a);

            Buffer.BlockCopy(rBytes, 0, bytes, 0, 4);
            Buffer.BlockCopy(gBytes, 0, bytes, 4, 4);
            Buffer.BlockCopy(bBytes, 0, bytes, 8, 4);
            Buffer.BlockCopy(aBytes, 0, bytes, 12, 4);

            return Base64Encode(bytes);
        }

        private static string Base64Encode(byte[] data)
        {
            return Convert.ToBase64String(data);
        }

        private static byte[] Base64Decode(string base64EncodedData)
        {
            return Convert.FromBase64String(base64EncodedData);
        }
    }
}
