#if UNITY_EDITOR
namespace GAS.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using GAS.Editor.General;
    using GAS.Editor.Validation;
    using GAS.Runtime;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.Utilities;
    using Sirenix.Utilities.Editor;
    using UnityEditor;
    using UnityEngine;
    using Debug = UnityEngine.Debug;

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

        [MenuItem("EX-GAS/Asset Aggregator", priority = 1)]
        private static void OpenWindow()
        {
            CheckLibPaths();
            var window = GetWindow<GASAssetAggregator>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(1050, 625);
            window.MenuWidth = 220;
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
                if (selected != null) GUILayout.Label(selected.Name);

                if (selected != null && (selected.Value is DirectoryInfo || selected.Value is AbilityOverview))
                {
                    DirectoryInfo directoryInfo = selected.Value is AbilityOverview
                        ? _directoryInfos[3]
                        : selected.Value as DirectoryInfo;

                    if (SirenixEditorGUI.ToolbarButton(new GUIContent("Open In Explorer")))
                        OpenDirectoryInExplorer(directoryInfo);

                    if (SirenixEditorGUI.ToolbarButton(new GUIContent("Create Sub Directory")))
                    {
                        CreateNewSubDirectory(directoryInfo);
                        GUIUtility
                            .ExitGUI(); // In order to solve: "EndLayoutGroup: BeginLayoutGroup must be called first."
                    }

                    if (SirenixEditorGUI.ToolbarButton(new GUIContent("Create Asset")))
                    {
                        CreateNewAsset(directoryInfo);
                        GUIUtility
                            .ExitGUI(); // In order to solve: "EndLayoutGroup: BeginLayoutGroup must be called first."
                    }

                    if (!directoryInfo.Root)
                        if (SirenixEditorGUI.ToolbarButton(new GUIContent("Remove")))
                        {
                            RemoveSubDirectory(directoryInfo);
                            GUIUtility
                                .ExitGUI(); // In order to solve: "EndLayoutGroup: BeginLayoutGroup must be called first."
                        }
                }

                if (selected is { Value: ScriptableObject asset })
                {
                    if (SirenixEditorGUI.ToolbarButton(new GUIContent("Show In Project")))
                        ShowInProject(asset);

                    if (SirenixEditorGUI.ToolbarButton(new GUIContent("Open In Explorer")))
                        OpenAssetInExplorer(asset);

                    if (SirenixEditorGUI.ToolbarButton(new GUIContent("Remove")))
                        RemoveAsset(asset);
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

        private void ShowInProject(ScriptableObject asset)
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
                    if (!AssetDatabase.IsValidFolder(newPath))
                    {
                        return ValidationResult.Invalid("Folder already exists!");
                    }

                    return ValidationResult.Valid;
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
            if (!EditorUtility.DisplayDialog("Warning", "Are you sure you want to delete this asset?", "Yes",
                    "No")) return;

            var name = asset.name; // Get the name before deleting
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(asset));
            Refresh();
            Debug.Log($"[EX] {name} asset deleted!");
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
#endif