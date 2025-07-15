using System;
using System.Diagnostics;
using System.IO;
using GAS.General;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

namespace GAS.Editor
{
    [SingletonFilePath(GasDefine.GAS_BASE_SETTING_PATH)]
    public class GASSettingAsset : ScriptableSingleton<GASSettingAsset>
    {
        private const int LABEL_WIDTH = 200;
        private const int SHORT_LABEL_WIDTH = 200;
        private static GASSettingAsset _setting;


        [Title(GASConstDefine.TITLE_SETTING, Bold = true)]
        [BoxGroup("A", false, order: 1)]
        [LabelText(GASConstDefine.LABEL_OF_CodeGeneratePath)]
        [LabelWidth(LABEL_WIDTH)]
        [FolderPath]
        [OnValueChanged(nameof(SaveAsset))]
        public string CodeGeneratePath = "Assets/Scripts/Gen";
        

        [BoxGroup("A")]
        [LabelText("表导出路径")]
        [LabelWidth(LABEL_WIDTH)]
        [FolderPath(RequireExistingPath = true)]
        [OnValueChanged(nameof(SaveAsset))]
        public string TableOutpuPath = "";
                
        [BoxGroup("A")]
        [LabelText("表class生成路径")]
        [InfoBox("注意【表class生成路径】和【自动化生成脚本路径】不要设置成同一个，" +
                 "因为luban的类生成工具会把输出的目录文件夹整个清空后再生成。所以请保证【表class生成路径】内没有其它文件。",
            InfoMessageType.Warning)]
        [LabelWidth(LABEL_WIDTH)]
        [FolderPath(RequireExistingPath = true)]
        [OnValueChanged(nameof(SaveAsset))]
        public string TableClassCodeOutpuPath = "";
        
        [FormerlySerializedAs("TableExportToolPath")]
        [BoxGroup("A")]
        [LabelText("配置表工程路径")]
        [LabelWidth(LABEL_WIDTH)]
        [FolderPath(RequireExistingPath = true)]
        [OnValueChanged(nameof(SaveAsset))]
        [InlineButton(nameof(OutputJsonTables),"导出Json表", ShowIf = nameof(IsShowOutputButton))]
        [InfoBox("配置表工程内没有luban导表工具gen.bat!", InfoMessageType.Error,VisibleIf = nameof(IsGenBatNotExist))]
        public string ConfigProjectPath = "";
        
        #region 生成文件路径一览
        
        [TitleGroup("A/生成文件路径一览")]
        [DisplayAsString(TextAlignment.Left, true)]
        [ShowInInspector]
        [LabelText("Tag配置Json路径")]
        public string PathOfJsonTag => $"{TableOutpuPath}/{GASConstDefine.JSON_FILE_NAME_OF_TAG}.json";

        [TitleGroup("A/生成文件路径一览")]
        [DisplayAsString(TextAlignment.Left, true)]
        [ShowInInspector]
        [LabelText("Tag配置Excel路径")]
        public string PathOfExcelTag
        {
            get
            {
                string path = $"{ConfigProjectPath}/Datas/{GASConstDefine.EXCEL_FILE_NAME_OF_TAG}.xlsx";;
                return path;
            }
        } 
        
        [TitleGroup("A/生成文件路径一览")]
        [DisplayAsString(TextAlignment.Left, true)]
        [ShowInInspector]
        [LabelText("Tag脚本路径")]
        public string PathOfCodeTag => $"{CodeGeneratePath}/{GASConstDefine.CODE_FILE_NAME_OF_TAG}.cs";

        #endregion

        private static GASSettingAsset Setting
        {
            get
            {
                if (_setting == null) _setting = LoadOrCreate();
                return _setting;
            }
        }

        [ShowInInspector]
        [BoxGroup("V", false, order: 0)]
        [HideLabel]
        [DisplayAsString(TextAlignment.Left, true)]
        private static string Version =>
            $"<size=15><b><color=white>EX-GAS Version: {GasDefine.GAS_VERSION}</color></b></size>";

        public static string CodeGenPath => Setting.CodeGeneratePath;

        [TitleGroup("A/生成脚本")]
        [HorizontalGroup("A/生成脚本/B")]
        [DisplayAsString(TextAlignment.Left, true)]
        [GUIColor(1, 1, 1)]
        [Button(SdfIconType.Activity, "一键生成所有", ButtonHeight = 30)]
        void GenerateAll() => CodeGenerator.GenerateAllCode();
        
        [HorizontalGroup("A/生成脚本/B")]
        [DisplayAsString(TextAlignment.Left, true)]
        [GUIColor(0.8f, 0.8f, 0)]
        [Button("Tag脚本", ButtonHeight = 30)]
        void GenerateTagCode() => CodeGenerator.GenerateTagCode();
        
        
        [HorizontalGroup("A/生成脚本/B")]
        [DisplayAsString(TextAlignment.Left, true)]
        [GUIColor(0.8f, 0.8f, 0)]
        [Button("属性脚本", ButtonHeight = 30)]
        void GenerateAttrCode() => CodeGenerator.GenerateAttrCode();

        [HorizontalGroup("A/生成脚本/B")]
        [DisplayAsString(TextAlignment.Left, true)]
        [GUIColor(0.8f, 0.8f, 0)]
        [Button("属性集脚本", ButtonHeight = 30)]
        void GenerateAttrSetCode() => CodeGenerator.GenerateAttrSetCode();
        
        [HorizontalGroup("A/生成脚本/B")]
        [DisplayAsString(TextAlignment.Left, true)]
        [GUIColor(0.8f, 0.8f, 0)]
        [Button("GameplayEffect脚本", ButtonHeight = 30)]
        void GenerateEffectCode() => CodeGenerator.GenerateEffectCode();

        [HorizontalGroup("A/生成脚本/B")]
        [DisplayAsString(TextAlignment.Left, true)]
        [GUIColor(0.8f, 0.8f, 0)]
        [Button("Ability脚本", ButtonHeight = 30)]
        void GenerateAbilityCode() => CodeGenerator.GenerateAbilityCode();

        public string FullGenBatPath()
        {
            var projectRootPath = Application.dataPath.Substring(0, Application.dataPath.Length - 6); 
            var fullBatPath = Path.Combine(projectRootPath,$"{Instance.ConfigProjectPath}/{GASConstDefine.LUBAN_GEN_BAT_TILE_NAME}");
            return fullBatPath;
        }
        
        public void OutputJsonTables() => CodeGenerator.GenerateGasConfigTables();

        bool IsShowOutputButton()
        {
            // 检查导出工具路径和输出路径是否存在
            // 获取项目根目录
            var projectRootPath = Application.dataPath.Substring(0, Application.dataPath.Length - 6); 
            var fullOutputPath = Path.Combine(projectRootPath,Instance.TableOutpuPath);
            return !IsGenBatNotExist() && Directory.Exists(fullOutputPath);
        }

        bool IsGenBatNotExist()
        {
            var fullBatPath = FullGenBatPath();
            return !File.Exists(fullBatPath);
        }
        
        private void SaveAsset()
        {
            if (Instance == this) return;
            UpdateAsset(this);
            Save();
        }

        private const string EX_GAS_ENABLE_HOT_KEYS = "EX_GAS_ENABLE_HOT_KEYS";

#if EX_GAS_ENABLE_HOT_KEYS
        public const bool EnableHotKeys = true;
#else
        public const bool EnableHotKeys = false;
#endif

        [TabGroup("Advance", "Advance", SdfIconType.Gear, TextColor = "#FF7F00"), PropertyOrder(1)]
        [InfoBox(
            "@\"当前快捷键状态: \" + (EnableHotKeys ? \"启用\":\"禁用\") + \", 冲突时可禁用快捷键\"")]
#if EX_GAS_ENABLE_HOT_KEYS
        [Button(SdfIconType.ToggleOn, "禁用快捷键")]
#else
        [Button(SdfIconType.ToggleOff, "开启快捷键")]
#endif
        private void ToggleScriptDefineSymbol_EX_GAS_ENABLE_HOT_KEYS()
        {
            if (EditorUtility.DisplayDialog("Ex-GAS",
                    "切换快捷键状态\n将在你的项目中切换\"EX_GAS_ENABLE_HOT_KEYS\"宏定义\n\n这会重新编译你的代码, 之后你可能需要手动保存你的项目(请留意ProjectSettings.asset的变化).",
                    "确定", "取消"))
            {
#pragma warning disable 162
                if (EnableHotKeys)
                    ScriptingDefineSymbolsHelper.Remove(EX_GAS_ENABLE_HOT_KEYS);
                else
                    ScriptingDefineSymbolsHelper.Add(EX_GAS_ENABLE_HOT_KEYS);
#pragma warning restore 162
            }
        }
    }
}