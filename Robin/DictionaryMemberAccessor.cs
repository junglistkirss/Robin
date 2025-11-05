using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Accessors;

internal sealed class DictionaryMemberAccessor : IMemberAccessor
{
    public readonly static DictionaryMemberAccessor Instance = new();
    private DictionaryMemberAccessor() { }
    public bool TryGetMember(string name, [NotNull] out Delegate value)
    {
        value = (object? source) =>
        {
            if (source is IDictionary dict && dict.Contains(name))
            {
                return dict[name];
            }
            return null;
        };
        return true; ;
    }
}