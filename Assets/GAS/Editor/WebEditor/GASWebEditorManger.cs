using System.Diagnostics;
using System.IO;
using UnityEditor;

namespace GAS.Editor
{
    public static class GASWebEditorManager
    {
        // Web 编辑器根目录（相对于项目根）  
        private const string WEB_EDITOR_ROOT = "Assets/GAS/Editor/WebEditor";

        // GameplayTag 编辑器子目录  
        private const string TAG_EDITOR_DIR = "GameplayTag";

        // Attribute 编辑器子目录  
        private const string ATTR_EDITOR_DIR = "Attribute";

        // AttributeSet 编辑器子目录  
        private const string ATTR_SET_EDITOR_DIR = "AttributeSet";

        // Effect 编辑器子目录 
        private const string EFFECT_EDITOR_DIR = "Effect"; 
        
        // ASC 编辑器子目录（新增）  
        private const string ASC_EDITOR_DIR = "ASC";
        
        // ── 环境部署（install_deps.bat 在根目录，所有编辑器共用）────────  
        [MenuItem("EXTool/EX-GAS/Web编辑器/📦 一键部署编辑器环境")]
        public static void DeployWebEditorEnv()
        {
            // install_deps.bat 直接在 WEB_EDITOR_ROOT 下，不在子目录  
            var batPath = Path.GetFullPath(Path.Combine(WEB_EDITOR_ROOT, "install_deps.bat"));
            RunBat(batPath);
        }

        // ── GameplayTag 编辑器 ────────────────────────────────────────────  
        [MenuItem("EXTool/EX-GAS/Web编辑器/Tag 网页编辑器")]
        public static void LaunchTagWebEditor()
        {
            var setting = GASSettingAsset.LoadOrCreate();
            var xlsxPath = Path.GetFullPath(setting.PathOfExcelTag);
            if (!File.Exists(xlsxPath))
            {
                EditorUtility.DisplayDialog("错误",
                    $"Tag Excel 文件未找到:\n{xlsxPath}\n\n请先在 Setting 页面配置正确的 ConfigProjectPath。",
                    "确定");
                return;
            }

            // start.bat 在 GameplayTag 子目录下  
            var batPath = GetFullPath(TAG_EDITOR_DIR, "start.bat");
            RunBat(batPath, $"\"{xlsxPath}\"");
        }

        [MenuItem("EXTool/EX-GAS/Web编辑器/Attribute 网页编辑器")]
        public static void LaunchAttrWebEditor()
        {
            var setting = GASSettingAsset.LoadOrCreate();
            var xlsxPath = Path.GetFullPath(setting.PathOfExcelAttr); // ← 用 PathOfExcelAttr  
            if (!File.Exists(xlsxPath))
            {
                EditorUtility.DisplayDialog("错误",
                    $"Attribute Excel 文件未找到:\n{xlsxPath}\n\n请先在 Setting 页面配置正确的 ConfigProjectPath。",
                    "确定");
                return;
            }

            // start.bat 在 Attribute 子目录下  
            var batPath = GetFullPath(ATTR_EDITOR_DIR, "start.bat"); // ← ATTR_EDITOR_DIR  
            RunBat(batPath, $"\"{xlsxPath}\""); // ← 传入 xlsx 路径  
        }

        [MenuItem("EXTool/EX-GAS/Web编辑器/AttributeSet 网页编辑器")]  
        public static void LaunchAttrSetWebEditor()  
        {  
            var setting = GASSettingAsset.LoadOrCreate();  
            var xlsxPath = Path.GetFullPath(setting.PathOfExcelAttrSet);  
            var xlsxAttr = Path.GetFullPath(setting.PathOfExcelAttr);  // ← 新增  
            if (!File.Exists(xlsxPath))  
            {  
                EditorUtility.DisplayDialog("错误",  
                    $"AttributeSet Excel 文件未找到:\n{xlsxPath}\n\n请先在 Setting 页面配置正确的 ConfigProjectPath。",  
                    "确定");  
                return;  
            }  
  
            var batPath = GetFullPath(ATTR_SET_EDITOR_DIR, "start.bat");  
            RunBat(batPath, $"\"{xlsxPath}\" --attr-xlsx \"{xlsxAttr}\"");  // ← 追加参数  
        }

        [MenuItem("EXTool/EX-GAS/Web编辑器/ASC预设 网页编辑器")]  
        public static void LaunchAscWebEditor()  
        {  
            var setting     = GASSettingAsset.LoadOrCreate();  
            var xlsxPath    = Path.GetFullPath(setting.PathOfExcelAsc);  
            var xlsxTag     = Path.GetFullPath(setting.PathOfExcelTag);  
            var xlsxAttrSet = Path.GetFullPath(setting.PathOfExcelAttrSet);  
            var xlsxAbility = Path.GetFullPath(setting.PathOfExcelAbility);  
  
            if (!File.Exists(xlsxPath))  
            {  
                EditorUtility.DisplayDialog("错误",  
                    $"ASC Excel 文件未找到:\n{xlsxPath}\n\n请先在 Setting 页面配置正确的 ConfigProjectPath。",  
                    "确定");  
                return;  
            }  
  
            var batPath = GetFullPath(ASC_EDITOR_DIR, "start.bat");  
            RunBat(batPath, $"\"{xlsxPath}\" --tag-xlsx \"{xlsxTag}\" --attrset-xlsx \"{xlsxAttrSet}\" --ability-xlsx \"{xlsxAbility}\"");  
        }

        [MenuItem("EXTool/EX-GAS/Web编辑器/Effect 网页编辑器")]  
        public static void LaunchEffectWebEditor()  
        {  
            var setting     = GASSettingAsset.LoadOrCreate();  
            var xlsxPath    = Path.GetFullPath(setting.PathOfExcelEffect);  
            var xlsxTag     = Path.GetFullPath(setting.PathOfExcelTag);  
            var xlsxAttrSet = Path.GetFullPath(setting.PathOfExcelAttrSet);  
            var xlsxAbility = Path.GetFullPath(setting.PathOfExcelAbility);  
            var xlsxCue     = Path.GetFullPath(setting.PathOfExcelCue);  
            var xlsxMmc     = Path.GetFullPath(setting.PathOfExcelMmc);  
            var xlsxAttr    = Path.GetFullPath(setting.PathOfExcelAttr);  // ← 新增  
  
            if (!File.Exists(xlsxPath))  
            {  
                EditorUtility.DisplayDialog("错误",  
                    $"Effect Excel 文件未找到:\n{xlsxPath}\n\n请先在 Setting 页面配置正确的 ConfigProjectPath。",  
                    "确定");  
                return;  
            }  
  
            var batPath = GetFullPath(EFFECT_EDITOR_DIR, "start.bat");  
            RunBat(batPath, $"\"{xlsxPath}\" --tag-xlsx \"{xlsxTag}\" --attrset-xlsx \"{xlsxAttrSet}\" --ability-xlsx \"{xlsxAbility}\" --cue-xlsx \"{xlsxCue}\" --mmc-xlsx \"{xlsxMmc}\" --attr-xlsx \"{xlsxAttr}\"");  
        }

        // ── 工具方法 ──────────────────────────────────────────────────────  
        private static string GetFullPath(string editorDir, string fileName)
        {
            return Path.GetFullPath(Path.Combine(WEB_EDITOR_ROOT, editorDir, fileName));
        }

        private static void RunBat(string batPath, string arguments = "")
        {
            if (!File.Exists(batPath))
            {
                EditorUtility.DisplayDialog("错误",
                    $"未找到脚本:\n{batPath}\n\n请确认 WebEditor 文件夹已正确放置。",
                    "确定");
                return;
            }

            new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = batPath,
                    Arguments = arguments,
                    WorkingDirectory = Path.GetDirectoryName(batPath),
                    UseShellExecute = true
                }
            }.Start();
        }
    }
}