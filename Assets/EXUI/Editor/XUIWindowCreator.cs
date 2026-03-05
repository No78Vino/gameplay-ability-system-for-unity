using System.Collections.Generic;  
using Sirenix.OdinInspector;  
using Sirenix.OdinInspector.Editor;  
using Sirenix.Utilities;  
using Sirenix.Utilities.Editor;  
using UnityEditor;  
using UnityEngine;  
  
namespace EXUI.Editor  
{  
    /// <summary>  
    /// EXUI 窗口生成器。  
    /// 整合 XUIPrefabScanner + XUICodeGenerator，提供可视化向导。  
    /// 菜单入口：EXTool/XUI/窗口生成器  
    /// </summary>  
    public class XUIWindowCreator : OdinMenuEditorWindow  
    {  
        [MenuItem(XUIConstDefine.MENU_WINDOW_CREATOR)]  
        public static void OpenWindow()  
        {  
            var window = GetWindow<XUIWindowCreator>();  
            window.titleContent = new GUIContent("XUI 窗口生成器");  
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(900, 600);  
        }  
  
        private XUICreatorView _creatorView;  
  
        protected override OdinMenuTree BuildMenuTree()  
        {  
            var tree = new OdinMenuTree();  
            tree.Add("XUI设置", XUISettingAsset.Instance);          // 直接展示 ScriptableSingleton  
            tree.Add("窗口生成器", CreatorView());  
            tree.Config.AutoScrollOnSelectionChanged = true;  
            tree.Config.DrawScrollView = true;  
            return tree;  
        }  
  
        private XUICreatorView CreatorView()  
        {  
            return _creatorView ??= new XUICreatorView();  
        }  
    }  
}