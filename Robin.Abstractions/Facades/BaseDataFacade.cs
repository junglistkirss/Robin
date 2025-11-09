using Robin.Abstractions.Iterators;

namespace Robin.Abstractions.Facades;

public abstract class BaseDataFacade<T> : IDataFacade<T>
{
    bool IDataFacade.IsTrue(object? obj) => obj is T typed && IsTrue(typed);
   public abstract bool IsTrue(T obj);
    bool IDataFacade.IsCollection(object? obj, out IIterator? collection)
    {
        if (obj is T typed && IsCollection(typed, out IIterator? typedCollection))
        {
            collection = typedCollection;
            return true;
        }
        collection = null;
        return false;
    }
    public abstract bool IsCollection(T obj, out IIterator? collection);
}