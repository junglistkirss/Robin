using Robin.Generators.Accessor;

namespace Robin.Evaluator.GeneratedAccessor.Tests;

[GenerateAccessor]
internal record TestModel
{
    public int IntValue { get; set; }
    public long? LongValue { get; set; }
    public double? DoubleValue { get; set; }
    public float? FloatValue { get; set; }
    public decimal? DecimalValue { get; set; }
    public string StringValue { get; set; } = default!;
    public char? CharValue { get; set; } = default!;
    public bool? BooleanValue { get; set; } = default!;
    public TestSubModel? SubModel { get; set; } = default!;
}


[GenerateAccessor]
internal record TestSubModel
{
    public DateTime? DateTimeValue { get; set; }
    public string[] StringCollection { get; set; } = default!;
    public IDictionary<string, object?> DictionaryCollection { get; set; } = default!;
    public IDictionary<ImplicitKey, object?> DictionaryImplicitKeyCollection { get; set; } = default!;
}

public readonly struct ImplicitKey(string value)
{
    public string Value { get; } = value;
    public static implicit operator string(ImplicitKey key) => key.Value;
    public static implicit operator ImplicitKey(string key) => new(key);
}