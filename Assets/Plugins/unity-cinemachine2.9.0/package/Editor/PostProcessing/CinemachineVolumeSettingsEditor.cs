#if CINEMACHINE_HDRP
    using UnityEngine;
    using UnityEditor;
    using UnityEngine.Rendering;
    using UnityEditor.Rendering;
    using System.Collections.Generic;
    #if CINEMACHINE_HDRP_7_3_1
        using UnityEngine.Rendering.HighDefinition;
    #else
        using UnityEngine.Experimental.Rendering.HDPipeline;
    #endif
#elif CINEMACHINE_LWRP_7_3_1
    using UnityEngine;
    using UnityEditor;
    using UnityEngine.Rendering;
    using UnityEditor.Rendering;
    using System.Collections.Generic;
    using UnityEngine.Rendering.Universal;
#endif

namespace Cinemachine.PostFX.Editor
{
#if CINEMACHINE_HDRP || CINEMACHINE_LWRP_7_3_1
    [CustomEditor(typeof(CinemachineVolumeSettings))]
    public sealed class CinemachineVolumeSettingsEditor : Cinemachine.Editor.BaseEditor<CinemachineVolumeSettings>
    {
        SerializedProperty m_Profile;
        SerializedProperty m_FocusTracking;

        VolumeComponentListEditor m_ComponentList;

        GUIContent m_ProfileLabel;
        GUIContent m_NewLabel;
        GUIContent m_CloneLabel;

        void OnEnable()
        {
            m_ProfileLabel = new GUIContent("Profile", "A reference to a profile asset");
            m_NewLabel = new GUIContent("New", "Create a new profile.");
            m_CloneLabel = new GUIContent("Clone", "Create a new profile and copy the content of the currently assigned profile.");

            m_FocusTracking = FindProperty(x => x.m_FocusTracking);
            m_Profile = FindProperty(x => x.m_Profile);

            RefreshVolumeComponentEditor(Target.m_Profile);
        }

        void OnDisable()
        {
            if (m_ComponentList != null)
                m_ComponentList.Clear();
        }

        void RefreshVolumeComponentEditor(VolumeProfile asset)
        {
            if (m_ComponentList == null)
                m_ComponentList = new VolumeComponentListEditor(this);
            m_ComponentList.Clear();
            if (asset != null)
                m_ComponentList.Init(asset, new SerializedObject(asset));
        }

        /// <summary>Get the property names to exclude in the inspector.</summary>
        /// <param name="excluded">Add the names to this list</param>
        protected override void GetExcludedPropertiesInInspector(List<string> excluded)
        {
            base.GetExcludedPropertiesInInspector(excluded);
            var mode = (CinemachineVolumeSettings.FocusTrackingMode)m_FocusTracking.intValue;
            if (mode != CinemachineVolumeSettings.FocusTrackingMode.CustomTarget)
                excluded.Add(FieldPath(x => x.m_FocusTarget));
            if (mode == CinemachineVolumeSettings.FocusTrackingMode.None)
                excluded.Add(FieldPath(x => x.m_FocusOffset));
            excluded.Add(FieldPath(x => x.m_Profile));
        }

        public override void OnInspectorGUI()
        {
            BeginInspector();

            var focusMode = (CinemachineVolumeSettings.FocusTrackingMode)m_FocusTracking.intValue;
            if (focusMode != CinemachineVolumeSettings.FocusTrackingMode.None)
            {
                bool valid = false;
                DepthOfField dof;
                if (Target.m_Profile != null && Target.m_Profile.TryGet(out dof))
#if CINEMACHINE_LWRP_7_3_1 && !CINEMACHINE_HDRP
                {
                    valid = dof.active && dof.focusDistance.overrideState
                        && dof.mode.overrideState && dof.mode == DepthOfFieldMode.Bokeh;
                }
                if (!valid)
                    EditorGUILayout.HelpBox(
                        "Focus Tracking requires an active Depth Of Field override in the profile "
                            + "with Focus Distance activated and Mode activated and set to Bokeh",
                        MessageType.Warning);
#else
                {
                    valid = dof.active && dof.focusDistance.overrideState
                        && dof.focusMode.overrideState && dof.focusMode == DepthOfFieldMode.UsePhysicalCamera;
                }
                if (!valid)
                    EditorGUILayout.HelpBox(
                        "Focus Tracking requires an active Depth Of Field override in the profile "
                            + "with Focus Distance activated and Focus Mode activated and set to Use Physical Camera",
                        MessageType.Warning);
#endif
            }

            DrawRemainingPropertiesInInspector();

            EditorGUI.BeginChangeCheck();
            DrawProfileInspectorGUI();
            if (EditorGUI.EndChangeCheck())
            {
                Target.InvalidateCachedProfile();
                serializedObject.ApplyModifiedProperties();
            }
        }

        void DrawProfileInspectorGUI()
        {
            EditorGUILayout.Space();

            bool assetHasChanged = false;
            bool showCopy = m_Profile.objectReferenceValue != null;

            // The layout system sort of break alignement when mixing inspector fields with custom
            // layouted fields, do the layout manually instead
            int buttonWidth = showCopy ? 45 : 60;
            float indentOffset = EditorGUI.indentLevel * 15f;
            var lineRect = GUILayoutUtility.GetRect(1, EditorGUIUtility.singleLineHeight);
            var labelRect = new Rect(lineRect.x, lineRect.y, EditorGUIUtility.labelWidth - indentOffset, lineRect.height);
            var fieldRect = new Rect(labelRect.xMax, lineRect.y, lineRect.width - labelRect.width - buttonWidth * (showCopy ? 2 : 1), lineRect.height);
            var buttonNewRect = new Rect(fieldRect.xMax, lineRect.y, buttonWidth, lineRect.height);
            var buttonCopyRect = new Rect(buttonNewRect.xMax, lineRect.y, buttonWidth, lineRect.height);

            EditorGUI.PrefixLabel(labelRect, m_ProfileLabel);

            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                m_Profile.objectReferenceValue
                    = (VolumeProfile)EditorGUI.ObjectField(
                        fieldRect, m_Profile.objectReferenceValue, typeof(VolumeProfile), false);
                assetHasChanged = scope.changed;
            }

            if (GUI.Button(buttonNewRect, m_NewLabel,
                showCopy ? EditorStyles.miniButtonLeft : EditorStyles.miniButton))
            {
                // By default, try to put assets in a folder next to the currently active
                // scene file. If the user isn't a scene, put them in root instead.
                var targetName = Target.name;
                var scene = Target.gameObject.scene;
                var asset = CreateVolumeProfile(scene, targetName);
                m_Profile.objectReferenceValue = asset;
                assetHasChanged = true;
            }

            if (showCopy && GUI.Button(buttonCopyRect, m_CloneLabel, EditorStyles.miniButtonRight))
            {
                // Duplicate the currently assigned profile and save it as a new profile
                var origin = (VolumeProfile)m_Profile.objectReferenceValue;
                var path = AssetDatabase.GetAssetPath(origin);
                path = AssetDatabase.GenerateUniqueAssetPath(path);

                var asset = Instantiate(origin);
                asset.components.Clear();
                AssetDatabase.CreateAsset(asset, path);

                foreach (var item in origin.components)
                {
                    var itemCopy = Instantiate(item);
                    itemCopy.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
                    itemCopy.name = item.name;
                    asset.components.Add(itemCopy);
                    AssetDatabase.AddObjectToAsset(itemCopy, asset);
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                m_Profile.objectReferenceValue = asset;
                assetHasChanged = true;
            }

            if (m_Profile.objectReferenceValue == null)
            {
                if (assetHasChanged && m_ComponentList != null)
                    m_ComponentList.Clear(); // Asset wasn't null before, do some cleanup

                EditorGUILayout.HelpBox(
                    "Assign an existing Volume Profile by choosing an asset, or create a new one by clicking the \"New\" button.\n"
                    + "New assets are automatically put in a folder next to your scene file. If your scene hasn't "
                    + "been saved yet they will be created at the root of the Assets folder.",
                    MessageType.Info);
            }
            else
            {
                EditorGUILayout.Space();
                if (assetHasChanged)
                    RefreshVolumeComponentEditor((VolumeProfile)m_Profile.objectReferenceValue);
                if (m_ComponentList != null)
                    m_ComponentList.OnGUI();
            }
        }

        // Copied from UnityEditor.Rendering.PostProcessing.ProfileFactory.CreateVolumeProfile() because it's internal
        static VolumeProfile CreateVolumeProfile(UnityEngine.SceneManagement.Scene scene, string targetName)
        {
            var path = string.Empty;

            if (string.IsNullOrEmpty(scene.path))
            {
                path = "Assets/";
            }
            else
            {
                var scenePath = System.IO.Path.GetDirectoryName(scene.path);
                var extPath = scene.name + "_Profiles";
                var profilePath = scenePath + "/" + extPath;

                if (!AssetDatabase.IsValidFolder(profilePath))
                    AssetDatabase.CreateFolder(scenePath, extPath);

                path = profilePath + "/";
            }

            path += targetName + " Profile.asset";
            path = AssetDatabase.GenerateUniqueAssetPath(path);

            var profile = ScriptableObject.CreateInstance<VolumeProfile>();
            AssetDatabase.CreateAsset(profile, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return profile;
        }
    }
#endif
}
