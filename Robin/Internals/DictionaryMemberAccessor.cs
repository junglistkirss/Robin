using Robin.Abstractions.Accessors;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Internals;

internal sealed class DictionaryMemberAccessor : IMemberDelegateAccessor, IMemberAccessor<IDictionary>
{
    public readonly static DictionaryMemberAccessor Instance = new();
    private DictionaryMemberAccessor() { }
    public bool TryGetMember(string name, [NotNull] out Delegate value)
    {
        value = (object? source) =>
        {
            if (source is IDictionary dic && dic.Contains(name))
            {
                return dic[name];
            }
            return null;
        };
        return true; ;
    }

    public bool TryGetMember(IDictionary obj, string name, out object? value)
    {
        if (obj is not null && obj.Contains(name))
        {
            value = obj[name];
            return true;
        }
        value = null;
        return true;
    }
}