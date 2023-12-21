using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GAS.Editor
{
    public static class EditorUtil
    {
        public static bool IsValidClassName(string input)
        {
            // 使用正则表达式匹配规则
            // 类名必须以字母、下划线或@开头，并且后续可以是字母、下划线、@或数字
            var pattern = @"^[a-zA-Z_@][a-zA-Z_@0-9]*$";

            // 使用 Regex.IsMatch 方法进行匹配
            return Regex.IsMatch(input, pattern);
        }
        
        public static string MakeValidIdentifier(string name)
        {
            // Replace '.' with '_'
            name = name.Replace('.', '_');

            // If starts with a digit, add '_' at the beginning
            if (char.IsDigit(name[0])) name = "_" + name;

            // Ensure the identifier is valid
            return string.Join("",
                name.Split(
                    new[]
                    {
                        ' ', '-', '.', ':', ',', '!', '?', '#', '$', '%', '^', '&', '*', '(', ')', '[', ']', '{', '}',
                        '/', '\\', '|'
                    }, StringSplitOptions.RemoveEmptyEntries));
        }
        
        /// <summary>
        /// Keep the string list unique.
        /// </summary>
        /// <param name="inputList"></param>
        /// <returns></returns>
        public static List<string> RemoveDuplicates(List<string> inputList)
        {
            HashSet<string> seen = new HashSet<string>();
            List<string> uniqueList = new List<string>();

            foreach (string value in inputList)
            {
                if (!seen.Contains(value))
                {
                    seen.Add(value);
                    uniqueList.Add(value);
                }
            }

            return uniqueList;
        }
    }
}