using System;  
using System.Collections.Generic;  
using UnityEditor;  
using UnityEngine;  
using UnityEngine.UI;  
  
namespace EXUI.Editor  
{  
    /// <summary>  
    /// 扫描 UI Prefab 的节点树，枚举所有挂载了 UI 组件的节点。  
    /// 输出结果作为 XUICodeGenerator 的数据输入。  
    /// </summary>  
    public static class XUIPrefabScanner  
    {  
        // ✅ 支持扫描的 UI 组件类型白名单，按优先级排列  
        // （一个节点可能同时有 Text 和 Button，优先级决定取哪个）  
        private static readonly Type[] SupportedComponentTypes = new[]  
        {  
            typeof(Button),  
            typeof(InputField),  
            typeof(Toggle),  
            typeof(Slider),  
            typeof(ScrollRect),  
            typeof(Text),  
            typeof(Image),  
            typeof(RawImage),  
        };  
  
        /// <summary>  
        /// 单条扫描结果  
        /// </summary>  
        public struct NodeScanResult  
        {  
            /// <summary>  
            /// 相对于 Prefab 根节点的路径，例如 "guide/label_guide"  
            /// 格式与 BaseView.GetComponentByNode 的参数一致  
            /// </summary>  
            public string NodePath;  
  
            /// <summary>  
            /// 节点上检测到的最高优先级 UI 组件类型  
            /// </summary>  
            public Type ComponentType;  
  
            /// <summary>  
            /// 建议的 C# 字段名，例如 "_labelGuide"  
            /// </summary>  
            public string SuggestedFieldName;  
  
            /// <summary>  
            /// 建议的 VM ObservableVariable 属性名，例如 "LabelGuide"  
            /// </summary>  
            public string SuggestedVMPropertyName;  
        }  
  
        /// <summary>  
        /// 扫描指定 Prefab，返回所有带 UI 组件的节点信息。  
        /// </summary>  
        /// <param name="prefab">要扫描的 UI Prefab</param>  
        /// <param name="excludeRoot">是否排除根节点自身</param>  
        public static List<NodeScanResult> Scan(GameObject prefab, bool excludeRoot = true)  
        {  
            var results = new List<NodeScanResult>();  
            if (prefab == null) return results;  
  
            var root = prefab.transform;  
            ScanRecursive(root, root, excludeRoot ? null : root, results);  
            return results;  
        }  
  
        private static void ScanRecursive(  
            Transform current,  
            Transform root,  
            Transform skipNode,  
            List<NodeScanResult> results)  
        {  
            if (current != skipNode)  
            {  
                // 按优先级顺序检查组件白名单  
                foreach (var type in SupportedComponentTypes)  
                {  
                    if (current.TryGetComponent(type, out _))  
                    {  
                        var path = GetRelativePath(current, root);  
                        results.Add(new NodeScanResult  
                        {  
                            NodePath = path,  
                            ComponentType = type,  
                            SuggestedFieldName = BuildFieldName(current.name, type),  
                            SuggestedVMPropertyName = BuildVMPropertyName(current.name, type),  
                        });  
                        break; // 只取优先级最高的一个组件  
                    }  
                }  
            }  
  
            foreach (Transform child in current)  
                ScanRecursive(child, root, null, results);  
        }  
  
        /// <summary>  
        /// 构建相对于根节点的路径，格式为 "parent/child/grandchild"  
        /// 与 BaseView.GetComponentByNode 的节点路径格式一致  
        /// </summary>  
        private static string GetRelativePath(Transform node, Transform root)  
        {  
            if (node == root) return string.Empty;  
            var parts = new System.Text.StringBuilder();  
            var current = node;  
            while (current != root && current != null)  
            {  
                if (parts.Length > 0) parts.Insert(0, "/");  
                parts.Insert(0, current.name);  
                current = current.parent;  
            }  
            return parts.ToString();  
        }  
  
        /// <summary>  
        /// 根据节点名和组件类型生成建议的 C# 字段名  
        /// 例如：节点名 "label_guide" + Text → "_labelGuide"  
        /// </summary>  
        private static string BuildFieldName(string nodeName, Type componentType)  
        {  
            string prefix = GetComponentPrefix(componentType);  
            string camel = ToPascalCase(nodeName);  
            return $"_{prefix}{camel}";  
        }  
  
        /// <summary>  
        /// 根据节点名和组件类型生成建议的 VM 属性名（ObservableVariable 的属性名）  
        /// 例如：节点名 "label_guide" + Text → "LabelGuide"  
        /// </summary>  
        private static string BuildVMPropertyName(string nodeName, Type componentType)  
        {  
            return ToPascalCase(nodeName);  
        }  
  
        private static string GetComponentPrefix(Type type)  
        {  
            if (type == typeof(Button))     return "btn";  
            if (type == typeof(Text))       return "label";  
            if (type == typeof(Image))      return "img";  
            if (type == typeof(InputField)) return "input";  
            if (type == typeof(Toggle))     return "toggle";  
            if (type == typeof(Slider))     return "slider";  
            if (type == typeof(ScrollRect)) return "scroll";  
            if (type == typeof(RawImage))   return "rawImg";  
            return "ui";  
        }  
  
        /// <summary>  
        /// 将 snake_case 或 camelCase 的节点名转换为 PascalCase  
        /// 例如：label_guide → LabelGuide，hpBar → HpBar  
        /// </summary>  
        private static string ToPascalCase(string name)  
        {  
            var parts = name.Split('_', '-', ' ');  
            var sb = new System.Text.StringBuilder();  
            foreach (var part in parts)  
            {  
                if (string.IsNullOrEmpty(part)) continue;  
                sb.Append(char.ToUpperInvariant(part[0]));  
                if (part.Length > 1) sb.Append(part.Substring(1));  
            }  
            return sb.ToString();  
        }  
    }  
}