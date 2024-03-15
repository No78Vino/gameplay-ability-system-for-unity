#if UNITY_EDITOR
namespace GAS.Editor
{
    using System.Collections.Generic;
    using System;
    using System.Reflection;
    using Runtime;

    public static class AbilityEditorUtil
    {
        public static List<string> GetAbilityClassNames()
        {
            var classNames = new List<string>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                try
                {
                    var types = assembly.GetTypes();
                    foreach (var type in types)
                    {
                        if (type.IsSubclassOf(typeof(AbstractAbility)))
                        {
                            classNames.Add(type.FullName);
                        }
                    }
                }
                catch (ReflectionTypeLoadException)
                {
                    continue;
                }
            }
            return classNames;
        }
    }
}
#endif