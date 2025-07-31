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

                    var parameterCol = _headerMap["cue_logic"];
                    var cueParams = new List<object>();
                    for (var i = parameterCol + 1; i < 51 + parameterCol; i++)
                    {
                        var v = worksheet.Cells[row, i].Value;
                        rowData.Add(i, v);
                        cueParams.Add(v);
                    }

                    _data.Add(id, rowData);
                    _abilityLogicParameter.Add(id, cueParams);
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
                worksheet.Cells[row, _headerMap["cost"]].Value = cost;
                worksheet.Cells[row, _headerMap["cdEffect"]].Value = cdEffect;
                worksheet.Cells[row, _headerMap["cd"]].Value = cd;
			
                worksheet.Cells[row, _headerMap["assetTags"]].Value = assetTags.Count > 0
                    ? string.Join(";", assetTags)
                    : string.Empty;
                worksheet.Cells[row, _headerMap["cancelAbilityWithTags"]].Value = cancelAbilityWithTags.Count > 0
                    ? string.Join(";", cancelAbilityWithTags)
                    : string.Empty;
                worksheet.Cells[row, _headerMap["blockAbilityWithTags"]].Value = blockAbilityWithTags.Count > 0
                    ? string.Join(";", blockAbilityWithTags)
                    : string.Empty;
                worksheet.Cells[row, _headerMap["activationOwnedTags"]].Value = activationOwnedTags.Count > 0
                    ? string.Join(";", activationOwnedTags)
                    : string.Empty;
                worksheet.Cells[row, _headerMap["activationRequiredTags"]].Value = activationRequiredTags.Count > 0
                    ? string.Join(";", activationRequiredTags)
                    : string.Empty;
                worksheet.Cells[row, _headerMap["activationBlockedTags"]].Value = activationBlockedTags.Count > 0
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

        private int MaxRowForNewID()
        {
            var maxRowForNewID = -1;
            var rowIndexes = _idToRowMap.Values;
            if (rowIndexes.Count > 0) maxRowForNewID = rowIndexes.Max() + 1;

            return maxRowForNewID;
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

            AssetTags = 
                selectInfo.ContainsKey(_headerMap["assetTags"]) 
                && selectInfo[_headerMap["assetTags"]] != null
                && !string.IsNullOrEmpty(selectInfo[_headerMap["assetTags"]].ToString())
                ? selectInfo[_headerMap["assetTags"]].ToString().Split(';').Select(int.Parse).ToList()
                : new List<int>();

            GrantedTags = 
                selectInfo.ContainsKey(_headerMap["grantedTags"]) 
                && selectInfo[_headerMap["grantedTags"]] != null
                && !string.IsNullOrEmpty(selectInfo[_headerMap["grantedTags"]].ToString())
                ? selectInfo[_headerMap["grantedTags"]].ToString().Split(';').Select(int.Parse).ToList()
                : new List<int>();

            ApplicationRequiredTags = 
                selectInfo.ContainsKey(_headerMap["applicationRequiredTags"]) 
                && selectInfo[_headerMap["applicationRequiredTags"]] != null
                && !string.IsNullOrEmpty(selectInfo[_headerMap["applicationRequiredTags"]].ToString())
                ? selectInfo[_headerMap["applicationRequiredTags"]].ToString().Split(';').Select(int.Parse).ToList()
                : new List<int>();

            OngoingRequiredTags = 
                selectInfo.ContainsKey(_headerMap["ongoingRequiredTags"]) 
                && selectInfo[_headerMap["ongoingRequiredTags"]] != null
                && !string.IsNullOrEmpty(selectInfo[_headerMap["ongoingRequiredTags"]].ToString())
                ? selectInfo[_headerMap["ongoingRequiredTags"]].ToString().Split(';').Select(int.Parse).ToList()
                : new List<int>();

            RemoveGameplayEffectsWithTags =
                selectInfo.ContainsKey(_headerMap["removeGameplayEffectsWithTags"]) 
                && selectInfo[_headerMap["removeGameplayEffectsWithTags"]] != null
                && !string.IsNullOrEmpty(selectInfo[_headerMap["removeGameplayEffectsWithTags"]].ToString())
                    ? selectInfo[_headerMap["removeGameplayEffectsWithTags"]].ToString().Split(';').Select(int.Parse)
                        .ToList()
                    : new List<int>();

            ImmunityTags = 
                selectInfo.ContainsKey(_headerMap["immunityTags"]) 
                && selectInfo[_headerMap["immunityTags"]] != null
                && !string.IsNullOrEmpty(selectInfo[_headerMap["immunityTags"]].ToString())
                ? selectInfo[_headerMap["immunityTags"]].ToString().Split(';').Select(int.Parse).ToList()
                : new List<int>();

            CueOnApply = 
                selectInfo.ContainsKey(_headerMap["cueOnApply"]) 
                && selectInfo[_headerMap["cueOnApply"]] != null
                && !string.IsNullOrEmpty(selectInfo[_headerMap["cueOnApply"]].ToString())
                ? selectInfo[_headerMap["cueOnApply"]].ToString().Split(';').Select(int.Parse).ToList()
                : new List<int>();

            CueOnTick = 
                selectInfo.ContainsKey(_headerMap["cueOnTick"]) 
                && selectInfo[_headerMap["cueOnTick"]] != null
                && !string.IsNullOrEmpty(selectInfo[_headerMap["cueOnTick"]].ToString())
                ? selectInfo[_headerMap["cueOnTick"]].ToString().Split(';').Select(int.Parse).ToList()
                : new List<int>();

            CueOnAdd = 
                selectInfo.ContainsKey(_headerMap["cueOnAdd"]) 
                && selectInfo[_headerMap["cueOnAdd"]] != null
                && !string.IsNullOrEmpty(selectInfo[_headerMap["cueOnAdd"]].ToString())
                ? ((string)selectInfo[_headerMap["cueOnAdd"]]).Split(';').Select(int.Parse).ToList()
                : new List<int>();

            CueOnRemove = 
                selectInfo.ContainsKey(_headerMap["cueOnRemove"]) 
                && selectInfo[_headerMap["cueOnRemove"]] != null
                && !string.IsNullOrEmpty(selectInfo[_headerMap["cueOnRemove"]].ToString())
                ? selectInfo[_headerMap["cueOnRemove"]].ToString().Split(';').Select(int.Parse).ToList()
                : new List<int>();

            CueOnActivate = 
                selectInfo.ContainsKey(_headerMap["cueOnActivate"]) 
                && selectInfo[_headerMap["cueOnActivate"]] != null
                && !string.IsNullOrEmpty(selectInfo[_headerMap["cueOnActivate"]].ToString())
                ? selectInfo[_headerMap["cueOnActivate"]].ToString().Split(';').Select(int.Parse).ToList()
                : new List<int>();

            CueOnDeactivate = 
                selectInfo.ContainsKey(_headerMap["cueOnDeactivate"]) 
                && selectInfo[_headerMap["cueOnDeactivate"]] != null
                && !string.IsNullOrEmpty(selectInfo[_headerMap["cueOnDeactivate"]].ToString())
                ? selectInfo[_headerMap["cueOnDeactivate"]].ToString().Split(';').Select(int.Parse).ToList()
                : new List<int>();


            if (selectInfo[_headerMap["duration"]] != null 
                && !string.IsNullOrEmpty(selectInfo[_headerMap["cueOnDeactivate"]].ToString()))
                Duration = new GEEditDurarion
                {
                    Unit = (TimeUnit)selectInfo[_headerMap["duration"]],
                    time = selectInfo[_headerMap["duration"] + 1] != null
                        ? int.Parse(selectInfo[_headerMap["duration"] + 1].ToString())
                        : 0
                };
            else
                Duration = new GEEditDurarion();

            if (selectInfo[_headerMap["period"]] != null 
                && !string.IsNullOrEmpty(selectInfo[_headerMap["period"]].ToString()))
                Period = new GEEditPeriod
                {
                    time = selectInfo[_headerMap["period"]] != null
                        ? int.Parse(selectInfo[_headerMap["period"]].ToString())
                        : 0,
                    effects = selectInfo[_headerMap["period"] + 1] != null
                        ? ((string)selectInfo[_headerMap["period"] + 1]).Split(';').Select(int.Parse).ToList()
                        : new List<int>(),
                    firstTrigger = selectInfo[_headerMap["period"] + 2] != null &&
                                   bool.Parse(selectInfo[_headerMap["period"] + 2].ToString())
                };
            else
                Period = new GEEditPeriod();

            if (selectInfo.ContainsKey(_headerMap["stacking"]) 
                && selectInfo[_headerMap["stacking"]] != null
                && !string.IsNullOrEmpty(selectInfo[_headerMap["stacking"]].ToString()))
                Stacking = new GEEditStacking
                {
                    code = selectInfo[_headerMap["stacking"]] != null
                        ? int.Parse(selectInfo[_headerMap["stacking"]].ToString())
                        : 0,
                    stackingType = selectInfo[_headerMap["stacking"] + 1] != null
                        ? (StackingType)int.Parse(selectInfo[_headerMap["stacking"] + 1].ToString())
                        : 0,
                    limitCount = selectInfo[_headerMap["stacking"] + 2] != null
                        ? int.Parse(selectInfo[_headerMap["stacking"] + 2].ToString())
                        : 0,
                    durationRefreshPolicy = selectInfo[_headerMap["stacking"] + 3] != null
                        ? (DurationRefreshPolicy)int.Parse(selectInfo[_headerMap["stacking"] + 3].ToString())
                        : DurationRefreshPolicy.NeverRefresh,
                    periodResetPolicy = selectInfo[_headerMap["stacking"] + 4] != null
                        ? (PeriodResetPolicy)int.Parse(selectInfo[_headerMap["stacking"] + 4].ToString())
                        : PeriodResetPolicy.NeverRefresh,
                    expirationPolicy = selectInfo[_headerMap["stacking"] + 5] != null
                        ? (ExpirationPolicy)int.Parse(selectInfo[_headerMap["stacking"] + 5].ToString())
                        : ExpirationPolicy.ClearEntireStack,
                    DenyOverflowApplication = selectInfo[_headerMap["stacking"] + 6] != null &&
                                              bool.Parse(selectInfo[_headerMap["stacking"] + 6].ToString()),
                    clearStackOnOverflow = selectInfo[_headerMap["stacking"] + 7] != null &&
                                           bool.Parse(selectInfo[_headerMap["stacking"] + 7].ToString()),
                    overflowEffects = selectInfo[_headerMap["stacking"] + 8] != null
                        ? selectInfo[_headerMap["stacking"] + 8].ToString().Split(';').Select(int.Parse).ToList()
                        : new List<int>()
                };
            else
                Stacking = new GEEditStacking();

            //	modifiers
            Modifiers = new List<GEEditPeriodModifier>();
            if (selectInfo.ContainsKey(_headerMap["modifiers"]) 
                && selectInfo[_headerMap["modifiers"]] != null
                && !string.IsNullOrEmpty(selectInfo[_headerMap["modifiers"]].ToString()))
            {
                var mods = selectInfo[_headerMap["modifiers"]].ToString().Split('|').ToList();
                foreach (var v in mods)
                {
                    var cfg = v.Split(';').ToList();
                    var mod = new GEEditPeriodModifier();
                    mod.AttrSet = int.Parse(cfg[0]);
                    mod.Attribute = int.Parse(cfg[1]);
                    mod.Magnitude = float.Parse(cfg[2]);
                    mod.Operation = (GEOperation)int.Parse(cfg[3]);
                    mod.MMC = int.Parse(cfg[4]);
                    Modifiers.Add(mod);
                }
            }

            // grantedAbility	
            GrantedAbility = new List<GEEditGrantedAbility>();
            if (selectInfo.ContainsKey(_headerMap["grantedAbility"]) 
                && selectInfo[_headerMap["grantedAbility"]] != null
                && !string.IsNullOrEmpty(selectInfo[_headerMap["grantedAbility"]].ToString()))
            {
                var abilities = selectInfo[_headerMap["grantedAbility"]].ToString().Split('|').ToList();
                foreach (var v in abilities)
                {
                    var cfg = v.Split(';').Select(int.Parse).ToList();
                    var a = new GEEditGrantedAbility
                    {
                        abilityID = cfg[0],
                        level = cfg[1],
                        ActivationPolicy = (GrantedAbilityActivationPolicy)cfg[2],
                        DeactivationPolicy = (GrantedAbilityDeactivationPolicy)cfg[3],
                        RemovePolicy = (GrantedAbilityRemovePolicy)cfg[4]
                    };
                    GrantedAbility.Add(a);
                }
            }

            // Components加载
            ComponentTypes = new List<AbilityEditComponent>();
            if (AssetTags.Count > 0) ComponentTypes.Add(AbilityEditComponent.AssetTags);
            if (GrantedTags.Count > 0) ComponentTypes.Add(AbilityEditComponent.GrantedTags);
            if (ApplicationRequiredTags.Count > 0) ComponentTypes.Add(AbilityEditComponent.ApplicationRequiredTags);
            if (OngoingRequiredTags.Count > 0) ComponentTypes.Add(AbilityEditComponent.OngoingRequiredTags);
            if (RemoveGameplayEffectsWithTags.Count > 0)
                ComponentTypes.Add(AbilityEditComponent.RemoveGameplayEffectsWithTags);
            if (ImmunityTags.Count > 0) ComponentTypes.Add(AbilityEditComponent.ImmunityTags);
            if (Duration.time != 0) ComponentTypes.Add(AbilityEditComponent.Duration);
            if (Period.time != 0) ComponentTypes.Add(AbilityEditComponent.Period);
            if (CueOnApply.Count > 0) ComponentTypes.Add(AbilityEditComponent.CueOnApply);
            if (CueOnTick.Count > 0) ComponentTypes.Add(AbilityEditComponent.CueOnTick);
            if (CueOnAdd.Count > 0) ComponentTypes.Add(AbilityEditComponent.CueOnAdd);
            if (CueOnRemove.Count > 0) ComponentTypes.Add(AbilityEditComponent.CueOnRemove);
            if (CueOnActivate.Count > 0) ComponentTypes.Add(AbilityEditComponent.CueOnActivate);
            if (CueOnDeactivate.Count > 0) ComponentTypes.Add(AbilityEditComponent.CueOnDeactivate);
            if (GrantedAbility.Count > 0) ComponentTypes.Add(AbilityEditComponent.GrantedAbility);
            if (Stacking.code != 0) ComponentTypes.Add(AbilityEditComponent.Stacking);
            if (Modifiers.Count > 0) ComponentTypes.Add(AbilityEditComponent.Modifiers);
        }

        #endregion


        #region 可视化读写编辑 UI

        private const string T_G = "编辑配置";
        private const string T_G_A = "编辑配置/A";
        private const string T_G_A_B = "编辑配置/A/组件详情";

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

        [TitleGroup(T_G)] [LabelText("描述")] [LabelWidth(50)]
        public string description;

        [HorizontalGroup(T_G_A, 200)] 
        [ValueDropdown(nameof(ComponentChoice), IsUniqueList = true)] 
        [LabelText("Ability组件")]
        public List<AbilityEditComponent> ComponentTypes;

        [VerticalGroup(T_G_A_B)]
        [Title("消耗", Bold = false)]
        [ShowIf("@HasComponent(AbilityEditComponent.Cost)")]
        //[ValueDropdown(nameof(TagChoices), IsUniqueList = true)]
        [LabelText(" ")]
        public int Cost;
        
        [VerticalGroup(T_G_A_B)]
        [Title("冷却CD", Bold = false)]
        [ShowIf("@HasComponent(AbilityEditComponent.Cooldown)")]
        //[ValueDropdown(nameof(TagChoices), IsUniqueList = true)]
        [LabelText(" ")]
        public int CD;
        
        [VerticalGroup(T_G_A_B)]
        [Title("描述tag", Bold = false)]
        [ShowIf("@HasComponent(AbilityEditComponent.AssetTags)")]
        [ValueDropdown(nameof(TagChoices), IsUniqueList = true)]
        [LabelText(" ")]
        public List<int> assetTags;

        [VerticalGroup(T_G_A_B)]
        [Title("拥有【任意】Tag的Ability会被取消", Bold = false)]
        [ShowIf("@HasComponent(AbilityEditComponent.CancelAbilityWithTags)")]
        [ValueDropdown(nameof(TagChoices), IsUniqueList = true)]
        [LabelText(" ")]
        public List<int> cancelAbilityWithTags;

        [VerticalGroup(T_G_A_B)]
        [Title("拥有【任意】Tag的Ability会被阻止", Bold = false)]
        [ShowIf("@HasComponent(AbilityEditComponent.BlockAbilityWithTags)")]
        [ValueDropdown(nameof(TagChoices), IsUniqueList = true)]
        [LabelText(" ")]
        public List<int> blockAbilityWithTags;

        [VerticalGroup(T_G_A_B)]
        [Title("激活后获得的Tag", Bold = false)]
        [ShowIf("@HasComponent(AbilityEditComponent.ActivationOwnedTags)")]
        [ValueDropdown(nameof(TagChoices), IsUniqueList = true)]
        [LabelText(" ")]
        public List<int> activationOwnedTags;

        [VerticalGroup(T_G_A_B)]
        [Title("激活需要的Tag", Bold = false)]
        [ShowIf("@HasComponent(AbilityEditComponent.ActivationRequiredTags)")]
        [ValueDropdown(nameof(TagChoices), IsUniqueList = true)]
        [LabelText(" ")]
        public List<int> activationRequiredTags;

        [VerticalGroup(T_G_A_B)]
        [Title("阻止激活的Tag", Bold = false)]
        [ShowIf("@HasComponent(AbilityEditComponent.ActivationBlockedTags)")]
        [ValueDropdown(nameof(TagChoices), IsUniqueList = true)]
        [LabelText(" ")]
        public List<int> activationBlockedTags;
        
        // TODO
        // AbilityLogic
        #endregion
    }
}