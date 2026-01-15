using System;
using System.Collections.Generic;
using System.Linq;
using GAS.Runtime;
using Sirenix.OdinInspector;

namespace GAS.Editor
{
    public enum AbilityEditComponent
    {
        [LabelText("消耗[GE]")]
        Cost,
        
        [LabelText("冷却[GE]")]
        Cooldown,
        
        [LabelText("描述标签")]
        AssetTags,
        
        [LabelText("拥有【任意】Tag的Ability会被取消")]
        CancelAbilityWithTags,
        
        [LabelText("拥有【任意】Tag的Ability会被阻止")]
        BlockAbilityWithTags,
        
        [LabelText("激活后获得的Tag")]
        ActivationOwnedTags,
        
        [LabelText("激活需要的Tag")]
        ActivationRequiredTags,
        
        [LabelText("阻止激活的Tag")]
        ActivationBlockedTags,
    }
    
    public static class EditorAbilityHelper
    {
        public static IEnumerable<AbilityEditComponent> ComponentTypes()
        {
            return new[]
            {
                AbilityEditComponent.Cost,
                AbilityEditComponent.Cooldown,
                AbilityEditComponent.AssetTags,
                AbilityEditComponent.CancelAbilityWithTags,
                AbilityEditComponent.BlockAbilityWithTags,
                AbilityEditComponent.ActivationOwnedTags,
                AbilityEditComponent.ActivationRequiredTags,
                AbilityEditComponent.ActivationBlockedTags,
            };
        }


        private static IEnumerable<Type> _cachedAbilityLogicTypes;
        public static IEnumerable<Type> GetCachedAbilityLogicTypes()
        {
            if (_cachedAbilityLogicTypes != null) return _cachedAbilityLogicTypes;
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            _cachedAbilityLogicTypes = assemblies
                .SelectMany(asm => asm.GetTypes())
                .Where(type =>
                    type.IsSubclassOf(typeof(AbilityLogicBase)) &&
                    !type.IsAbstract
                )
                .ToList();

            return _cachedAbilityLogicTypes;
        }
        
        private static Dictionary<string, Type> _cachedAbilityToParamTypeMap;
        public static Dictionary<string, Type> AbilityToAbilityParamTypeMap()
        {
            if (_cachedAbilityToParamTypeMap != null)
                return _cachedAbilityToParamTypeMap;
            var abilityLogicTypes = GetCachedAbilityLogicTypes() ;
            _cachedAbilityToParamTypeMap = new Dictionary<string, Type>();
            foreach (var derivedType in abilityLogicTypes)
            {
                var baseType = derivedType.BaseType; // 获取基类类型

                if (baseType != null && baseType.IsGenericType)
                {
                    var genericBaseDef = baseType.GetGenericTypeDefinition();

                    // 确认是否是所需的基类泛型定义
                    if (genericBaseDef == typeof(AbilityLogicBase<>))
                    {
                        // 获取实际使用的泛型参数
                        var genericArgs = baseType.GetGenericArguments();
                        var paramType = genericArgs[0];

                        if (derivedType.FullName != null) _cachedAbilityToParamTypeMap[derivedType.Name] = paramType;
                    }
                }
            }

            return _cachedAbilityToParamTypeMap;
        }
        
        public static IExParameterBase CreateAbilityParameter(string type, List<object> paramData = null)
        {
            var map = AbilityToAbilityParamTypeMap();
            if (!map.TryGetValue(type, out var abilityParamConfigType))
                throw new KeyNotFoundException($"未找到类型为 {type} 的 IExParameterBase 类型。");
            var abilityParamEditor = (IExParameterBase)Activator.CreateInstance(abilityParamConfigType);
            if (paramData != null) abilityParamEditor.DecodeExcelData(paramData);
            return abilityParamEditor;
        }
        
        public static IEnumerable<string> GetCachedAbilityLogicTypesName()
        {
            var types = GetCachedAbilityLogicTypes();
            return types
                .Select(type => type.Name)
                .Where(name => !string.IsNullOrEmpty(name))
                .ToList();
        }
    }
}