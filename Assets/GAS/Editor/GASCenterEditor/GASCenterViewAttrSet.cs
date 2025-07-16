using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using System.IO;
using UnityEngine;

namespace GAS.Editor
{
    public class GASCenterViewAttrSet
    {
        private readonly GASSettingAsset _settingAsset;
        private AttrSetInEditor[] _attrSets;

        [TitleGroup("当前配置表内属性集", order: 2)] 
        [LabelText(" ")]
        [ShowInInspector]
        [ReadOnly]
        [ListDrawerSettings(IsReadOnly = true,ShowFoldout = false)]
        public List<AttrSetInEditor> AttrSets;
        
        public GASCenterViewAttrSet()
        {
            _settingAsset = GASSettingAsset.Instance;
            RefreshAttrSetInfo();
        }
        
        [TitleGroup("属性集总览",order:1)]
        [HorizontalGroup("属性集总览/A")]
        [Button("打开属性集Excel文件所在文件夹")]
        void OpenExcelFileExplore()
        {
            var excelFilePath = _settingAsset.PathOfExcelAttrSet;
            if (File.Exists(excelFilePath))
            {
                if (excelFilePath != null)
                    EditorUtility.RevealInFinder(excelFilePath);
            }
            else
                EditorUtility.DisplayDialog("错误", "Tag JSON文件未找到，请检查设置。", "确定");
        }
        
        [HorizontalGroup("属性集总览/A")]
        [Button("打开属性集Json文件所在文件夹")]
        void OpenJsonFileExplore()
        {
            var jsonFilePath = _settingAsset.PathOfJsonAttrSet;
            if (File.Exists(jsonFilePath))
            {
                if (jsonFilePath != null)
                    EditorUtility.RevealInFinder(jsonFilePath);
            }
            else
                EditorUtility.DisplayDialog("错误", "Tag JSON文件未找到，请检查设置。", "确定");
        }
        
        [HorizontalGroup("属性集总览/A")]
        [Button("导出更新Json表")]
        void ExportJson()
        {
            CodeGenerator.GenerateGasConfigTables();
        }
        
        [HorizontalGroup("属性集总览/A")]
        [Button("刷新",Icon = SdfIconType.Recycle)]
        void RefreshAttrSetInfo()
        {
            var jsonFilePath = _settingAsset.PathOfJsonAttrSet;
            // 检查文件是否存在
            if (!File.Exists(jsonFilePath))
            {
                EditorUtility.DisplayDialog("错误", $"JSON文件未找到: {jsonFilePath}", "确定");
                Debug.LogError($"JSON file not found at {jsonFilePath}");
                return;
            }

            var jsonText = File.ReadAllText(jsonFilePath);
            _attrSets = GasJsonReader.ReadAttributeSets(jsonText);
            AttrSets = new List<AttrSetInEditor>(_attrSets);
        }
    }
}