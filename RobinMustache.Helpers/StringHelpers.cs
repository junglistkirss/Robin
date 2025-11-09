using RobinMustache.Abstractions.Helpers;

namespace RobinMustache.Helpers
{
    public static class StringHelpers
    {
        private static object? Lowercase(object?[] args)
        {
            if (args.Length == 1 && args[0] is string str) return str.ToLower();
            return null;
        }
        private static object? Uppercase(object?[] args)
        {
            if (args.Length == 1 && args[0] is string str) return str.ToUpper();
            return null;
        }
        private static object? LowercaseInvariant(object?[] args)
        {
            if (args.Length == 1 && args[0] is string str) return str.ToLowerInvariant();
            return null;
        }
        private static object? UppercaseInvariant(object?[] args)
        {
            if (args.Length == 1 && args[0] is string str) return str.ToUpperInvariant();
            return null;
        }
        private static object? Trim(object?[] args)
        {
            if (args.Length == 1 && args[0] is string str) return str.Trim();
            if (args.Length == 2 && args[0] is string str1 && args[1] is string c) return str1.Trim(c.ToCharArray());
            return null;
        }
        private static object? ToCharArray(object?[] args)
        {
            if (args.Length == 1 && args[0] is string str) return str.ToCharArray();
            return null;
        }
        public static void AsGlobalHelpers()
        {
            GlobalHelpers.TryAddFunction(nameof(Lowercase), Lowercase);
            GlobalHelpers.TryAddFunction(nameof(Uppercase),Uppercase);
            GlobalHelpers.TryAddFunction(nameof(LowercaseInvariant), LowercaseInvariant);
            GlobalHelpers.TryAddFunction(nameof(UppercaseInvariant), UppercaseInvariant);
            GlobalHelpers.TryAddFunction(nameof(Trim), Trim);
            GlobalHelpers.TryAddFunction(nameof(ToCharArray), ToCharArray);
        }

        public static Helper AddStringGloabHelpers( this Helper helper)
        {
            helper.TryAddFunction(nameof(Lowercase), Lowercase);
            helper.TryAddFunction(nameof(Uppercase), Uppercase);
            helper.TryAddFunction(nameof(LowercaseInvariant), LowercaseInvariant);
            helper.TryAddFunction(nameof(UppercaseInvariant), UppercaseInvariant);
            helper.TryAddFunction(nameof(Trim), Trim);
            helper.TryAddFunction(nameof(ToCharArray), ToCharArray);
            return helper;
        }
    }
}