using System;  
using System.Collections.Generic;  
using System.IO;
using GAS.Editor;
using UnityEditor;  
using UnityEngine;  
using UnityEngine.UI;  
  
namespace EXUI.Editor  
{  
    /// <summary>  
    /// EXUI View/ViewModel 代码生成器。  
    /// 输入：窗口名 + Prefab 扫描结果（来自 XUIPrefabScanner）  
    /// 输出：XxxWindow.cs + VMXxxWindow.cs  
    /// </summary>  
    public static class XUICodeGenerator  
    {  
        // ── 组件类型 → 默认 UI 属性名映射（用于 [BindOneWay] 的第三参数）──  
        private static readonly Dictionary<Type, string> ComponentDefaultPropertyMap = new()  
        {  
            [typeof(UnityEngine.UI.Text)]       = "text",  
            [typeof(UnityEngine.UI.Image)]      = "fillAmount",  
            [typeof(UnityEngine.UI.RawImage)]   = "texture",  
            [typeof(UnityEngine.UI.Slider)]     = "value",  
            [typeof(UnityEngine.UI.Toggle)]     = "isOn",  
            [typeof(UnityEngine.UI.InputField)] = "text",  
            [typeof(UnityEngine.UI.Button)]     = null,  // Button 不生成 BindOneWay，保留手写区域  
            [typeof(UnityEngine.UI.ScrollRect)] = null,  
        };  
  
        // ── 组件类型 → using 命名空间（避免生成代码里出现全限定名）──  
        private static readonly Dictionary<Type, string> ComponentTypeAlias = new()  
        {  
            [typeof(UnityEngine.UI.Text)]       = "Text",  
            [typeof(UnityEngine.UI.Image)]      = "Image",  
            [typeof(UnityEngine.UI.RawImage)]   = "RawImage",  
            [typeof(UnityEngine.UI.Slider)]     = "Slider",  
            [typeof(UnityEngine.UI.Toggle)]     = "Toggle",  
            [typeof(UnityEngine.UI.InputField)] = "InputField",  
            [typeof(UnityEngine.UI.Button)]     = "Button",  
            [typeof(UnityEngine.UI.ScrollRect)] = "ScrollRect",  
        };  
  
        /// <summary>  
        /// 生成 XxxWindow.cs 和 VMXxxWindow.cs 两个文件。  
        /// </summary>  
        /// <param name="windowName">窗口名（不含 "Window" 后缀），如 "Shop"</param>  
        /// <param name="scanResults">XUIPrefabScanner.Scan() 返回的节点扫描结果</param>  
        [MenuItem("EXTool/XUI/生成脚本/生成所有（需在 WindowCreator 中选择 Prefab）")]  
        public static void GenerateAll()  
        {  
            // 此处仅作为菜单触发示例，实际调用由 XUIWindowCreator 传入参数  
            Debug.LogWarning("[XUICodeGenerator] 请通过 EXTool/XUI/窗口生成器 使用可视化向导生成代码。");  
        }  
  
        /// <summary>  
        /// 核心生成入口，由 XUIWindowCreator 调用。  
        /// </summary>  
        public static void Generate(  
            string windowName,  
            List<XUIPrefabScanner.NodeScanResult> scanResults)  
        {  
            var setting = XUISettingAsset.Instance;  
  
            EnsureDirectory(setting.ViewCodeOutputPath);  
            EnsureDirectory(setting.ViewModelCodeOutputPath);  
  
            GenerateViewScript(windowName, scanResults, setting);  
            GenerateViewModelScript(windowName, scanResults, setting);  
  
            AssetDatabase.Refresh();  
            Debug.Log($"[XUICodeGenerator] ✅ 已生成 {windowName}Window.cs 和 VM{windowName}Window.cs");  
        }  
  
        // ────────────────────────────────────────────────────────────────────  
        // View 脚本生成  
        // ────────────────────────────────────────────────────────────────────  
  
        private static void GenerateViewScript(  
            string windowName,  
            List<XUIPrefabScanner.NodeScanResult> scanResults,  
            XUISettingAsset setting)  
        {  
            var className  = $"{windowName}Window";  
            var vmClass    = $"VM{windowName}Window";  
            var filePath   = setting.GetViewCodePath(className);  
  
            using var writer = new IndentedWriter(new StreamWriter(filePath));  
  
            // ── 文件头 ──  
            WriteFileHeader(writer);  
  
            // ── using ──  
            writer.WriteLine("using EXUI;");  
            writer.WriteLine("using UnityEngine.UI;");  
            writer.WriteLine("using Loxodon.Framework.Binding.Builder;");  
            writer.WriteLine("");  
  
            // ── namespace ──  
            writer.WriteLine($"namespace {setting.ViewNamespace}");  
            writer.WriteLine("{");  
            writer.Indent++;  
            {  
                writer.WriteLine($"public class {className} : BaseView<{vmClass}>");  
                writer.WriteLine("{");  
                writer.Indent++;  
                {  
                    // ── [BindOneWay] 字段声明区 ──  
                    var hasBindFields = false;  
                    foreach (var result in scanResults)  
                    {  
                        if (!ComponentDefaultPropertyMap.TryGetValue(result.ComponentType, out var uiProp))  
                            continue;  
                        if (uiProp == null) continue; // Button 等跳过，保留手写区域  
  
                        hasBindFields = true;  
                        var typeAlias = ComponentTypeAlias[result.ComponentType];  
                        var vmProp    = result.SuggestedVMPropertyName;  
                        var nodePath  = result.NodePath;  
  
                        // 生成带 Attribute 的字段声明  
                        writer.WriteLine(  
                            $"[BindOneWay(\"{nodePath}\", nameof({vmClass}.{vmProp}), \"{uiProp}\")]");  
                        writer.WriteLine(  
                            $"private {typeAlias} {result.SuggestedFieldName};");  
                        writer.WriteLine("");  
                    }  
  
                    if (!hasBindFields)  
                        writer.WriteLine("// 暂无可自动绑定的节点，请手动补充字段");  
  
                    writer.WriteLine("");  
  
                    // ── Button 等事件绑定的手写扩展区 ──  
                    var hasButtons = scanResults.Exists(r =>  
                        r.ComponentType == typeof(Button) ||  
                        r.ComponentType == typeof(Toggle) ||  
                        r.ComponentType == typeof(ScrollRect));  
  
                    if (hasButtons)  
                    {  
                        // 生成 Button 字段（无 Attribute，手动绑定）  
                        foreach (var result in scanResults)  
                        {  
                            if (result.ComponentType != typeof(Button) &&  
                                result.ComponentType != typeof(Toggle) &&  
                                result.ComponentType != typeof(ScrollRect))  
                                continue;  
  
                            var typeAlias = ComponentTypeAlias[result.ComponentType];  
                            writer.WriteLine(  
                                $"private {typeAlias} {result.SuggestedFieldName};");  
                        }  
                        writer.WriteLine("");  
                    }  
  
                    // ── InitViewComponents() ──  
                    writer.WriteLine("protected override void InitViewComponents()");  
                    writer.WriteLine("{");  
                    writer.Indent++;  
                    {  
                        foreach (var result in scanResults)  
                        {  
                            if (!ComponentTypeAlias.TryGetValue(result.ComponentType, out var typeAlias))  
                                continue;  
  
                            writer.WriteLine(  
                                $"{result.SuggestedFieldName} = GetComponentByNode<{typeAlias}>(\"{result.NodePath}\");");  
                        }  
                    }  
                    writer.Indent--;  
                    writer.WriteLine("}");  
                    writer.WriteLine("");  
  
                    // ── BindData() — 仅保留事件绑定手写区，自动绑定由 BaseView<T> 的 Attribute 缓存处理 ──  
                    writer.WriteLine("protected override void BindData()");  
                    writer.WriteLine("{");  
                    writer.Indent++;  
                    {  
                        if (hasButtons)  
                        {  
                            writer.WriteLine(  
                                $"var bindingSet = new BindingSet<{className}, {vmClass}>(_bindingContext, this);");  
                            writer.WriteLine("// TODO: 在此添加 Button/Toggle 等事件绑定");  
                            writer.WriteLine("// 例：bindingSet.Bind(btnXxx).For(v => v.onClick).To(vm => vm.XxxCommand);");  
                            writer.WriteLine("bindingSet.Build();");  
                        }  
                        else  
                        {  
                            writer.WriteLine("// 所有绑定已由 [BindOneWay]/[BindTwoWay] Attribute 自动处理");  
                        }  
                    }  
                    writer.Indent--;  
                    writer.WriteLine("}");  
                }  
                writer.Indent--;  
                writer.WriteLine("}");  
            }  
            writer.Indent--;  
            writer.WriteLine("}");  
        }  
  
        // ────────────────────────────────────────────────────────────────────  
        // ViewModel 脚本生成  
        // ────────────────────────────────────────────────────────────────────  
  
        private static void GenerateViewModelScript(  
            string windowName,  
            List<XUIPrefabScanner.NodeScanResult> scanResults,  
            XUISettingAsset setting)  
        {  
            var className = $"VM{windowName}Window";  
            var filePath  = setting.GetViewModelCodePath(className);  
  
            using var writer = new IndentedWriter(new StreamWriter(filePath));  
  
            WriteFileHeader(writer);  
  
            writer.WriteLine("using EXUI;");  
            writer.WriteLine("using Loxodon.Framework.Extension;");  
            writer.WriteLine("");  
  
            writer.WriteLine($"namespace {setting.ViewModelNamespace}");  
            writer.WriteLine("{");  
            writer.Indent++;  
            {  
                writer.WriteLine($"public class {className} : ViewModelCommon");  
                writer.WriteLine("{");  
                writer.Indent++;  
                {  
                    // ── 每个可绑定节点对应一个只读 ObservableVariable 属性 ──  
                    foreach (var result in scanResults)  
                    {  
                        if (!ComponentDefaultPropertyMap.TryGetValue(result.ComponentType, out var uiProp))  
                            continue;  
                        if (uiProp == null) continue; // Button 跳过  
  
                        // 根据 UI 属性类型推断 ObservableVariable 的泛型参数  
                        var vmPropType = uiProp switch  
                        {  
                            "text"        => "string",  
                            "isOn"        => "bool",  
                            "value"       => "float",  
                            "fillAmount"  => "float",  
                            _             => "string",  
                        };  
  
                        writer.WriteLine(  
                            $"public ObservableVariable<{vmPropType}> {result.SuggestedVMPropertyName} {{ get; }} = new();");  
                    }  
  
                    writer.WriteLine("");  
                    writer.WriteLine("// TODO: 在此添加业务方法");  
                }  
                writer.Indent--;  
                writer.WriteLine("}");  
            }  
            writer.Indent--;  
            writer.WriteLine("}");  
        }  
  
        // ────────────────────────────────────────────────────────────────────  
        // 辅助方法  
        // ────────────────────────────────────────────────────────────────────  
  
        private static void WriteFileHeader(IndentedWriter writer)  
        {  
            writer.WriteLine("///////////////////////////////////");  
            writer.WriteLine("//// This is a generated file. ////");  
            writer.WriteLine("////   Modify BindData() only.  ////");  
            writer.WriteLine("///////////////////////////////////");  
            writer.WriteLine("");  
        }  
  
        private static void EnsureDirectory(string path)  
        {  
            if (!Directory.Exists(path))  
                Directory.CreateDirectory(path);  
        }  
    }  
}