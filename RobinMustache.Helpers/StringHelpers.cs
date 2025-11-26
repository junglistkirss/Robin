using RobinMustache.Abstractions.Helpers;

namespace RobinMustache.Helpers;


public static class StringHelpers
{
    public static string Capitalize(string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        return char.ToUpper(value[0]) + value.Substring(1).ToLower();
    }
    
    private static string Lowercase(string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;
        return value.ToLower();
    }
    private static string LowercaseInvariant(string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;
        return value.ToLowerInvariant();
    }
    
    private static string Uppercase(string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;
        return value.ToUpper();
    }
    private static string UppercaseInvariant(string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;
        return value.ToUpperInvariant();
    }
    
    private static char[] ToCharArray(string value)
    {
        if (string.IsNullOrEmpty(value))
            return [];
        return value.ToCharArray();
    }
    
    private static string TrimSpaces(string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;
        return value.Trim();
    }
    private static string TrimChars(string value, string chars)
    {
        if (string.IsNullOrEmpty(value))
            return value;
        return value.Trim(ToCharArray(chars));
    }
    private static object? Trim(object?[] args)
    {
        if (args.Length == 1 && args[0] is string str) return TrimSpaces(str);
        if (args.Length == 2 && args[0] is string str1 && args[1] is string c) return TrimChars(str1, c);
        return null;
    }
    
    private static string TrimStartSpaces(string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;
        return value.TrimStart();
    }
    private static string TrimStartChars(string value, string chars)
    {
        if (string.IsNullOrEmpty(value))
            return value;
        return value.TrimStart(ToCharArray(chars));
    }
    private static object? TrimStart(object?[] args)
    {
        if (args.Length == 1 && args[0] is string str) return TrimStartSpaces(str);
        if (args.Length == 2 && args[0] is string str1 && args[1] is string c) return TrimStartChars(str1, c);
        return null;
    }
    
    private static string TrimEndSpaces(string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;
        return value.TrimEnd();
    }
    private static string TrimEndChars(string value, string chars)
    {
        if (string.IsNullOrEmpty(value))
            return value;
        return value.TrimEnd(ToCharArray(chars));
    }
    private static object? TrimEnd(object?[] args)
    {
        if (args.Length == 1 && args[0] is string str) return TrimEndSpaces(str);
        if (args.Length == 2 && args[0] is string str1 && args[1] is string c) return TrimEndChars(str1, c);
        return null;
    }

    public static void AsGlobalHelpers()
    {
        GlobalHelpers.TryAddFunction(nameof(Lowercase), HelperFactory.ToHelper<string, string>(Lowercase));
        GlobalHelpers.TryAddFunction(nameof(Uppercase), HelperFactory.ToHelper<string, string>(Uppercase));
        GlobalHelpers.TryAddFunction(nameof(LowercaseInvariant), HelperFactory.ToHelper<string, string>(LowercaseInvariant));
        GlobalHelpers.TryAddFunction(nameof(UppercaseInvariant), HelperFactory.ToHelper<string, string>(UppercaseInvariant));
        GlobalHelpers.TryAddFunction(nameof(Capitalize), HelperFactory.ToHelper<string, string>(Capitalize));
        GlobalHelpers.TryAddFunction(nameof(ToCharArray), HelperFactory.ToHelper<string, char[]>(ToCharArray));
        GlobalHelpers.TryAddFunction(nameof(TrimChars), HelperFactory.ToHelper<string, string, string>(TrimChars));
        GlobalHelpers.TryAddFunction(nameof(TrimSpaces), HelperFactory.ToHelper<string, string>(TrimSpaces));
        GlobalHelpers.TryAddFunction(nameof(Trim), Trim);
        GlobalHelpers.TryAddFunction(nameof(TrimStartChars), HelperFactory.ToHelper<string, string, string>(TrimStartChars));
        GlobalHelpers.TryAddFunction(nameof(TrimStartSpaces), HelperFactory.ToHelper<string, string>(TrimStartSpaces));
        GlobalHelpers.TryAddFunction(nameof(TrimStart), TrimStart);
        GlobalHelpers.TryAddFunction(nameof(TrimEndChars), HelperFactory.ToHelper<string, string, string>(TrimEndChars));
        GlobalHelpers.TryAddFunction(nameof(TrimEndSpaces), HelperFactory.ToHelper<string, string>(TrimEndSpaces));
        GlobalHelpers.TryAddFunction(nameof(TrimEnd), TrimEnd);
    }
    public static Helper AddStringHelpers(this Helper helper)
    {
        helper.TryAddFunction(nameof(Lowercase), HelperFactory.ToHelper<string, string>(Lowercase));
        helper.TryAddFunction(nameof(Uppercase), HelperFactory.ToHelper<string, string>(Uppercase));
        helper.TryAddFunction(nameof(LowercaseInvariant), HelperFactory.ToHelper<string, string>(LowercaseInvariant));
        helper.TryAddFunction(nameof(UppercaseInvariant), HelperFactory.ToHelper<string, string>(UppercaseInvariant));
        helper.TryAddFunction(nameof(Capitalize), HelperFactory.ToHelper<string, string>(Capitalize));
        helper.TryAddFunction(nameof(ToCharArray), HelperFactory.ToHelper<string, char[]>(ToCharArray));
        helper.TryAddFunction(nameof(TrimChars), HelperFactory.ToHelper<string, string, string>(TrimChars));
        helper.TryAddFunction(nameof(TrimSpaces), HelperFactory.ToHelper<string, string>(TrimSpaces));
        helper.TryAddFunction(nameof(Trim), Trim);
        helper.TryAddFunction(nameof(TrimStartChars), HelperFactory.ToHelper<string, string, string>(TrimStartChars));
        helper.TryAddFunction(nameof(TrimStartSpaces), HelperFactory.ToHelper<string, string>(TrimStartSpaces));
        helper.TryAddFunction(nameof(TrimStart), TrimStart);
        helper.TryAddFunction(nameof(TrimEndChars), HelperFactory.ToHelper<string, string, string>(TrimEndChars));
        helper.TryAddFunction(nameof(TrimEndSpaces), HelperFactory.ToHelper<string, string>(TrimEndSpaces));
        helper.TryAddFunction(nameof(TrimEnd), TrimEnd);
        return helper;
    }
}
