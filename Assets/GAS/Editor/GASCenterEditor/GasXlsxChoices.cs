using System;
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
        private static List<ValueDropdownItem> _tags;
        private static List<ValueDropdownItem> _attrSets;
        private static Dictionary<int,List<ValueDropdownItem>> _attrs;
        
        public static void LoadChoices()
        {
            var setting = GASSettingAsset.LoadOrCreate();
            using (var package = new ExcelPackage(new FileInfo(setting.PathOfExcelCue)))
            {
                var worksheet = package.Workbook.Worksheets[1];
                // 读取数据行,从第4行开始，第二列为key，即id。
                // 以第2列是否有值为结束标志
                _cues = new List<ValueDropdownItem>();
                var safeCnt = 99999;
                var row = 4;
                while (safeCnt > 0 && worksheet.Cells[row, 2].Value != null)
                {
                    safeCnt--;
                    var id = int.Parse(worksheet.Cells[row, 2].Value.ToString());
                    var name = worksheet.Cells[row, 3].Value.ToString();
                    _cues.Add(new ValueDropdownItem($"[{id}]{name}", id));
                    row++;
                }
            }
            
            using (var package = new ExcelPackage(new FileInfo(setting.PathOfExcelMmc)))
            {
                var worksheet = package.Workbook.Worksheets[1];
                // 读取数据行,从第4行开始，第二列为key，即id。
                // 以第2列是否有值为结束标志
                _mmcs = new List<ValueDropdownItem>();
                var safeCnt = 99999;
                var row = 4;
                while (safeCnt > 0 && worksheet.Cells[row, 2].Value != null)
                {
                    safeCnt--;
                    var id = int.Parse(worksheet.Cells[row, 2].Value.ToString());
                    var name = worksheet.Cells[row, 3].Value.ToString();
                    _mmcs.Add(new ValueDropdownItem($"[{id}]{name}", id));
                    row++;
                }
            }
            
            // using (var package = new ExcelPackage(new FileInfo(setting.PathOfExcelAttrSet)))
            // {
            //     var worksheet = package.Workbook.Worksheets[1];
            //     // 读取数据行,从第4行开始，第二列为key，即id。
            //     // 以第2列是否有值为结束标志
            //     _attrSets = new List<ValueDropdownItem>();
            //     var safeCnt = 99999;
            //     var row = 4;
            //     while (safeCnt > 0 && worksheet.Cells[row, 2].Value != null)
            //     {
            //         safeCnt--;
            //         var id = int.Parse(worksheet.Cells[row, 2].Value.ToString());
            //         var name = worksheet.Cells[row, 3].Value.ToString();
            //         _attrSets.Add(new ValueDropdownItem($"[{id}]{name}", id));
            //         row++;
            //     }
            // }
            
            using (var package = new ExcelPackage(new FileInfo(setting.PathOfExcelTag)))
            {
                var worksheet = package.Workbook.Worksheets[1];
                // 读取数据行,从第4行开始，第二列为key，即id。
                // 以第2列是否有值为结束标志
                _tags = new List<ValueDropdownItem>();
                var safeCnt = 99999;
                var row = 4;
                while (safeCnt > 0 && worksheet.Cells[row, 2].Value != null)
                {
                    safeCnt--;
                    var id = int.Parse(worksheet.Cells[row, 2].Value.ToString());
                    var name = worksheet.Cells[row, 3].Value.ToString();
                    _tags.Add(new ValueDropdownItem($"{name}", id));
                    row++;
                }
            }
            
            using (var package = new ExcelPackage(new FileInfo(setting.PathOfExcelEffect)))
            {
                var worksheet = package.Workbook.Worksheets[1];
                // 读取数据行,从第4行开始，第二列为key，即id。
                // 以第2列是否有值为结束标志
                _effects = new List<ValueDropdownItem>();
                var safeCnt = 99999;
                var row = 4;
                while (safeCnt > 0 && worksheet.Cells[row, 2].Value != null)
                {
                    safeCnt--;
                    var id = int.Parse(worksheet.Cells[row, 2].Value.ToString());
                    var name = worksheet.Cells[row, 3].Value.ToString();
                    _effects.Add(new ValueDropdownItem($"[{id}]{name}", id));
                    row++;
                }
            }
            
            
            // _abilities = GASXlsxReader.AbilityChoices();
            // _attrs = GASXlsxReader.AttrChoices();
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