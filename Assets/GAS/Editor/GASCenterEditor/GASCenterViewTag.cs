using System;
using System.IO;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace GAS.Editor
{
    /// <summary>
    /// GAS中心 当前Tag总览
    /// </summary>
    public class GASCenterViewTag
    {
        private GASSettingAsset _settingAsset;
        private TagInEditor[] _tags;
        private OdinMenuTree tree;
        
        /// <summary>
        ///  加载Tag的Json数据
        /// </summary>
        private void LoadTagJsonData()
        {
            var tagJsonFilePath = _settingAsset.PathOfJsonTag;
            // 检查文件是否存在
            if (!File.Exists(tagJsonFilePath))
            {
                EditorUtility.DisplayDialog("错误", $"Tag JSON文件未找到: {tagJsonFilePath}", "确定");
                UnityEngine.Debug.LogError($"Tag JSON file not found at {tagJsonFilePath}");
                return;
            }
            var tagJsonText = File.ReadAllText(tagJsonFilePath);
            _tags = GasJsonReader.ReadTags(tagJsonText);
        }
        
        public GASCenterViewTag()
        {
            _settingAsset = GASSettingAsset.LoadOrCreate();
            LoadTagJsonData();
            //Show();
            tree = BuildMenuTree();
            Init();
        }

        [OnInspectorGUI]
        protected void Draw()
        {
            // 只绘制树形菜单，不绘制其他任何内容
            if (tree != null)
            {
                // 更新配置
                // tree.Config = treeConfig;
                // tree.DefaultMenuStyle = treeConfig.DefaultMenuStyle;
                //
                // 绘制树形菜单
                tree.DrawMenuTree();
            }
        }
        
        protected void OnEnable()
        {
            
        }

        void Init()
        {
            BuildMenuTree();
        }
        

        protected OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree();
            foreach (var tag in _tags)
            {
                var tagMenu = tag.name;
                tagMenu = tagMenu.Replace('.', '/'); // 替换点为斜杠
                tree.Add(tagMenu,new TagDesc()
                {
                    Description = $"<color=white>{tag.desc}</color>"
                });
            }

            // tree.Add("Setting基本设置",Setting());
            // tree.Add("GameplayTag标签", GameplayTagEditor());
            // tree.Add("Attribute属性", AttributeEditor());
            // tree.Add("Attribute Set属性集", AttributeSetEditor());
            // tree.Add("GameplayEffect效果buff", GameplayEffectEditor());
            // tree.Add("GameplayAbility技能", GameplayAbilitySystemEditor());

            tree.Config.AutoScrollOnSelectionChanged = true;
            tree.Config.DrawScrollView = true;
            tree.Config.AutoHandleKeyboardNavigation = true;
            return tree;
        }

        [Serializable]
        class TagDesc
        {
            [TitleGroup("Tag描述")]
            [HideLabel]
            [DisplayAsString(EnableRichText=true)]
            public string Description;
        }
    }
}