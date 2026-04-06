using System.IO;
using GAS.General;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace GAS.Editor
{
    [SingletonFilePath(GASConstDefine.GAS_BASE_SETTING_PATH)]
    public class GASSettingAsset : ScriptableSingleton<GASSettingAsset>
    {
        private const int LABEL_WIDTH = 200;
        private const int SHORT_LABEL_WIDTH = 200;
        private static GASSettingAsset _setting;
        private const string DEFAULT_TABLE_OUTPUT_PATH = "Assets/DataGenerated/Luban/Json/GAS";
        private const string DEFAULT_TABLE_CODE_OUTPUT_PATH = "Assets/DataGenerated/Luban/CSharp";
        private const string DEFAULT_CONFIG_PROJECT_PATH = "EX_GAS_Config/ProjectConfigTable/exgas_config";


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
        public string TableOutpuPath = DEFAULT_TABLE_OUTPUT_PATH;
                
        [BoxGroup("A")]
        [LabelText("表class生成路径")]
        [InfoBox("注意【表class生成路径】和【自动化生成脚本路径】不要设置成同一个，" +
                 "因为luban的类生成工具会把输出的目录文件夹整个清空后再生成。所以请保证【表class生成路径】内没有其它文件。",
            InfoMessageType.Warning)]
        [LabelWidth(LABEL_WIDTH)]
        [FolderPath(RequireExistingPath = true)]
        [OnValueChanged(nameof(SaveAsset))]
        public string TableClassCodeOutpuPath = DEFAULT_TABLE_CODE_OUTPUT_PATH;
        
        [FormerlySerializedAs("TableExportToolPath")]
        [BoxGroup("A")]
        [LabelText("配置表工程路径")]
        [LabelWidth(LABEL_WIDTH)]
        [FolderPath(RequireExistingPath = true)]
        [OnValueChanged(nameof(SaveAsset))]
        [InlineButton(nameof(OutputJsonTables),"导出Json表", ShowIf = nameof(IsShowOutputButton))]
        [InfoBox("配置表工程内没有luban导表工具gen.bat!", InfoMessageType.Error,VisibleIf = nameof(IsGenBatNotExist))]
        public string ConfigProjectPath = DEFAULT_CONFIG_PROJECT_PATH;
        
        #region 生成文件路径一览

        public string PathOfJsonTag => $"{TableOutpuPath}/{GASConstDefine.JSON_FILE_NAME_OF_TAG}.json";
        public string PathOfExcelTag => $"{ConfigProjectPath}/Datas/{GASConstDefine.EXCEL_FILE_NAME_OF_TAG}.xlsx";
        public string PathOfCodeTag => $"{CodeGeneratePath}/{GASConstDefine.CODE_FILE_NAME_OF_TAG}.cs";
        
        public string PathOfJsonAttr => $"{TableOutpuPath}/{GASConstDefine.JSON_FILE_NAME_OF_ATTR}.json";
        public string PathOfExcelAttr => $"{ConfigProjectPath}/Datas/{GASConstDefine.EXCEL_FILE_NAME_OF_ATTR}.xlsx";
        public string PathOfCodeAttr => $"{CodeGeneratePath}/{GASConstDefine.CODE_FILE_NAME_OF_ATTR}.cs";
        
        public string PathOfJsonAttrSet => $"{TableOutpuPath}/{GASConstDefine.JSON_FILE_NAME_OF_ATTR_SET}.json";
        public string PathOfExcelAttrSet => $"{ConfigProjectPath}/Datas/{GASConstDefine.EXCEL_FILE_NAME_OF_ATTR_SET}.xlsx";
        public string PathOfCodeAttrSet => $"{CodeGeneratePath}/{GASConstDefine.CODE_FILE_NAME_OF_ATTR_SET}.cs";
        
        public string PathOfJsonEffect => $"{TableOutpuPath}/{GASConstDefine.JSON_FILE_NAME_OF_EFFECT}.json";
        public string PathOfExcelEffect => $"{ConfigProjectPath}/Datas/{GASConstDefine.EXCEL_FILE_NAME_OF_EFFECT}.xlsx";

        public string PathOfJsonAbility => $"{TableOutpuPath}/{GASConstDefine.JSON_FILE_NAME_OF_ABILITY}.json";
        public string PathOfExcelAbility => $"{ConfigProjectPath}/Datas/{GASConstDefine.EXCEL_FILE_NAME_OF_ABILITY}.xlsx";
        public string PathOfCodeAbility => $"{CodeGeneratePath}/{GASConstDefine.CODE_FILE_NAME_OF_ABILITY}.cs";
        
        public string PathOfJsonCue => $"{TableOutpuPath}/{GASConstDefine.JSON_FILE_NAME_OF_CUE}.json";
        public string PathOfExcelCue => $"{ConfigProjectPath}/Datas/{GASConstDefine.EXCEL_FILE_NAME_OF_CUE}.xlsx";
        public string PathOfCodeCue => $"{CodeGeneratePath}/{GASConstDefine.CODE_FILE_NAME_OF_CUE}.cs";
        
        public string PathOfJsonMmc => $"{TableOutpuPath}/{GASConstDefine.JSON_FILE_NAME_OF_MMC}.json";
        public string PathOfExcelMmc => $"{ConfigProjectPath}/Datas/{GASConstDefine.EXCEL_FILE_NAME_OF_MMC}.xlsx";
        public string PathOfCodeMmc => $"{CodeGeneratePath}/{GASConstDefine.CODE_FILE_NAME_OF_MMC}.cs";
        
        public string PathOfJsonAsc => $"{TableOutpuPath}/{GASConstDefine.JSON_FILE_NAME_OF_ASC}.json";
        public string PathOfExcelAsc => $"{ConfigProjectPath}/Datas/{GASConstDefine.EXCEL_FILE_NAME_OF_ASC}.xlsx";

        public string PathOfCodeLubanExtesion => $"{CodeGeneratePath}/{GASConstDefine.CODE_FILE_NAME_OF_LUBAN}.cs";
        public string PathOfCodeLauncher => $"{CodeGeneratePath}/{GASConstDefine.CODE_FILE_NAME_OF_LAUNCHER}.cs";
        
        public string PathOfJsonTimelineAbility => $"{TableOutpuPath}/{GASConstDefine.JSON_FILE_NAME_OF_TIMELINE_ABILITY}.json";
        public string PathOfExcelTimelineAbility => $"{ConfigProjectPath}/Datas/{GASConstDefine.EXCEL_FILE_NAME_OF_TIMELINE_ABILITY}.xlsx";
        
        
        [TitleGroup("A/文件路径一览",Order = 99)]
        [DisplayAsString(TextAlignment.Left, true)]
        [HideLabel]
        [ShowInInspector]
        public string ShowGenPaths
        {
            get
            {
                var content =
                    $"Launcher脚本路径: {PathOfCodeLauncher}\n\n" +
                    $"Tag配置Json路径: {PathOfJsonTag}\n" +
                    $"Tag配置Excel路径: {PathOfExcelTag}\n" +
                    $"Tag脚本路径: {PathOfCodeTag}\n\n" +
                    $"属性配置Json路径: {PathOfJsonAttr}\n" +
                    $"属性配置Excel路径: {PathOfExcelAttr}\n" +
                    $"属性脚本路径: {PathOfCodeAttr}\n\n" +
                    $"属性集配置Json路径: {PathOfJsonAttrSet}\n" +
                    $"属性集配置Excel路径: {PathOfExcelAttrSet}\n" +
                    $"属性集脚本路径: {PathOfCodeAttrSet}\n\n" +
                    $"Effect配置Json路径: {PathOfJsonEffect}\n" +
                    $"Effect配置Excel路径: {PathOfExcelEffect}\n\n" +
                    $"Ability配置Json路径: {PathOfJsonAbility}\n" +
                    $"Ability配置Excel路径: {PathOfExcelAbility}\n" +
                    $"Ability脚本路径: {PathOfCodeAbility}\n\n" +
                    $"Cue配置Json路径: {PathOfJsonCue}\n" +
                    $"Cue配置Excel路径: {PathOfExcelCue}\n" +
                    $"Cue脚本路径: {PathOfCodeCue}\n\n" +
                    $"Mmc配置Json路径: {PathOfJsonMmc}\n" +
                    $"Mmc配置Excel路径: {PathOfExcelMmc}\n" +
                    $"Mmc脚本路径: {PathOfCodeMmc}\n\n";
                return $"<color=white>{content}</color>";
            }
        }

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
            $"<size=15><b><color=white>EX-GAS Version: {GASConstDefine.GAS_VERSION}</color></b></size>";

        public static string CodeGenPath => Setting.CodeGeneratePath;

        [TitleGroup("A/生成脚本",Order = 98)]
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
        [Button("Cue脚本", ButtonHeight = 30)]
        void GenerateCueCode() => CodeGenerator.GenerateCueCode();
        
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
