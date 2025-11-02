using Robin.Abstractions;
using Robin.Abstractions.Facades;
using Robin.Contracts.Expressions;
using Robin.Contracts.Variables;
using System.Text.Json.Nodes;

namespace Robin.Evaluator.System.Text.Json.tests;


public class JsonEvaluatorTests
{
    //public JsonEvaluatorTests()
    //{
    //    Facades.RegisterFacadeFactory<JsonNode>(JsonFacades.FromJsonNode);
    //    Facades.RegisterFacadeFactory<JsonArray>(JsonFacades.FromJsonNode);
    //    Facades.RegisterFacadeFactory<JsonObject>(JsonFacades.FromJsonNode);
    //}

    [Fact]
    public void ResolveThis()
    {
        JsonObject json = [];
        VariablePath path = AccessPathParser.Parse(".");
        Assert.IsType<ThisAccessor>(Assert.Single(path.Segments));
        IExpressionNode expression = new IdentifierExpressionNode(path);
        DataContext context = new(json, null);
        IDataFacade found = JsonEvaluator.Instance.Resolve(expression, context);
        Assert.NotNull(found);
        Assert.True(found.IsTrue());
        JsonObject foundjson = Assert.IsType<JsonObject>(found.RawValue);
        Assert.Same(json, foundjson);
    }

    [Fact]
    public void ResolveNumberConstant()
    {
        JsonObject json = [];
        IExpressionNode expression = new NumberExpressionNode(0.5);
        DataContext context = new(json, null);
        IDataFacade found = JsonEvaluator.Instance.Resolve(expression, context);
        Assert.NotNull(found);
        Assert.True(found.IsTrue());
        Assert.Equal(0.5, found!.RawValue);
    }

    [Fact]
    public void ResolveLiteralConstant()
    {
        JsonObject json = [];
        IExpressionNode expression = new LiteralExpressionNode("test");
        DataContext context = new(json, null);
        IDataFacade found = JsonEvaluator.Instance.Resolve(expression, context);
        Assert.NotNull(found);
        Assert.True(found.IsTrue());
        Assert.Equal("test", found.RawValue);
    }

    [Fact]
    public void ResolveMember()
    {
        JsonObject json = new()
        {
            ["prop"] = "test"
        };
        IExpressionNode expression = new IdentifierExpressionNode(AccessPathParser.Parse("prop"));
        DataContext context = new(json, null);
        IDataFacade found = JsonEvaluator.Instance.Resolve(expression, context);
        Assert.NotNull(found);
        Assert.True(found.IsTrue());
        Assert.Equal("test", found.RawValue?.ToString());
    }


    [Fact]
    public void ResolveIndex()
    {
        JsonObject json = new()
        {
            ["prop"] = new JsonArray { "test", "test2" }
        };
        IExpressionNode expression = new IdentifierExpressionNode(AccessPathParser.Parse("prop[1]"));
        DataContext context = new(json, null);
        IDataFacade found = JsonEvaluator.Instance.Resolve(expression, context);
        Assert.NotNull(found);
        Assert.True(found.IsTrue());
        Assert.Equal("test2", found.RawValue?.ToString());
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
        DataContext context = new(json, null);
        IDataFacade found = JsonEvaluator.Instance.Resolve(expression, context);
        Assert.NotNull(found);
        Assert.True(found.IsTrue());
        Assert.Equal("inner test", found.RawValue?.ToString());
    }
    [Fact]
    public void ResolveKeyMemberPath()
    {
        JsonObject json = new()
        {
            ["key"] = "inner",
            ["prop"] = new JsonObject()
            {
                ["inner"] = "inner prop test",
            }
        };
        IExpressionNode expression = new IdentifierExpressionNode(AccessPathParser.Parse("prop[key]"));
        DataContext context = new(json, null);
        IDataFacade found = JsonEvaluator.Instance.Resolve(expression, context);
        Assert.NotNull(found);
        Assert.True(found.IsTrue());
        Assert.Equal("inner prop test", found.RawValue?.ToString());
    }

    [Fact]
    public void ResolveKeyMemberPathParentStack()
    {
        JsonObject json = new()
        {
            ["key"] = "inner",
            ["prop"] = new JsonObject()
            {
                ["prop"] = new JsonObject()
                {
                    ["inner"] = "inner prop test",
                }
            }
        };
        IExpressionNode expression = new IdentifierExpressionNode(AccessPathParser.Parse("prop.prop[key]"));
        DataContext context = new(json, null);
        IDataFacade found = JsonEvaluator.Instance.Resolve(expression, context);
        Assert.NotNull(found);
        Assert.True(found.IsTrue());
        Assert.Equal("inner prop test", found.RawValue?.ToString()!);
    }


    [Fact]
    public void ResolveParent()
    {
        JsonObject jsonParent = [];
        JsonObject json = [];
        IExpressionNode that = new IdentifierExpressionNode(AccessPathParser.Parse("~"));
        DataContext context = new(json, new(jsonParent, null));
        IDataFacade found = JsonEvaluator.Instance.Resolve(that, context);
        Assert.NotNull(found);
        Assert.True(found.IsTrue());
        Assert.Same(jsonParent, found.RawValue);
    }
}
