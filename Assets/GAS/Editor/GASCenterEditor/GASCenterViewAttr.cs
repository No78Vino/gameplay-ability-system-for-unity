using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using System.IO;
using UnityEngine;

namespace GAS.Editor
{
    public class GASCenterViewAttr
    {
        private readonly GASSettingAsset _settingAsset;
        private AttrInEditor[] _attrs;

        public GASCenterViewAttr()
        {
            _settingAsset = GASSettingAsset.Instance;
            RefreshAttrInfo();
        }
        
        [TitleGroup("属性总览",order:1)]
        [HorizontalGroup("属性总览/A")]
        [Button("打开属性Excel文件所在文件夹")]
        void OpenExcelFileExplore()
        {
            var tagExcelFilePath = _settingAsset.PathOfExcelTag;
            if (File.Exists(tagExcelFilePath))
            {
                if (tagExcelFilePath != null)
                    EditorUtility.RevealInFinder(tagExcelFilePath);
            }
            else
                EditorUtility.DisplayDialog("错误", "Tag JSON文件未找到，请检查设置。", "确定");
        }
        
        [HorizontalGroup("属性总览/A")]
        [Button("打开属性Json文件所在文件夹")]
        void OpenJsonFileExplore()
        {
            var tagJsonFilePath = _settingAsset.PathOfJsonTag;
            if (File.Exists(tagJsonFilePath))
            {
                if (tagJsonFilePath != null)
                    EditorUtility.RevealInFinder(tagJsonFilePath);
            }
            else
                EditorUtility.DisplayDialog("错误", "Tag JSON文件未找到，请检查设置。", "确定");
        }
        
        [HorizontalGroup("属性总览/A")]
        [Button("导出更新Json表")]
        void ExportJson()
        {
            CodeGenerator.GenerateGasConfigTables();
        }
        
        [HorizontalGroup("Tag总览/A")]
        [Button("刷新",Icon = SdfIconType.Recycle)]
        void RefreshAttrInfo()
        {
            var jsonFilePath = _settingAsset.PathOfJsonAttr;
            // 检查文件是否存在
            if (!File.Exists(jsonFilePath))
            {
                EditorUtility.DisplayDialog("错误", $"属性 JSON文件未找到: {jsonFilePath}", "确定");
                Debug.LogError($"属性 JSON file not found at {jsonFilePath}");
                return;
            }

            var jsonText = File.ReadAllText(jsonFilePath);
            _attrs = GasJsonReader.ReadAttributes(jsonText);
            Attrs = new List<AttrInEditor>(_attrs);
        }

        [TitleGroup("当前配置表内属性", order: 2)] 
        [HideLabel]
        [TableMatrix]
        public List<AttrInEditor> Attrs;
    }
}