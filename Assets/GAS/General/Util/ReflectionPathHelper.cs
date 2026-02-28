using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace GAS.General
{
    public static class ReflectionPathHelper
    {
        /// <summary>
        /// 通用：从根静态类开始，按成员名链式获取最终对象。
        /// rootFullClassName 比如 "GAS.Runtime.XLuban"
        /// memberPath 例如：["Tables", "TbgameplayTags", "DataMap", "Keys"]
        /// </summary>
        public static object GetNestedMemberValue(string rootFullClassName, params string[] memberPath)
        {
            if (string.IsNullOrEmpty(rootFullClassName) || memberPath == null || memberPath.Length == 0)
                return null;

            // 1. 拿根类型（静态类）
            Type currentType = GetTypeFromFullName(rootFullClassName);
            if (currentType == null)
            {
                Debug.LogError($"[ReflectionPathHelper] 未找到类型：{rootFullClassName}");
                return null;
            }

            object currentValue = null;

            for (int i = 0; i < memberPath.Length; i++)
            {
                string memberName = memberPath[i];

                // 第一层：在静态类上拿静态成员
                if (i == 0)
                {
                    // 先按属性找
                    var prop = currentType.GetProperty(
                        memberName,
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

                    if (prop != null && prop.CanRead)
                    {
                        currentValue = prop.GetValue(null);
                        if (currentValue == null) return null;
                        currentType = currentValue.GetType();
                        continue;
                    }

                    // 再按字段找
                    var field = currentType.GetField(
                        memberName,
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

                    if (field != null)
                    {
                        currentValue = field.GetValue(null);
                        if (currentValue == null) return null;
                        currentType = currentValue.GetType();
                        continue;
                    }

                    Debug.LogError($"[ReflectionPathHelper] 静态成员未找到：{rootFullClassName}.{memberName}");
                    return null;
                }
                else
                {
                    // 后续层：在实例上拿实例成员
                    if (currentValue == null)
                    {
                        Debug.LogError($"[ReflectionPathHelper] 第 {i} 层对象为 null，成员名：{memberName}");
                        return null;
                    }

                    // 先属性
                    var prop = currentType.GetProperty(
                        memberName,
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                    if (prop != null && prop.CanRead)
                    {
                        currentValue = prop.GetValue(currentValue);
                        if (currentValue == null) return null;
                        currentType = currentValue.GetType();
                        continue;
                    }

                    // 再字段
                    var field = currentType.GetField(
                        memberName,
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                    if (field != null)
                    {
                        currentValue = field.GetValue(currentValue);
                        if (currentValue == null) return null;
                        currentType = currentValue.GetType();
                        continue;
                    }

                    Debug.LogError($"[ReflectionPathHelper] 成员未找到：{currentType.FullName}.{memberName}");
                    return null;
                }
            }

            return currentValue;
        }

        /// <summary>
        /// 通过完整类名获取 Type（简单跨程序集搜索）。
        /// 如：GAS.Runtime.XLuban
        /// </summary>
        private static Type GetTypeFromFullName(string fullClassName)
        {
            if (string.IsNullOrWhiteSpace(fullClassName))
                return null;

            // 直接 Type.GetType
            var type = Type.GetType(fullClassName);
            if (type != null) return type;

            // 遍历当前 AppDomain 的所有程序集
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    type = asm.GetType(fullClassName);
                    if (type != null) return type;
                }
                catch
                {
                }
            }

            return null;
        }
        
        
        
        
        
        
        
        /// <summary>
        /// 通用：GAS.Runtime.XLuban.Tables.{tableName}.Get(id).{memberName}
        /// </summary>
        public static T GetRawMemberById<T>(
            string tableName,     // 如 "TbgameplayTags"
            object id,            // Get(id) 的 id
            string memberName,    // 如 "Name"、"Desc" 等
            string rootClass)
        {
            // 1. XLuban.Tables
            object tablesObj = ReflectionHelper.GetStaticFieldOrProperty(rootClass, "Tables");
            if (tablesObj == null)
            {
                Debug.LogError("[XLubanGenericHelper] XLuban.Tables 为 null 或未找到");
                return default;
            }

            // 2. Tables.tableName（这里假设都是属性，如果有字段可以像上面那样兼容一下）
            Type tablesType = tablesObj.GetType();
            object tableObj = null;

            var tableProp = tablesType.GetProperty(tableName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (tableProp != null && tableProp.CanRead)
            {
                tableObj = tableProp.GetValue(tablesObj, null);
            }
            else
            {
                var tableField = tablesType.GetField(tableName,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (tableField != null)
                    tableObj = tableField.GetValue(tablesObj);
            }

            if (tableObj == null)
            {
                Debug.LogError($"[XLubanGenericHelper] Tables.{tableName} 为 null 或未找到");
                return default;
            }

            // 3. 调用表的 Get(id)
            object rowObj = ReflectionHelper.InvokeInstanceMethod(tableObj, "Get", id);
            if (rowObj == null)
            {
                Debug.LogWarning($"[XLubanGenericHelper] {tableName}.Get({id}) 返回 null");
                return default;
            }

            // 4. 从行对象取 memberName 属性/字段
            return ReflectionHelper.GetProperty<T>(rowObj, memberName);
        }
    }
    
    public static class LinqReflectionHelper
    {
        /// <summary>
        /// 通用：对一个 IEnumerable 源对象调用 LINQ 的 ToList&lt;T&gt; 扩展方法。
        /// 你只需要提供元素类型 T。
        /// </summary>
        public static object InvokeToList(Type elementType, object enumerableObj)
        {
            if (elementType == null || enumerableObj == null)
                return null;

            // 1. 拿到 System.Linq.Enumerable 类型
            Type enumerableType = typeof(Enumerable);

            // 2. 找到名字为 "ToList" 且为泛型方法的 MethodInfo
            MethodInfo toListGeneric = null;
            foreach (var m in enumerableType.GetMethods(
                         BindingFlags.Public | BindingFlags.Static))
            {
                if (m.Name != "ToList") continue;
                if (!m.IsGenericMethodDefinition) continue;

                var ps = m.GetParameters();
                if (ps.Length == 1)
                {
                    // 参数类型应该是 IEnumerable<TSource>
                    toListGeneric = m;
                    break;
                }
            }

            if (toListGeneric == null)
            {
                Debug.LogError("[LinqReflectionHelper] 未找到 Enumerable.ToList<T> 方法");
                return null;
            }

            // 3. 构造具体的泛型版本 ToList<T>
            MethodInfo toListConcrete = toListGeneric.MakeGenericMethod(elementType);

            // 4. 调用：注意第一个参数是 null（静态方法），第二个是参数数组
            object result = toListConcrete.Invoke(null, new object[] { enumerableObj });
            return result;
        }

        /// <summary>
        /// 强类型包装：返回 List&lt;T&gt;。
        /// </summary>
        public static List<T> InvokeToList<T>(IEnumerable<T> enumerable)
        {
            if (enumerable == null) return null;
            var obj = InvokeToList(typeof(T), enumerable);
            return obj as List<T>;
        }
    }
    
    
    public static class GasChoiceRawAccessor
    {
        /// <summary>
        /// 通用：通过路径获取 Dictionary 的 Keys，并对 Keys 调用 ToList&lt;TKey&gt;。
        /// 适用于：GAS.Runtime.XLuban.Tables.xxx.DataMap.Keys.ToList()
        /// </summary>
        /// <typeparam name="TKey">字典 Key 的类型</typeparam>
        /// <param name="rootFullClassName">根静态类完整名，如 "GAS.Runtime.XLuban"</param>
        /// <param name="memberPathToKeys">
        /// 从根静态类到 Keys 的路径（不含 ToList），
        /// 比如：new[] { "Tables", "TbgameplayTags", "DataMap", "Keys" }
        /// </param>
        /// <returns>List&lt;TKey&gt;，如果任何一步失败则返回 null</returns>
        public static List<TKey> GetDictionaryKeysToList<TKey>(
            string rootFullClassName,
            params string[] memberPathToKeys
        )
        {
            // 1. 先通过通用路径反射拿到 Keys 对象
            object keysObj = ReflectionPathHelper.GetNestedMemberValue(
                rootFullClassName,
                memberPathToKeys
            );

            if (keysObj == null)
            {
                Debug.LogError("[GasChoiceRawAccessor] Keys 对象为 null，请检查路径或生成代码是否存在。");
                return null;
            }

            // 2. 确认它是 IEnumerable<TKey>，然后调用 ToList<TKey>()
            // 这里用 is IEnumerable<TKey> 让你在类型不一致时早点发现问题
            if (keysObj is IEnumerable<TKey> typedEnumerable)
            {
                return LinqReflectionHelper.InvokeToList(typedEnumerable);
            }
            else
            {
                Debug.LogError(
                    $"[GasChoiceRawAccessor] Keys 对象类型不兼容 IEnumerable<{typeof(TKey).Name}>，实际类型：{keysObj.GetType().FullName}");
                return null;
            }
        }


        public static List<int> GetLubanTableKeysToList(string tableName)
        {
            return GetDictionaryKeysToList<int>(
                "GAS.Runtime.XLuban",
                "Tables",
                tableName,
                "DataMap",
                "Keys"
            );
        }
        
        public static List<int> GetGameplayTagsKeysToList() => GetLubanTableKeysToList("TbgameplayTags");
        public static List<int> GetTimelineAbilityIDs() => GetLubanTableKeysToList("TbtimelineAbility");
        public static List<int> GetAbilityIDs() => GetLubanTableKeysToList("Tbability");
        public static List<int> GetGameplayCueIDs() => GetLubanTableKeysToList("TbgameplayCue");
        public static List<int> GetGameplayEffectIDs() => GetLubanTableKeysToList("TbgameplayEffect");
        public static List<int> GetAttributeIDs() => GetLubanTableKeysToList("Tbattribute");
        public static List<int> GetAttributeSetIDs() => GetLubanTableKeysToList("TbattributeSet");
        public static List<int> GetMmcIDs() => GetLubanTableKeysToList("Tbmmc");
        

        public static string GetGameplayTagName(int id)
        {
            return ReflectionPathHelper.GetRawMemberById<string>(
                "TbgameplayTags",
                id,
                "Name",
                "GAS.Runtime.XLuban"
            );
        }
        
        public static string GetTimelineAbilityName(int id)
        {
            return ReflectionPathHelper.GetRawMemberById<string>(
                "TbtimelineAbility",
                id,
                "Name",
                "GAS.Runtime.XLuban"
            );
        }  
        
        public static string GetGameplayEffectName(int id)
        {
            return ReflectionPathHelper.GetRawMemberById<string>(
                "TbgameplayEffect",
                id,
                "Name",
                "GAS.Runtime.XLuban"
            );
        }
        
        public static string GetGameplayCueName(int id)
        {
            return ReflectionPathHelper.GetRawMemberById<string>(
                "TbgameplayCue",
                id,
                "Name",
                "GAS.Runtime.XLuban"
            );
        }
        
        public static string GetAttrSetNameByCode(int id)  
        {  
            return ReflectionPathHelper.GetRawMemberById<string>(  
                "TbattributeSet",  
                id,  
                "Name",  
                "GAS.Runtime.XLuban"  
            );  
        }  
  
        public static string GetAttributeNameByCode(int id)  
        {  
            return ReflectionPathHelper.GetRawMemberById<string>(  
                "Tbattribute",  
                id,  
                "Name",  
                "GAS.Runtime.XLuban"  
            );  
        }
    }
}