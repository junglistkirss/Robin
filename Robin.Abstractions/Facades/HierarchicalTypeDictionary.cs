namespace Robin.Abstractions.Facades;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

public class HierarchicalTypeDictionary<TValue>
{
    private readonly Dictionary<Type, TValue> _storage = [];

    public bool TryAdd(Type key, TValue value)
    {
        return _storage.TryAdd(key, value);
    }

    public bool TryAdd<T>(TValue value)
    {
        return _storage.TryAdd(typeof(T), value);
    }

    // Get value by searching type hierarchy (most specific to least specific)
    public TValue Get<T>()
    {
        return Get(typeof(T));
    }

    public TValue Get(Type type)
    {
        // 1. Try exact match first
        if (_storage.TryGetValue(type, out TValue? exactMatch))
            return exactMatch;

        // 2. Build hierarchy from most specific to least specific
        IEnumerable<Type> hierarchy = HierarchicalTypeDictionary<TValue>.GetTypeHierarchy(type);

        // 3. Search through hierarchy
        foreach (Type t in hierarchy)
        {
            if (_storage.TryGetValue(t, out TValue? value))
                return value;
        }

        throw new KeyNotFoundException($"No value found for type '{type.Name}' or any of its base types/interfaces");
    }

    // Try get with hierarchy search
    public bool TryGetValue<T>([MaybeNullWhen(false)] out TValue? value)
    {
        return TryGetValue(typeof(T), out value);
    }

    public bool TryGetValue(Type type, [MaybeNullWhen(false)] out TValue? value)
    {
        // Try exact match first
        if (_storage.TryGetValue(type, out value))
            return true;

        // Search hierarchy
        IEnumerable<Type> hierarchy = HierarchicalTypeDictionary<TValue>.GetTypeHierarchy(type);
        foreach (Type t in hierarchy)
        {
            if (_storage.TryGetValue(t, out value))
                return true;
        }

        value = default;
        return false;
    }

    // Get all values where the stored key is assignable from searchType
    public IEnumerable<TValue> GetAllDerived(Type baseType)
    {
        return _storage
            .Where(kvp => baseType.IsAssignableFrom(kvp.Key))
            .Select(kvp => kvp.Value);
    }

    public IEnumerable<TValue> GetAllDerived<T>()
    {
        return GetAllDerived(typeof(T));
    }

    // Build ordered list: [type, base classes (closest first), interfaces]
    private static IEnumerable<Type> GetTypeHierarchy(Type type)
    {

        // Add all base classes (closest first)
        Type? current = type.BaseType;
        while (current != null && current != typeof(object))
        {
            yield return current;
            current = current.BaseType;
        }
        foreach (Type @interface in type.GetInterfaces())
        {
            yield return @interface;
        }
    }

    public IEnumerable<Type> Keys => _storage.Keys;
    public IEnumerable<TValue> Values => _storage.Values;
    public int Count => _storage.Count;
    public bool ContainsKey(Type key) => _storage.ContainsKey(key);
}