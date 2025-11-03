using Microsoft.Extensions.DependencyInjection;
using Robin.Abstractions;
using Robin.Abstractions.Facades;
using Robin.Contracts.Expressions;
using Robin.Contracts.Variables;
using Robin.Evaluator.System.Text.Json;
using System;
using System.Text.Json.Nodes;

namespace Robin.Evaluator.System.Text.Json.tests;


public class ServiceEvaluatorJsonTests
{
    public IServiceProvider ServiceProvider { get; private set; } = default!;

    public ServiceEvaluatorJsonTests()
    {
        DataFacade.RegisterFacadeFactory<JsonNode>(JsonFacades.FromJsonNode);
        DataFacade.RegisterFacadeFactory<JsonArray>(JsonFacades.FromJsonNode);
        DataFacade.RegisterFacadeFactory<JsonObject>(JsonFacades.FromJsonNode);
        ServiceCollection services = [];
        services.AddServiceEvaluator();
        ServiceProvider = services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateOnBuild = true,
            ValidateScopes = true,
        });
    }


}

public class JsonEvaluatorTests
{

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
        IExpressionNode expression = new NumberExpressionNode(42);
        DataContext context = new(json, null);
        IDataFacade found = JsonEvaluator.Instance.Resolve(expression, context);
        Assert.NotNull(found);
        Assert.True(found.IsTrue());
        Assert.Equal(42, found!.RawValue);
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
}
