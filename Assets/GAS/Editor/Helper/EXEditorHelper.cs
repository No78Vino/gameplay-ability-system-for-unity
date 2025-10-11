using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace GAS.Editor
{
    public static class EXEditorHelper
    {
        public static void ShowNotification(string message, float duration = 2f)
        {
            // 获取当前激活的编辑器窗口（通常是Inspector或场景视图）
            var targetWindow = EditorWindow.mouseOverWindow ? EditorWindow.mouseOverWindow : EditorWindow.focusedWindow;

            if (targetWindow != null)
                targetWindow.ShowNotification(new GUIContent(message));
            else
                Debug.LogWarning(message);
        }

        public static List<string> GetAllReadOnlyFieldNames(Type targetType)
        {
            var readonlyFieldNames = new List<string>();

            // 绑定标志：获取实例字段（包括public和非public），但不包括静态字段
            var bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            // 获取所有字段，并通过IsInitOnly识别readonly字段
            if (targetType != null)
            {
                var allFields = targetType.GetFields(bindingFlags);

                foreach (var field in allFields)
                    // 关键：IsInitOnly为true表示readonly字段
                    if (field.IsInitOnly)
                        readonlyFieldNames.Add(field.Name);
            }

            return readonlyFieldNames;
        }

        public static int GetFrameRate()
        {
            return (int)Math.Round(1 / Time.fixedDeltaTime);
        }


        #region TypeFinder

        private static readonly Dictionary<string, Type> _typeCache = new(StringComparer.Ordinal);

        private static bool _assembliesScanned;

        /// <summary>
        ///     通过类名字符串获取Type类型
        /// </summary>
        /// <param name="className">类名（支持完整名称或简单名称）</param>
        /// <returns>对应的Type对象，未找到时返回null</returns>
        public static Type GetTypeByName(string className)
        {
            if (string.IsNullOrWhiteSpace(className))
                return null;

            // 首先尝试从缓存获取
            if (_typeCache.TryGetValue(className, out var cachedType))
                return cachedType;

            // 尝试直接获取类型（当className是完整类型名时）
            var type = Type.GetType(className, false);
            if (type != null)
            {
                _typeCache[className] = type;
                return type;
            }

            // 如果尚未扫描所有程序集，执行全面扫描
            if (!_assembliesScanned)
            {
                ScanAllAssemblies();
                _assembliesScanned = true;

                // 扫描后再次尝试缓存
                if (_typeCache.TryGetValue(className, out type))
                    return type;
            }

            // 如果仍然未找到，尝试在已加载类型中查找
            type = FindTypeInLoadedTypes(className);
            if (type != null) _typeCache[className] = type;

            return type;
        }

        /// <summary>
        ///     扫描所有已加载程序集的类型并缓存
        /// </summary>
        private static void ScanAllAssemblies()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
                try
                {
                    // 跳过系统程序集提升性能
                    if (IsSystemAssembly(assembly))
                        continue;

                    foreach (var type in assembly.GetTypes())
                    {
                        var fullName = type.FullName;
                        var shortName = type.Name;

                        // 缓存完整类型名
                        if (!string.IsNullOrEmpty(fullName) && !_typeCache.ContainsKey(fullName))
                            _typeCache[fullName] = type;

                        // 缓存简单类型名（不包含命名空间）
                        if (!_typeCache.ContainsKey(shortName)) _typeCache[shortName] = type;
                    }
                }
                catch (ReflectionTypeLoadException)
                {
                    // 忽略无法加载类型的程序集
                }
        }

        /// <summary>
        ///     在已加载类型中查找指定类名
        /// </summary>
        private static Type FindTypeInLoadedTypes(string className)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
                try
                {
                    if (IsSystemAssembly(assembly))
                        continue;

                    var type = assembly.GetType(className, false);
                    if (type != null)
                        return type;

                    // 如果直接获取失败，遍历程序集的所有类型
                    foreach (var t in assembly.GetTypes())
                        if (t.Name.Equals(className, StringComparison.Ordinal) ||
                            t.FullName.Equals(className, StringComparison.Ordinal))
                            return t;
                }
                catch (ReflectionTypeLoadException)
                {
                    // 忽略加载错误的程序集
                }

            return null;
        }

        /// <summary>
        ///     判断是否系统程序集（提升性能）
        /// </summary>
        private static bool IsSystemAssembly(Assembly assembly)
        {
            var assemblyName = assembly.FullName;
            return assemblyName.StartsWith("System.") ||
                   assemblyName.StartsWith("Microsoft.") ||
                   assemblyName.StartsWith("mscorlib") ||
                   assemblyName.StartsWith("netstandard") ||
                   assemblyName.StartsWith("System,");
        }

        #endregion
    }
}