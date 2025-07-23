using System.Collections.Generic;
using System.IO;
using System.Linq;
using GAS.Editor.General;
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
        }

        [BoxGroup("GameplayEffect")]
        [Button("保存", Icon = SdfIconType.Save)]
        private void SaveConfig()
        {
            // TODO
        }


        #region 可视化读写编辑 GameplayEffect 配置xlsx文件

        private FileInfo _xlsxFileInfo;
        private Dictionary<string, int> _headerMap;
        private Dictionary<int, Dictionary<int, object>> _data;
        private Dictionary<int, int> _idToRowMap;
        
        private void LoadFile()
        {
            var excelFilePath = _settingAsset.PathOfExcelEffect;
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

        public IEnumerable<int> GetAllEffectIds() => _data == null ? new List<int>() : _data.Keys;
        public List<ValueDropdownItem> TagChoices => GasXlsxChoice.Tags();
        public IEnumerable<EffectEditComponent> ComponentChoice => EditorEffectHelper.ComponentTypes();
        
        private void OnSelectedIdChanged()
        {
            var selectInfo = _data[SelectedId];

            name = selectInfo.ContainsKey(_headerMap["name"])
                ? selectInfo[_headerMap["name"]]?.ToString()
                : string.Empty;

            description = selectInfo.ContainsKey(_headerMap["desc"])
                ? selectInfo[_headerMap["desc"]]?.ToString()
                : string.Empty;

            // requiredTags = selectInfo.TryGetValue(_headerMap["required_tag"], out var requiredTagValue)
            //     ? ListIntFromString(requiredTagValue.ToString())
            //     : new List<int>();
            //
            // immunityTags = selectInfo.TryGetValue(_headerMap["immunity_tag"], out var immunityTagValue)
            //     ? ListIntFromString(immunityTagValue.ToString())
            //     : new List<int>();
            //
            // // 加载Cue逻辑
            // type = selectInfo.ContainsKey(_headerMap["cue_logic"])
            //     ? selectInfo[_headerMap["cue_logic"]]?.ToString()
            //     : string.Empty;
            //
            // cueParam = _cueLogicParameter.TryGetValue(SelectedId, out var cueParams) ? EditorCueHelper.CreateCueParameter(type, cueParams) : null;
        }
        #endregion
        
        
        #region 可视化读写编辑 UI

        private const string T_G = "编辑配置";
        private const string T_G_A = "编辑配置/A";
        private const string T_G_A_B = "编辑配置/A/组件详情";
        
        [TitleGroup(T_G, order: 2)]
        [ValueDropdown(nameof(GetAllEffectIds))]
        [OnValueChanged(nameof(OnSelectedIdChanged))]
        [LabelText("当前Effect"),LabelWidth(100)]
        [InlineButton(nameof(AddNewEffect), Label = "添加", Icon = SdfIconType.Plus)]
        [InlineButton(nameof(DeleteEffect), Label = "删除", Icon = SdfIconType.Trash)]
        public int SelectedId;

        private void AddNewEffect()
        {
            StringEditWindow.OpenWindow("创建新Effect", "0", newID =>
            {
                if (int.TryParse(newID, out var id))
                {
                    if (_data.Keys.Contains(id)) return GAS.General.Validation.ValidationResult.Invalid("ID已存在!");
                }
                else
                {
                    EditorWindow.focusedWindow.ShowNotification(new GUIContent("ID必须是数字!"));
                    return GAS.General.Validation.ValidationResult.Invalid("ID必须是数字!");
                }

                return GAS.General.Validation.ValidationResult.Valid;
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

        private bool HasComponent(EffectEditComponent component) =>
            ComponentTypes != null && ComponentTypes.Contains(component);
        
        [TitleGroup(T_G)] [LabelText("名字"),LabelWidth(50)] [Tooltip("部分GA编辑页的GE选项会用到这个参数")]
        public string name;

        [TitleGroup(T_G)] [LabelText("描述"),LabelWidth(50)] public string description;

        [HorizontalGroup(T_G_A,200)] 
        [ValueDropdown(nameof(ComponentChoice), IsUniqueList = true)] [LabelText("GE组件")]
        public List<EffectEditComponent> ComponentTypes;

        [VerticalGroup(T_G_A_B)]
        [Title("AssetTag 描述tag",Bold = false)]
        [ShowIf("@HasComponent(EffectEditComponent.AssetTags)")] 
        [ValueDropdown(nameof(TagChoices), IsUniqueList = true)] [LabelText(" ")]
        public List<int> AssetTags;
        
        [VerticalGroup(T_G_A_B)]
        [Title("GrantedTag 获得的tag",Bold = false)]
        [ShowIf("@HasComponent(EffectEditComponent.GrantedTags)")] 
        [ValueDropdown(nameof(TagChoices), IsUniqueList = true)] [LabelText(" ")]
        public List<int> GrantedTags;
        
        [VerticalGroup(T_G_A_B)]
        [Title("ApplicationRequiredTags 应用需求tag",Bold = false)]
        [ShowIf("@HasComponent(EffectEditComponent.ApplicationRequiredTags)")] 
        [ValueDropdown(nameof(TagChoices), IsUniqueList = true)] [LabelText(" ")]
        public List<int> ApplicationRequiredTags;
        
        [VerticalGroup(T_G_A_B)]
        [Title("OngoingRequiredTags 持续需求tag",Bold = false)]
        [ShowIf("@HasComponent(EffectEditComponent.OngoingRequiredTags)")] 
        [ValueDropdown(nameof(TagChoices), IsUniqueList = true)] [LabelText(" ")]
        public List<int> OngoingRequiredTags;
        
        [VerticalGroup(T_G_A_B)]
        [Title("RemoveGameplayEffectsWithTags 移除持有tag的GE",Bold = false)]
        [ShowIf("@HasComponent(EffectEditComponent.RemoveGameplayEffectsWithTags)")] 
        [ValueDropdown(nameof(TagChoices), IsUniqueList = true)] [LabelText(" ")]
        public List<int> RemoveGameplayEffectsWithTags;
        
        [VerticalGroup(T_G_A_B)]
        [Title("ImmunityTags 免疫的tag",Bold = false)]
        [ShowIf("@HasComponent(EffectEditComponent.ImmunityTags)")] 
        [ValueDropdown(nameof(TagChoices), IsUniqueList = true)] [LabelText(" ")]
        public List<int> ImmunityTags;
        
        [VerticalGroup(T_G_A_B)]
        [Title("持续时间",Bold = false)]
        [HideLabel]
        [ShowIf("@HasComponent(EffectEditComponent.Duration)")] 
        public GEEditDurarion Duration;
        
        [VerticalGroup(T_G_A_B)]
        [Title("间隔效果GE",Bold = false)]
        [HideLabel]
        [ShowIf("@HasComponent(EffectEditComponent.Period)")]
        [InfoBox("必须有Duration组件，Period组件才会生效！",
            InfoMessageType = InfoMessageType.Error,
            VisibleIf = "@!HasComponent(EffectEditComponent.Duration)")]
        public GEEditPeriod Period;
        
        [VerticalGroup(T_G_A_B)]
        [Title("间隔效果GE",Bold = false)]
        [HideLabel]
        [ShowIf("@HasComponent(EffectEditComponent.Modifiers)")]
        public List<GEEditPeriodModifier> Modifiers;

        [VerticalGroup(T_G_A_B)]
        [Title("应用时触发的Cue",Bold = false)]
        [HideLabel]
        [ShowIf("@HasComponent(EffectEditComponent.CueOnApply)")]
        [LabelText(" ")]
        [ValueDropdown("@GasXlsxChoice.Cues()", IsUniqueList = true)]
        public List<int> CueOnApply;
            
        [VerticalGroup(T_G_A_B)]
        [Title("帧更新的Cue",Bold = false)]
        [HideLabel]
        [ShowIf("@HasComponent(EffectEditComponent.CueOnTick)")]
        [LabelText(" ")]
        [ValueDropdown("@GasXlsxChoice.Cues()", IsUniqueList = true)]
        public List<int> CueOnTick;
        
        [VerticalGroup(T_G_A_B)]
        [Title("添加时触发的Cue",Bold = false)]
        [HideLabel]
        [ShowIf("@HasComponent(EffectEditComponent.CueOnAdd)")]
        [LabelText(" ")]
        [ValueDropdown("@GasXlsxChoice.Cues()", IsUniqueList = true)]
        public List<int> CueOnAdd;
        
        [VerticalGroup(T_G_A_B)]
        [Title("移除时触发的Cue",Bold = false)]
        [HideLabel]
        [ShowIf("@HasComponent(EffectEditComponent.CueOnRemove)")]
        [LabelText(" ")]
        [ValueDropdown("@GasXlsxChoice.Cues()", IsUniqueList = true)]
        public List<int> CueOnRemove;
        
        [VerticalGroup(T_G_A_B)]
        [Title("激活时触发的Cue",Bold = false)]
        [HideLabel]
        [ShowIf("@HasComponent(EffectEditComponent.CueOnActivate)")]
        [LabelText(" ")]
        [ValueDropdown("@GasXlsxChoice.Cues()", IsUniqueList = true)]
        public List<int> CueOnActivate;
        
        [VerticalGroup(T_G_A_B)]
        [Title("失活时触发的Cue",Bold = false)]
        [HideLabel]
        [ShowIf("@HasComponent(EffectEditComponent.CueOnDeactivate)")]
        [LabelText(" ")]
        [ValueDropdown("@GasXlsxChoice.Cues()", IsUniqueList = true)]
        public List<int> CueOnDeactivate;
        
        [VerticalGroup(T_G_A_B)]
        [Title("获得技能",Bold = false)]
        [HideLabel]
        [ShowIf("@HasComponent(EffectEditComponent.GrantedAbility)")]
        [LabelText(" ")]
        public List<GEEditGrantedAbility> GrantedAbility;
        
        #endregion
    }
}