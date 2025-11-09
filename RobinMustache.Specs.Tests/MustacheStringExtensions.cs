using System.Text.RegularExpressions;

namespace RobinMustache.MustacheSpecs.Tests;

internal static partial class MustacheStringExtensions
{
    public static bool EqualsIgnoringWhitespace(this string a, string b)
    {
        string normA = Spaces().Replace(a ?? "", "");
        string normB = Spaces().Replace(b ?? "", "");
        return string.Equals(normA, normB, StringComparison.Ordinal);
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex Spaces();
}
