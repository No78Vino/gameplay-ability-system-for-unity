using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GAS.General
{
    public static class TypeUtil
    {
        public static Type[] GetAllSonTypesOf(Type parentType)
        {
            List<Type> sonTypes = new List<Type>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                try
                {
                    var types = assembly.GetTypes();

                    sonTypes.AddRange(types.Where(type => type.IsSubclassOf(parentType) && !type.IsAbstract));
                }
                catch (ReflectionTypeLoadException)
                {
                }
            }

            return sonTypes.ToArray();
        }

        public static Type FindTypeInAllAssemblies(string typeName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    Type type = assembly.GetType(typeName);
                    if (type != null)
                    {
                        // 如果找到了类型，返回它
                        return type;
                    }
                }
                catch (ReflectionTypeLoadException)
                {
                    // 忽略因无法加载类型而引发的异常
                }
            }

            // 如果没有找到类型，返回null
            return null;
        }

        /// <summary>
        /// 获取类型的继承链
        /// </summary>
        public static string[] GetInheritanceChain(this Type type, bool fullName = true)
        {
            var inheritanceChain = new List<string>();
            var currentType = type;

            while (currentType != null)
            {
                var name = fullName ? currentType.FullName : currentType.Name;
                inheritanceChain.Add(name);
                currentType = currentType.BaseType;
            }

            return inheritanceChain.ToArray();
        }
    }
}