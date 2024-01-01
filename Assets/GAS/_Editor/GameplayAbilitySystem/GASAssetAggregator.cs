using System;
using System.Collections.Generic;
using System.Linq;
using GAS.Core;
using GAS.Runtime.Ability;
using GAS.Runtime.Component;
using GAS.Runtime.Cue;
using GAS.Runtime.Effects;
using GAS.Runtime.Effects.Modifier;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;
using YooAsset;

namespace GAS.Editor.GameplayAbilitySystem
{
    public class GASAssetAggregator : OdinMenuEditorWindow
    {
        private string[] _menuNames = new string[5] { "MMC", "Cue", "Effect", "Ability", "ASC" };
        
        static readonly Type[] _types = new Type[5]
        {
            typeof(ModifierMagnitudeCalculation),
            typeof(GameplayCue),
            typeof(GameplayEffectAsset),
            typeof(AbilityAsset),
            typeof(AbilitySystemComponentPreset)
        };
        
        private static string[] _libPaths;
        
        [MenuItem("EX-GAS/Asset Aggregator")]
        private static void OpenWindow()
        {
            CheckLibPaths();
            var window = GetWindow<GASAssetAggregator>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(900, 600);
        }
        
        static void CheckLibPaths()
        {
            _libPaths = new[]
            {
                GASSettingAsset.MMCLibPath,
                GASSettingAsset.GameplayCueLibPath,
                GASSettingAsset.GameplayEffectLibPath,
                GASSettingAsset.GameplayAbilityLibPath,
                GASSettingAsset.ASCLibPath,
            };
        }
        
        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree();
            
            
            for (var i = 0; i < _menuNames.Length; i++)
            {
                var menuName = _menuNames[i];
                var libPath = _libPaths[i];
                
                var type = _types[i];
                tree.Add(menuName,new AssetsInfo(libPath));
                tree.AddAllAssetsAtPath(menuName, libPath, type, true, false)
                    .AddThumbnailIcons();
            }
            
            tree.Config.DrawSearchToolbar = true;
            tree.Config.SearchToolbarHeight = 30;
            tree.Config.AutoHandleKeyboardNavigation = true;
            return tree;
        }
        
        protected override void OnBeginDrawEditors()
        {
            var selected = this.MenuTree.Selection.FirstOrDefault();
            var toolbarHeight = this.MenuTree.Config.SearchToolbarHeight;

            // Draws a toolbar with the name of the currently selected menu item.
            SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
            {
                if (selected != null)
                {
                    GUILayout.Label(selected.Name);
                }

                if (SirenixEditorGUI.ToolbarButton(new GUIContent("Create Item")))
                {
                    // ScriptableObjectCreator.ShowDialog<Item>("Assets/Plugins/Sirenix/Demos/Sample - RPG Editor/Items", obj =>
                    // {
                    //     obj.Name = obj.name;
                    //     base.TrySelectMenuItemWithObject(obj); // Selects the newly created item in the editor
                    // });
                }

                if (SirenixEditorGUI.ToolbarButton(new GUIContent("Create Character")))
                {
                    // ScriptableObjectCreator.ShowDialog<Character>("Assets/Plugins/Sirenix/Demos/Sample - RPG Editor/Character", obj =>
                    // {
                    //     obj.Name = obj.name;
                    //     base.TrySelectMenuItemWithObject(obj); // Selects the newly created item in the editor
                    // });
                }
            }
            SirenixEditorGUI.EndHorizontalToolbar();
        }
        
        class AssetsInfo
        {
            private string _libPath;
            
            //[TableList(IsReadOnly = true, AlwaysExpanded = true), ShowInInspector]
            [LabelText("SubDirs")]
            public List<string> _subDirs = new List<string>();
            
            public AssetsInfo(string libPath)
            {
                _libPath = libPath;
                GetAllSubDir(_libPath,_subDirs);
            }
            
            void GetAllSubDir(string path, List<string> subDirs)
            {
                var dirs = System.IO.Directory.GetDirectories(path);
                foreach (var dir in dirs)
                {
                    subDirs.Add(dir);
                    GetAllSubDir(dir, subDirs);
                }
            }
        }
    }
}