using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using Cinemachine.Utility;

namespace Cinemachine.Editor
{
    class VcamStageEditor
    {
        // Static state and caches - Call UpdateStaticData() to refresh this
        struct StageData
        {
            public bool IsExpanded { get; set; }
            public string Name;
            public Type[] types;   // first entry is null
            public GUIContent[] PopupOptions;
        }
        static StageData[] sStageData = null;

        [InitializeOnLoad]
        class EditorInitialize
        {
            // This code dynamically discovers eligible classes and builds the menu
            // data for the various component pipeline stages.
            static EditorInitialize()
            {
                sStageData = new StageData[Enum.GetValues(typeof(CinemachineCore.Stage)).Length];

                var stageTypes = new List<Type>[Enum.GetValues(typeof(CinemachineCore.Stage)).Length];
                for (int i = 0; i < stageTypes.Length; ++i)
                {
                    sStageData[i].Name = ((CinemachineCore.Stage)i).ToString();
                    stageTypes[i] = new List<Type>();
                }

                // Get all ICinemachineComponents
                var allTypes
                    = ReflectionHelpers.GetTypesInAllDependentAssemblies(
                            (Type t) => typeof(CinemachineComponentBase).IsAssignableFrom(t) && !t.IsAbstract);

                // GML todo: use class attribute instead
                // Create a temp game object so we can instance behaviours
                GameObject go = new GameObject("Cinemachine Temp Object");
                go.hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor;
                foreach (Type t in allTypes)
                {
                    MonoBehaviour b = go.AddComponent(t) as MonoBehaviour;
                    CinemachineComponentBase c = b != null ? (CinemachineComponentBase)b : null;
                    if (c != null)
                    {
                        CinemachineCore.Stage stage = c.Stage;
                        stageTypes[(int)stage].Add(t);
                    }
                }
                GameObject.DestroyImmediate(go);

                // Create the static lists
                for (int i = 0; i < stageTypes.Length; ++i)
                {
                    stageTypes[i].Insert(0, null);  // first item is "none"
                    sStageData[i].types = stageTypes[i].ToArray();
                    GUIContent[] names = new GUIContent[sStageData[i].types.Length];
                    for (int n = 0; n < names.Length; ++n)
                    {
                        if (n == 0)
                        {
                            bool useSimple
                                = (i == (int)CinemachineCore.Stage.Aim)
                                    || (i == (int)CinemachineCore.Stage.Body);
                            names[n] = new GUIContent((useSimple) ? "Do nothing" : "none");
                        }
                        else
                            names[n] = new GUIContent(InspectorUtility.NicifyClassName(sStageData[i].types[n].Name));
                    }
                    sStageData[i].PopupOptions = names;
                }
            }
        }

        /// <summary>
        /// Hack for vcams because there's no way to find out if an editor is collaped
        /// </summary>
        internal static class ActiveEditorRegistry
        {
            static HashSet<UnityEditor.Editor> s_ActiveEditorRegistry = new HashSet<UnityEditor.Editor>();
            public static void SetActiveEditor(UnityEditor.Editor e, bool active)
            {
                if (e != null && active != s_ActiveEditorRegistry.Contains(e))
                {
                    if (active)
                        s_ActiveEditorRegistry.Add(e);
                    else 
                        s_ActiveEditorRegistry.Remove(e);
                    InspectorUtility.RepaintGameView();
                }
            }
            public static bool IsActiveEditor(UnityEditor.Editor e)
            {
                return e == null ? false : s_ActiveEditorRegistry.Contains(e);
            }
        }

        int m_StageSelection;
        bool m_StageError;
        bool m_IsMixedType;
        CinemachineCore.Stage m_Stage;
        List<CinemachineComponentBase> m_EditedComponents;
        List<CinemachineComponentBase> m_ScratchComponentList;
        UnityEditor.Editor m_ComponentEditor;
        System.Reflection.MethodInfo m_ComponentEditorOnSceneGUI;

        // Call this from OnEnable()
        public VcamStageEditor(CinemachineCore.Stage stage)
        {
            m_StageSelection = 0;
            m_StageError = false;
            m_Stage = stage;
            m_EditedComponents = new List<CinemachineComponentBase>();
            m_ScratchComponentList = new List<CinemachineComponentBase>();
        }

        // Call this from OnDisable()
        public void Shutdown()
        {
            ActiveEditorRegistry.SetActiveEditor(m_ComponentEditor, false);
            if (m_ComponentEditor != null)
                UnityEngine.Object.DestroyImmediate(m_ComponentEditor);
            m_ComponentEditor = null;
            m_ComponentEditorOnSceneGUI = null;
            m_EditedComponents.Clear();
            m_ScratchComponentList.Clear();
        }

        // The current editor for the component (may be null)
        public UnityEditor.Editor ComponentEditor { get { return m_ComponentEditor; } }

        // Returns true if there are more than zero options for this pipeline stage
        public bool HasImplementation { get { return sStageData[(int)m_Stage].PopupOptions.Length > 1; } }

        // Can the component type be changed by the user?
        public bool TypeIsLocked { get; set; }

        // Call this from Editor's OnInspectorGUI
        public void OnInspectorGUI()
        {
            m_ScratchComponentList.Clear();
            int numNullComponents = GetComponent(m_Stage, m_ScratchComponentList);

            // Have the edited components changed?
            int numComponents = m_ScratchComponentList.Count;
            bool dirty = numComponents != m_EditedComponents.Count;
            for (int i = 0; !dirty && i < numComponents; ++i)
                dirty = m_ScratchComponentList[i] != m_EditedComponents[i];
            if (dirty)
            {
                if (m_ComponentEditor != null)
                {
                    ActiveEditorRegistry.SetActiveEditor(m_ComponentEditor, false);
                    m_ComponentEditor.ResetTarget();
                    UnityEngine.Object.DestroyImmediate(m_ComponentEditor);
                }
                m_ComponentEditor = null;
                m_ComponentEditorOnSceneGUI = null;

                m_EditedComponents.Clear();
                m_EditedComponents.AddRange(m_ScratchComponentList);
                m_IsMixedType = false;
                for (int i = 1; !m_IsMixedType && i < numComponents; ++i)
                    m_IsMixedType = m_EditedComponents[i].GetType() != m_EditedComponents[i-1].GetType();
            }
            if (numNullComponents > 0 && numComponents > 0)
                m_IsMixedType = true;
            if (numComponents > 0 && m_ComponentEditor == null && !m_IsMixedType)
            {
                UnityEditor.Editor.CreateCachedEditor(m_EditedComponents.ToArray(), null, ref m_ComponentEditor);
                if (m_ComponentEditor != null)
                    m_ComponentEditorOnSceneGUI = m_ComponentEditor.GetType().GetMethod("OnSceneGUI", 
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            }
            m_StageSelection = GetPopupIndexForComponent(numComponents == 0 ? null : m_EditedComponents[0]);

            m_StageError = false;
            for (int i = 0; !m_StageError && i < numComponents; ++i)
                m_StageError = !m_EditedComponents[i].IsValid;

            DrawComponentInspector();
        }

        public void OnSceneGUI()
        {
            // Forward to embedded editor
            if (m_ComponentEditor != null && m_ComponentEditorOnSceneGUI != null)
                m_ComponentEditorOnSceneGUI.Invoke(m_ComponentEditor, null);
        }

        private int GetPopupIndexForComponent(CinemachineComponentBase c)
        {
            if (c != null)
            {
                var types = sStageData[(int)m_Stage].types;
                for (int i = types.Length-1; i > 0; --i)
                    if (c.GetType() == types[i])
                        return i;
            }
            return 0; // none
        }

        private void DrawComponentInspector()
        {
            const float indentSize = 15; // GML wtf get rid of this
            int index = (int)m_Stage;
            Rect rect = EditorGUILayout.GetControlRect(true);

            // Don't use PrefixLabel() because it will link the enabled status of field and label
            GUIContent label = new GUIContent(InspectorUtility.NicifyClassName(m_Stage.ToString()));
            if (m_StageError)
                label.image = EditorGUIUtility.IconContent("console.warnicon.sml").image;
            float labelWidth = EditorGUIUtility.labelWidth - EditorGUI.indentLevel * indentSize;
            Rect r = rect; r.width = labelWidth;
            EditorGUI.LabelField(r, label);

            r = rect; r.width -= labelWidth; r.x += labelWidth;

            EditorGUI.BeginChangeCheck();
            bool wasEnabled = GUI.enabled;
            if (TypeIsLocked)
                GUI.enabled = false;
            EditorGUI.showMixedValue = m_IsMixedType;
            m_StageSelection = EditorGUI.Popup(r, m_StageSelection, sStageData[index].PopupOptions);
            EditorGUI.showMixedValue = false;
            GUI.enabled = wasEnabled;
            Type type = sStageData[index].types[m_StageSelection];
            if (EditorGUI.EndChangeCheck())
            {
                SetComponent(m_Stage, type);
                if (m_StageSelection != 0)
                    sStageData[index].IsExpanded = true;
                Shutdown();
                GUIUtility.ExitGUI();
                return; // let the component editor be recreated
            }

            // Draw the embedded editor
            if (type != null)
            {
                r = new Rect(rect.x, rect.y, labelWidth, rect.height);
                var isExpanded = m_IsMixedType ? false : EditorGUI.Foldout(
                        r, sStageData[index].IsExpanded, GUIContent.none, true);
                if (isExpanded || isExpanded != sStageData[index].IsExpanded)
                {
                    // Make the editor for that stage
                    ActiveEditorRegistry.SetActiveEditor(m_ComponentEditor, isExpanded);
                    if (isExpanded && m_ComponentEditor != null)
                    {
                        ++EditorGUI.indentLevel;
                        m_ComponentEditor.OnInspectorGUI();
                        --EditorGUI.indentLevel;
                    }
                }
                sStageData[index].IsExpanded = isExpanded;
            }
        }

        // Returns the number of null components in this stage
        public delegate int GetComponentDelegate(
            CinemachineCore.Stage stage, List<CinemachineComponentBase> result);
        public GetComponentDelegate GetComponent;

        public delegate void SetComponentDelegate(CinemachineCore.Stage stage, Type type);
        public SetComponentDelegate SetComponent;
    }

    internal class VcamStageEditorPipeline
    {
        static GUIContent ProceduralMotionLabel = new GUIContent(
            "Procedural Motion", 
            "Use the procedural motion algorithms to automatically drive the transform in "
                + "relation to the LookAt and Follow targets.  \n\n"
                + "Body controls the position, and Aim controls the rotation.\n\n"
                + "If Do Nothing is selected, "
                + "then the transform will not be written to, and can be controlled manually "
                + "or otherwise driven by script.");
                
        VcamStageEditor[] m_subeditors;

        // Call from editor's OnEnable
        public void Initialize(
            VcamStageEditor.GetComponentDelegate getComponent,
            VcamStageEditor.SetComponentDelegate setComponent)
        {
            m_subeditors = new VcamStageEditor[(int)CinemachineCore.Stage.Finalize];
            for (CinemachineCore.Stage stage = CinemachineCore.Stage.Body;
                stage < CinemachineCore.Stage.Finalize; ++stage)
            {
                var ed = new VcamStageEditor(stage);
                m_subeditors[(int)stage] = ed;
                ed.GetComponent = getComponent;
                ed.SetComponent = setComponent;
            }
        }

        public void SetStageIsLocked(CinemachineCore.Stage stage)
        {
            m_subeditors[(int)stage].TypeIsLocked = true;
        }

        // Call from editor's OnDisable
        public void Shutdown()
        {
            if (m_subeditors != null)
            {
                for (int i = 0; i < m_subeditors.Length; ++i)
                {
                    if (m_subeditors[i] != null)
                        m_subeditors[i].Shutdown();
                    m_subeditors[i] = null;
                }
                m_subeditors = null;
            }
        }

        // Call from editor's OnInspectorGUI
        public void OnInspectorGUI(bool withHeader)
        {
            if (withHeader)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField(ProceduralMotionLabel, EditorStyles.boldLabel);
            }
            for (int i = 0; i < m_subeditors.Length; ++i)
            {
                var ed = m_subeditors[i];
                if (ed == null)
                    continue;
                if (!ed.HasImplementation)
                    continue;
                ed.OnInspectorGUI(); // may destroy component
            }
        }

        // Call from editor's OnSceneGUI
        public void OnSceneGUI()
        {
            // Forward to the embedded editors
            for (int i = 0; i < m_subeditors.Length; ++i)
            {
                var ed = m_subeditors[i];
                if (ed == null)
                    continue;
                
                ed.OnSceneGUI();
            }
        }
    }
}
