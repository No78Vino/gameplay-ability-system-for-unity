using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace GAS.General
{
    /// <summary>
    ///     Unity 插件静态反射工具类
    ///     提供对其他程序集静态方法、静态字段/属性的统一访问接口，并带缓存优化。
    /// </summary>
    public static class ReflectionHelper
    {
        #region 缓存

        // 类型缓存：fullClassName -> Type
        private static readonly Dictionary<string, Type> TypeCache = new();

        // 方法缓存：fullClassName.methodName(参数类型列表) -> MethodInfo
        private static readonly Dictionary<string, MethodInfo> MethodCache = new();

        #endregion

        #region 调用静态方法

        /// <summary>
        ///     调用静态方法。
        ///     使用方式：
        ///     object result = ReflectionHelper.InvokeStaticMethod("命名空间.类名", "方法名", 参数...);
        ///     如果方法为 void，则返回 null。
        /// </summary>
        /// <param name="fullClassName">完整类名（包含命名空间），例如 "UnityEngine.Mathf"</param>
        /// <param name="methodName">方法名</param>
        /// <param name="parameters">参数列表（类型不确定，直接传 object[] 即可）</param>
        /// <returns>方法返回值；void 方法返回 null</returns>
        public static object InvokeStaticMethod(string fullClassName, string methodName, params object[] parameters)
        {
            try
            {
                // 1. 获取类型
                var type = GetTypeFromFullName(fullClassName);
                if (type == null) return null;

                // 2. 构建缓存 key：类名.方法名(参数类型...)
                var cacheKey = BuildCacheKey(fullClassName, methodName, parameters);

                // 3. 从缓存取 MethodInfo
                if (!MethodCache.TryGetValue(cacheKey, out var method))
                {
                    // 基础获取（如果你需要精确重载匹配，可以再根据参数类型数组做 GetMethod 的重载匹配）
                    method = type.GetMethod(methodName,
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                    if (method == null)
                    {
                        Debug.LogError($"[ReflectionHelper] 静态方法未找到：{fullClassName}.{methodName}");
                        return null;
                    }

                    MethodCache[cacheKey] = method;
                }

                // 4. Invoke 调用
                try
                {
                    return method.Invoke(null, parameters);
                }
                catch (TargetInvocationException ex)
                {
                    Debug.LogError(
                        $"[ReflectionHelper] 调用 {fullClassName}.{methodName} 发生异常：{ex.InnerException?.Message ?? ex.Message}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ReflectionHelper] 反射调用错误：{ex.Message}");
                return null;
            }
        }

        /// <summary>
        ///     调用泛型静态方法（可选扩展）。
        /// </summary>
        public static object InvokeGenericStaticMethod(string fullClassName, string methodName,
            Type[] genericArguments, params object[] parameters)
        {
            try
            {
                var type = GetTypeFromFullName(fullClassName);
                if (type == null) return null;

                var method = type.GetMethod(methodName,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                if (method == null)
                {
                    Debug.LogError($"[ReflectionHelper] 泛型方法未找到：{fullClassName}.{methodName}");
                    return null;
                }

                var genericMethod = method.MakeGenericMethod(genericArguments);
                return genericMethod.Invoke(null, parameters);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ReflectionHelper] 泛型方法调用错误：{ex.Message}");
                return null;
            }
        }

        #endregion

        #region 获取/设置静态字段或属性

        /// <summary>
        ///     获取静态字段或属性值（object 版本）。
        ///     使用方式：
        ///     object val = ReflectionHelper.GetStaticFieldOrProperty("命名空间.类名", "成员名");
        /// </summary>
        public static object GetStaticFieldOrProperty(string fullClassName, string memberName)
        {
            var type = GetTypeFromFullName(fullClassName);
            if (type == null) return null;

            // 字段
            var field = type.GetField(memberName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if (field != null) return field.GetValue(null);

            // 属性
            var property = type.GetProperty(memberName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if (property != null) return property.GetValue(null, null);

            Debug.LogError($"[ReflectionHelper] 静态字段 / 属性未找到：{fullClassName}.{memberName}");
            return null;
        }

        /// <summary>
        ///     获取静态字段或属性（泛型版本，更方便直接拿到强类型）。
        ///     使用方式：
        ///     int width = ReflectionHelper.GetStaticFieldOrProperty<int>("UnityEngine.Screen", "width");
        /// </summary>
        public static T GetStaticFieldOrProperty<T>(string fullClassName, string memberName)
        {
            var value = GetStaticFieldOrProperty(fullClassName, memberName);
            if (value == null) return default;

            try
            {
                return (T)value;
            }
            catch
            {
                Debug.LogWarning($"[ReflectionHelper] {fullClassName}.{memberName} 无法转换为类型 {typeof(T).Name}");
                return default;
            }
        }

        /// <summary>
        ///     设置静态字段或属性值。
        ///     使用方式：
        ///     ReflectionHelper.SetStaticFieldOrProperty("命名空间.类名", "成员名", 值);
        /// </summary>
        public static void SetStaticFieldOrProperty(string fullClassName, string memberName, object value)
        {
            var type = GetTypeFromFullName(fullClassName);
            if (type == null) return;

            // 字段
            var field = type.GetField(memberName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if (field != null)
            {
                field.SetValue(null, value);
                return;
            }

            // 属性
            var property = type.GetProperty(memberName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if (property != null && property.CanWrite)
            {
                property.SetValue(null, value, null);
                return;
            }

            Debug.LogError($"[ReflectionHelper] 静态成员未找到或只读：{fullClassName}.{memberName}");
        }

        #endregion

        #region 衍生功能：类型/成员检查与元数据获取

        /// <summary>检查类型是否存在。</summary>
        public static bool TypeExists(string fullClassName)
        {
            return GetTypeFromFullName(fullClassName) != null;
        }

        /// <summary>检查指定静态方法是否存在。</summary>
        public static bool MethodExists(string fullClassName, string methodName)
        {
            var type = GetTypeFromFullName(fullClassName);
            if (type == null) return false;

            return type.GetMethod(methodName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static) != null;
        }

        /// <summary>检查字段或属性是否存在。</summary>
        public static bool MemberExists(string fullClassName, string memberName)
        {
            var type = GetTypeFromFullName(fullClassName);
            if (type == null) return false;

            return type.GetField(memberName,
                       BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static) != null
                   || type.GetProperty(memberName,
                       BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static) != null;
        }

        /// <summary>获取所有静态方法。</summary>
        public static MethodInfo[] GetAllStaticMethods(string fullClassName)
        {
            var type = GetTypeFromFullName(fullClassName);
            return type != null
                ? type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                : Array.Empty<MethodInfo>();
        }

        /// <summary>获取所有静态字段。</summary>
        public static FieldInfo[] GetAllStaticFields(string fullClassName)
        {
            var type = GetTypeFromFullName(fullClassName);
            return type != null
                ? type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                : Array.Empty<FieldInfo>();
        }

        /// <summary>获取所有静态属性。</summary>
        public static PropertyInfo[] GetAllStaticProperties(string fullClassName)
        {
            var type = GetTypeFromFullName(fullClassName);
            return type != null
                ? type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                : Array.Empty<PropertyInfo>();
        }

        /// <summary>获取某个字段/属性的类型。</summary>
        public static Type GetMemberType(string fullClassName, string memberName)
        {
            var type = GetTypeFromFullName(fullClassName);
            if (type == null) return null;

            var field = type.GetField(memberName);
            if (field != null) return field.FieldType;

            var property = type.GetProperty(memberName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            return property?.PropertyType;
        }

        /// <summary>获取静态方法的参数信息。</summary>
        public static ParameterInfo[] GetMethodParameters(string fullClassName, string methodName)
        {
            var type = GetTypeFromFullName(fullClassName);
            if (type == null) return null;

            var method = type.GetMethod(methodName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            return method?.GetParameters();
        }

        /// <summary>
        ///     打印一个类型的所有静态方法 / 字段 / 属性（调试用）。
        /// </summary>
        public static void PrintTypeInfo(string fullClassName)
        {
            var type = GetTypeFromFullName(fullClassName);
            if (type == null)
            {
                Debug.Log($"[ReflectionHelper] 类型未找到：{fullClassName}");
                return;
            }

            Debug.Log($"=== Type Info for {type.FullName} ===");
            Debug.Log($"Assembly: {type.Assembly.GetName().Name}");
            Debug.Log($"Is Static: {type.IsAbstract && type.IsSealed}");

            Debug.Log("\nMethods:");
            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                Debug.Log($"  {method.ReturnType.Name} {method.Name}");

            Debug.Log("\nFields:");
            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                Debug.Log($"  {field.FieldType.Name} {field.Name}");

            Debug.Log("\nProperties:");
            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                Debug.Log($"  {prop.PropertyType.Name} {prop.Name}");
        }

        #endregion

        #region 内部辅助方法

        /// <summary>
        ///     通过完整类名获取 Type（带缓存 + 跨程序集搜索）。
        /// </summary>
        private static Type GetTypeFromFullName(string fullClassName)
        {
            if (string.IsNullOrWhiteSpace(fullClassName))
                return null;

            fullClassName = fullClassName.Trim();

            // 缓存命中
            if (TypeCache.TryGetValue(fullClassName, out var cachedType))
                return cachedType;

            // 1. 先尝试 Type.GetType（对 mscorlib、自定义程序集有时能直接拿到）
            var type = Type.GetType(fullClassName);

            // 2. 不行的话，遍历当前 AppDomain 下所有已加载程序集
            if (type == null)
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                    try
                    {
                        type = assembly.GetType(fullClassName);
                        if (type != null) break;
                    }
                    catch
                    {
                        // 某些动态/损坏程序集可能抛异常，忽略即可
                    }

            if (type != null)
                TypeCache[fullClassName] = type;
            else
                Debug.LogWarning($"[ReflectionHelper] 在已加载的任何程序集里都没有找到类型：{fullClassName}");

            return type;
        }

        /// <summary>
        ///     构建方法缓存 key，用于区分重载（通过参数实际类型名进行粗略区分）。
        /// </summary>
        private static string BuildCacheKey(string fullClassName, string methodName, object[] parameters)
        {
            var cacheKey = $"{fullClassName}.{methodName}";

            if (parameters != null && parameters.Length > 0)
            {
                var paramTypes = new string[parameters.Length];
                for (var i = 0; i < parameters.Length; i++) paramTypes[i] = parameters[i]?.GetType().Name ?? "null";

                cacheKey += $"({string.Join(",", paramTypes)})";
            }

            return cacheKey;
        }

        #endregion

        #region 新增：实例方法调用

        /// <summary>
        /// 调用实例方法，例如：tableObj.Get(id)
        /// </summary>
        public static object InvokeInstanceMethod(object instance, string methodName, params object[] parameters)
        {
            if (instance == null)
            {
                Debug.LogError("[ReflectionHelper] 实例对象为 null");
                return null;
            }

            Type type = instance.GetType();
            MethodInfo method = type.GetMethod(methodName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (method == null)
            {
                Debug.LogError($"[ReflectionHelper] 实例方法未找到：{type.FullName}.{methodName}");
                return null;
            }

            try
            {
                return method.Invoke(instance, parameters);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ReflectionHelper] 调用实例方法 {type.FullName}.{methodName} 发生异常：{ex.Message}");
                return null;
            }
        }

        #endregion

        #region 新增：实例属性/字段读取

        /// <summary>
        /// 读取实例上的属性或字段值（object 版）。
        /// </summary>
        public static object GetProperty(object obj, string memberName)
        {
            if (obj == null)
            {
                Debug.LogError("[ReflectionHelper] 对 null 对象读取属性/字段");
                return null;
            }

            Type type = obj.GetType();

            // 先属性
            PropertyInfo prop = type.GetProperty(memberName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (prop != null && prop.CanRead)
            {
                return prop.GetValue(obj, null);
            }

            // 再字段
            FieldInfo field = type.GetField(memberName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
            {
                return field.GetValue(obj);
            }

            Debug.LogError($"[ReflectionHelper] 实例属性/字段未找到：{type.FullName}.{memberName}");
            return null;
        }

        /// <summary>
        /// 读取实例上的属性或字段值（泛型版，直接拿强类型）。
        /// </summary>
        public static T GetProperty<T>(object obj, string memberName)
        {
            object value = GetProperty(obj, memberName);
            if (value == null) return default;

            try
            {
                return (T)value;
            }
            catch
            {
                Debug.LogWarning($"[ReflectionHelper] {obj.GetType().FullName}.{memberName} 无法转换为类型 {typeof(T).Name}");
                return default;
            }
        }

        #endregion
    }
}