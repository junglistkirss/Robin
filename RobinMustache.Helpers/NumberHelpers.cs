using RobinMustache.Abstractions.Helpers;

namespace RobinMustache.Helpers;

public static class NumberHelpers
{
    public static double Round(double value, int decimals)
    {
        return Math.Round(value, decimals);
    }
    public static double Ceiling(double value)
    {
        return Math.Ceiling(value);
    }
    public static double Floor(double value)
    {
        return Math.Floor(value);
    }
    public static double Truncate(double value, int decimals)
    {
        double factor = Math.Pow(10, decimals);
        return Math.Floor(value * factor) / factor;
    }

    public static double Min(double a, double b)
    {
        return Math.Min(a, b);
    }
    public static double Max(double a, double b)
    {
        return Math.Max(a, b);
    }

    public static double Add(double a, double b)
    {
        return a + b;
    }
    public static double Subtract(double a, double b)
    {
        return a - b;
    }
    public static double Multiply(double a, double b)
    {
        return a * b;
    }
    public static double Divide(double a, double b)
    {
        if (b == 0)
        {
            throw new ArgumentException("Division by zero is not allowed.");
        }
        return a / b;
    }

    public static double Power(double @base, double exponent)
    {
        return Math.Pow(@base, exponent);
    }

    public static void AsGlobalHelpers()
    {
        GlobalHelpers.TryAddFunction(nameof(Round), HelperFactory.ToHelper<double, int, double>(Round));
        GlobalHelpers.TryAddFunction(nameof(Min), HelperFactory.ToHelper<double, double, double>(Min));
        GlobalHelpers.TryAddFunction(nameof(Max), HelperFactory.ToHelper<double, double, double>(Max));
        GlobalHelpers.TryAddFunction(nameof(Ceiling), HelperFactory.ToHelper<double, double>(Ceiling));
        GlobalHelpers.TryAddFunction(nameof(Floor), HelperFactory.ToHelper<double, double>(Floor));
        GlobalHelpers.TryAddFunction(nameof(Truncate), HelperFactory.ToHelper<double, int, double>(Truncate));
        GlobalHelpers.TryAddFunction(nameof(Add), HelperFactory.ToHelper<double, double, double>(Add));
        GlobalHelpers.TryAddFunction(nameof(Subtract), HelperFactory.ToHelper<double, double, double>(Subtract));
        GlobalHelpers.TryAddFunction(nameof(Multiply), HelperFactory.ToHelper<double, double, double>(Multiply));
        GlobalHelpers.TryAddFunction(nameof(Divide), HelperFactory.ToHelper<double, double, double>(Divide));
        GlobalHelpers.TryAddFunction(nameof(Power), HelperFactory.ToHelper<double, double, double>(Power));
    }
    public static Helper AddNumberHelpers(this Helper helper)
    {
        helper.TryAddFunction(nameof(Round), HelperFactory.ToHelper<double, int, double>(Round));
        helper.TryAddFunction(nameof(Min), HelperFactory.ToHelper<double, double, double>(Min));
        helper.TryAddFunction(nameof(Max), HelperFactory.ToHelper<double, double, double>(Max));
        helper.TryAddFunction(nameof(Ceiling), HelperFactory.ToHelper<double, double>(Ceiling));
        helper.TryAddFunction(nameof(Floor), HelperFactory.ToHelper<double, double>(Floor));
        helper.TryAddFunction(nameof(Truncate), HelperFactory.ToHelper<double, int, double>(Truncate));
        helper.TryAddFunction(nameof(Add), HelperFactory.ToHelper<double, double, double>(Add));
        helper.TryAddFunction(nameof(Subtract), HelperFactory.ToHelper<double, double, double>(Subtract));
        helper.TryAddFunction(nameof(Multiply), HelperFactory.ToHelper<double, double, double>(Multiply));
        helper.TryAddFunction(nameof(Divide), HelperFactory.ToHelper<double, double, double>(Divide));
        helper.TryAddFunction(nameof(Power), HelperFactory.ToHelper<double, double, double>(Power));
        return helper;
    }
}
