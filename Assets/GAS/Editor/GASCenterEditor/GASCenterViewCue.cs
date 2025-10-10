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
    public class GASCenterViewCue
    {
        private const string TITLE_GRP = "GameplayCue编辑页";
        private const string TITLE_GRP_H_A = "GameplayCue编辑页/A";

        private readonly GASSettingAsset _settingAsset = GASSettingAsset.Instance;

        public GASCenterViewCue()
        {
            LoadFile();
            SelectedId = _data.Keys.FirstOrDefault();
            OnSelectedIdChanged();
        }
        
        [TitleGroup(TITLE_GRP, order: 1)]
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
        private Dictionary<int, List<object>> _cueLogicParameter;
        private Dictionary<int, int> _idToRowMap;

        private void LoadFile()
        {
            var excelFilePath = _settingAsset.PathOfExcelCue;
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
                _cueLogicParameter = new Dictionary<int, List<object>>();
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

                    var parameterCol = _headerMap["cueLogic"];
                    var cueParams = new List<object>();
                    for (var i = parameterCol + 1; i < 51 + parameterCol; i++)
                    {
                        var v = worksheet.Cells[row, i].Value;
                        rowData.Add(i, v);
                        cueParams.Add(v);
                    }

                    _data.Add(id, rowData);
                    _cueLogicParameter.Add(id, cueParams);
                    _idToRowMap.Add(id, row);
                    row++;
                }
            }
        }

        private void SaveFile()
        {
            var excelFilePath = _settingAsset.PathOfExcelCue;
            _xlsxFileInfo = new FileInfo(excelFilePath);
            using (var package = new ExcelPackage(_xlsxFileInfo))
            {
                var worksheet = package.Workbook.Worksheets[1];

                // 写入当前cue数据到Excel
                var row = _idToRowMap.TryGetValue(SelectedId, out var existingRow) ? existingRow : MaxRowForNewID();
                worksheet.Cells[row, _headerMap["id"]].Value = SelectedId;
                worksheet.Cells[row, _headerMap["name"]].Value = name;
                worksheet.Cells[row, _headerMap["desc"]].Value = description;
                worksheet.Cells[row, _headerMap["required_tag"]].Value = requiredTags.Count > 0
                    ? string.Join(";", requiredTags)
                    : string.Empty;
                worksheet.Cells[row, _headerMap["immunity_tag"]].Value = immunityTags.Count > 0
                    ? string.Join(";", immunityTags)
                    : string.Empty;
                // cue_logic需要特殊处理
                worksheet.Cells[row, _headerMap["cueLogic"]].Value = type;
                var cueParams = cueParam.EncodeExcelData();
                for (var i = 0; i < cueParams.Count; i++)
                {
                    var colIndex = _headerMap["cueLogic"] + 1 + i;
                    if (colIndex > worksheet.Dimension.End.Column) break; // 防止超出列数
                    worksheet.Cells[row, colIndex].Value = cueParams[i];
                }

                package.Save();
            }
        }

        public IEnumerable<int> GetAllCueIds()
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

        public IEnumerable<string> CueClassChoice => EditorCueHelper.GetCachedCueTypeNames();

        public List<int> ListIntFromString(string str)
        {
            return string.IsNullOrEmpty(str) ? new List<int>() : str.Split(';').Select(int.Parse).ToList();
        }

        private void OnTypeChange()
        {
            cueParam = EditorCueHelper.CreateCueParameter(type);
        }

        public ValueDropdownItem[] TagChoices => GasJsonReader.TagChoices();

        private void OnSelectedIdChanged()
        {
            var selectInfo = _data[SelectedId];

            name = selectInfo.ContainsKey(_headerMap["name"])
                ? selectInfo[_headerMap["name"]]?.ToString()
                : string.Empty;

            description = selectInfo.ContainsKey(_headerMap["desc"])
                ? selectInfo[_headerMap["desc"]]?.ToString()
                : string.Empty;

            requiredTags = selectInfo.TryGetValue(_headerMap["required_tag"], out var requiredTagValue)
                ? ListIntFromString(requiredTagValue.ToString())
                : new List<int>();

            immunityTags = selectInfo.TryGetValue(_headerMap["immunity_tag"], out var immunityTagValue)
                ? ListIntFromString(immunityTagValue.ToString())
                : new List<int>();

            // 加载Cue逻辑
            type = selectInfo.ContainsKey(_headerMap["cueLogic"])
                ? selectInfo[_headerMap["cueLogic"]]?.ToString()
                : string.Empty;

            cueParam = _cueLogicParameter.TryGetValue(SelectedId, out var cueParams) ? EditorCueHelper.CreateCueParameter(type, cueParams) : null;
        }

        #endregion

        #region 可视化读写编辑 UI

        [TitleGroup("编辑配置", order: 2)]
        [HorizontalGroup("编辑配置/A")]
        [ValueDropdown(nameof(GetAllCueIds))]
        [OnValueChanged(nameof(OnSelectedIdChanged))]
        [LabelText("当前编辑Cue")]
        [InlineButton(nameof(AddNewCue), Label = "添加", Icon = SdfIconType.Plus)]
        [InlineButton(nameof(DeleteCue), Label = "删除", Icon = SdfIconType.Trash)]
        public int SelectedId;

        private void AddNewCue()
        {
            StringEditWindow.OpenWindow("创建新Cue", "0", newID =>
            {
                if (int.TryParse(newID, out var id))
                {
                    if (_data.Keys.Contains(id)) return ValidationResult.Invalid("Cue ID已存在!");
                }
                else
                {
                    EditorWindow.focusedWindow.ShowNotification(new GUIContent("Cue ID必须是数字!"));
                    return ValidationResult.Invalid("Cue ID必须是数字!");
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


        private void DeleteCue()
        {
            if (_data.ContainsKey(SelectedId))
            {
                // 二次弹窗确认
                if (!EditorUtility.DisplayDialog("确认删除", $"你确定要删除Cue ID: {SelectedId}吗？", "是", "否"))
                    return;
                _data.Remove(SelectedId);
                EditorWindow.focusedWindow.ShowNotification(new GUIContent($"已删除Cue ID: {SelectedId}"));
                SelectedId = _idToRowMap.Keys.First(); // 重置选择ID
                OnSelectedIdChanged();
            }
            else
            {
                EditorWindow.focusedWindow.ShowNotification(new GUIContent($"Cue ID: {SelectedId} 不存在!"));
            }
        }

        [TitleGroup("编辑配置")] [LabelText("名字")] [Tooltip("GE和GA编辑页的cue选项会用到这个参数，方便在编辑GE和GA时选择对应的Cue")]
        public string name;

        [TitleGroup("编辑配置")] [LabelText("描述")] public string description;

        [HorizontalGroup("编辑配置/tag")] [ValueDropdown(nameof(TagChoices), IsUniqueList = true)] [LabelText("播放时需求的tag")]
        public List<int> requiredTags;

        [HorizontalGroup("编辑配置/tag")] [ValueDropdown(nameof(TagChoices), IsUniqueList = true)] [LabelText("播放时免疫的tag")]
        public List<int> immunityTags;

        [BoxGroup("编辑配置/Cue逻辑")]
        [LabelText("Cue类型")]
        [ValueDropdown(nameof(CueClassChoice))]
        [OnValueChanged(nameof(OnTypeChange))]
        public string type;

        [BoxGroup("编辑配置/Cue逻辑")] [HideLabel] [ShowInInspector] [HideReferenceObjectPicker]
        public ICueParameter cueParam;

        #endregion
    }
}