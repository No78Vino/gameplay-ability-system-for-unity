using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;
using Sirenix.OdinInspector;

namespace GAS.Editor
{
    public static class GasXlsxChoice
    {
        private static List<ValueDropdownItem> _cues;
        private static List<ValueDropdownItem> _effects;
        private static List<ValueDropdownItem> _mmcs;
        private static List<ValueDropdownItem> _abilities;
        private static List<ValueDropdownItem> _ascs;
        private static List<ValueDropdownItem> _tags;
        private static List<ValueDropdownItem> _attrSets;
        private static Dictionary<int,List<ValueDropdownItem>> _attrs;
        
        public static void LoadChoices()
        {
            var setting = GASSettingAsset.LoadOrCreate();
            _cues = LoadChoiceListFromExcel(setting.PathOfExcelCue, "Cue", true);
            _mmcs = LoadChoiceListFromExcel(setting.PathOfExcelMmc, "MMC", true);
            _tags = LoadChoiceListFromExcel(setting.PathOfExcelTag, "Tag", false);
            _effects = LoadChoiceListFromExcel(setting.PathOfExcelEffect, "Effect", true);
            _abilities = LoadChoiceListFromExcel(setting.PathOfExcelAbility, "Ability", true);
            _ascs = LoadChoiceListFromExcel(setting.PathOfExcelAsc, "ASC", true);
            
            // _attrs = GASXlsxReader.AttrChoices();
            
            // 属性集，属性需要特殊处理，读Json
            GasJsonReader.ReadAllAndCache();
            _attrSets = new List<ValueDropdownItem>();
            _attrs = new Dictionary<int, List<ValueDropdownItem>>();
            var attrSetMap = GasJsonReader.AttrSetMap();
            foreach (var kv in attrSetMap)
            {
                _attrSets.Add(new ValueDropdownItem(kv.Value.name,kv.Key));
                _attrs.Add(kv.Key,new List<ValueDropdownItem>());
                var attributes = kv.Value.attribute ?? System.Array.Empty<AttrInSetInEditor>();
                foreach (var attr in attributes)
                {
                    _attrs[kv.Key].Add(new ValueDropdownItem(attr.GetAttrName(),attr.id));
                }
            }
        }

        private static List<ValueDropdownItem> LoadChoiceListFromExcel(string path, string label, bool showIdInName)
        {
            var result = new List<ValueDropdownItem>();
            if (string.IsNullOrWhiteSpace(path))
            {
                UnityEngine.Debug.LogWarning($"[EX-GAS] {label} Excel 路径为空。");
                return result;
            }

            if (!File.Exists(path))
            {
                UnityEngine.Debug.LogWarning($"[EX-GAS] {label} Excel 文件不存在: {path}");
                return result;
            }

            try
            {
                using var package = new ExcelPackage(new FileInfo(path));
                var worksheet = package.Workbook.Worksheets.Count > 0 ? package.Workbook.Worksheets[1] : null;
                if (worksheet == null)
                {
                    UnityEngine.Debug.LogWarning($"[EX-GAS] {label} Excel 没有可读取工作表: {path}");
                    return result;
                }

                var safeCnt = 99999;
                var row = 4;
                while (safeCnt > 0 && worksheet.Cells[row, 2].Value != null)
                {
                    safeCnt--;
                    var rawId = worksheet.Cells[row, 2].Value.ToString();
                    if (!int.TryParse(rawId, out var id))
                    {
                        UnityEngine.Debug.LogWarning($"[EX-GAS] {label} 第 {row} 行ID不是整数: {rawId}");
                        row++;
                        continue;
                    }

                    var name = worksheet.Cells[row, 3].Value?.ToString() ?? string.Empty;
                    var showName = showIdInName ? $"[{id}]{name}" : name;
                    result.Add(new ValueDropdownItem(showName, id));
                    row++;
                }
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogWarning($"[EX-GAS] 读取 {label} Excel 失败: {path}\n{ex.GetType().Name}: {ex.Message}");
            }

            return result;
        }

        public static List<ValueDropdownItem> Cues()
        {
            if (_cues == null)
                LoadChoices();
            return _cues;
        }
        
        public static List<ValueDropdownItem> Effects()
        {
            if (_effects == null)
                LoadChoices();
            return _effects;
        }
        
        public static List<ValueDropdownItem> MMCs()
        {
            if (_mmcs == null)
                LoadChoices();
            return _mmcs;
        }
        
        public static List<ValueDropdownItem> Abilities()
        {
            if (_abilities == null)
                LoadChoices();
            return _abilities;
        }
        
        public static List<ValueDropdownItem> Ascs()
        {
            if (_ascs == null)
                LoadChoices();
            return _ascs;
        }
        
        public static List<ValueDropdownItem> Tags()
        {
            if (_tags == null)
                LoadChoices();
            return _tags;
        }
        
        public static List<ValueDropdownItem> AttrSets()
        {
            if (_attrSets == null)
                LoadChoices();
            return _attrSets;
        }
        
        public static List<ValueDropdownItem> Attributes(int attrSetId)
        {
            if (_attrs == null)
                LoadChoices();
            if (_attrs != null && _attrs.TryGetValue(attrSetId, out var choices))
                return choices;
            return new List<ValueDropdownItem>();
        }
    }
}
