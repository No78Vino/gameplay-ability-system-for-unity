using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GAS.Editor.General;
using GAS.General.Validation;
using GAS.Runtime;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace GAS.Editor
{
    public class GASAssetAggregator : OdinMenuEditorWindow
    {
        private static readonly Type[] _types = new Type[5]
        {
            typeof(ModifierMagnitudeCalculation),
            typeof(GameplayCue),
            typeof(GameplayEffectAsset),
            typeof(AbilityAsset),
            typeof(AbilitySystemComponentPreset)
        };

        private static string[] _libPaths;

        static string[] LibPaths
        {
            get
            {
                if (_libPaths == null) CheckLibPaths();
                return _libPaths;
            }
        }

        private static readonly DirectoryInfo[] _directoryInfos = new DirectoryInfo[5];
        private static readonly List<DirectoryInfo> _subDirectoryInfos = new List<DirectoryInfo>();

        private static readonly string[] MenuNames = new string[5]
        {
            "A- Mod Magnitude Calculation",
            "A- Gameplay Cue",
            "B- Gameplay Effect",
            "C- Ability",
            "D- Ability System Component"
        };

        private const string OpenWindow_MenuItemName = "EX-GAS/Asset Aggregator";
#if EX_GAS_ENABLE_HOT_KEYS
        private const string OpenWindow_MenuItemNameEnh = OpenWindow_MenuItemName + " %F9";
#else
        private const string OpenWindow_MenuItemNameEnh = OpenWindow_MenuItemName;
#endif
        [MenuItem(OpenWindow_MenuItemNameEnh, priority = 1)]
        private static void OpenWindow()
        {
            CheckLibPaths();
            var window = GetWindow<GASAssetAggregator>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(1600, 900);
            window.MenuWidth = 240;
        }

        private void ShowButton(Rect rect)
        {
            if (SirenixEditorGUI.SDFIconButton(rect, "GitHub", SdfIconType.Github))
            {
                Application.OpenURL("https://github.com/No78Vino/gameplay-ability-system-for-unity");
            }
        }

        private static void CheckLibPaths()
        {
            _libPaths = new[]
            {
                GASSettingAsset.MMCLibPath,
                GASSettingAsset.GameplayCueLibPath,
                GASSettingAsset.GameplayEffectLibPath,
                GASSettingAsset.GameplayAbilityLibPath,
                GASSettingAsset.ASCLibPath,
            };

            _subDirectoryInfos.Clear();
            for (var i = 0; i < _directoryInfos.Length; i++)
            {
                var rootMenuName = MenuNames[i];
                _directoryInfos[i] = new DirectoryInfo(rootMenuName, _libPaths[i], _libPaths[i], _types[i], true);

                foreach (var subDir in _directoryInfos[i].SubDirectory)
                    _subDirectoryInfos.Add(new DirectoryInfo(rootMenuName, _libPaths[i], subDir, _types[i], false));
            }
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree();
            tree.Selection.SelectionChanged += OnMenuSelectionChange; //x => Debug.Log(x);
            for (var i = 0; i < MenuNames.Length; i++)
            {
                var menuName = MenuNames[i];
                var libPath = LibPaths[i];
                var type = _types[i];
                tree.Add(menuName, _directoryInfos[i]);
                if (menuName == MenuNames[3])
                {
                    tree.Add(menuName, new AbilityOverview());
                }

                tree.AddAllAssetsAtPath(menuName, libPath, type, true)
                    .AddThumbnailIcons();
            }

            foreach (var subDirectoryInfo in _subDirectoryInfos) tree.Add(subDirectoryInfo.MenuName, subDirectoryInfo);

            tree.Config.DrawSearchToolbar = true;
            tree.Config.SearchToolbarHeight = 30;
            tree.Config.AutoScrollOnSelectionChanged = true;
            tree.Config.DrawScrollView = true;
            tree.Config.AutoHandleKeyboardNavigation = true;
            tree.SortMenuItemsByName(true);

            return tree;
        }

        protected override void OnBeginDrawEditors()
        {
            var selected = MenuTree.Selection.FirstOrDefault();
            var toolbarHeight = MenuTree.Config.SearchToolbarHeight;

            // Draws a toolbar with the name of the currently selected menu item.
            SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
            {
                if (selected != null) GUILayout.Label(selected.Name + " (" + selected.Value.GetType().FullName + ")");

                if (selected != null && (selected.Value is DirectoryInfo || selected.Value is AbilityOverview))
                {
                    var directoryInfo = selected.Value is AbilityOverview
                        ? _directoryInfos[3]
                        : selected.Value as DirectoryInfo;

                    if (SirenixEditorGUI.ToolbarButton(new GUIContent("浏览")))
                    {
                        OpenDirectoryInExplorer(directoryInfo);
                    }

                    if (SirenixEditorGUI.ToolbarButton(new GUIContent("新建子文件夹")))
                    {
                        CreateNewSubDirectory(directoryInfo);
                        GUIUtility
                            .ExitGUI(); // In order to solve: "EndLayoutGroup: BeginLayoutGroup must be called first."
                    }

                    if (SirenixEditorGUI.ToolbarButton(new GUIContent("新建")))
                    {
                        CreateNewAsset(directoryInfo);
                        GUIUtility
                            .ExitGUI(); // In order to solve: "EndLayoutGroup: BeginLayoutGroup must be called first."
                    }

                    if (!directoryInfo.Root)
                    {
                        if (SirenixEditorGUI.ToolbarButton(new GUIContent("删除")))
                        {
                            RemoveSubDirectory(directoryInfo);
                            GUIUtility
                                .ExitGUI(); // In order to solve: "EndLayoutGroup: BeginLayoutGroup must be called first."
                        }
                    }
                }

                if (selected is { Value: ScriptableObject asset })
                {
                    if (SirenixEditorGUI.ToolbarButton(new GUIContent("定位")))
                    {
                        ShowInProject(asset);
                    }

                    if (SirenixEditorGUI.ToolbarButton(new GUIContent("浏览")))
                    {
                        OpenAssetInExplorer(asset);
                    }

                    if (SirenixEditorGUI.ToolbarButton(new GUIContent("定位脚本")))
                    {
                        var monoScript = MonoScript.FromScriptableObject(asset);
                        string path = AssetDatabase.GetAssetPath(monoScript);

                        var obj = AssetDatabase.LoadAssetAtPath<Object>(path);
                        ShowInProject(obj);
                    }

                    if (SirenixEditorGUI.ToolbarButton(new GUIContent("编辑脚本")))
                    {
                        AssetDatabase.OpenAsset(MonoScript.FromScriptableObject(asset));
                    }

                    if (SirenixEditorGUI.ToolbarButton(new GUIContent("删除")))
                    {
                        RemoveAsset(asset);
                    }
                }

                if (SirenixEditorGUI.ToolbarButton(new GUIContent("GAS设置")))
                {
                    GASSettingAggregator.OpenWindow();
                }
            }
            SirenixEditorGUI.EndHorizontalToolbar();
        }

        private void Refresh()
        {
            AssetDatabase.Refresh();
            CheckLibPaths();
            ForceMenuTreeRebuild();
        }

        private void OpenDirectoryInExplorer(DirectoryInfo directoryInfo)
        {
            var path = directoryInfo.Directory.Replace("/", "\\");
            Process.Start("explorer.exe", path);
        }

        private void ShowInProject(Object asset)
        {
            if (asset != null)
            {
                EditorGUIUtility.PingObject(asset);
                Selection.SetActiveObjectWithContext(asset, null);
            }
        }

        private void OpenAssetInExplorer(ScriptableObject asset)
        {
            var path = AssetDatabase.GetAssetPath(asset).Replace("/", "\\");
            Process.Start("explorer.exe", path);
        }

        private void CreateNewSubDirectory(DirectoryInfo directoryInfo)
        {
            StringEditWindow.OpenWindow("Sub Directory Name", "",
                s =>
                {
                    var newPath = directoryInfo.Directory + "/" + s;
                    var isExist = AssetDatabase.IsValidFolder(newPath);
                    return isExist ? ValidationResult.Invalid("Folder already exists!") : ValidationResult.Valid;
                },
                s =>
                {
                    var newPath = directoryInfo.Directory + "/" + s;
                    AssetDatabase.CreateFolder(directoryInfo.Directory, s);
                    Refresh();
                    Debug.Log($"[EX] {newPath} folder created!");
                });
        }

        private void RemoveSubDirectory(DirectoryInfo directoryInfo)
        {
            if (!EditorUtility.DisplayDialog("Warning", "Are you sure you want to delete this folder?", "Yes",
                    "No")) return;

            if (!EditorUtility.DisplayDialog("Second Warning", "ALL FILES in this folder will be DELETED!" +
                                                               "\nAre you sure you want to DELETE this Folder?", "Yes",
                    "No")) return;

            AssetDatabase.DeleteAsset(directoryInfo.Directory);
            Refresh();
            Debug.Log($"[EX] {directoryInfo.Directory} folder deleted!");
        }

        private void CreateNewAsset(DirectoryInfo directoryInfo)
        {
            if (directoryInfo.AssetType == _types[0])
                ScriptableObjectCreator.ShowDialog<ModifierMagnitudeCalculation>(directoryInfo.RootDirectory,
                    TrySelectMenuItemWithObject);
            else if (directoryInfo.AssetType == _types[1])
                ScriptableObjectCreator.ShowDialog<GameplayCue>(directoryInfo.RootDirectory,
                    TrySelectMenuItemWithObject);
            else if (directoryInfo.AssetType == _types[2])
                ScriptableObjectCreator.ShowDialog<GameplayEffectAsset>(directoryInfo.RootDirectory,
                    TrySelectMenuItemWithObject);
            else if (directoryInfo.AssetType == _types[3])
                ScriptableObjectCreator.ShowDialog<AbilityAsset>(directoryInfo.RootDirectory,
                    TrySelectMenuItemWithObject);
            else if (directoryInfo.AssetType == _types[4])
                ScriptableObjectCreator.ShowDialog<AbilitySystemComponentPreset>(directoryInfo.RootDirectory,
                    TrySelectMenuItemWithObject);
        }

        private void RemoveAsset(ScriptableObject asset)
        {
            if (asset == null)
            {
                EditorUtility.DisplayDialog("Warning", "The asset you want to delete is null", "Ok");
                return;
            }
            
            var assetName = asset.name; // Get the name before deleting
            var assetPath = AssetDatabase.GetAssetPath(asset);
            if (EditorUtility.DisplayDialog("Warning",
                    $"Are you sure you want to delete this asset?\n\nName=\"{assetName}\"\nPath=\"{assetPath}\""
                    , "Yes", "No"))
            {
                AssetDatabase.DeleteAsset(assetPath);
                Refresh();
                Debug.Log($"[EX] delete asset: Name=\"{assetName}\", Path=\"{assetPath}\"");
            }
        }

        void OnMenuSelectionChange(SelectionChangedType selectionChangedType)
        {
            var selected = MenuTree.Selection.FirstOrDefault();
            if (selected is { Value: AbilityOverview abilityOverview })
            {
                abilityOverview.Refresh();
            }
        }
    }
}