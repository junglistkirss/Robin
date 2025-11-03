using Robin.Abstractions;
using Robin.Abstractions.Facades;
using Robin.Contracts.Expressions;
using Robin.Contracts.Variables;
using System.CodeDom.Compiler;
using System.Text.Json.Nodes;

namespace Robin.Evaluator.GeneratedAccessor.Tests;

public class GeneratedAccessorTests
{

    // [Fact]
    // public void ResolveThis()
    // {
    //     JsonObject json = [];
    //     VariablePath path = AccessPathParser.Parse(".");
    //     Assert.IsType<ThisAccessor>(Assert.Single(path.Segments));
    //     IExpressionNode expression = new IdentifierExpressionNode(path);
    //     DataContext context = new(json, null);
    //     IDataFacade found = JsonEvaluator.Instance.Resolve(expression, context);
    //     Assert.NotNull(found);
    //     Assert.True(found.IsTrue());
    //     JsonObject foundjson = Assert.IsType<JsonObject>(found.RawValue);
    //     Assert.Same(json, foundjson);
    // }
}
