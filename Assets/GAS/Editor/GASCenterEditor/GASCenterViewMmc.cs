using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using System.IO;
using System.Linq;
using GAS.Editor.General;
using GAS.General.Validation;
using GAS.Runtime;
using OfficeOpenXml;
using UnityEngine;

namespace GAS.Editor
{
    public class GASCenterViewMmc
    {
        private const string TITLE_GRP = "MMC编辑页";
        private const string TITLE_GRP_H_A = "MMC编辑页/A";
        
        private readonly GASSettingAsset _settingAsset = GASSettingAsset.Instance;

        public GASCenterViewMmc()
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
                type = string.Empty;
                mmcParam = null;
            }
        }
        
        [TitleGroup(TITLE_GRP, order: 1)]
        [HorizontalGroup(TITLE_GRP_H_A)]
        [Button("打开Excel文件所在文件夹")]
        private void OpenExcelFileExplore()
        {
            var excelFilePath = _settingAsset.PathOfExcelMmc;
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
            var jsonFilePath = _settingAsset.PathOfJsonMmc;
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
        
        #region 可视化读写编辑 Mmc 配置xlsx文件

        private FileInfo _xlsxFileInfo;
        private Dictionary<string, int> _headerMap;
        private Dictionary<int, Dictionary<int, object>> _data;
        private Dictionary<int, List<object>> _mmcParameter;
        private Dictionary<int, int> _idToRowMap;

        private void LoadFile()
        {
            var excelFilePath = _settingAsset.PathOfExcelMmc;
            if (!File.Exists(excelFilePath))
            {
                Debug.LogWarning($"[EX-GAS] MMC配置Excel文件不存在，将以空数据加载: {excelFilePath}");
                _headerMap = new Dictionary<string, int>();
                _data = new Dictionary<int, Dictionary<int, object>>();
                _mmcParameter = new Dictionary<int, List<object>>();
                _idToRowMap = new Dictionary<int, int>();
                return;
            }
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
                _mmcParameter = new Dictionary<int, List<object>>();
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

                    var parameterCol = _headerMap["MmcLogic"];
                    var mmcParams = new List<object>();
                    for (var i = parameterCol + 1; i < 51 + parameterCol; i++)
                    {
                        var v = worksheet.Cells[row, i].Value;
                        rowData.Add(i, v);
                        mmcParams.Add(v);
                    }

                    _data.Add(id, rowData);
                    _mmcParameter.Add(id, mmcParams);
                    _idToRowMap.Add(id, row);
                    row++;
                }
            }
        }

        private void SaveFile()
        {
            var excelFilePath = _settingAsset.PathOfExcelMmc;
            _xlsxFileInfo = new FileInfo(excelFilePath);
            using (var package = new ExcelPackage(_xlsxFileInfo))
            {
                var worksheet = package.Workbook.Worksheets[1];

                // 写入当前cue数据到Excel
                var row = _idToRowMap.TryGetValue(SelectedId, out var existingRow) ? existingRow : MaxRowForNewID();
                worksheet.Cells[row, _headerMap["ID"]].Value = SelectedId;
                worksheet.Cells[row, _headerMap["Name"]].Value = name;
                worksheet.Cells[row, _headerMap["Desc"]].Value = description;
                
                // mmc_logic需要特殊处理
                worksheet.Cells[row, _headerMap["MmcLogic"]].Value = type;
                var mmcParams = mmcParam.EncodeExcelData();
                for (var i = 0; i < mmcParams.Count; i++)
                {
                    var colIndex = _headerMap["MmcLogic"] + 1 + i;
                    if (colIndex > worksheet.Dimension.End.Column) break; // 防止超出列数
                    worksheet.Cells[row, colIndex].Value = mmcParams[i];
                }

                package.Save();
            }
        }

        public IEnumerable<int> GetAllMmcIds()
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

        public IEnumerable<string> MmcClassChoice => EditorMmcHelper.GetCachedMmcTypeNames();

        public List<int> ListIntFromString(string str)
        {
            return GASCenterParseHelper.ParseIntListLoose(str);
        }

        private void OnTypeChange()
        {
            mmcParam = EditorMmcHelper.CreateMmcParameter(type);
        }

        public ValueDropdownItem[] TagChoices => GasJsonReader.TagChoices();

        private void OnSelectedIdChanged()
        {
            if (_data == null || _data.Count == 0 || !_data.TryGetValue(SelectedId, out var selectInfo))
            {
                name = string.Empty;
                description = string.Empty;
                type = string.Empty;
                mmcParam = null;
                return;
            }

            name = selectInfo.ContainsKey(_headerMap["Name"])
                ? selectInfo[_headerMap["Name"]]?.ToString()
                : string.Empty;

            description = selectInfo.ContainsKey(_headerMap["Desc"])
                ? selectInfo[_headerMap["Desc"]]?.ToString()
                : string.Empty;

            // 加载Mmc逻辑
            type = selectInfo.ContainsKey(_headerMap["MmcLogic"])
                ? selectInfo[_headerMap["MmcLogic"]]?.ToString()
                : string.Empty;

            mmcParam = _mmcParameter.TryGetValue(SelectedId, out var mmcParams) ? EditorMmcHelper.CreateMmcParameter(type, mmcParams) : null;
        }

        #endregion

        #region 可视化读写编辑 UI

        [TitleGroup("编辑配置", order: 2)]
        [HorizontalGroup("编辑配置/A")]
        [ValueDropdown(nameof(GetAllMmcIds))]
        [OnValueChanged(nameof(OnSelectedIdChanged))]
        [LabelText("当前编辑Mmc")]
        [InlineButton(nameof(AddNewMmc), Label = "添加", Icon = SdfIconType.Plus)]
        [InlineButton(nameof(DeleteMmc), Label = "删除", Icon = SdfIconType.Trash)]
        public int SelectedId;

        private void AddNewMmc()
        {
            StringEditWindow.OpenWindow("创建新Mmc", "0", newID =>
            {
                if (int.TryParse(newID, out var id))
                {
                    if (_data.Keys.Contains(id)) return ValidationResult.Invalid("Mmc ID已存在!");
                }
                else
                {
                    EditorWindow.focusedWindow.ShowNotification(new GUIContent("Mmc ID必须是数字!"));
                    return ValidationResult.Invalid("Mmc ID必须是数字!");
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


        private void DeleteMmc()
        {
            if (_data.ContainsKey(SelectedId))
            {
                // 二次弹窗确认
                if (!EditorUtility.DisplayDialog("确认删除", $"你确定要删除Mmc ID: {SelectedId}吗？", "是", "否"))
                    return;
                _data.Remove(SelectedId);
                _idToRowMap.Remove(SelectedId);
                _mmcParameter.Remove(SelectedId);
                EditorWindow.focusedWindow.ShowNotification(new GUIContent($"已删除Mmc ID: {SelectedId}"));
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
                    type = string.Empty;
                    mmcParam = null;
                }
            }
            else
            {
                EditorWindow.focusedWindow.ShowNotification(new GUIContent($"Mmc ID: {SelectedId} 不存在!"));
            }
        }

        [TitleGroup("编辑配置")] [LabelText("名字")] [Tooltip("GE编辑页的mmc选项会用到这个参数，方便在编辑GE时选择对应的Mmc")]
        public string name;

        [TitleGroup("编辑配置")] [LabelText("描述")] public string description;

        [BoxGroup("编辑配置/Mmc逻辑")]
        [LabelText("Mmc类型")]
        [ValueDropdown(nameof(MmcClassChoice))]
        [OnValueChanged(nameof(OnTypeChange))]
        public string type;

        [BoxGroup("编辑配置/Mmc逻辑")] [HideLabel] [ShowInInspector] [HideReferenceObjectPicker]
        public XParam mmcParam;

        #endregion
    }
}
