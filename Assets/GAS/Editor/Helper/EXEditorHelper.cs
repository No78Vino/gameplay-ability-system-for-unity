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
        
        public static List<(string Name, Type FieldType)> GetAllReadOnlyFields(Type targetType)  
        {  
            var result = new List<(string, Type)>();  
            var bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;  
            if (targetType != null)  
            {  
                var allFields = targetType.GetFields(bindingFlags);  
                foreach (var field in allFields)  
                    if (field.IsInitOnly)  
                        result.Add((field.Name, field.FieldType));  
            }  
            return result;  
        }


        /// <summary>  
        /// 从运行时 XParam 类型中提取所有 [BeanField] 标注的字段/属性信息  
        /// </summary>  
        public struct BeanFieldInfo  
        {  
            public string Name;       // Bean 字段名（attr.Name ?? 成员名）  
            public string Setter;     // Setter 方法名（attr.Setter）  
            public string LubanType;  // Luban 类型（attr.LubanType，可能为 null）  
            public Type MemberType;   // 成员的 C# 类型  
            public string Comment;    // 注释 
            public int Order;         // 排序权重（来自 BeanFieldAttribute.Order）  
        }  
  
        public static List<BeanFieldInfo> GetBeanFields(Type runtimeParamType)    
        {    
            var result = new List<BeanFieldInfo>();    
            if (runtimeParamType == null) return result;    
  
            var bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;    
  
            // 扫描字段    
            foreach (var field in runtimeParamType.GetFields(bindingFlags))    
            {    
                var attr = field.GetCustomAttribute<GAS.Runtime.BeanFieldAttribute>();    
                if (attr == null) continue;    
                result.Add(new BeanFieldInfo    
                {    
                    Name = attr.Name ?? field.Name,    
                    Setter = attr.Setter,    
                    LubanType = attr.LubanType,    
                    MemberType = field.FieldType,    
                    Comment = attr.Comment,    
                    Order = attr.Order,  
                });    
            }    
  
            // 扫描属性    
            foreach (var prop in runtimeParamType.GetProperties(bindingFlags))    
            {    
                var attr = prop.GetCustomAttribute<GAS.Runtime.BeanFieldAttribute>();    
                if (attr == null) continue;    
                result.Add(new BeanFieldInfo    
                {    
                    Name = attr.Name ?? prop.Name,    
                    Setter = attr.Setter,    
                    LubanType = attr.LubanType,    
                    MemberType = prop.PropertyType,    
                    Comment = attr.Comment,    
                    Order = attr.Order,  
                });    
            }    
  
            // 按 Order 排序（稳定排序，Order 相同时保持收集顺序）  
            result.Sort((a, b) => a.Order.CompareTo(b.Order));  
  
            return result;    
        }
        
        
        /// <summary>  
        /// 从运行时 XParam 类型中提取所有 [BeanPolymorphicField] 标注的字段/属性信息  
        /// </summary>  
        public struct BeanPolymorphicFieldInfo  
        {  
            public string BeanFieldName;        // 写入 __beans__.xlsx 的字段名  
            public string LubanPolymorphicType; // Luban 多态抽象 Bean 类型名  
            public string TypeSetter;           // 类型判别符 Setter  
            public string ParamSetter;          // Param Setter  
            public string ParamTypeResolver;    // 运行时 Param 类型解析方法  
            public string HelperCategory;       // Editor Helper 类别  
        }  
  
        public static List<BeanPolymorphicFieldInfo> GetBeanPolymorphicFields(Type runtimeParamType)  
        {  
            var result = new List<BeanPolymorphicFieldInfo>();  
            if (runtimeParamType == null) return result;  
  
            var bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;  
  
            foreach (var field in runtimeParamType.GetFields(bindingFlags))  
            {  
                var attr = field.GetCustomAttribute<GAS.Runtime.BeanPolymorphicFieldAttribute>();  
                if (attr == null) continue;  
                result.Add(MapAttrToInfo(attr));  
            }  
  
            foreach (var prop in runtimeParamType.GetProperties(bindingFlags))  
            {  
                var attr = prop.GetCustomAttribute<GAS.Runtime.BeanPolymorphicFieldAttribute>();  
                if (attr == null) continue;  
                result.Add(MapAttrToInfo(attr));  
            }  
  
            return result;  
  
            static BeanPolymorphicFieldInfo MapAttrToInfo(GAS.Runtime.BeanPolymorphicFieldAttribute attr)  
            {  
                return new BeanPolymorphicFieldInfo  
                {  
                    BeanFieldName = attr.BeanFieldName,  
                    LubanPolymorphicType = attr.LubanPolymorphicType,  
                    TypeSetter = attr.TypeSetter,  
                    ParamSetter = attr.ParamSetter,  
                    ParamTypeResolver = attr.ParamTypeResolver,  
                    HelperCategory = attr.HelperCategory,  
                };  
            }  
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

        #region ReflectionHelper

        public static object InvokeStaticMethod(string classTypeFullName,string functionName, params object[] param)
        {
            try
            {
                // 1. 反射获取静态类A的类型（替换为实际的命名空间+类名）
                // 格式："命名空间.类名, 程序集名"（同程序集可省略程序集名）
                Type staticType = GetTypeByName(classTypeFullName);

                if (staticType == null)
                {
                    // 类A未生成，执行备选逻辑
                    Debug.LogWarning($"静态类{classTypeFullName}不存在，执行默认处理");
                    return $"无静态类{classTypeFullName}";
                }

                // 2. 获取目标静态方法（根据实际方法名和参数类型调整）
                var fieldTypes = new Type[param.Length];
                for (var i = 0; i < param.Length; i++) fieldTypes[i] = param[i].GetType();
                
                MethodInfo method = staticType.GetMethod(
                    functionName, // 方法名
                    BindingFlags.Static | BindingFlags.Public, // 静态+公有
                    null,
                    fieldTypes, // 方法参数类型（无参数则传空数组）
                    null
                );

                if (method == null)
                {
                    // 方法不存在，执行备选逻辑
                    Debug.LogWarning($"静态方法{functionName}不存在，执行默认处理");
                    return $"静态方法{functionName}不存在";
                }

                // 3. 调用静态方法（参数为null表示静态方法，第二个参数是实际参数数组）
                return method.Invoke(null, param);
            }
            catch (Exception ex)
            {
                // 捕获可能的异常（如参数不匹配等）
                Console.WriteLine($"调用静态方法出错：{ex.Message}");
                return "错误返回值";
            }
        }

        public static object InvokeStaticXLubanMethod(string functionName,params object[] param)
        {
            return InvokeStaticMethod("GAS.Runtime.XLuban", functionName, param);
        }

        #endregion
    }
}