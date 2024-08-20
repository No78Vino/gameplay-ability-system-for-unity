using System.Linq;

namespace GAS.Runtime
{
    public static class AttributeValidator
    {
        /// <summary>
        /// 校验属性名称是否有效.
        /// <example>
        /// 使用示例：
        /// <code>
        /// [ValidateInput("@AttributeValidator.IsValidAttributeName($value)", "属性名无效")]
        /// public string AttributeName;
        /// </code>
        /// </example>
        /// </summary>
        public static bool IsValidAttributeName(string attributeName)
        {
            return !string.IsNullOrWhiteSpace(attributeName) && ReflectionHelper.AttributeNames.Any(x => x == attributeName);
        }
    }
}