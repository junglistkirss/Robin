using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Accessors;

internal sealed class DictionaryMemberAccessor : IMemberAccessor
{
    public bool TryGetMember(object? source, string name, [MaybeNullWhen(false)] out object? value)
    {
        if(source is IDictionary dict && dict.Contains(name))
        {
            value = dict[name];
            return true;
        }
        value = null;
        return false;
    }
}