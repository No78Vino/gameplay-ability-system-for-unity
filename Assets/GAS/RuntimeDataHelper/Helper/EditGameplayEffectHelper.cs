using System;
using System.Collections.Generic;
using System.Linq;
using GAS.RuntimeDataHelper.Ability;
using GAS.RuntimeDataHelper.GameplayEffect;
using Sirenix.OdinInspector;

namespace GAS.RuntimeDataHelper.Helper
{
    public static class EditGameplayEffectHelper
    {
        private static ValueDropdownItem[] _effectComponentTypeChoices;
        
        public static IEnumerable<ValueDropdownItem> EffectComponentTypeChoices
        {
            get
            {
                _effectComponentTypeChoices ??= GetCachedEffectComponentSubTypes()
                    .Select(type => new ValueDropdownItem(type.Name, type.FullName))
                    .ToArray();
                return _effectComponentTypeChoices;
            }
        }
        
               
        private static IEnumerable<Type> _cachedEffectComponentSubTypes;
        
        public static IEnumerable<Type> GetCachedEffectComponentSubTypes()
        {
            if (_cachedEffectComponentSubTypes != null) return _cachedEffectComponentSubTypes;
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            _cachedEffectComponentSubTypes = assemblies
                .SelectMany(asm => asm.GetTypes())
                .Where(type =>
                    type.IsSubclassOf(typeof(BaseGameplayEffectComponentConfigAsset)) &&
                    !type.IsAbstract &&
                    type.IsDefined(typeof(SerializableAttribute), false)
                )
                .ToList();

            return _cachedEffectComponentSubTypes;
        }
    }
}