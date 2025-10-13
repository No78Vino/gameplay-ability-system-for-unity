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
    public class GASCenterViewAbility
    {
        private const string TITLE_GRP = "Ability编辑页";
        private const string TITLE_GRP_H_A = "Ability编辑页/A";

        private readonly GASSettingAsset _settingAsset = GASSettingAsset.Instance;

        [TitleGroup(TITLE_GRP)]
        [HorizontalGroup(TITLE_GRP_H_A)]
        [Button("打开Excel文件所在文件夹")]
        private void OpenExcelFileExplore()
        {
            var excelFilePath = _settingAsset.PathOfExcelAbility;
            if (File.Exists(excelFilePath))
                EditorUtility.RevealInFinder(excelFilePath);
            else
                EditorUtility.DisplayDialog("错误", "Excel文件未找到，请检查设置。", "确定");
        }

        [HorizontalGroup(TITLE_GRP_H_A)]
        [Button("打开Json文件所在文件夹")]
        private void OpenJsonFileExplore()
        {
            var jsonFilePath = _settingAsset.PathOfJsonAbility;
            if (File.Exists(jsonFilePath))
                EditorUtility.RevealInFinder(jsonFilePath);
            else
                EditorUtility.DisplayDialog("错误", "JSON文件未找到，请检查设置。", "确定");
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
            OnSelectedIdChanged();
        }

        [HorizontalGroup(TITLE_GRP_H_A)]
        [GUIColor("green")]
        [Button("保存", Icon = SdfIconType.Save)]
        private void SaveConfig()
        {
            SaveFile();
            RefreshAll();
        }


        #region 可视化读写编辑 Ability 配置xlsx文件

        private FileInfo _xlsxFileInfo;
        private Dictionary<string, int> _headerMap;
        private Dictionary<int, Dictionary<int, object>> _data;
        private Dictionary<int, int> _idToRowMap;
        private Dictionary<int, List<object>> _abilityLogicParameter;
        
        private void LoadFile()
        {
            var excelFilePath = _settingAsset.PathOfExcelAbility;
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
                _abilityLogicParameter = new Dictionary<int, List<object>>();
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

                    var parameterCol = _headerMap["abilityLogic"];
                    var abilityParams = new List<object>();
                    for (var i = parameterCol + 1; i < 51 + parameterCol; i++)
                    {
                        var v = worksheet.Cells[row, i].Value;
                        rowData.Add(i, v);
                        abilityParams.Add(v);
                    }

                    _data.Add(id, rowData);
                    _abilityLogicParameter.Add(id, abilityParams);
                    _idToRowMap.Add(id, row);
                    row++;
                }
            }
        }

        private void SaveFile()
        {
            var excelFilePath = _settingAsset.PathOfExcelAbility;
            _xlsxFileInfo = new FileInfo(excelFilePath);
            using (var package = new ExcelPackage(_xlsxFileInfo))
            {
                var worksheet = package.Workbook.Worksheets[1];

                // 写入当前ability数据到Excel
                var row = _idToRowMap.TryGetValue(SelectedId, out var existingRow) ? existingRow : MaxRowForNewID();
                worksheet.Cells[row, _headerMap["id"]].Value = SelectedId;
                worksheet.Cells[row, _headerMap["name"]].Value = name;
                worksheet.Cells[row, _headerMap["desc"]].Value = description;
                worksheet.Cells[row, _headerMap["cost"]].Value =
                    ComponentTypes.Contains(AbilityEditComponent.Cost) ? Cost : string.Empty;
                
                worksheet.Cells[row, _headerMap["cdEffect"]].Value =
                    ComponentTypes.Contains(AbilityEditComponent.Cooldown) ? CDEffect : string.Empty;
                worksheet.Cells[row, _headerMap["cd"]].Value = 
                    ComponentTypes.Contains(AbilityEditComponent.Cooldown) ? CD : string.Empty;
			
                worksheet.Cells[row, _headerMap["assetTags"]].Value = 
                    ComponentTypes.Contains(AbilityEditComponent.AssetTags) && assetTags.Count > 0
                    ? string.Join(";", assetTags)
                    : string.Empty;
                worksheet.Cells[row, _headerMap["cancelAbilityWithTags"]].Value = 
                    ComponentTypes.Contains(AbilityEditComponent.CancelAbilityWithTags) && cancelAbilityWithTags.Count > 0
                    ? string.Join(";", cancelAbilityWithTags)
                    : string.Empty;
                worksheet.Cells[row, _headerMap["blockAbilityWithTags"]].Value = 
                    ComponentTypes.Contains(AbilityEditComponent.BlockAbilityWithTags) && blockAbilityWithTags.Count > 0
                    ? string.Join(";", blockAbilityWithTags)
                    : string.Empty;
                worksheet.Cells[row, _headerMap["activationOwnedTags"]].Value = 
                    ComponentTypes.Contains(AbilityEditComponent.ActivationOwnedTags) && activationOwnedTags.Count > 0
                    ? string.Join(";", activationOwnedTags)
                    : string.Empty;
                worksheet.Cells[row, _headerMap["activationRequiredTags"]].Value = 
                    ComponentTypes.Contains(AbilityEditComponent.ActivationRequiredTags) && activationRequiredTags.Count > 0
                    ? string.Join(";", activationRequiredTags)
                    : string.Empty;
                worksheet.Cells[row, _headerMap["activationBlockedTags"]].Value = 
                    ComponentTypes.Contains(AbilityEditComponent.ActivationBlockedTags) && activationBlockedTags.Count > 0
                    ? string.Join(";", activationBlockedTags)
                    : string.Empty;
                
                // abilityLogic需要特殊处理
                worksheet.Cells[row, _headerMap["abilityLogic"]].Value = type;
                var abilityParams = abilityParam.EncodeExcelData();
                for (var i = 0; i < abilityParams.Count; i++)
                {
                    var colIndex = _headerMap["abilityLogic"] + 1 + i;
                    if (colIndex > worksheet.Dimension.End.Column) break; // 防止超出列数
                    worksheet.Cells[row, colIndex].Value = abilityParams[i];
                }

                package.Save();
            }
        }

        public IEnumerable<int> GetAllAbilityIds()
        {
            return _data == null ? new List<int>() : _data.Keys;
        }

        public List<ValueDropdownItem> TagChoices => GasXlsxChoice.Tags();
        public IEnumerable<AbilityEditComponent> ComponentChoice => EditorAbilityHelper.ComponentTypes();
        public IEnumerable<string> AbilityLogicClassChoice => EditorAbilityHelper.GetCachedAbilityLogicTypesName();

        private int MaxRowForNewID()
        {
            var maxRowForNewID = -1;
            var rowIndexes = _idToRowMap.Values;
            if (rowIndexes.Count > 0) maxRowForNewID = rowIndexes.Max() + 1;

            return maxRowForNewID;
        }

        private void OnTypeChange()
        {
            abilityParam = EditorAbilityHelper.CreateAbilityParameter(type);
        }
        
        private void OnSelectedIdChanged()
        {
            if (!_data.ContainsKey(SelectedId)) return;
            
            var selectInfo = _data[SelectedId];

            name = selectInfo.ContainsKey(_headerMap["name"])
                ? selectInfo[_headerMap["name"]]?.ToString()
                : string.Empty;

            description = selectInfo.ContainsKey(_headerMap["desc"])
                ? selectInfo[_headerMap["desc"]]?.ToString()
                : string.Empty;

											
            Cost = selectInfo.ContainsKey(_headerMap["cost"]) 
                && selectInfo[_headerMap["cost"]] != null 
                && int.TryParse(selectInfo[_headerMap["cost"]].ToString(), out var cost) 
                ? cost 
                : 0;
         
            CDEffect = selectInfo.ContainsKey(_headerMap["cdEffect"]) 
                && selectInfo[_headerMap["cdEffect"]] != null 
                && int.TryParse(selectInfo[_headerMap["cdEffect"]].ToString(), out var cdEffect) 
                ? cdEffect 
                : 0;
            CD = selectInfo.ContainsKey(_headerMap["cd"]) 
                && selectInfo[_headerMap["cd"]] != null 
                && int.TryParse(selectInfo[_headerMap["cd"]].ToString(), out var cd) 
                ? cd 
                : 0;
            
            assetTags = 
                selectInfo.ContainsKey(_headerMap["assetTags"]) 
                && selectInfo[_headerMap["assetTags"]] != null
                && !string.IsNullOrEmpty(selectInfo[_headerMap["assetTags"]].ToString())
                ? selectInfo[_headerMap["assetTags"]].ToString().Split(';').Select(int.Parse).ToList()
                : new List<int>();
            
            cancelAbilityWithTags = 
                selectInfo.ContainsKey(_headerMap["cancelAbilityWithTags"]) 
                && selectInfo[_headerMap["cancelAbilityWithTags"]] != null
                && !string.IsNullOrEmpty(selectInfo[_headerMap["cancelAbilityWithTags"]].ToString())
                ? selectInfo[_headerMap["cancelAbilityWithTags"]].ToString().Split(';').Select(int.Parse).ToList()
                : new List<int>();
            
            blockAbilityWithTags = 
                selectInfo.ContainsKey(_headerMap["blockAbilityWithTags"]) 
                && selectInfo[_headerMap["blockAbilityWithTags"]] != null
                && !string.IsNullOrEmpty(selectInfo[_headerMap["blockAbilityWithTags"]].ToString())
                ? selectInfo[_headerMap["blockAbilityWithTags"]].ToString().Split(';').Select(int.Parse).ToList()
                : new List<int>();
            
            activationOwnedTags =
                selectInfo.ContainsKey(_headerMap["activationOwnedTags"]) 
                && selectInfo[_headerMap["activationOwnedTags"]] != null
                && !string.IsNullOrEmpty(selectInfo[_headerMap["activationOwnedTags"]].ToString())
                ? selectInfo[_headerMap["activationOwnedTags"]].ToString().Split(';').Select(int.Parse).ToList()
                : new List<int>();
            
            activationRequiredTags =
                selectInfo.ContainsKey(_headerMap["activationRequiredTags"]) 
                && selectInfo[_headerMap["activationRequiredTags"]] != null
                && !string.IsNullOrEmpty(selectInfo[_headerMap["activationRequiredTags"]].ToString())
                ? selectInfo[_headerMap["activationRequiredTags"]].ToString().Split(';').Select(int.Parse).ToList()
                : new List<int>();
            
            activationBlockedTags =
                selectInfo.ContainsKey(_headerMap["activationBlockedTags"]) 
                && selectInfo[_headerMap["activationBlockedTags"]] != null
                && !string.IsNullOrEmpty(selectInfo[_headerMap["activationBlockedTags"]].ToString())
                ? selectInfo[_headerMap["activationBlockedTags"]].ToString().Split(';').Select(int.Parse).ToList()
                : new List<int>();
            
            // abilityLogic	
            type = selectInfo.ContainsKey(_headerMap["abilityLogic"])
                ? selectInfo[_headerMap["abilityLogic"]]?.ToString()
                : string.Empty;
            abilityParam = _abilityLogicParameter.TryGetValue(SelectedId, out var abilityParams) ? EditorAbilityHelper.CreateAbilityParameter(type, abilityParams) : null;
            
            // Components加载
            ComponentTypes = new List<AbilityEditComponent>();
            if (assetTags.Count > 0) ComponentTypes.Add(AbilityEditComponent.AssetTags);
            if (cancelAbilityWithTags.Count > 0) ComponentTypes.Add(AbilityEditComponent.CancelAbilityWithTags);
            if (blockAbilityWithTags.Count > 0) ComponentTypes.Add(AbilityEditComponent.BlockAbilityWithTags);
            if (activationOwnedTags.Count > 0) ComponentTypes.Add(AbilityEditComponent.ActivationOwnedTags);
            if (activationRequiredTags.Count > 0) ComponentTypes.Add(AbilityEditComponent.ActivationRequiredTags);
            if (activationBlockedTags.Count > 0) ComponentTypes.Add(AbilityEditComponent.ActivationBlockedTags);
            if (Cost > 0) ComponentTypes.Add(AbilityEditComponent.Cost);
            if (CDEffect > 0) ComponentTypes.Add(AbilityEditComponent.Cooldown);
        }

        #endregion


        #region 可视化读写编辑 UI

        private const string T_G = "编辑配置";
        private const string T_G_A = "编辑配置/A";
        private const string T_G_A_B = "编辑配置/A/组件详情";
        private const string T_G_AL = "编辑配置/技能逻辑";
        private const string T_G_A_B_CD = "编辑配置/TAB/详情/冷却CD";
        private const string T_G_TAB = "编辑配置/TAB";

        [TitleGroup(T_G, order: 2)]
        [ValueDropdown(nameof(GetAllAbilityIds))]
        [OnValueChanged(nameof(OnSelectedIdChanged))]
        [LabelText("当前Ability")]
        [LabelWidth(100)]
        [InlineButton(nameof(AddNewAbility), Label = "添加", Icon = SdfIconType.Plus)]
        [InlineButton(nameof(Delete), Label = "删除", Icon = SdfIconType.Trash)]
        public int SelectedId;

        private void AddNewAbility()
        {
            StringEditWindow.OpenWindow("创建新Ability", "0", newID =>
            {
                if (int.TryParse(newID, out var id))
                {
                    if (_data.Keys.Contains(id)) return ValidationResult.Invalid("ID已存在!");
                }
                else
                {
                    EditorWindow.focusedWindow.ShowNotification(new GUIContent("ID必须是数字!"));
                    return ValidationResult.Invalid("ID必须是数字!");
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


        private void Delete()
        {
            if (_data.ContainsKey(SelectedId))
            {
                // 二次弹窗确认
                if (!EditorUtility.DisplayDialog("确认删除", $"你确定要删除Ability ID: {SelectedId}吗？", "是", "否"))
                    return;
                _data.Remove(SelectedId);
                EditorWindow.focusedWindow.ShowNotification(new GUIContent($"已删除Ability ID: {SelectedId}"));
                SelectedId = _idToRowMap.Keys.First(); // 重置选择ID
                OnSelectedIdChanged();
            }
            else
            {
                EditorWindow.focusedWindow.ShowNotification(new GUIContent($"Ability ID: {SelectedId} 不存在!"));
            }
        }

        private bool HasComponent(AbilityEditComponent component)
        {
            return ComponentTypes != null && ComponentTypes.Contains(component);
        }

        [TitleGroup(T_G)] [LabelText("名字")] [LabelWidth(50)] [Tooltip("部分GA/GE编辑页的GA选项会用到这个参数")]
        public string name;

        [TitleGroup(T_G)] [LabelText("描述")] [LabelWidth(50)][Multiline]
        public string description;

        [BoxGroup(T_G_AL)]
        [LabelText("技能逻辑类型")]
        [ValueDropdown(nameof(AbilityLogicClassChoice))]
        [OnValueChanged(nameof(OnTypeChange))]
        public string type;

        [BoxGroup(T_G_AL)][HideLabel] [ShowInInspector] [HideReferenceObjectPicker]
        public IAbilityParam abilityParam;
        
        [TabGroup(T_G_TAB, "组件列表")] 
        [ValueDropdown(nameof(ComponentChoice), IsUniqueList = true)] 
        [LabelText("Ability组件")]
        public List<AbilityEditComponent> ComponentTypes;
        
        [TabGroup(T_G_TAB,"详情")]
        [Title("消耗", Bold = false)]
        [ShowIf("@HasComponent(AbilityEditComponent.Cost)")]
        [ValueDropdown("@GasXlsxChoice.Effects()", IsUniqueList = true)]
        [LabelText("消耗效果")]
        public int Cost;
        
        [TitleGroup(T_G_A_B_CD, BoldTitle = false)]
        [ShowIf("@HasComponent(AbilityEditComponent.Cooldown)")]
        [ValueDropdown("@GasXlsxChoice.Effects()", IsUniqueList = true)]
        [LabelText("CD效果")]
        public int CDEffect;
        
        [TitleGroup(T_G_A_B_CD)]
        [ShowIf("@HasComponent(AbilityEditComponent.Cooldown)")]
        [LabelText("CD时长")]
        public int CD;
        
        [TabGroup(T_G_TAB,"详情")]
        [Title("描述tag", Bold = false)]
        [ShowIf("@HasComponent(AbilityEditComponent.AssetTags)")]
        [ValueDropdown(nameof(TagChoices), IsUniqueList = true)]
        [LabelText(" ")]
        public List<int> assetTags;

        [TabGroup(T_G_TAB,"详情")]
        [Title("拥有【任意】Tag的Ability会被取消", Bold = false)]
        [ShowIf("@HasComponent(AbilityEditComponent.CancelAbilityWithTags)")]
        [ValueDropdown(nameof(TagChoices), IsUniqueList = true)]
        [LabelText(" ")]
        public List<int> cancelAbilityWithTags;

        [TabGroup(T_G_TAB,"详情")]
        [Title("拥有【任意】Tag的Ability会被阻止", Bold = false)]
        [ShowIf("@HasComponent(AbilityEditComponent.BlockAbilityWithTags)")]
        [ValueDropdown(nameof(TagChoices), IsUniqueList = true)]
        [LabelText(" ")]
        public List<int> blockAbilityWithTags;

        [TabGroup(T_G_TAB,"详情")]
        [Title("激活后获得的Tag", Bold = false)]
        [ShowIf("@HasComponent(AbilityEditComponent.ActivationOwnedTags)")]
        [ValueDropdown(nameof(TagChoices), IsUniqueList = true)]
        [LabelText(" ")]
        public List<int> activationOwnedTags;

        [TabGroup(T_G_TAB,"详情")]
        [Title("激活需要的Tag", Bold = false)]
        [ShowIf("@HasComponent(AbilityEditComponent.ActivationRequiredTags)")]
        [ValueDropdown(nameof(TagChoices), IsUniqueList = true)]
        [LabelText(" ")]
        public List<int> activationRequiredTags;

        [TabGroup(T_G_TAB,"详情")]
        [Title("阻止激活的Tag", Bold = false)]
        [ShowIf("@HasComponent(AbilityEditComponent.ActivationBlockedTags)")]
        [ValueDropdown(nameof(TagChoices), IsUniqueList = true)]
        [LabelText(" ")]
        public List<int> activationBlockedTags;
        
        #endregion
    }
}