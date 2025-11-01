using Robin.Contracts.Expressions;
using Robin.Contracts.Nodes;
using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Robin.Contracts.Context;


public interface IEvaluator
{
    bool TryResolve(IExpressionNode expression, object? data, out object? value);
    bool IsCollection(object? value, [NotNullWhen(true)] out IEnumerable? collection);
    bool IsTrue(object? value);
}



