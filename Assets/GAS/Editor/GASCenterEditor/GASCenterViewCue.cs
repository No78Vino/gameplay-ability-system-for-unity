using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;
using Sirenix.OdinInspector;
using UnityEditor;

namespace GAS.Editor
{
    public class GASCenterViewCue
    {
        private const string TITLE_GRP = "GameplayCue编辑页";
        private const string TITLE_GRP_H_A = "GameplayCue编辑页/A";

        private readonly GASSettingAsset _settingAsset = GASSettingAsset.Instance;

        [TitleGroup(TITLE_GRP)]
        [HorizontalGroup(TITLE_GRP_H_A)]
        [Button("打开Excel文件所在文件夹")]
        private void OpenExcelFileExplore()
        {
            var excelFilePath = _settingAsset.PathOfExcelCue;
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
            var jsonFilePath = _settingAsset.PathOfJsonCue;
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


        #region 可视化读写编辑 GameplayCue 配置xlsx文件

        private FileInfo _xlsxFileInfo;
        private Dictionary<string,int> _headerMap;
        private Dictionary<int, Dictionary<int, object>> _data;
        private void LoadFile()
        {
            var excelFilePath = _settingAsset.PathOfExcelCue;
            _xlsxFileInfo = new FileInfo(excelFilePath);
            using (ExcelPackage package = new ExcelPackage(_xlsxFileInfo))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                // 注册表头
                _headerMap = new Dictionary<string, int>();
                for (var i = 0; i < 500; i++)
                {
                    if( worksheet.Cells[1, i + 1].Value == null) continue;
                    var header = worksheet.Cells[1, i + 1].Value.ToString();
                    // 去除格式后缀（即#之后的内容）
                    header = header.Split('#')[0];
                    if(string.IsNullOrEmpty(header)) continue;
                    _headerMap[header] = i + 1; // Excel列从1开始
                }
                
                // 读取数据行,从第4行开始，第二列为key，即id。
                // 以第2列是否有值为结束标志
                _data = new Dictionary<int, Dictionary<int, object>>();
                int safeCnt = 99999;
                int row = 4;
                while (safeCnt > 0 && worksheet.Cells[row, 2].Value != null)
                {
                    safeCnt--;
                    var rowData = new Dictionary<int, object>();
                    var id = int.Parse(worksheet.Cells[row, 2].Value.ToString());
                    
                    foreach (var colIndex in _headerMap.Values)
                        rowData.Add(colIndex,worksheet.Cells[row, colIndex].Value);
                    
                    _data.Add(id,rowData);
                    row++;
                }
            }

            string a = "LLL";
        }

        #endregion
    }
}