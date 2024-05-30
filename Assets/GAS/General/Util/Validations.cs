using System.Text.RegularExpressions;

namespace GAS.General.Validation
{
    public readonly struct ValidationResult
    {
        public readonly bool IsValid;
        public readonly string Message;

        public ValidationResult(bool isValid, string message)
        {
            IsValid = isValid;
            Message = message;
        }

        public static readonly ValidationResult Valid = new ValidationResult(true, null);
        public static ValidationResult Invalid(string message) => new ValidationResult(false, message);
    }

    public delegate ValidationResult ValidationDelegate(string input);

    public static class Validations
    {
        // https://learn.microsoft.com/zh-cn/dotnet/csharp/fundamentals/coding-style/identifier-names
        // 可以在标识符上使用 @ 前缀来声明与 C# 关键字匹配的标识符。 @ 不是标识符名称的一部分。 例如，@if 声明名为 if 的标识符。
        // 因此类似 @123abc 这样的标识符是不合法的。因为抛开@之后, 它实际上是以数字开头。
        private const string VariableNamePattern = @"^@?[a-zA-Z_][a-zA-Z0-9_]*$";
        public static readonly Regex VariableNameRegex = new Regex(VariableNamePattern);

        public static ValidationResult ValidateVariableName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return ValidationResult.Invalid("The name is empty!");

            return VariableNameRegex.IsMatch(name)
                ? ValidationResult.Valid
                : ValidationResult.Invalid($"The name(\"{name}\") is invalid!");
        }

        public static bool IsValidVariableName(string name)
        {
            return ValidateVariableName(name).IsValid;
        }
    }
}