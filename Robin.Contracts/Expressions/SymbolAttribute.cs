namespace Robin.Contracts.Expressions;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
internal sealed class SymbolAttribute : Attribute
{
    public string Text { get; }
    public SymbolAttribute(string text) => Text = text;
}

