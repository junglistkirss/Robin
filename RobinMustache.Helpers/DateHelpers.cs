using RobinMustache.Abstractions.Helpers;

namespace RobinMustache.Helpers;

public static class DateHelpers
{
    private static string FormatDate(DateTime date, string format)
    {
        return date.ToString(format);
    }
    private static TimeSpan DateDiff(DateTime date1, DateTime date2)
    {
        return (date1 - date2);
    }
    
    private static int DiffDays(DateTime date1, DateTime date2)
    {
        return DateDiff(date1, date2).Days;
    }

    private static double DiffTotalDays(DateTime date1, DateTime date2)
    {
        return DateDiff(date1, date2).TotalDays;
    }


    private static int DiffHours(DateTime date1, DateTime date2)
    {
        return DateDiff(date1, date2).Hours;
    }

    private static double DiffTotalHours(DateTime date1, DateTime date2)
    {
        return DateDiff(date1, date2).TotalHours;
    }
    private static int DiffMinutes(DateTime date1, DateTime date2)
    {
        return DateDiff(date1, date2).Minutes;
    }

    private static double DiffTotalMinutes(DateTime date1, DateTime date2)
    {
        return DateDiff(date1, date2).TotalMinutes;
    }
    private static int DiffSeconds(DateTime date1, DateTime date2)
    {
        return DateDiff(date1, date2).Seconds;
    }

    private static double DiffTotalSeconds(DateTime date1, DateTime date2)
    {
        return DateDiff(date1, date2).TotalSeconds;
    }

    private static int DiffMilliseconds(DateTime date1, DateTime date2)
    {
        return DateDiff(date1, date2).Milliseconds;
    }

    private static double DiffTotalMilliseconds(DateTime date1, DateTime date2)
    {
        return DateDiff(date1, date2).TotalMilliseconds;
    }
    public static void AsGlobalHelpers()
    {
        GlobalHelpers.TryAddFunction(nameof(FormatDate), HelperFactory.ToHelper<DateTime, string, string>(FormatDate));
        GlobalHelpers.TryAddFunction(nameof(DateDiff), HelperFactory.ToHelper<DateTime, DateTime, TimeSpan>(DateDiff));
        GlobalHelpers.TryAddFunction(nameof(DiffDays), HelperFactory.ToHelper<DateTime, DateTime, int>(DiffDays));
        GlobalHelpers.TryAddFunction(nameof(DiffTotalDays), HelperFactory.ToHelper<DateTime, DateTime, double>(DiffTotalDays));
        GlobalHelpers.TryAddFunction(nameof(DiffHours), HelperFactory.ToHelper<DateTime, DateTime, int>(DiffHours));
        GlobalHelpers.TryAddFunction(nameof(DiffTotalHours), HelperFactory.ToHelper<DateTime, DateTime, double>(DiffTotalHours));
        GlobalHelpers.TryAddFunction(nameof(DiffMinutes), HelperFactory.ToHelper<DateTime, DateTime, int>(DiffMinutes));
        GlobalHelpers.TryAddFunction(nameof(DiffTotalMinutes), HelperFactory.ToHelper<DateTime, DateTime, double>(DiffTotalMinutes));
        GlobalHelpers.TryAddFunction(nameof(DiffSeconds), HelperFactory.ToHelper<DateTime, DateTime, int>(DiffSeconds));
        GlobalHelpers.TryAddFunction(nameof(DiffTotalSeconds), HelperFactory.ToHelper<DateTime, DateTime, double>(DiffTotalSeconds));
        GlobalHelpers.TryAddFunction(nameof(DiffMilliseconds), HelperFactory.ToHelper<DateTime, DateTime, int>(DiffMilliseconds));
        GlobalHelpers.TryAddFunction(nameof(DiffTotalMilliseconds), HelperFactory.ToHelper<DateTime, DateTime, double>(DiffTotalMilliseconds));
    }
    public static Helper AddDateHelpers(this Helper helper)
    {
        helper.TryAddFunction(nameof(FormatDate), HelperFactory.ToHelper<DateTime, string, string>(FormatDate));
        helper.TryAddFunction(nameof(DateDiff), HelperFactory.ToHelper<DateTime, DateTime, TimeSpan>(DateDiff));
        helper.TryAddFunction(nameof(DiffDays), HelperFactory.ToHelper<DateTime, DateTime, int>(DiffDays));
        helper.TryAddFunction(nameof(DiffTotalDays), HelperFactory.ToHelper<DateTime, DateTime, double>(DiffTotalDays));
        helper.TryAddFunction(nameof(DiffHours), HelperFactory.ToHelper<DateTime, DateTime, int>(DiffHours));
        helper.TryAddFunction(nameof(DiffTotalHours), HelperFactory.ToHelper<DateTime, DateTime, double>(DiffTotalHours));
        helper.TryAddFunction(nameof(DiffMinutes), HelperFactory.ToHelper<DateTime, DateTime, int>(DiffMinutes));
        helper.TryAddFunction(nameof(DiffTotalMinutes), HelperFactory.ToHelper<DateTime, DateTime, double>(DiffTotalMinutes));
        helper.TryAddFunction(nameof(DiffSeconds), HelperFactory.ToHelper<DateTime, DateTime, int>(DiffSeconds));
        helper.TryAddFunction(nameof(DiffTotalSeconds), HelperFactory.ToHelper<DateTime, DateTime, double>(DiffTotalSeconds));
        helper.TryAddFunction(nameof(DiffMilliseconds), HelperFactory.ToHelper<DateTime, DateTime, int>(DiffMilliseconds));
        helper.TryAddFunction(nameof(DiffTotalMilliseconds), HelperFactory.ToHelper<DateTime, DateTime, double>(DiffTotalMilliseconds));
        return helper;
    }

}
