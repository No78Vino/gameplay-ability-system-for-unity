#if UNITY_2021_2_OR_NEWER
using System;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.Overlays;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

namespace Cinemachine.Editor
{
#if UNITY_2022_1_OR_NEWER
    /// <summary>
    /// This is a generic Tool class for Cinemachine tools.
    /// To create a new tool, inherit from CinemachineTool and implement GetIcon().
    /// Your new tool will need to have the [EditorTool("tool-name", typeof(CinemachineVirtualCameraBase))] attribute.
    ///
    /// A tool will be drawn iff it has been registered using CinemachineSceneToolUtility.RegisterTool.
    /// This is generally done in the OnEnable function of the editor script of the cinemachine component
    /// (CinemachineVirtualCamera, CinemachineComponentBase), for which the tool was meant.
    /// To unregister, call CinemachineSceneToolUtility.UnregisterTool in the same script's OnDisable function.
    ///
    /// To draw the handles related to the tool, you need to implement your drawing function and call it in the
    /// editor script's OnSceneGUI function. An alternative for drawing handles is to override this function's
    /// OnToolGUI or OnDrawHandles functions (see EditorTool or IDrawSelectedHandles docs for more information).
    ///
    /// To check, if a tool has been enabled/disabled in the editor script, use CinemachineSceneToolUtility.IsToolActive.
    /// </summary>
    public abstract class CinemachineTool : EditorTool, IDrawSelectedHandles
    {
        GUIContent m_IconContent;
        
        /// <summary>Implement this to set your Tool's icon and tooltip.</summary>
        /// <returns>A GUIContent with an icon set.</returns>
        protected abstract GUIContent GetIcon();

        /// <summary>This lets the editor find the icon of the tool.</summary>
        public override GUIContent toolbarIcon
        {
            get
            {
                if (m_IconContent == null || m_State.refreshIcon)
                {
                    m_IconContent = GetIcon();
                    m_State.refreshIcon = false;
                }
                return m_IconContent;
            }
        }

        /// <summary>This is called when the Tool is selected in the editor.</summary>
        public override void OnActivated()
        {
            base.OnActivated();
            CinemachineSceneToolUtility.SetTool(true, GetType());
            m_State.refreshIcon = true;
            m_State.isSelected = true;
        }

        /// <summary>This is called when the Tool is deselected in the editor.</summary>
        public override void OnWillBeDeactivated()
        {
            base.OnWillBeDeactivated();
            CinemachineSceneToolUtility.SetTool(false, GetType());
            m_State.refreshIcon = true;
            m_State.isSelected = false;
        }
        
        /// <summary>This checks whether this tool should be displayed or not.</summary>
        /// <returns>True, when this tool is to be drawn. False, otherwise.</returns>
        public override bool IsAvailable() => CinemachineSceneToolUtility.IsToolRequired(GetType());

        /// <summary>Implement IDrawSelectedHandles to draw gizmos for this tool even if it is not the active tool.</summary>
        public void OnDrawHandles()
        {
        }

        private protected string GetIconPath()
        {
            m_State.refreshIcon = m_State.isProSkin != EditorGUIUtility.isProSkin;
            m_State.isProSkin = EditorGUIUtility.isProSkin;
            return ScriptableObjectUtility.CinemachineRealativeInstallPath + "/Editor/EditorResources/Handles/" +
                (m_State.isProSkin ? 
                    (m_State.isSelected ? "Dark-Selected" : "Dark") : 
                    (m_State.isSelected ? "Light-Selected" : "Light")) + "/";
        }

        struct ToolState
        {
            public bool isSelected;
            public bool isProSkin;
            public bool refreshIcon;
        }
        ToolState m_State = new ToolState { refreshIcon = true };
    }
    
    [EditorTool("Field of View Tool", typeof(CinemachineVirtualCameraBase))]
    class FoVTool : CinemachineTool
    {
        protected override GUIContent GetIcon() =>
            new GUIContent
            {
                image = AssetDatabase.LoadAssetAtPath<Texture2D>(GetIconPath() + "FOV.png"),
                tooltip = "Field of View Tool",
            };
    }
    
    [EditorTool("Far-Near Clip Tool", typeof(CinemachineVirtualCameraBase))]
    class FarNearClipTool : CinemachineTool
    {
        protected override GUIContent GetIcon() =>
            new GUIContent
            {
                image = AssetDatabase.LoadAssetAtPath<Texture2D>(GetIconPath() + "FarNearClip.png"),
                tooltip = "Field of View Tool",
            };
    }
    
    [EditorTool("Follow Offset Tool", typeof(CinemachineVirtualCameraBase))]
    class FollowOffsetTool : CinemachineTool
    {
        protected override GUIContent GetIcon() =>
            new GUIContent
            {
                image = AssetDatabase.LoadAssetAtPath<Texture2D>(GetIconPath() + "FollowOffset.png"),
                tooltip = "Field of View Tool",
            };
    }

    [EditorTool("Tracked Object Offset Tool", typeof(CinemachineVirtualCameraBase))]
    class TrackedObjectOffsetTool : CinemachineTool
    {
        protected override GUIContent GetIcon() =>
            new GUIContent
            {
                image = AssetDatabase.LoadAssetAtPath<Texture2D>(GetIconPath() + "TrackedObjectOffset.png"),
                tooltip = "Field of View Tool",
            };
    }

    /// <summary>
    /// To add your custom tools (EditorToolbarElement) to the Cinemachine Tool Settings toolbar,
    /// set CinemachineToolSettingsOverlay.customToolbarItems with your custom tools' IDs.
    ///
    /// By default, CinemachineToolSettingsOverlay.customToolbarItems is null.
    /// </summary>
    [Overlay(typeof(SceneView), "Cinemachine Tool Settings")]
    [Icon("Packages/com.unity.cinemachine/Gizmos/cm_logo.png")]
    public class CinemachineToolSettingsOverlay : Overlay, ICreateToolbar
    {
        static readonly string[] k_CmToolbarItems = { FreelookRigSelection.id };

        /// <summary>
        /// Override this method to return your visual element content.
        /// By default, this draws the same visual element as the HorizontalToolbar
        /// </summary>
        /// <returns>VisualElement for the Panel conent.</returns>
        public override VisualElement CreatePanelContent() => CreateContent(Layout.HorizontalToolbar);
        
        /// <summary>Set this with your custom tools' IDs.</summary>
        public static string[] customToolbarItems = null;
        
        /// <summary>The list of tools that this toolbar is going to contain.</summary>
        public IEnumerable<string> toolbarElements
        {
            get
            {
                if (customToolbarItems != null)
                {
                    var toolbarItems = new List<string>(k_CmToolbarItems);
                    toolbarItems.AddRange(customToolbarItems);
                    return toolbarItems;
                }
                
                return k_CmToolbarItems;
            }
        }
    }
    
    [EditorToolbarElement(id, typeof(SceneView))]
    class FreelookRigSelection : EditorToolbarDropdown
    {
        public const string id = "FreelookRigSelection/Dropdown";
        public static int SelectedRig;
        Texture2D[] m_Icons;

        public FreelookRigSelection()
        {
            tooltip = "Freelook Rig Selection";
            clicked += FreelookRigSelectionMenu;
            EditorApplication.update += ShadowSelectedRigName;
            EditorApplication.update += DisplayIfRequired;
            
            m_Icons = new Texture2D[]
            {
                AssetDatabase.LoadAssetAtPath<Texture2D>(ScriptableObjectUtility.CinemachineRealativeInstallPath
                    + "/Editor/EditorResources/Handles/FreelookRigTop.png"),
                AssetDatabase.LoadAssetAtPath<Texture2D>(ScriptableObjectUtility.CinemachineRealativeInstallPath
                    + "/Editor/EditorResources/Handles/FreelookRigMiddle.png"),
                AssetDatabase.LoadAssetAtPath<Texture2D>(ScriptableObjectUtility.CinemachineRealativeInstallPath
                    + "/Editor/EditorResources/Handles/FreelookRigBottom.png"),
            };
        }

        ~FreelookRigSelection()
        {
            clicked -= FreelookRigSelectionMenu;
            EditorApplication.update -= ShadowSelectedRigName;
            EditorApplication.update -= DisplayIfRequired;
        }

        Type m_FreelookRigSelectionType = typeof(FreelookRigSelection);
        void DisplayIfRequired() => style.display = 
            CinemachineSceneToolUtility.IsToolRequired(m_FreelookRigSelectionType) 
                ? DisplayStyle.Flex : DisplayStyle.None;

        // text is currently only visibly in Panel mode due to this bug: https://jira.unity3d.com/browse/STO-2278
        void ShadowSelectedRigName()
        {
            var index = Mathf.Clamp(SelectedRig, 0, CinemachineFreeLookEditor.RigNames.Length - 1);
            text = CinemachineFreeLookEditor.RigNames[index].text;
            icon = m_Icons[index];
        }

        void FreelookRigSelectionMenu()
        {
            var menu = new GenericMenu();
            for (var i = 0; i < CinemachineFreeLookEditor.RigNames.Length; ++i)
            {
                var rigIndex = i; // vital to capture the index here for the lambda below
                menu.AddItem(CinemachineFreeLookEditor.RigNames[i], false, () =>
                {
                    SelectedRig = rigIndex;
                    var active = Selection.activeObject as GameObject;
                    if (active != null)
                    {
                        var freelook = active.GetComponent<CinemachineFreeLook>();
                        if (freelook != null)
                            CinemachineFreeLookEditor.SetSelectedRig(freelook, rigIndex);
                    }
                });
            }
            menu.DropDown(worldBound);
        }
    }
#else
    /// <summary>
    /// To display a CinemachineExclusiveEditorToolbarToggle in the Cinemachine Toolbar.
    /// </summary>
    [Overlay(typeof(SceneView), "Cinemachine")]
    [Icon("Packages/com.unity.cinemachine/Gizmos/cm_logo.png")]
    class CinemachineToolbarOverlay : ToolbarOverlay
    {
        public CinemachineToolbarOverlay()
            : base(
                FreelookRigSelection.id,
                FoVTool.id,
                FarNearClipTool.id,
                FollowOffsetTool.id,
                TrackedObjectOffsetTool.id
            )
        {
            CinemachineSceneToolUtility.RegisterToolbarIsDisplayedHandler(() => displayed);
            CinemachineSceneToolUtility.RegisterToolbarDisplayHandler(v =>
            {
                if (displayed == v)
                {
                    return false;
                }
                displayed = v;
                return true;
            });
        }
    }
    
    

    /// <summary>
    /// Creates a toggle tool on the Cinemachine toolbar that is exclusive with other
    /// CinemachineExclusiveEditorToolbarToggles. Meaning, that maximum one
    /// CinemachineExclusiveEditorToolbarToggle can be active at any time.
    /// </summary>
    abstract class CinemachineExclusiveEditorToolbarToggle : EditorToolbarToggle
    {
        protected CinemachineExclusiveEditorToolbarToggle()
        {
            var type = GetType();
            this.RegisterValueChangedCallback(
                v => CinemachineSceneToolUtility.SetTool(v.newValue, type));
            CinemachineSceneToolUtility.RegisterExclusiveToolHandlers(type, isOn => value = isOn, 
                display => style.display = display ? DisplayStyle.Flex : DisplayStyle.None);
        }
    }
    
    [EditorToolbarElement(id, typeof(SceneView))]
    class FoVTool : CinemachineExclusiveEditorToolbarToggle
    {
        public const string id = "FoVTool/Toggle";

        public FoVTool()
        {
            icon = AssetDatabase.LoadAssetAtPath<Texture2D>(ScriptableObjectUtility.CinemachineRealativeInstallPath 
                + "/Editor/EditorResources/Handles/Dark/FOV.png");
            tooltip = "Field of View Tool";
        }
    }

    [EditorToolbarElement(id, typeof(SceneView))]
    class FarNearClipTool : CinemachineExclusiveEditorToolbarToggle
    {
        public const string id = "FarNearClipTool/Toggle";

        public FarNearClipTool()
        {
            icon = AssetDatabase.LoadAssetAtPath<Texture2D>(ScriptableObjectUtility.CinemachineRealativeInstallPath 
                + "/Editor/EditorResources/Handles/Dark/FarNearClip.png");
            tooltip = "Far/Near Clip Tool";
        }
    }

    [EditorToolbarElement(id, typeof(SceneView))]
    class FollowOffsetTool : CinemachineExclusiveEditorToolbarToggle
    {
        public const string id = "FollowOffsetTool/Toggle";

        public FollowOffsetTool()
        {
            icon = AssetDatabase.LoadAssetAtPath<Texture2D>(ScriptableObjectUtility.CinemachineRealativeInstallPath 
                + "/Editor/EditorResources/Handles/Dark/FollowOffset.png");
            tooltip = "Follow Offset Tool";
        }
    }

    [EditorToolbarElement(id, typeof(SceneView))]
    class TrackedObjectOffsetTool : CinemachineExclusiveEditorToolbarToggle
    {
        public const string id = "TrackedObjectOffsetTool/Toggle";

        public TrackedObjectOffsetTool()
        {
            icon = AssetDatabase.LoadAssetAtPath<Texture2D>(ScriptableObjectUtility.CinemachineRealativeInstallPath 
                + "/Editor/EditorResources/Handles/Dark/TrackedObjectOffset.png");
            tooltip = "Tracked Object Offset Tool";
        }
    }
    
    /// <summary>
    /// Creates a toggle tool on the Cinemachine toolbar.
    /// </summary>
    abstract class CinemachineEditorToolbarToggle : EditorToolbarToggle
    {
        protected CinemachineEditorToolbarToggle()
        {
            CinemachineSceneToolUtility.RegisterToolHandlers(GetType(), isOn => value = isOn, 
                display => style.display = display ? DisplayStyle.Flex : DisplayStyle.None);
        }
    }
    
    [EditorToolbarElement(id, typeof(SceneView))]
    class FreelookRigSelection : EditorToolbarDropdown
    {
        public const string id = "FreelookRigSelection/Dropdown";
        public static int SelectedRig;
        Texture2D[] m_Icons;

        public FreelookRigSelection()
        {
            tooltip = "Freelook Rig Selection";
            clicked += FreelookRigSelectionMenu;
            CinemachineSceneToolUtility.RegisterToolHandlers(GetType(), isOn => {}, 
                display => style.display = display ? DisplayStyle.Flex : DisplayStyle.None);
            EditorApplication.update += ShadowSelectedRigName;
            
            m_Icons = new Texture2D[]
            {
                AssetDatabase.LoadAssetAtPath<Texture2D>(ScriptableObjectUtility.CinemachineRealativeInstallPath
                    + "/Editor/EditorResources/Handles/FreelookRigTop.png"),
                AssetDatabase.LoadAssetAtPath<Texture2D>(ScriptableObjectUtility.CinemachineRealativeInstallPath
                    + "/Editor/EditorResources/Handles/FreelookRigMiddle.png"),
                AssetDatabase.LoadAssetAtPath<Texture2D>(ScriptableObjectUtility.CinemachineRealativeInstallPath
                    + "/Editor/EditorResources/Handles/FreelookRigBottom.png"),
            };
        }

        ~FreelookRigSelection()
        {
            clicked -= FreelookRigSelectionMenu;
            EditorApplication.update -= ShadowSelectedRigName;
        }

        void ShadowSelectedRigName()
        {
            var index = Mathf.Clamp(SelectedRig, 0, CinemachineFreeLookEditor.RigNames.Length - 1);
            icon = m_Icons[index];
            text = CinemachineFreeLookEditor.RigNames[index].text;
        }
        
        void FreelookRigSelectionMenu()
        {
            var menu = new GenericMenu();
            for (var i = 0; i < CinemachineFreeLookEditor.RigNames.Length; ++i)
            {
                var rigIndex = i; // vital to capture the index here for the lambda below
                menu.AddItem(CinemachineFreeLookEditor.RigNames[i], false, () =>
                {
                    SelectedRig = rigIndex;
                    var active = Selection.activeObject as GameObject;
                    if (active != null)
                    {
                        var freelook = active.GetComponent<CinemachineFreeLook>();
                        if (freelook != null)
                            CinemachineFreeLookEditor.SetSelectedRig(freelook, rigIndex);
                    }
                });
            }
            menu.DropDown(worldBound);
        }
    }
#endif
}
#endif