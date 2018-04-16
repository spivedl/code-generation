using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace CodeGeneration.Extensions
{
    public static class StringExtensions
    {
        public static string ToCamelCase(this string input, string delimiter = "")
        {
            return input
                .Split(new[] { "_" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => char.ToUpperInvariant(s[0]) + s.Substring(1, s.Length - 1))
                .Aggregate(string.Empty, (firstCharUpper, restOfString) => $"{firstCharUpper}{restOfString}{delimiter}")
                .Trim();
        }

        public static string ToUpperDelimited(this string input, string delimiter = " ")
        {
            return Regex.Replace(input, "([a-z](?=[A-Z]|[0-9])|[A-Z](?=[A-Z][a-z]|[0-9])|[0-9](?=[^0-9]))", $"$1{delimiter}");
        }

        public static string ToDelimited(this string input, string delimiter = "_")
        {
            return string.Concat(input.Select((ch, i) => (i > 0) && char.IsUpper(ch) ? delimiter + ch.ToString() : ch.ToString()));
        }

        public static string ToFirstCharLowerCase(this string input)
        {
            return char.ToLowerInvariant(input[0]) + input.Substring(1);
        }

        public static string ToPath(this string input)
        {
            return input.Replace(".", "\\\\");
        }

        public static string ToInterfaceName(this string input, string type = "")
        {
            return $"I{input}{type}";
        }

        public static string ToClassName(this string input, string type = "")
        {
            return $"{input}{type}";
        }

        public static string ToVariableName(this string input, string type = "")
        {
            return $"{input.ToFirstCharLowerCase()}{type}";
        }

        public static string ToPrivateVariableName(this string input, string type = "")
        {
            return $"_{input.ToFirstCharLowerCase()}{type}";
        }
    }
}
