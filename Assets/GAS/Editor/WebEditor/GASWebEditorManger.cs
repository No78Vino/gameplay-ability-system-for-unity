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
  
        // ── 环境部署（install_deps.bat 在根目录，所有编辑器共用）────────  
        [MenuItem("EXTool/EX-GAS/Web编辑器/📦 一键部署编辑器环境")]  
        public static void DeployWebEditorEnv()  
        {  
            // install_deps.bat 直接在 WEB_EDITOR_ROOT 下，不在子目录  
            var batPath = Path.GetFullPath(Path.Combine(WEB_EDITOR_ROOT, "install_deps.bat"));  
            RunBat(batPath);  
        }  
  
        // ── GameplayTag 编辑器 ────────────────────────────────────────────  
        [MenuItem("EXTool/EX-GAS/Web编辑器/🌐 启动 Tag 网页编辑器")]  
        public static void LaunchTagWebEditor()  
        {  
            var setting = GASSettingAsset.LoadOrCreate();  
            var xlsxPath = setting.PathOfExcelTag;  
            if (!File.Exists(xlsxPath))  
            {  
                EditorUtility.DisplayDialog("错误",  
                    $"Tag Excel 文件未找到:\n{xlsxPath}\n\n请先在 Setting 页面配置正确的 ConfigProjectPath。",  
                    "确定");  
                return;  
            }  
            // start.bat 在 GameplayTag 子目录下  
            var batPath = GetFullPath(TAG_EDITOR_DIR, "start.bat");  
            RunBat(batPath, arguments: $"\"{xlsxPath}\"");  
        }  
  
        // ── 未来扩展 ──────────────────────────────────────────────────────  
        // [MenuItem("EXTool/EX-GAS/Web编辑器/🌐 启动 Effect 网页编辑器")]  
        // public static void LaunchEffectWebEditor() { ... }  
  
        // ── 工具方法 ──────────────────────────────────────────────────────  
        private static string GetFullPath(string editorDir, string fileName)  
            => Path.GetFullPath(Path.Combine(WEB_EDITOR_ROOT, editorDir, fileName));  
  
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
                    FileName         = batPath,  
                    Arguments        = arguments,  
                    WorkingDirectory = Path.GetDirectoryName(batPath),  
                    UseShellExecute  = true,  
                }  
            }.Start();  
        }  
    }  
}