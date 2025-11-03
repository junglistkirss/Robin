using Microsoft.Extensions.DependencyInjection;
using Robin.Abstractions;
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
        DataFacade.RegisterFacadeFactory<JsonNode>(JsonFacades.FromJsonNode);
        DataFacade.RegisterFacadeFactory<JsonArray>(JsonFacades.FromJsonNode);
        DataFacade.RegisterFacadeFactory<JsonObject>(JsonFacades.FromJsonNode);
        ServiceCollection services = [];
        services
            .AddServiceEvaluator()
            .AddJsonAccessors();
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
        IDataFacade found = eval.Resolve(expression, context);
        Assert.NotNull(found);
        Assert.True(found.IsTrue());
        JsonObject foundjson = Assert.IsType<JsonObject>(found.RawValue);
        Assert.Same(json, foundjson);
    }

    [Fact]
    public void ResolveNumberConstant()
    {
        IEvaluator eval = ServiceProvider.GetRequiredService<IEvaluator>();
        JsonObject json = [];
        IExpressionNode expression = new NumberExpressionNode(42);
        DataContext context = new(json, null);
        IDataFacade found = eval.Resolve(expression, context);
        Assert.NotNull(found);
        Assert.True(found.IsTrue());
        Assert.Equal(42, found!.RawValue);
    }

    [Fact]
    public void ResolveLiteralConstant()
    {
        IEvaluator eval = ServiceProvider.GetRequiredService<IEvaluator>();
        JsonObject json = [];
        IExpressionNode expression = new LiteralExpressionNode("test");
        DataContext context = new(json, null);
        IDataFacade found = eval.Resolve(expression, context);
        Assert.NotNull(found);
        Assert.True(found.IsTrue());
        Assert.Equal("test", found.RawValue);
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
        IDataFacade found = eval.Resolve(expression, context);
        Assert.NotNull(found);
        Assert.True(found.IsTrue());
        Assert.Equal("test", found.RawValue?.ToString());
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
        IDataFacade found = eval.Resolve(expression, context);
        Assert.NotNull(found);
        Assert.True(found.IsTrue());
        Assert.Equal("test2", found.RawValue?.ToString());
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
        IDataFacade found = eval.Resolve(expression, context);
        Assert.NotNull(found);
        Assert.True(found.IsTrue());
        Assert.Equal("inner test", found.RawValue?.ToString());
    }
}
