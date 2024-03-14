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
        
        
    }
}