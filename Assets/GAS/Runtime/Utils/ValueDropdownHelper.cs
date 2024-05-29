#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;

namespace GAS.Runtime
{
    public static class ValueDropdownHelper
    {
        /// <summary>
        /// 显示一个下列列表, 包含所有的属性集名称.
        /// <example>
        /// 使用示例：
        /// <code>
        /// [ValueDropdown("@ValueDropdownHelper.AttributeSetChoices", IsUniqueList = true)]
        /// public string AttributeSet;
        /// </code>
        /// </example>
        /// </summary>
        public static IEnumerable<string> AttributeSetChoices => ReflectionHelper.AttributeSetNames;

        /// <summary>
        /// 显示一个下拉列表，包含所有的属性名称.
        /// <example>
        /// 使用示例：
        /// <code>
        /// [ValueDropdown("@ValueDropdownHelper.AttributeChoices", IsUniqueList = true)]
        /// public string Attribute;
        /// </code>
        /// </example>
        /// </summary>
        public static IEnumerable<string> AttributeChoices => ReflectionHelper.AttributeNames;

        private static ValueDropdownItem[] _gameplayTagChoices;

        /// <summary>
        /// 显示一个下拉列表，包含所有的GameplayTag.
        /// <example>
        /// 使用示例：
        /// <code>
        /// [ValueDropdown("@ValueDropdownHelper.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        /// public GameplayTag[] Tags;
        /// </code>
        /// </example>
        /// </summary>
        public static IEnumerable<ValueDropdownItem> GameplayTagChoices
        {
            get
            {
                _gameplayTagChoices ??= ReflectionHelper.GameplayTags
                    .Select(gameplayTag => new ValueDropdownItem(gameplayTag.Name, gameplayTag))
                    .ToArray();
                return _gameplayTagChoices;
            }
        }
    }
}
#endif