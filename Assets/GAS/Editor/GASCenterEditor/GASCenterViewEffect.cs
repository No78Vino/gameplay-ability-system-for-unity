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
                EditorUtility.RevealInFinder(excelFilePath);
            else
                EditorUtility.DisplayDialog("错误", "Excel文件未找到，请检查设置。", "确定");
        }

        [HorizontalGroup(TITLE_GRP_H_A)]
        [Button("打开Json文件所在文件夹")]
        private void OpenJsonFileExplore()
        {
            var jsonFilePath = _settingAsset.PathOfJsonEffect;
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


        #region 可视化读写编辑 GameplayEffect 配置xlsx文件

        private FileInfo _xlsxFileInfo;
        private Dictionary<string, int> _headerMap;
        private Dictionary<int, Dictionary<int, object>> _data;
        private Dictionary<int, int> _idToRowMap;

        private void LoadFile()
        {
            var excelFilePath = _settingAsset.PathOfExcelEffect;
            if (!File.Exists(excelFilePath))
            {
                Debug.LogWarning($"[EX-GAS] GameplayEffect配置Excel文件不存在，将以空数据加载: {excelFilePath}");
                _headerMap = new Dictionary<string, int>();
                _data = new Dictionary<int, Dictionary<int, object>>();
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

                    // duration特殊处理
                    rowData.Add(_headerMap["Duration"] + 1, worksheet.Cells[row, _headerMap["Duration"] + 1].Value);
                    rowData.Add(_headerMap["Duration"] + 2, worksheet.Cells[row, _headerMap["Duration"] + 2].Value);
                    // period特殊处理
                    rowData.Add(_headerMap["Period"] + 1, worksheet.Cells[row, _headerMap["Period"] + 1].Value);
                    rowData.Add(_headerMap["Period"] + 2, worksheet.Cells[row, _headerMap["Period"] + 2].Value);
                    // stacking特殊处理
                    rowData.Add(_headerMap["Stacking"] + 1, worksheet.Cells[row, _headerMap["Stacking"] + 1].Value);
                    rowData.Add(_headerMap["Stacking"] + 2, worksheet.Cells[row, _headerMap["Stacking"] + 2].Value);
                    rowData.Add(_headerMap["Stacking"] + 3, worksheet.Cells[row, _headerMap["Stacking"] + 3].Value);
                    rowData.Add(_headerMap["Stacking"] + 4, worksheet.Cells[row, _headerMap["Stacking"] + 4].Value);
                    rowData.Add(_headerMap["Stacking"] + 5, worksheet.Cells[row, _headerMap["Stacking"] + 5].Value);
                    rowData.Add(_headerMap["Stacking"] + 6, worksheet.Cells[row, _headerMap["Stacking"] + 6].Value);
                    rowData.Add(_headerMap["Stacking"] + 7, worksheet.Cells[row, _headerMap["Stacking"] + 7].Value);
                    rowData.Add(_headerMap["Stacking"] + 8, worksheet.Cells[row, _headerMap["Stacking"] + 8].Value);

                    _data.Add(id, rowData);
                    _idToRowMap.Add(id, row);
                    row++;
                }
            }
        }

        private void SaveFile()
        {
            var excelFilePath = _settingAsset.PathOfExcelEffect;
            _xlsxFileInfo = new FileInfo(excelFilePath);
            using (var package = new ExcelPackage(_xlsxFileInfo))
            {
                var worksheet = package.Workbook.Worksheets[1];

                // 写入当前cue数据到Excel
                var row = _idToRowMap.TryGetValue(SelectedId, out var existingRow) ? existingRow : MaxRowForNewID();
                worksheet.Cells[row, _headerMap["ID"]].Value = SelectedId;
                worksheet.Cells[row, _headerMap["Name"]].Value = name;
                worksheet.Cells[row, _headerMap["Desc"]].Value = description;

                worksheet.Cells[row, _headerMap["AssetTags"]].Value =
                    ComponentTypes.Contains(EffectEditComponent.AssetTags) && AssetTags.Count > 0
                        ? string.Join(";", AssetTags)
                        : string.Empty;
                worksheet.Cells[row, _headerMap["GrantedTags"]].Value =
                    ComponentTypes.Contains(EffectEditComponent.GrantedTags) && GrantedTags.Count > 0
                        ? string.Join(";", GrantedTags)
                        : string.Empty;

                GEEditTagRequirement GetRequirement(EffectEditComponent component)
                {
                    return component switch
                    {
                        EffectEditComponent.ApplicationRequiredTags => ApplicationRequiredTags,
                        EffectEditComponent.OngoingRequiredTags => OngoingRequiredTags,
                        EffectEditComponent.RemoveGameplayEffectsWithTags => RemoveGameplayEffectsWithTags,
                        EffectEditComponent.ImmunityTags => ImmunityTags,
                        _ => null
                    };
                }

                foreach (var field in EditorEffectHelper.TagRequirementProtocolFields)
                {
                    worksheet.Cells[row, _headerMap[field.ExcelHeader]].Value =
                        ComponentTypes.Contains(field.Component)
                            ? EditorEffectHelper.EncodeTagRequirementCell(GetRequirement(field.Component), field.Mode)
                            : string.Empty;
                }

                worksheet.Cells[row, _headerMap["CueOnApply"]].Value =
                    ComponentTypes.Contains(EffectEditComponent.CueOnApply) && CueOnApply.Count > 0
                        ? string.Join(";", CueOnApply)
                        : string.Empty;
                worksheet.Cells[row, _headerMap["CueOnTick"]].Value =
                    ComponentTypes.Contains(EffectEditComponent.CueOnTick) && CueOnTick.Count > 0
                        ? string.Join(";", CueOnTick)
                        : string.Empty;
                worksheet.Cells[row, _headerMap["CueOnAdd"]].Value =
                    ComponentTypes.Contains(EffectEditComponent.CueOnAdd) && CueOnAdd.Count > 0
                        ? string.Join(";", CueOnAdd)
                        : string.Empty;
                worksheet.Cells[row, _headerMap["CueOnRemove"]].Value =
                    ComponentTypes.Contains(EffectEditComponent.CueOnRemove) && CueOnRemove.Count > 0
                        ? string.Join(";", CueOnRemove)
                        : string.Empty;
                worksheet.Cells[row, _headerMap["CueOnActivate"]].Value =
                    ComponentTypes.Contains(EffectEditComponent.CueOnActivate) && CueOnActivate.Count > 0
                        ? string.Join(";", CueOnActivate)
                        : string.Empty;
                worksheet.Cells[row, _headerMap["CueOnDeactivate"]].Value =
                    ComponentTypes.Contains(EffectEditComponent.CueOnDeactivate) && CueOnDeactivate.Count > 0
                        ? string.Join(";", CueOnDeactivate)
                        : string.Empty;

                if (ComponentTypes.Contains(EffectEditComponent.Duration))
                {
                    worksheet.Cells[row, _headerMap["Duration"]].Value = (int)Duration.Unit;
                    worksheet.Cells[row, _headerMap["Duration"] + 1].Value = Duration.time;
                    worksheet.Cells[row, _headerMap["Duration"] + 2].Value = Duration.ResetStartTimeWhenActivated;
                }
                else
                {
                    worksheet.Cells[row, _headerMap["Duration"]].Value = null;
                    worksheet.Cells[row, _headerMap["Duration"] + 1].Value = null;
                    worksheet.Cells[row, _headerMap["Duration"] + 2].Value = null;
                }

                if (ComponentTypes.Contains(EffectEditComponent.Period))
                {
                    worksheet.Cells[row, _headerMap["Period"]].Value = Period.time;
                    worksheet.Cells[row, _headerMap["Period"] + 1].Value =
                        Period.effects.Count > 0 ? string.Join(";", Period.effects) : string.Empty;
                    worksheet.Cells[row, _headerMap["Period"] + 2].Value = Period.firstTrigger.ToString();
                }
                else
                {
                    worksheet.Cells[row, _headerMap["Period"]].Value = null;
                    worksheet.Cells[row, _headerMap["Period"] + 1].Value = null;
                    worksheet.Cells[row, _headerMap["Period"] + 2].Value = null;
                }


                if (ComponentTypes.Contains(EffectEditComponent.Modifiers))
                {
                    var modifiers = Modifiers.Select(mod =>
                        $"{mod.AttrSet};{mod.Attribute};{mod.Magnitude};{(int)mod.Operation};{mod.MMC}").ToList();
                    worksheet.Cells[row, _headerMap["Modifiers"]].Value = string.Join("|", modifiers);
                }
                else
                {
                    worksheet.Cells[row, _headerMap["Modifiers"]].Value = null;
                }

                if (ComponentTypes.Contains(EffectEditComponent.GrantedAbility) && GrantedAbility.Count > 0)
                {
                    var grantedAbilities = GrantedAbility.Select(a =>
                            $"{a.abilityID};{a.level};{(int)a.ActivationPolicy};{(int)a.DeactivationPolicy};{(int)a.RemovePolicy}")
                        .ToList();
                    worksheet.Cells[row, _headerMap["GrantedAbility"]].Value = string.Join("|", grantedAbilities);
                }
                else
                {
                    worksheet.Cells[row, _headerMap["GrantedAbility"]].Value = null;
                }

                if (ComponentTypes.Contains(EffectEditComponent.Stacking))
                {
                    worksheet.Cells[row, _headerMap["Stacking"]].Value = Stacking.code;
                    worksheet.Cells[row, _headerMap["Stacking"] + 1].Value = (int)Stacking.stackingType;
                    worksheet.Cells[row, _headerMap["Stacking"] + 2].Value = Stacking.limitCount;
                    worksheet.Cells[row, _headerMap["Stacking"] + 3].Value = (int)Stacking.durationRefreshPolicy;
                    worksheet.Cells[row, _headerMap["Stacking"] + 4].Value = (int)Stacking.periodResetPolicy;
                    worksheet.Cells[row, _headerMap["Stacking"] + 5].Value = (int)Stacking.stackingExpirationPolicy;
                    worksheet.Cells[row, _headerMap["Stacking"] + 6].Value =
                        Stacking.DenyOverflowApplication.ToString();
                    worksheet.Cells[row, _headerMap["Stacking"] + 7].Value = Stacking.clearStackOnOverflow.ToString();
                    worksheet.Cells[row, _headerMap["Stacking"] + 8].Value =
                        Stacking.overflowEffects.Count > 0 ? string.Join(";", Stacking.overflowEffects) : string.Empty;
                }
                else
                {
                    worksheet.Cells[row, _headerMap["Stacking"]].Value = null;
                    worksheet.Cells[row, _headerMap["Stacking"] + 1].Value = null;
                    worksheet.Cells[row, _headerMap["Stacking"] + 2].Value = null;
                    worksheet.Cells[row, _headerMap["Stacking"] + 3].Value = null;
                    worksheet.Cells[row, _headerMap["Stacking"] + 4].Value = null;
                    worksheet.Cells[row, _headerMap["Stacking"] + 5].Value = null;
                    worksheet.Cells[row, _headerMap["Stacking"] + 6].Value = null;
                    worksheet.Cells[row, _headerMap["Stacking"] + 7].Value = null;
                    worksheet.Cells[row, _headerMap["Stacking"] + 8].Value = null;
                }

                package.Save();
            }
        }

        public IEnumerable<int> GetAllEffectIds()
        {
            return _data == null ? new List<int>() : _data.Keys;
        }

        public List<ValueDropdownItem> TagChoices => GasXlsxChoice.Tags();
        public IEnumerable<EffectEditComponent> ComponentChoice => EditorEffectHelper.ComponentTypes();

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

            name = selectInfo.ContainsKey(_headerMap["Name"])
                ? selectInfo[_headerMap["Name"]]?.ToString()
                : string.Empty;

            description = selectInfo.ContainsKey(_headerMap["Desc"])
                ? selectInfo[_headerMap["Desc"]]?.ToString()
                : string.Empty;

            AssetTags = 
                selectInfo.ContainsKey(_headerMap["AssetTags"]) 
                && selectInfo[_headerMap["AssetTags"]] != null
                && !string.IsNullOrEmpty(selectInfo[_headerMap["AssetTags"]].ToString())
                ? GASCenterParseHelper.ParseIntListLoose(selectInfo[_headerMap["AssetTags"]].ToString())
                : new List<int>();

            GrantedTags = 
                selectInfo.ContainsKey(_headerMap["GrantedTags"]) 
                && selectInfo[_headerMap["GrantedTags"]] != null
                && !string.IsNullOrEmpty(selectInfo[_headerMap["GrantedTags"]].ToString())
                ? GASCenterParseHelper.ParseIntListLoose(selectInfo[_headerMap["GrantedTags"]].ToString())
                : new List<int>();

            string ReadCell(string key)
            {
                if (!_headerMap.ContainsKey(key)) return string.Empty;
                if (!selectInfo.ContainsKey(_headerMap[key])) return string.Empty;
                return selectInfo[_headerMap[key]]?.ToString() ?? string.Empty;
            }

            foreach (var field in EditorEffectHelper.TagRequirementProtocolFields)
            {
                var requirement = EditorEffectHelper.ParseTagRequirementCell(ReadCell(field.ExcelHeader), field.Mode);
                switch (field.Component)
                {
                    case EffectEditComponent.ApplicationRequiredTags:
                        ApplicationRequiredTags = requirement;
                        break;
                    case EffectEditComponent.OngoingRequiredTags:
                        OngoingRequiredTags = requirement;
                        break;
                    case EffectEditComponent.RemoveGameplayEffectsWithTags:
                        RemoveGameplayEffectsWithTags = requirement;
                        break;
                    case EffectEditComponent.ImmunityTags:
                        ImmunityTags = requirement;
                        break;
                }
            }

            CueOnApply = 
                selectInfo.ContainsKey(_headerMap["CueOnApply"]) 
                && selectInfo[_headerMap["CueOnApply"]] != null
                && !string.IsNullOrEmpty(selectInfo[_headerMap["CueOnApply"]].ToString())
                ? GASCenterParseHelper.ParseIntListLoose(selectInfo[_headerMap["CueOnApply"]].ToString())
                : new List<int>();

            CueOnTick = 
                selectInfo.ContainsKey(_headerMap["CueOnTick"]) 
                && selectInfo[_headerMap["CueOnTick"]] != null
                && !string.IsNullOrEmpty(selectInfo[_headerMap["CueOnTick"]].ToString())
                ? GASCenterParseHelper.ParseIntListLoose(selectInfo[_headerMap["CueOnTick"]].ToString())
                : new List<int>();

            CueOnAdd = 
                selectInfo.ContainsKey(_headerMap["CueOnAdd"]) 
                && selectInfo[_headerMap["CueOnAdd"]] != null
                && !string.IsNullOrEmpty(selectInfo[_headerMap["CueOnAdd"]].ToString())
                ? GASCenterParseHelper.ParseIntListLoose(selectInfo[_headerMap["CueOnAdd"]].ToString())
                : new List<int>();

            CueOnRemove = 
                selectInfo.ContainsKey(_headerMap["CueOnRemove"]) 
                && selectInfo[_headerMap["CueOnRemove"]] != null
                && !string.IsNullOrEmpty(selectInfo[_headerMap["CueOnRemove"]].ToString())
                ? GASCenterParseHelper.ParseIntListLoose(selectInfo[_headerMap["CueOnRemove"]].ToString())
                : new List<int>();

            CueOnActivate = 
                selectInfo.ContainsKey(_headerMap["CueOnActivate"]) 
                && selectInfo[_headerMap["CueOnActivate"]] != null
                && !string.IsNullOrEmpty(selectInfo[_headerMap["CueOnActivate"]].ToString())
                ? GASCenterParseHelper.ParseIntListLoose(selectInfo[_headerMap["CueOnActivate"]].ToString())
                : new List<int>();

            CueOnDeactivate = 
                selectInfo.ContainsKey(_headerMap["CueOnDeactivate"]) 
                && selectInfo[_headerMap["CueOnDeactivate"]] != null
                && !string.IsNullOrEmpty(selectInfo[_headerMap["CueOnDeactivate"]].ToString())
                ? GASCenterParseHelper.ParseIntListLoose(selectInfo[_headerMap["CueOnDeactivate"]].ToString())
                : new List<int>();


            if (selectInfo[_headerMap["Duration"]] != null 
                && !string.IsNullOrEmpty(selectInfo[_headerMap["Duration"]].ToString()))
                Duration = new GEEditDurarion
                {
                    Unit = (TimeUnit)int.Parse(selectInfo[_headerMap["Duration"]].ToString()),
                    time = selectInfo[_headerMap["Duration"] + 1] != null
                        ? int.Parse(selectInfo[_headerMap["Duration"] + 1].ToString())
                        : 0,
                    ResetStartTimeWhenActivated = selectInfo[_headerMap["Duration"] + 2] != null && bool.Parse(selectInfo[_headerMap["Duration"] + 2].ToString()),
                };
            else
                Duration = new GEEditDurarion();

            if (selectInfo[_headerMap["Period"]] != null 
                && !string.IsNullOrEmpty(selectInfo[_headerMap["Period"]].ToString()))
                Period = new GEEditPeriod
                {
                    time = selectInfo[_headerMap["Period"]] != null
                        ? int.Parse(selectInfo[_headerMap["Period"]].ToString())
                        : 0,
                    effects = selectInfo[_headerMap["Period"] + 1] != null
                        ? GASCenterParseHelper.ParseIntListLoose(selectInfo[_headerMap["Period"] + 1].ToString())
                        : new List<int>(),
                    firstTrigger = selectInfo[_headerMap["Period"] + 2] != null &&
                                   bool.Parse(selectInfo[_headerMap["Period"] + 2].ToString())
                };
            else
                Period = new GEEditPeriod();

            if (selectInfo.ContainsKey(_headerMap["Stacking"]) 
                && selectInfo[_headerMap["Stacking"]] != null
                && !string.IsNullOrEmpty(selectInfo[_headerMap["Stacking"]].ToString()))
                Stacking = new GEEditStacking
                {
                    code = selectInfo[_headerMap["Stacking"]] != null
                        ? int.Parse(selectInfo[_headerMap["Stacking"]].ToString())
                        : 0,
                    stackingType = selectInfo[_headerMap["Stacking"] + 1] != null
                        ? (StackingType)int.Parse(selectInfo[_headerMap["Stacking"] + 1].ToString())
                        : 0,
                    limitCount = selectInfo[_headerMap["Stacking"] + 2] != null
                        ? int.Parse(selectInfo[_headerMap["Stacking"] + 2].ToString())
                        : 0,
                    durationRefreshPolicy = selectInfo[_headerMap["Stacking"] + 3] != null
                        ? (DurationRefreshPolicy)int.Parse(selectInfo[_headerMap["Stacking"] + 3].ToString())
                        : DurationRefreshPolicy.NeverRefresh,
                    periodResetPolicy = selectInfo[_headerMap["Stacking"] + 4] != null
                        ? (PeriodResetPolicy)int.Parse(selectInfo[_headerMap["Stacking"] + 4].ToString())
                        : PeriodResetPolicy.NeverRefresh,
                    stackingExpirationPolicy = selectInfo[_headerMap["Stacking"] + 5] != null
                        ? (StackingExpirationPolicy)int.Parse(selectInfo[_headerMap["Stacking"] + 5].ToString())
                        : StackingExpirationPolicy.ClearEntireStack,
                    DenyOverflowApplication = selectInfo[_headerMap["Stacking"] + 6] != null &&
                                              bool.Parse(selectInfo[_headerMap["Stacking"] + 6].ToString()),
                    clearStackOnOverflow = selectInfo[_headerMap["Stacking"] + 7] != null &&
                                           bool.Parse(selectInfo[_headerMap["Stacking"] + 7].ToString()),
                    overflowEffects = selectInfo[_headerMap["Stacking"] + 8] != null
                        ? GASCenterParseHelper.ParseIntListLoose(selectInfo[_headerMap["Stacking"] + 8].ToString())
                        : new List<int>()
                };
            else
                Stacking = new GEEditStacking();

            //	modifiers
            Modifiers = new List<GEEditPeriodModifier>();
            if (selectInfo.ContainsKey(_headerMap["Modifiers"]) 
                && selectInfo[_headerMap["Modifiers"]] != null
                && !string.IsNullOrEmpty(selectInfo[_headerMap["Modifiers"]].ToString()))
            {
                var mods = selectInfo[_headerMap["Modifiers"]].ToString().Split('|').ToList();
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
            if (selectInfo.ContainsKey(_headerMap["GrantedAbility"]) 
                && selectInfo[_headerMap["GrantedAbility"]] != null
                && !string.IsNullOrEmpty(selectInfo[_headerMap["GrantedAbility"]].ToString()))
            {
                var abilities = selectInfo[_headerMap["GrantedAbility"]].ToString().Split('|').ToList();
                foreach (var v in abilities)
                {
                    var cfg = GASCenterParseHelper.ParseIntListLoose(v, positiveOnly: false);
                    if (cfg.Count < 5) continue;
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
            ComponentTypes = new List<EffectEditComponent>();
            if (AssetTags.Count > 0) ComponentTypes.Add(EffectEditComponent.AssetTags);
            if (GrantedTags.Count > 0) ComponentTypes.Add(EffectEditComponent.GrantedTags);
            if (ApplicationRequiredTags != null && ApplicationRequiredTags.HasAnyValue())
                ComponentTypes.Add(EffectEditComponent.ApplicationRequiredTags);
            if (OngoingRequiredTags != null && OngoingRequiredTags.HasAnyValue())
                ComponentTypes.Add(EffectEditComponent.OngoingRequiredTags);
            if (RemoveGameplayEffectsWithTags != null && RemoveGameplayEffectsWithTags.HasAnyValue())
                ComponentTypes.Add(EffectEditComponent.RemoveGameplayEffectsWithTags);
            if (ImmunityTags != null && ImmunityTags.HasAnyValue())
                ComponentTypes.Add(EffectEditComponent.ImmunityTags);
            if (Duration.time != 0) ComponentTypes.Add(EffectEditComponent.Duration);
            if (Period.time != 0) ComponentTypes.Add(EffectEditComponent.Period);
            if (CueOnApply.Count > 0) ComponentTypes.Add(EffectEditComponent.CueOnApply);
            if (CueOnTick.Count > 0) ComponentTypes.Add(EffectEditComponent.CueOnTick);
            if (CueOnAdd.Count > 0) ComponentTypes.Add(EffectEditComponent.CueOnAdd);
            if (CueOnRemove.Count > 0) ComponentTypes.Add(EffectEditComponent.CueOnRemove);
            if (CueOnActivate.Count > 0) ComponentTypes.Add(EffectEditComponent.CueOnActivate);
            if (CueOnDeactivate.Count > 0) ComponentTypes.Add(EffectEditComponent.CueOnDeactivate);
            if (GrantedAbility.Count > 0) ComponentTypes.Add(EffectEditComponent.GrantedAbility);
            if (Stacking.code != 0) ComponentTypes.Add(EffectEditComponent.Stacking);
            if (Modifiers.Count > 0) ComponentTypes.Add(EffectEditComponent.Modifiers);
        }

        #endregion


        #region 可视化读写编辑 UI

        private const string T_G = "编辑配置";
        private const string T_G_TAB = "编辑配置/TAB";

        [TitleGroup(T_G, order: 2)]
        [ValueDropdown(nameof(GetAllEffectIds))]
        [OnValueChanged(nameof(OnSelectedIdChanged))]
        [LabelText("当前Effect")]
        [LabelWidth(100)]
        [InlineButton(nameof(AddNewEffect), Label = "添加", Icon = SdfIconType.Plus)]
        [InlineButton(nameof(DeleteEffect), Label = "删除", Icon = SdfIconType.Trash)]
        public int SelectedId;

        private void AddNewEffect()
        {
            StringEditWindow.OpenWindow("创建新Effect", "0", newID =>
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


        private void DeleteEffect()
        {
            if (_data.ContainsKey(SelectedId))
            {
                // 二次弹窗确认
                if (!EditorUtility.DisplayDialog("确认删除", $"你确定要删除Effect ID: {SelectedId}吗？", "是", "否"))
                    return;
                _data.Remove(SelectedId);
                EditorWindow.focusedWindow.ShowNotification(new GUIContent($"已删除Effect ID: {SelectedId}"));
                SelectedId = _idToRowMap.Keys.First(); // 重置选择ID
                OnSelectedIdChanged();
            }
            else
            {
                EditorWindow.focusedWindow.ShowNotification(new GUIContent($"Effect ID: {SelectedId} 不存在!"));
            }
        }

        private bool HasComponent(EffectEditComponent component)
        {
            return ComponentTypes != null && ComponentTypes.Contains(component);
        }

        [TitleGroup(T_G)] [LabelText("名字")] [LabelWidth(50)] [Tooltip("部分GA编辑页的GE选项会用到这个参数")]
        public string name;

        [TitleGroup(T_G)] [LabelText("描述")] [LabelWidth(50)]
        public string description;

        [TabGroup(T_G_TAB,"组件列表")]
        [ValueDropdown(nameof(ComponentChoice), IsUniqueList = true)] [LabelText("GE组件")]
        public List<EffectEditComponent> ComponentTypes;

        [TabGroup(T_G_TAB,"详情")]
        //[GUIColor("cyan")]
        [Title("AssetTag 描述tag", Bold = false)]
        [ShowIf("@HasComponent(EffectEditComponent.AssetTags)")]
        [ValueDropdown(nameof(TagChoices), IsUniqueList = true)]
        [LabelText(" ")]
        public List<int> AssetTags;

        [TabGroup(T_G_TAB,"详情")]
        [Title("GrantedTag 获得的tag", Bold = false)]
        [ShowIf("@HasComponent(EffectEditComponent.GrantedTags)")]
        [ValueDropdown(nameof(TagChoices), IsUniqueList = true)]
        [LabelText(" ")]
        public List<int> GrantedTags;

        [TabGroup(T_G_TAB,"详情")]
        [Title("ApplicationRequiredTags 应用GE需要匹配的Tag规则", Bold = false)]
        [ShowIf("@HasComponent(EffectEditComponent.ApplicationRequiredTags)")]
        [LabelText(" ")]
        public GEEditTagRequirement ApplicationRequiredTags;

        [TabGroup(T_G_TAB,"详情")]
        [Title("OngoingRequiredTags 持续激活GE需要匹配的Tag规则", Bold = false)]
        [ShowIf("@HasComponent(EffectEditComponent.OngoingRequiredTags)")]
        [LabelText(" ")]
        public GEEditTagRequirement OngoingRequiredTags;

        [TabGroup(T_G_TAB,"详情")]
        [Title("RemoveGameplayEffectsWithTags 移除匹配Tag规则的Effect", Bold = false)]
        [ShowIf("@HasComponent(EffectEditComponent.RemoveGameplayEffectsWithTags)")]
        [LabelText(" ")]
        public GEEditTagRequirement RemoveGameplayEffectsWithTags;

        [TabGroup(T_G_TAB,"详情")]
        [Title("ImmunityTags 免疫GE需要匹配的Tag规则", Bold = false)]
        [ShowIf("@HasComponent(EffectEditComponent.ImmunityTags)")]
        [LabelText(" ")]
        public GEEditTagRequirement ImmunityTags;

        [TabGroup(T_G_TAB,"详情")]
        [Title("持续时间", Bold = false)]
        [HideLabel]
        [ShowIf("@HasComponent(EffectEditComponent.Duration)")]
        public GEEditDurarion Duration;

        [TabGroup(T_G_TAB,"详情")]
        [Title("间隔效果GE", Bold = false)]
        [HideLabel]
        [ShowIf("@HasComponent(EffectEditComponent.Period)")]
        [InfoBox("必须有Duration组件，Period组件才会生效！",
            InfoMessageType = InfoMessageType.Error,
            VisibleIf = "@!HasComponent(EffectEditComponent.Duration)")]
        public GEEditPeriod Period;

        [TabGroup(T_G_TAB,"详情")]
        [Title("修改器", Bold = false)]
        [LabelText(" ")]
        [ShowIf("@HasComponent(EffectEditComponent.Modifiers)")]
        public List<GEEditPeriodModifier> Modifiers;

        [TabGroup(T_G_TAB,"详情")]
        [Title("应用时触发的Cue", Bold = false)]
        [ShowIf("@HasComponent(EffectEditComponent.CueOnApply)")]
        [LabelText(" ")]
        [ValueDropdown("@GasXlsxChoice.Cues()", IsUniqueList = true)]
        public List<int> CueOnApply;

        [TabGroup(T_G_TAB,"详情")]
        [Title("帧更新的Cue", Bold = false)]
        [ShowIf("@HasComponent(EffectEditComponent.CueOnTick)")]
        [LabelText(" ")]
        [ValueDropdown("@GasXlsxChoice.Cues()", IsUniqueList = true)]
        public List<int> CueOnTick;

        [TabGroup(T_G_TAB,"详情")]
        [Title("添加时触发的Cue", Bold = false)]
        [ShowIf("@HasComponent(EffectEditComponent.CueOnAdd)")]
        [LabelText(" ")]
        [ValueDropdown("@GasXlsxChoice.Cues()", IsUniqueList = true)]
        public List<int> CueOnAdd;

        [TabGroup(T_G_TAB,"详情")]
        [Title("移除时触发的Cue", Bold = false)]
        [ShowIf("@HasComponent(EffectEditComponent.CueOnRemove)")]
        [LabelText(" ")]
        [ValueDropdown("@GasXlsxChoice.Cues()", IsUniqueList = true)]
        public List<int> CueOnRemove;

        [TabGroup(T_G_TAB,"详情")]
        [Title("激活时触发的Cue", Bold = false)]
        [ShowIf("@HasComponent(EffectEditComponent.CueOnActivate)")]
        [LabelText(" ")]
        [ValueDropdown("@GasXlsxChoice.Cues()", IsUniqueList = true)]
        public List<int> CueOnActivate;

        [TabGroup(T_G_TAB,"详情")]
        [Title("失活时触发的Cue", Bold = false)]
        [ShowIf("@HasComponent(EffectEditComponent.CueOnDeactivate)")]
        [LabelText(" ")]
        [ValueDropdown("@GasXlsxChoice.Cues()", IsUniqueList = true)]
        public List<int> CueOnDeactivate;

        [TabGroup(T_G_TAB,"详情")]
        [Title("获得技能", Bold = false)]
        [ShowIf("@HasComponent(EffectEditComponent.GrantedAbility)")]
        [LabelText(" ")]
        public List<GEEditGrantedAbility> GrantedAbility;

        [TabGroup(T_G_TAB,"详情")]
        [Title("堆叠", Bold = false)]
        [HideLabel]
        [ShowIf("@HasComponent(EffectEditComponent.Stacking)")]
        [InfoBox("必须有Duration组件，Stacking组件才会生效！",
            InfoMessageType = InfoMessageType.Error,
            VisibleIf = "@!HasComponent(EffectEditComponent.Duration)")]
        public GEEditStacking Stacking;

        #endregion
    }
}
