using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using UnityEditor;
using OfficeOpenXml;

namespace GAS.Editor
{
    public class GASCenterViewEffect
    {
        private const string TITLE_GRP = "GameplayEffect编辑页";
        private const string TITLE_GRP_H_A = "GameplayEffect编辑页/A";

        private readonly GASSettingAsset _settingAsset = GASSettingAsset.Instance;

        [TitleGroup(TITLE_GRP)]
        [HorizontalGroup(TITLE_GRP_H_A)]
        [Button("打开Excel文件所在文件夹")]
        private void OpenExcelFileExplore()
        {
            var excelFilePath = _settingAsset.PathOfExcelEffect;
            if (File.Exists(excelFilePath))
            {
                if (excelFilePath != null)
                    EditorUtility.RevealInFinder(excelFilePath);
            }
            else
            {
                EditorUtility.DisplayDialog("错误", "Excel文件未找到，请检查设置。", "确定");
            }
        }

        [HorizontalGroup(TITLE_GRP_H_A)]
        [Button("打开Json文件所在文件夹")]
        private void OpenJsonFileExplore()
        {
            var jsonFilePath = _settingAsset.PathOfJsonEffect;
            if (File.Exists(jsonFilePath))
            {
                if (jsonFilePath != null)
                    EditorUtility.RevealInFinder(jsonFilePath);
            }
            else
            {
                EditorUtility.DisplayDialog("错误", "JSON文件未找到，请检查设置。", "确定");
            }
        }

        [HorizontalGroup(TITLE_GRP_H_A)]
        [Button("导出更新Json表")]
        private void ExportJson()
        {
            CodeGenerator.GenerateGasConfigTables();
        }
        
        [HorizontalGroup(TITLE_GRP_H_A)]
        [Button("刷新", Icon = SdfIconType.Upload)]
        private void RefreshAll()
        {
            LoadFile();
        }

        [BoxGroup("GameplayEffect")]
        [Button("保存", Icon = SdfIconType.Save)]
        private void SaveConfig()
        {
            // TODO
        }


        #region 可视化读写编辑 GameplayEffect 配置xlsx文件

        private FileInfo _xlsxFileInfo;
        private Dictionary<string,int> _headerMap;
        private void LoadFile()
        {
            var excelFilePath = _settingAsset.PathOfExcelEffect;
            _xlsxFileInfo = new FileInfo(excelFilePath);
            using (ExcelPackage package = new ExcelPackage(_xlsxFileInfo))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                // 注册表头
                _headerMap = new Dictionary<string, int>();
                for (var i = 0; i < 200; i++)
                {
                    if( worksheet.Cells[1, i + 1].Value == null) break;
                    var header = worksheet.Cells[1, i + 1].Value.ToString();
                    // 去除格式后缀（即#之后的内容）
                    header = header.Split('#')[0];
                    _headerMap[header] = i + 1; // Excel列从1开始
                }
                
                // 读取数据行,从第4行开始，第二列为key，即id。
                // 以第2列是否有值为结束标志
                
            }

            string a = "LLL";
        }

        #endregion
    }
}