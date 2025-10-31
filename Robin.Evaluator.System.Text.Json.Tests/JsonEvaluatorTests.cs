using System.Text.Json.Nodes;
using Robin.Contracts.Expressions;
using Robin.Contracts.Variables;
using Robin.Evaluator.System.Text.Json;

namespace Robin.Evaluator.System.Text.Json.tests;


public class JsonEvaluatorTests
{
    [Fact(Skip = "?")]
    public void ResolveThis()
    {
        JsonObject json = [];
        AccesorPath path = AccessPathParser.Parse(".");
        Assert.IsType<ThisAccessor>( Assert.Single(path.Segments));
        IExpressionNode expression = new IdentifierExpressionNode(path);
        bool resolved = JsonEvaluator.Instance.TryResolve(expression, json, out object? found);
        Assert.True(resolved);
        Assert.Same(json, found);
    }

    [Fact]
    public void ResolveNumberConstant()
    {
        JsonObject json = [];
        IExpressionNode expression = new NumberExpressionNode(0.5);
        bool resolved = JsonEvaluator.Instance.TryResolve(expression, json, out object? found);
        Assert.True(resolved);
        Assert.Equal(0.5, found);
    }
    [Fact]
    public void ResolveLiteralConstant()
    {
        JsonObject json = [];
        IExpressionNode expression = new LiteralExpressionNode("test");
        bool resolved = JsonEvaluator.Instance.TryResolve(expression, json, out object? found);
        Assert.True(resolved);
        Assert.Equal("test", found);
    }

    [Fact]
    public void ResolveMember()
    {
        JsonObject json = new()
        {
            ["prop"] = "test"
        };
        IExpressionNode expression = new IdentifierExpressionNode(AccessPathParser.Parse("prop"));
        bool resolved = JsonEvaluator.Instance.TryResolve(expression, json, out object? found);
        Assert.True(resolved);
        Assert.Equal("test", found?.ToString());
    }

    [Fact]
    public void ResolveMemberPath()
    {
        JsonObject json = new()
        {
            ["prop"] = new JsonObject()
            {
                ["inner"] = "inner test"
            }
        };
        IExpressionNode expression = new IdentifierExpressionNode(AccessPathParser.Parse("prop.inner"));
        bool resolved = JsonEvaluator.Instance.TryResolve(expression, json, out object? found);
        Assert.True(resolved);
        Assert.Equal("inner test", found?.ToString());
    }

    [Fact(Skip = "overflow")]
    public void ResolveParent()
    {
        JsonObject json = [];
        IExpressionNode that = new IdentifierExpressionNode(AccessPathParser.Parse("~"));
        bool resolved = JsonEvaluator.Instance.TryResolve(that, json, out object? found);
        Assert.False(resolved);
        Assert.Null(found);
    }
}
