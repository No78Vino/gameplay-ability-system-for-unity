using System.Text.RegularExpressions;

namespace GAS.Editor.Validation
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
        private const string VariableNamePattern = @"^[a-zA-Z_][a-zA-Z0-9_]*$";
        public static readonly Regex VariableNameRegex = new Regex(VariableNamePattern);

        public static ValidationResult ValidateVariableName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return ValidationResult.Invalid("The name is empty!");

            return VariableNameRegex.IsMatch(name)
                ? ValidationResult.Valid
                : ValidationResult.Invalid($"The name(\"{name}\") is invalid!");
        }
    }
}