using System.IO;
using Sirenix.OdinInspector;
using UnityEditor;

namespace GAS.Editor
{
    public class GASCenterViewEffect
    {
        private const string TITLE_GRP = "GameplayEffect编辑页";
        private const string TITLE_GRP_H_A = "GameplayEffect编辑页/A";
        
        private readonly GASSettingAsset _settingAsset;

        public GASCenterViewEffect()
        {
            _settingAsset = GASSettingAsset.Instance;
        }
        
        [TitleGroup(TITLE_GRP)]
        [HorizontalGroup(TITLE_GRP_H_A)]
        [Button("打开Excel文件所在文件夹")]
        void OpenExcelFileExplore()
        {
            var excelFilePath = _settingAsset.PathOfExcelEffect;
            if (File.Exists(excelFilePath))
            {
                if (excelFilePath != null)
                    EditorUtility.RevealInFinder(excelFilePath);
            }
            else
                EditorUtility.DisplayDialog("错误", "Excel文件未找到，请检查设置。", "确定");
        }
        
        [HorizontalGroup(TITLE_GRP_H_A)]
        [Button("打开Json文件所在文件夹")]
        void OpenJsonFileExplore()
        {
            var jsonFilePath = _settingAsset.PathOfJsonEffect;
            if (File.Exists(jsonFilePath))
            {
                if (jsonFilePath != null)
                    EditorUtility.RevealInFinder(jsonFilePath);
            }
            else
                EditorUtility.DisplayDialog("错误", "JSON文件未找到，请检查设置。", "确定");
        }
        
        [HorizontalGroup(TITLE_GRP_H_A)]
        [Button("导出更新Json表")]
        void ExportJson() => CodeGenerator.GenerateGasConfigTables();
        
        [BoxGroup("GameplayEffect")]
        [Button("保存",Icon = SdfIconType.Save)]
        void SaveConfig()
        {
            // TODO
        }
    }
}