using RobinMustache.Abstractions.Helpers;

namespace RobinMustache.Helpers;

public static class CurrencyHelpers
{
    public static string FormatCurrency(double amount, string currencySymbol, int decimals)
    {
        return string.Format("{0}{1:N" + decimals + "}", currencySymbol, amount);
    }

    public static double ConvertCurrency(double amount, double exchangeRate)
    {
        return amount * exchangeRate;
    }

    public static void AsGlobalHelpers()
    {
        GlobalHelpers.TryAddFunction(nameof(FormatCurrency), HelperFactory.ToHelper<double, string, int, string>(FormatCurrency));
        GlobalHelpers.TryAddFunction(nameof(ConvertCurrency), HelperFactory.ToHelper<double, double, double>(ConvertCurrency));
    }
    public static Helper AddCurrencyHelpers(this Helper helper)
    {
        helper.TryAddFunction(nameof(FormatCurrency), HelperFactory.ToHelper<double, string, int, string>(FormatCurrency));
        helper.TryAddFunction(nameof(ConvertCurrency), HelperFactory.ToHelper<double, double, double>(ConvertCurrency));
        return helper;
    }
}
