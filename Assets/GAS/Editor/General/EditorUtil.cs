#if UNITY_EDITOR
namespace GAS.Editor
{
    using UnityEditor;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using UnityEngine;
    
    public static class EditorUtil
    {
        public static bool IsValidClassName(string input)
        {
            // 使用正则表达式匹配规则
            // 类名必须以字母、下划线或@开头，并且后续可以是字母、下划线、@或数字
            var pattern = @"^[a-zA-Z_@][a-zA-Z_@0-9]*$";

            // 使用 Regex.IsMatch 方法进行匹配
            return Regex.IsMatch(input, pattern);
        }
        
        public static string MakeValidIdentifier(string name)
        {
            // Replace '.' with '_'
            name = name.Replace('.', '_');

            // If starts with a digit, add '_' at the beginning
            if (char.IsDigit(name[0])) name = "_" + name;

            // Ensure the identifier is valid
            return string.Join("",
                name.Split(
                    new[]
                    {
                        ' ', '-', '.', ':', ',', '!', '?', '#', '$', '%', '^', '&', '*', '(', ')', '[', ']', '{', '}',
                        '/', '\\', '|'
                    }, StringSplitOptions.RemoveEmptyEntries));
        }
        
        /// <summary>
        /// Keep the string list unique.
        /// </summary>
        /// <param name="inputList"></param>
        /// <returns></returns>
        public static List<string> RemoveDuplicates(List<string> inputList)
        {
            HashSet<string> seen = new HashSet<string>();
            List<string> uniqueList = new List<string>();

            foreach (string value in inputList)
            {
                if (!seen.Contains(value))
                {
                    seen.Add(value);
                    uniqueList.Add(value);
                }
            }

            return uniqueList;
        }
        
        public static List<Type> FindAllTypesInheritingFrom(Type baseType,Type ban = null)
        {
            List<Type> results = new List<Type>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                try
                {
                    var types = assembly.GetTypes();

                    foreach (var type in types)
                    {
                        if (type.IsSubclassOf(baseType) && !type.IsAbstract)
                        {
                            if(ban == null || !type.IsSubclassOf(ban))
                                results.Add(type);
                        }
                    }
                }
                catch (ReflectionTypeLoadException)
                {
                    continue;
                }
            }

            return results;
        }
        
        public static List<Type> GetScriptableObjectTypes(Type baseType)
        {
            List<Type> scriptableObjectTypes = new List<Type>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                try
                {
                    var types = assembly.GetTypes();

                    foreach (var type in types)
                    {
                        if (type.IsSubclassOf(baseType) && type.IsDefined(typeof(CreateAssetMenuAttribute), false))
                        {
                            scriptableObjectTypes.Add(type);
                        }
                    }
                }
                catch (ReflectionTypeLoadException)
                {
                    continue;
                }
            }

            return scriptableObjectTypes;
        }
        
        public static void Separator()
        {
            GUILayout.Space(5);
            GUILayout.Box("------------------------------------------------------------------------------------------------------------");
            GUILayout.Space(5);
        }
        
        public static List<T> FindAssetsByType<T>(string folderPath) where T : ScriptableObject
        {
            List<T> assets = new List<T>();

            string[] guids = AssetDatabase.FindAssets("", new string[] { folderPath });

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);

                if (asset != null)
                {
                    assets.Add(asset);
                }
            }

            return assets;
        }
    }
}
#endif