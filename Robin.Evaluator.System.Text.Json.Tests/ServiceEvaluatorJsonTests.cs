using Microsoft.Extensions.DependencyInjection;
using Robin.Abstractions;
using Robin.Abstractions.Context;
using Robin.Abstractions.Extensions;
using Robin.Abstractions.Facades;
using Robin.Contracts.Expressions;
using Robin.Contracts.Variables;
using System.Text.Json.Nodes;

namespace Robin.Evaluator.System.Text.Json.tests;

public class ServiceEvaluatorJsonTests
{
    public IServiceProvider ServiceProvider { get; private set; } = default!;

    public ServiceEvaluatorJsonTests()
    {
        //DataFacade.RegisterFacadeFactory<JsonNode>(JsonFacades.FromJsonNode);
        //DataFacade.RegisterFacadeFactory<JsonArray>(JsonFacades.FromJsonNode);
        //DataFacade.RegisterFacadeFactory<JsonObject>(JsonFacades.FromJsonNode);

        ServiceCollection services = [];
        services
            .AddServiceEvaluator()
            .AddJsonAccessors()
            .AddDataFacade<JsonNode>(JsonFacades.FromJsonNode)
            .AddDataFacade<JsonArray>(JsonFacades.FromJsonNode)
            .AddDataFacade<JsonObject>(JsonFacades.FromJsonNode);
        ServiceProvider = services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateOnBuild = true,
            ValidateScopes = true,
        });
    }

    [Fact]
    public void ResolveThis()
    {
        IEvaluator eval = ServiceProvider.GetRequiredService<IEvaluator>();
        JsonObject json = [];
        VariablePath path = VariableParser.Parse(".");
        Assert.IsType<ThisSegment>(Assert.Single(path.Segments));
        IExpressionNode expression = new IdentifierExpressionNode(path);
        DataContext context = new(json, null);
        object? rawValue = eval.Resolve(expression, context, out IDataFacade facade);
        Assert.NotNull(rawValue);
        Assert.True(facade.IsTrue(rawValue));
        JsonObject rawValuejson = Assert.IsType<JsonObject>(rawValue);
        Assert.Same(json, rawValuejson);
    }

    [Fact]
    public void ResolveNumberConstant()
    {
        IEvaluator eval = ServiceProvider.GetRequiredService<IEvaluator>();
        JsonObject json = [];
        IExpressionNode expression = new IndexExpressionNode(42);
        DataContext context = new(json, null);
        object? rawValue = eval.Resolve(expression, context, out IDataFacade facade);
        Assert.NotNull(rawValue);
        Assert.True(facade.IsTrue(rawValue));
        Assert.Equal(42, rawValue);
    }

    [Fact]
    public void ResolveLiteralConstant()
    {
        IEvaluator eval = ServiceProvider.GetRequiredService<IEvaluator>();
        JsonObject json = [];
        IExpressionNode expression = new LiteralExpressionNode("test");
        DataContext context = new(json, null);
        object? rawValue = eval.Resolve(expression, context, out IDataFacade facade);
        Assert.NotNull(rawValue);
        Assert.True(facade.IsTrue(rawValue));
        Assert.Equal("test", rawValue);
    }

    [Fact]
    public void ResolveMember()
    {
        IEvaluator eval = ServiceProvider.GetRequiredService<IEvaluator>();
        JsonObject json = new()
        {
            ["prop"] = "test"
        };
        IExpressionNode expression = new IdentifierExpressionNode(VariableParser.Parse("prop"));
        DataContext context = new(json, null);
        object? rawValue = eval.Resolve(expression, context, out IDataFacade facade);
        Assert.NotNull(rawValue);
        Assert.True(facade.IsTrue(rawValue));
        Assert.Equal("test", rawValue?.ToString());
    }


    [Fact]
    public void ResolveIndex()
    {
        IEvaluator eval = ServiceProvider.GetRequiredService<IEvaluator>();
        JsonObject json = new()
        {
            ["prop"] = new JsonArray { "test", "test2" }
        };
        IExpressionNode expression = new IdentifierExpressionNode(VariableParser.Parse("prop[1]"));
        DataContext context = new(json, null);
        object? rawValue = eval.Resolve(expression, context, out IDataFacade facade);
        Assert.NotNull(rawValue);
        Assert.True(facade.IsTrue(rawValue));
        Assert.Equal("test2", rawValue?.ToString());
    }

    [Fact]
    public void ResolveMemberPath()
    {
        IEvaluator eval = ServiceProvider.GetRequiredService<IEvaluator>();
        JsonObject json = new()
        {
            ["prop"] = new JsonObject()
            {
                ["inner"] = "inner test"
            }
        };
        IExpressionNode expression = new IdentifierExpressionNode(VariableParser.Parse("prop.inner"));
        DataContext context = new(json, null);
        object? rawValue = eval.Resolve(expression, context, out IDataFacade facade);
        Assert.NotNull(rawValue);
        Assert.True(facade.IsTrue(rawValue));
        Assert.Equal("inner test", rawValue?.ToString());
    }
}
