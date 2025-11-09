using Robin.Abstractions.Accessors;
using System.Collections;

namespace Robin.Internals;

internal sealed class DictionaryMemberAccessor : BaseMemberAccessor<IDictionary>, IMemberDelegateAccessor
{
    public readonly static DictionaryMemberAccessor Instance = new();
    private DictionaryMemberAccessor() { }
    public bool TryGetMember(string name, out Delegate value)
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

    public override bool TryGetMember(IDictionary obj, string name, out object? value)
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