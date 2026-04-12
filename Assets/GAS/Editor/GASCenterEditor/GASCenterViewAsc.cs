using System.Collections.Generic;
using System.IO;
using System.Linq;
using GAS.Editor.General;
using GAS.General.Validation;
using GAS.Runtime;
using OfficeOpenXml;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace GAS.Editor
{
    public class GASCenterViewAsc
    {
        private const string TITLE_GRP = "ASC编辑页";
        private const string TITLE_GRP_H_A = "ASC编辑页/A";

        private readonly GASSettingAsset _settingAsset = GASSettingAsset.Instance;

        public GASCenterViewAsc()
        {
            LoadFile();
            if (_data != null && _data.Count > 0)
            {
                SelectedId = _data.Keys.First();
                OnSelectedIdChanged();
            }
            else
            {
                SelectedId = 0;
                name = string.Empty;
                description = string.Empty;
                level = 0;
                tags = new List<int>();
                attrSets = new List<int>();
                abilities = new List<int>();
            }
        }
        
        [TitleGroup(TITLE_GRP, order: 1)]
        [HorizontalGroup(TITLE_GRP_H_A)]
        [Button("打开Excel文件所在文件夹")]
        private void OpenExcelFileExplore()
        {
            var excelFilePath = _settingAsset.PathOfExcelAsc;
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
            var jsonFilePath = _settingAsset.PathOfJsonAsc;
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
            if (_data != null && _data.Count > 0)
            {
                if (!_data.ContainsKey(SelectedId))
                {
                    SelectedId = _data.Keys.First();
                }
                OnSelectedIdChanged();
            }
        }

        [HorizontalGroup(TITLE_GRP_H_A)]
        [Button("保存", Icon = SdfIconType.Save)]
        private void SaveConfig()
        {
            SaveFile();
            RefreshAll();
        }


        #region 可视化读写编辑 GameplayCue 配置xlsx文件

        private FileInfo _xlsxFileInfo;
        private Dictionary<string, int> _headerMap;
        private Dictionary<int, Dictionary<int, object>> _data;
        private Dictionary<int, int> _idToRowMap;

        private void LoadFile()
        {
            var excelFilePath = _settingAsset.PathOfExcelAsc;
            _xlsxFileInfo = new FileInfo(excelFilePath);
            using (var package = new ExcelPackage(_xlsxFileInfo))
            {
                var worksheet = package.Workbook.Worksheets[1];
                // 注册表头
                _headerMap = new Dictionary<string, int>();
                for (var i = 0; i < 500; i++)
                {
                    if (worksheet.Cells[1, i + 1].Value == null) continue;
                    var header = worksheet.Cells[1, i + 1].Value.ToString();
                    // 去除格式后缀（即#之后的内容）
                    header = header.Split('#')[0];
                    if (string.IsNullOrEmpty(header)) continue;
                    _headerMap[header] = i + 1; // Excel列从1开始
                }

                // 读取数据行,从第4行开始，第二列为key，即id。
                // 以第2列是否有值为结束标志
                _data = new Dictionary<int, Dictionary<int, object>>();
                _idToRowMap = new Dictionary<int, int>();
                var safeCnt = 99999;
                var row = 4;
                while (safeCnt > 0 && worksheet.Cells[row, 2].Value != null)
                {
                    safeCnt--;
                    var rowData = new Dictionary<int, object>();
                    var id = int.Parse(worksheet.Cells[row, 2].Value.ToString());

                    foreach (var colIndex in _headerMap.Values)
                        rowData.Add(colIndex, worksheet.Cells[row, colIndex].Value);

                    _data.Add(id, rowData);
                    _idToRowMap.Add(id, row);
                    row++;
                }
            }
        }

        private void SaveFile()
        {
            var excelFilePath = _settingAsset.PathOfExcelAsc;
            _xlsxFileInfo = new FileInfo(excelFilePath);
            using (var package = new ExcelPackage(_xlsxFileInfo))
            {
                var worksheet = package.Workbook.Worksheets[1];

                // 写入当前cue数据到Excel
                var row = _idToRowMap.TryGetValue(SelectedId, out var existingRow) ? existingRow : MaxRowForNewID();
                worksheet.Cells[row, _headerMap["ID"]].Value = SelectedId;
                worksheet.Cells[row, _headerMap["Name"]].Value = name;
                worksheet.Cells[row, _headerMap["Desc"]].Value = description;
                worksheet.Cells[row, _headerMap["Level"]].Value = level;
                
                worksheet.Cells[row, _headerMap["Tag"]].Value = tags.Count > 0
                    ? string.Join(";", tags)
                    : string.Empty;
                worksheet.Cells[row, _headerMap["AttrSet"]].Value = attrSets.Count > 0
                    ? string.Join(";", attrSets)
                    : string.Empty;
                worksheet.Cells[row, _headerMap["Ability"]].Value = abilities.Count > 0
                    ? string.Join(";", abilities)
                    : string.Empty;
                
                package.Save();
            }
        }

        public IEnumerable<int> GetAllAscIds()
        {
            if (_data == null) return new List<int>();
            return _data.Keys;
        }

        private int MaxRowForNewID()
        {
            var maxRowForNewID = -1;
            var rowIndexes = _idToRowMap.Values;
            if (rowIndexes.Count > 0) maxRowForNewID = rowIndexes.Max() + 1;

            return maxRowForNewID;
        }
        
        public List<int> ListIntFromString(string str)
        {
            return GASCenterParseHelper.ParseIntListLoose(str);
        }

        public List<ValueDropdownItem> TagChoices => GasXlsxChoice.Tags();
        public List<ValueDropdownItem> AttrSetChoices => GasXlsxChoice.AttrSets();
        public List<ValueDropdownItem> AbilityChoices => GasXlsxChoice.Abilities();

        private void OnSelectedIdChanged()
        {
            if (_data == null || _data.Count == 0 || !_data.TryGetValue(SelectedId, out var selectInfo))
            {
                name = string.Empty;
                description = string.Empty;
                level = 0;
                tags = new List<int>();
                attrSets = new List<int>();
                abilities = new List<int>();
                return;
            }

            name = selectInfo.ContainsKey(_headerMap["Name"])
                ? selectInfo[_headerMap["Name"]]?.ToString()
                : string.Empty;

            description = selectInfo.ContainsKey(_headerMap["Desc"])
                ? selectInfo[_headerMap["Desc"]]?.ToString()
                : string.Empty;
            level = selectInfo.ContainsKey(_headerMap["Level"])
                ? int.Parse(selectInfo[_headerMap["Level"]]?.ToString() ?? "0")
                : 0;
            
            tags = selectInfo.ContainsKey(_headerMap["Tag"])
                ? ListIntFromString(selectInfo[_headerMap["Tag"]]?.ToString())
                : new List<int>();
            attrSets = selectInfo.ContainsKey(_headerMap["AttrSet"])
                ? ListIntFromString(selectInfo[_headerMap["AttrSet"]]?.ToString())
                : new List<int>();
            abilities = selectInfo.ContainsKey(_headerMap["Ability"])
                ? ListIntFromString(selectInfo[_headerMap["Ability"]]?.ToString())
                : new List<int>();
        }

        #endregion

        #region 可视化读写编辑 UI

        [TitleGroup("编辑配置", order: 2)]
        [HorizontalGroup("编辑配置/A")]
        [ValueDropdown(nameof(GetAllAscIds))]
        [OnValueChanged(nameof(OnSelectedIdChanged))]
        [LabelText("当前编辑ASC")]
        [InlineButton(nameof(AddNewAsc), Label = "添加", Icon = SdfIconType.Plus)]
        [InlineButton(nameof(DeleteAsc), Label = "删除", Icon = SdfIconType.Trash)]
        public int SelectedId;

        private void AddNewAsc()
        {
            StringEditWindow.OpenWindow("创建新ASC", "0", newID =>
            {
                if (int.TryParse(newID, out var id))
                {
                    if (_data.Keys.Contains(id)) return ValidationResult.Invalid("ASC ID已存在!");
                }
                else
                {
                    EditorWindow.focusedWindow.ShowNotification(new GUIContent("ASC ID必须是数字!"));
                    return ValidationResult.Invalid("ASC ID必须是数字!");
                }

                return ValidationResult.Valid;
            }, x =>
            {
                var id = int.Parse(x);
                _data.Add(id, new Dictionary<int, object>());
                SelectedId = id; // 重置选择ID
                OnSelectedIdChanged();
            });
        }


        private void DeleteAsc()
        {
            if (_data.ContainsKey(SelectedId))
            {
                // 二次弹窗确认
                if (!EditorUtility.DisplayDialog("确认删除", $"你确定要删除ASC ID: {SelectedId}吗？", "是", "否"))
                    return;
                _data.Remove(SelectedId);
                _idToRowMap.Remove(SelectedId);
                EditorWindow.focusedWindow.ShowNotification(new GUIContent($"已删除ASC ID: {SelectedId}"));
                if (_data.Count > 0)
                {
                    SelectedId = _data.Keys.First(); // 重置选择ID
                    OnSelectedIdChanged();
                }
                else
                {
                    SelectedId = 0;
                    name = string.Empty;
                    description = string.Empty;
                    level = 0;
                    tags = new List<int>();
                    attrSets = new List<int>();
                    abilities = new List<int>();
                }
            }
            else
            {
                EditorWindow.focusedWindow.ShowNotification(new GUIContent($"ASC ID: {SelectedId} 不存在!"));
            }
        }

        [TitleGroup("编辑配置")] [LabelText("名字")]
        public string name;

        [TitleGroup("编辑配置")] 
        [LabelText("描述")] 
        [Multiline]
        public string description;
        
        [TitleGroup("编辑配置")] 
        [LabelText("等级")] 
        public int level;
        
        [HorizontalGroup("编辑配置/B")]
        [Space]
        [LabelText("标签")]
        [ValueDropdown(nameof(TagChoices), IsUniqueList = true)]
        [ListDrawerSettings(ShowFoldout = false)]
        public List<int> tags = new();
        
        [HorizontalGroup("编辑配置/B")]
        [Space]
        [LabelText("属性集")]
        [ValueDropdown(nameof(AttrSetChoices), IsUniqueList = true)]
        [ListDrawerSettings(ShowFoldout = false)]
        public List<int> attrSets = new();
        
        [HorizontalGroup("编辑配置/B")]
        [Space]
        [LabelText("技能")]
        [ValueDropdown(nameof(AbilityChoices), IsUniqueList = true)]
        [ListDrawerSettings(ShowFoldout = false)]
        public List<int> abilities = new();

        #endregion
    }
}
