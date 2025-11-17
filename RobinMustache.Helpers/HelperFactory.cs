using RobinMustache.Abstractions.Helpers;

namespace RobinMustache.Helpers;

public static class HelperFactory
{
    public delegate TResult TypedHelper<T1, TResult>(T1 value);
    public delegate TResult TypedHelper<T1, T2, TResult>(T1 value1, T2 value2);
    public delegate TResult TypedHelper<T1, T2, T3, TResult>(T1 value1, T2 value2, T3 value3);
    public delegate TResult TypedHelper<T1, T2, T3, T4, TResult>(T1 v1, T2 v2, T3 v3, T4 v4);
    public delegate TResult TypedHelper<T1, T2, T3, T4, T5, TResult>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5);

    public delegate bool TypeCaster<TResult>(object? value, out TResult result);

    private static bool CastType<TResult>(this TypeCaster<TResult>? typeCaster, object? value, out TResult result)
    {
        if (typeCaster is not null && typeCaster(value, out result))
            return true;
        if (value is TResult t)
        {
            result = t;
            return true;
        }
        result = default!;
        return false;
    }

    public static Helper.Function ToHelper<T1, TResult>(
        this TypedHelper<T1, TResult> func,
        TypeCaster<T1>? cast1 = null
    )
    {
        return args =>
        {
            if (args.Length == 1 
                && cast1.CastType(args[0], out T1 t1))
                return func(t1);
            return null;
        };
    }

    public static Helper.Function ToHelper<T1, T2, TResult>(
        this TypedHelper<T1, T2, TResult> func,
        TypeCaster<T1>? cast1 = null,
        TypeCaster<T2>? cast2 = null)
    {
        return args =>
        {
            if (args.Length == 2 
                && cast1.CastType(args[0], out T1 p1) 
                && cast2.CastType(args[1], out T2 p2))
                return func(p1, p2);
            return null;
        };
    }

    public static Helper.Function ToHelper<T1, T2, T3, TResult>(
        this TypedHelper<T1, T2, T3, TResult> func,
        TypeCaster<T1>? cast1 = null,
        TypeCaster<T2>? cast2 = null,
        TypeCaster<T3>? cast3 = null)
    {
        return args =>
        {
            if (args.Length == 3 
                && cast1.CastType(args[0], out T1 p1)
                && cast2.CastType(args[1], out T2 p2)
                && cast3.CastType(args[2], out T3 p3))
                return func(p1, p2, p3);
            return null;
        };
    }

    public static Helper.Function ToHelper<T1, T2, T3, T4, TResult>(
        this TypedHelper<T1, T2, T3, T4, TResult> func,
        TypeCaster<T1>? cast1 = null,
        TypeCaster<T2>? cast2 = null,
        TypeCaster<T3>? cast3 = null,
        TypeCaster<T4>? cast4 = null)
    {
        return args =>
        {
            if (args.Length == 4 
                && cast1.CastType(args[0], out T1 p1)
                && cast2.CastType(args[1], out T2 p2)
                && cast3.CastType(args[2], out T3 p3)
                && cast4.CastType(args[3], out T4 p4))
                return func(p1, p2, p3, p4);
            return null;
        };
    }

    public static Helper.Function ToHelper<T1, T2, T3, T4, T5, TResult>(
        this TypedHelper<T1, T2, T3, T4, T5, TResult> func,
        TypeCaster<T1>? cast1 = null,
        TypeCaster<T2>? cast2 = null,
        TypeCaster<T3>? cast3 = null,
        TypeCaster<T4>? cast4 = null,
        TypeCaster<T5>? cast5 = null)
    {
        return args =>
        {
            if (args.Length == 5
                && cast1.CastType(args[0], out T1 p1)
                && cast2.CastType(args[1], out T2 p2)
                && cast3.CastType(args[2], out T3 p3)
                && cast4.CastType(args[3], out T4 p4)
                && cast5.CastType(args[4], out T5 p5))
                return func(p1, p2, p3, p4, p5);
            return null;
        };
    }
}
