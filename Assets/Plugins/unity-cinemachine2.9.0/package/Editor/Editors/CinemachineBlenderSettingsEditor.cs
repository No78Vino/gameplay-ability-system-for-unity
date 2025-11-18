using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;

namespace Cinemachine.Editor
{
    [CustomEditor(typeof(CinemachineBlenderSettings))]
    internal sealed class CinemachineBlenderSettingsEditor : BaseEditor<CinemachineBlenderSettings>
    {
        private ReorderableList mBlendList;

        /// <summary>
        /// Called when building the Camera popup menus, to get the domain of possible
        /// cameras.  If no delegate is set, will find all top-level (non-slave)
        /// virtual cameras in the scene.
        /// </summary>
        public GetAllVirtualCamerasDelegate GetAllVirtualCameras;
        public delegate CinemachineVirtualCameraBase[] GetAllVirtualCamerasDelegate();

        /// <summary>Get the property names to exclude in the inspector.</summary>
        /// <param name="excluded">Add the names to this list</param>
        protected override void GetExcludedPropertiesInInspector(List<string> excluded)
        {
            base.GetExcludedPropertiesInInspector(excluded);
            excluded.Add(FieldPath(x => x.m_CustomBlends));
        }

        public override void OnInspectorGUI()
        {
            BeginInspector();
            if (mBlendList == null)
                SetupBlendList();

            DrawRemainingPropertiesInInspector();

            UpdateCameraCandidates();
            mBlendList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        private const string kNoneLabel = "(none)";
        private string[] mCameraCandidates;
        private Dictionary<string, int> mCameraIndexLookup;
        private void UpdateCameraCandidates()
        {
            List<string> vcams = new List<string>();
            mCameraIndexLookup = new Dictionary<string, int>();

            CinemachineVirtualCameraBase[] candidates;
            if (GetAllVirtualCameras != null)
                candidates = GetAllVirtualCameras();
            else
            {
                // Get all top-level (i.e. non-slave) virtual cameras
                candidates = Resources.FindObjectsOfTypeAll(
                        typeof(CinemachineVirtualCameraBase)) as CinemachineVirtualCameraBase[];

                for (int i = 0; i < candidates.Length; ++i)
                    if (candidates[i].ParentCamera != null)
                        candidates[i] = null;
            }
            vcams.Add(kNoneLabel);
            vcams.Add(CinemachineBlenderSettings.kBlendFromAnyCameraLabel);
            foreach (CinemachineVirtualCameraBase c in candidates)
                if (c != null && !vcams.Contains(c.Name))
                    vcams.Add(c.Name);

            mCameraCandidates = vcams.ToArray();
            for (int i = 0; i < mCameraCandidates.Length; ++i)
                mCameraIndexLookup[mCameraCandidates[i]] = i;
        }

        private int GetCameraIndex(string name)
        {
            if (name == null || mCameraIndexLookup == null)
                return 0;
            if (!mCameraIndexLookup.ContainsKey(name))
                return 0;
            return mCameraIndexLookup[name];
        }

        void DrawVcamSelector(Rect r, SerializedProperty prop)
        {
            r.width -= EditorGUIUtility.singleLineHeight;
            int current = GetCameraIndex(prop.stringValue);
            var oldColor = GUI.color;
            if (current == 0)
                GUI.color = new Color(1, 193.0f/255.0f, 7.0f/255.0f); // the "warning" icon color
            EditorGUI.PropertyField(r, prop, GUIContent.none);
            r.x += r.width; r.width = EditorGUIUtility.singleLineHeight;
            int sel = EditorGUI.Popup(r, current, mCameraCandidates);
            if (current != sel)
                prop.stringValue = (mCameraCandidates[sel] == kNoneLabel) 
                    ? string.Empty : mCameraCandidates[sel];
            GUI.color = oldColor;
        }
        
        void SetupBlendList()
        {
            mBlendList = new ReorderableList(serializedObject,
                    serializedObject.FindProperty(() => Target.m_CustomBlends),
                    true, true, true, true);

            // Needed for accessing string names of fields
            CinemachineBlenderSettings.CustomBlend def = new CinemachineBlenderSettings.CustomBlend();
            CinemachineBlendDefinition def2 = new CinemachineBlendDefinition();

            float vSpace = 2;
            float hSpace = 3;
            float floatFieldWidth = EditorGUIUtility.singleLineHeight * 2.5f;
            mBlendList.drawHeaderCallback = (Rect rect) =>
                {
                    rect.width -= (EditorGUIUtility.singleLineHeight + 2 * hSpace);
                    rect.width /= 3;
                    rect.x += EditorGUIUtility.singleLineHeight;
                    EditorGUI.LabelField(rect, "From");

                    rect.x += rect.width + hSpace;
                    EditorGUI.LabelField(rect, "To");

                    rect.x += rect.width + hSpace; rect.width -= floatFieldWidth + hSpace;
                    EditorGUI.LabelField(rect, "Style");

                    rect.x += rect.width + hSpace; rect.width = floatFieldWidth;
                    EditorGUI.LabelField(rect, "Time");
                };

            mBlendList.drawElementCallback
                = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    SerializedProperty element
                        = mBlendList.serializedProperty.GetArrayElementAtIndex(index);

                    rect.y += vSpace;
                    rect.height = EditorGUIUtility.singleLineHeight;
                    rect.width -= 2 * hSpace; rect.width /= 3;
                    DrawVcamSelector(rect, element.FindPropertyRelative(() => def.m_From));

                    rect.x += rect.width + hSpace;
                    DrawVcamSelector(rect, element.FindPropertyRelative(() => def.m_To));

                    SerializedProperty blendProp = element.FindPropertyRelative(() => def.m_Blend);
                    rect.x += rect.width + hSpace;
                    EditorGUI.PropertyField(rect, blendProp, GUIContent.none);
                };

            mBlendList.onAddCallback = (ReorderableList l) =>
                {
                    var index = l.serializedProperty.arraySize;
                    ++l.serializedProperty.arraySize;
                    SerializedProperty blendProp = l.serializedProperty.GetArrayElementAtIndex(
                            index).FindPropertyRelative(() => def.m_Blend);

                    blendProp.FindPropertyRelative(() => def2.m_Style).enumValueIndex
                        = (int)CinemachineBlendDefinition.Style.EaseInOut;
                    blendProp.FindPropertyRelative(() => def2.m_Time).floatValue = 2f;
                };
        }
    }
}
