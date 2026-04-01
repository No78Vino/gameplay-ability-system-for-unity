using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GAS.Editor
{
    internal static class GASCenterParseHelper
    {
        // Tolerant parser for Excel cells that may contain formats like:
        // "1;2;3", "1,2,3", "all=1,2;any=0;none=0", or JSON-like text.
        private static readonly Regex IntRegex = new(@"-?\d+", RegexOptions.Compiled);

        public static List<int> ParseIntListLoose(string raw, bool positiveOnly = true)
        {
            var result = new List<int>();
            if (string.IsNullOrWhiteSpace(raw))
            {
                return result;
            }

            var matches = IntRegex.Matches(raw);
            foreach (Match match in matches)
            {
                if (!int.TryParse(match.Value, out var value))
                {
                    continue;
                }

                if (positiveOnly && value <= 0)
                {
                    continue;
                }

                result.Add(value);
            }

            return result;
        }
    }
}
