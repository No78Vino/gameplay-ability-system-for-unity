using System.Collections.Generic;  
using System.IO;  
using Sirenix.OdinInspector;  
using UnityEditor;  
using UnityEngine;  
  
namespace EXUI.Editor  
{  
    /// <summary>  
    /// XUI 窗口生成器的核心 View。  
    /// 负责：选 Prefab → 扫描节点 → 预览/编辑字段名 → 生成 View + ViewModel 脚本。  
    /// </summary>  
    public class XUICreatorView  
    {  
        private const string TITLE = "XUI窗口生成器";  
        private const string TITLE_H = "XUI窗口生成器/操作栏";  
  
        private readonly XUISettingAsset _setting;  
  
        public XUICreatorView()  
        {  
            _setting = XUISettingAsset.Instance;  
        }  
  
        // ── 1. Prefab 选择 ────────────────────────────────────────────────  
  
        [TitleGroup(TITLE)]  
        [HorizontalGroup(TITLE_H)]  
        [LabelText("目标 Prefab")]  
        [LabelWidth(90)]  
        [Required("请先选择一个 UI Prefab")]  
        [OnValueChanged(nameof(OnPrefabChanged))]  
        [AssetsOnly]  
        public GameObject TargetPrefab;  
  
        [HorizontalGroup(TITLE_H)]  
        [LabelText("窗口名称")]  
        [LabelWidth(70)]  
        [Tooltip("生成的类名前缀，例如填 Guide 则生成 GuideWindow + VMGuideWindow")]  
        public string WindowName = "";  
  
        // ── 2. 扫描 ──────────────────────────────────────────────────────  
  
        [HorizontalGroup(TITLE_H)]  
        [Button("扫描节点", Icon = SdfIconType.Search)]  
        private void ScanPrefab()  
        {  
            if (TargetPrefab == null)  
            {  
                EditorUtility.DisplayDialog("错误", "请先选择 Prefab", "确定");  
                return;  
            }  
            if (string.IsNullOrEmpty(WindowName))  
            {  
                EditorUtility.DisplayDialog("错误", "请先填写窗口名称", "确定");  
                return;  
            }  
  
            var results = XUIPrefabScanner.Scan(TargetPrefab);  
            ScanResults = new List<ScanResultInEditor>();  
            foreach (var r in results)  
            {  
                ScanResults.Add(new ScanResultInEditor  
                {  
                    NodePath          = r.NodePath,  
                    ComponentTypeName = r.ComponentType.Name,  
                    FieldName         = r.SuggestedFieldName,  
                    VMPropertyName    = r.SuggestedVMPropertyName,  
                    IncludeInBind     = true, // 默认全选  
                });  
            }  
        }  
  
        // ── 3. 扫描结果预览 + 可编辑字段名 ──────────────────────────────  
  
        [TitleGroup(TITLE)]  
        [LabelText(" ")]  
        [ShowInInspector]  
        [ShowIf(nameof(HasScanResults))]  
        [TableList(AlwaysExpanded = true)]  
        public List<ScanResultInEditor> ScanResults = new();  
  
        private bool HasScanResults => ScanResults != null && ScanResults.Count > 0;  
  
        // ── 4. 生成代码 ──────────────────────────────────────────────────  
  
        [TitleGroup(TITLE)]  
        [HorizontalGroup(TITLE_H + "2")]  
        [GUIColor("green")]  
        [Button("生成脚本", Icon = SdfIconType.Code, ButtonHeight = 30)]  
        [ShowIf(nameof(HasScanResults))]  
        private void Generate()  
        {  
            if (string.IsNullOrEmpty(WindowName))  
            {  
                EditorUtility.DisplayDialog("错误", "窗口名称不能为空", "确定");  
                return;  
            }  
  
            // 将编辑后的 ScanResultInEditor 转回 NodeScanResult  
            var filtered = new List<XUIPrefabScanner.NodeScanResult>();  
            foreach (var r in ScanResults)  
            {  
                if (!r.IncludeInBind) continue;  
                filtered.Add(new XUIPrefabScanner.NodeScanResult  
                {  
                    NodePath              = r.NodePath,  
                    ComponentType         = System.Type.GetType($"UnityEngine.UI.{r.ComponentTypeName}, UnityEngine.UI"),  
                    SuggestedFieldName    = r.FieldName,  
                    SuggestedVMPropertyName = r.VMPropertyName,  
                });  
            }  
  
            XUICodeGenerator.Generate(WindowName, filtered);  
  
            AssetDatabase.Refresh();  
            EditorUtility.DisplayDialog("完成",  
                $"已生成:\n" +  
                $"  {_setting.ViewCodeOutputPath}/{WindowName}Window.cs\n" +  
                $"  {_setting.ViewModelCodeOutputPath}/VM{WindowName}Window.cs",  
                "OK");  
        }  
  
        [HorizontalGroup(TITLE_H + "2")]  
        [Button("打开 View 输出目录", Icon = SdfIconType.Folder)]  
        private void OpenViewOutputDir()  
        {  
            if (Directory.Exists(_setting.ViewCodeOutputPath))  
                EditorUtility.RevealInFinder(_setting.ViewCodeOutputPath);  
            else  
                EditorUtility.DisplayDialog("错误", $"目录不存在: {_setting.ViewCodeOutputPath}", "确定");  
        }  
  
        [HorizontalGroup(TITLE_H + "2")]  
        [Button("打开 ViewModel 输出目录", Icon = SdfIconType.Folder)]  
        private void OpenVMOutputDir()  
        {  
            if (Directory.Exists(_setting.ViewModelCodeOutputPath))  
                EditorUtility.RevealInFinder(_setting.ViewModelCodeOutputPath);  
            else  
                EditorUtility.DisplayDialog("错误", $"目录不存在: {_setting.ViewModelCodeOutputPath}", "确定");  
        }  
  
        // ── 内部辅助 ─────────────────────────────────────────────────────  
        private void OnPrefabChanged()  
        {  
            // Prefab 变更时清空上次扫描结果  
            ScanResults?.Clear();  
            // 自动推断窗口名（去掉 "Window" 后缀，如 GuideWindow.prefab → Guide）  
            if (TargetPrefab != null)  
            {  
                var n = TargetPrefab.name;  
                WindowName = n.EndsWith("Window") ? n.Substring(0, n.Length - "Window".Length) : n;  
            }  
        }  
    }  
  
    /// <summary>  
    /// 单条扫描结果的编辑器数据模型（支持在 TableList 中直接编辑）  
    /// </summary>  
    public class ScanResultInEditor  
    {  
        [TableColumnWidth(180, Resizable = false)]  
        [ReadOnly]  
        [LabelText("节点路径")]  
        public string NodePath;  
  
        [TableColumnWidth(100, Resizable = false)]  
        [ReadOnly]  
        [LabelText("组件类型")]  
        public string ComponentTypeName;  
  
        [TableColumnWidth(160)]  
        [LabelText("字段名")]  
        public string FieldName;  
  
        [TableColumnWidth(160)]  
        [LabelText("VM属性名")]  
        public string VMPropertyName;  
  
        [TableColumnWidth(60, Resizable = false)]  
        [LabelText("参与绑定")]  
        public bool IncludeInBind;  
    }  
}