using System;
using System.Collections.Generic;
using System.Linq;
using GAS.Runtime;
using GAS.RuntimeDataHelper.Helper;
using GAS.RuntimeWithECS.Cue;
using Sirenix.OdinInspector;

namespace GAS.Editor
{
    public static class EditCueHelper
    {
        private static Dictionary<Type, Type> _cachedCueParamTypeToCueParamConfigTypeMap;
        private static Dictionary<string, Type> _cachedCueToCueParamConfigTypeMap;
        private static IEnumerable<Type> _cachedCueParamConfigTypes;
        private static IEnumerable<Type> _cachedCueTypes;
        
        
        public static IEnumerable<Type> GetCachedCueTypes()
        {
            if (_cachedCueTypes != null) return _cachedCueTypes;
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            _cachedCueTypes = assemblies
                .SelectMany(asm => asm.GetTypes())
                .Where(type =>
                    type.IsSubclassOf(typeof(GameplayCueBase)) &&
                    !type.IsAbstract
                )
                .ToList();

            return _cachedCueTypes;
        }
        
        public static IEnumerable<string> GetCachedCueTypeNames()
        {
            var types = GetCachedCueTypes();
            return types
                .Select(type => type.Name)
                .Where(name => !string.IsNullOrEmpty(name))
                .ToList();
        }
        #region Instant Cue
        
        private static ValueDropdownItem[] _instantCueChoices;
        private static IEnumerable<Type> _cachedInstantCueTypes;

        public static IEnumerable<ValueDropdownItem> InstantCueChoices
        {
            get
            {
                if (_instantCueChoices == null || _instantCueChoices.Length == 0)
                {
                    var types = GetCachedInstantCueTypes();
                    _instantCueChoices = types
                        .Select(type => new ValueDropdownItem(type.Name, type.FullName))
                        .ToArray();
                }

                return _instantCueChoices;
            }
        }
        
        public static IEnumerable<Type> GetCachedInstantCueTypes()
        {
            if (_cachedInstantCueTypes != null) return _cachedInstantCueTypes;
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            _cachedInstantCueTypes = assemblies
                .SelectMany(asm => asm.GetTypes())
                .Where(type =>
                    type.IsSubclassOf(typeof(GameplayCueBase)) &&
                    !type.IsAbstract
                )
                .ToList();

            return _cachedInstantCueTypes;
        }
        
        #endregion
    }
}