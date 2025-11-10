using Microsoft.Extensions.DependencyInjection;
using RobinMustache.Abstractions.Context;
using RobinMustache.Abstractions.Expressions;
using RobinMustache.Abstractions.Facades;
using RobinMustache.Abstractions.Variables;
using RobinMustache.Extensions;
using System.Text.Json.Nodes;

namespace RobinMustache.Evaluator.System.Text.Json.tests;

public class JsonEvaluatorTests
{
    public IServiceProvider ServiceProvider { get; private set; } = default!;

    public JsonEvaluatorTests()
    {
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
        IJsonEvaluator eval = ServiceProvider.GetRequiredService<IJsonEvaluator>();
        JsonObject json = [];
        VariablePath path = VariableParser.Parse(".");
        Assert.IsType<ThisSegment>(Assert.Single(path.Segments));
        IExpressionNode expression = new IdentifierExpressionNode(path);
        using (DataContext.Push(json))
        {
            object? rawValue = eval.Resolve(expression, DataContext.Current, out IDataFacade facade);
            Assert.NotNull(rawValue);
            Assert.True(facade.IsTrue(rawValue));
            JsonObject foundjson = Assert.IsType<JsonObject>(rawValue);
            Assert.Same(json, foundjson);
        }
    }

    [Fact]
    public void ResolveNumberConstant()
    {
        IJsonEvaluator eval = ServiceProvider.GetRequiredService<IJsonEvaluator>();
        JsonObject json = [];
        IExpressionNode expression = new IndexExpressionNode(42);
        using (DataContext.Push(json))
        {
            object? rawValue = eval.Resolve(expression, DataContext.Current, out IDataFacade facade);
            Assert.NotNull(rawValue);
            Assert.True(facade.IsTrue(rawValue));
            Assert.Equal(42, rawValue);
        }
    }

    [Fact]
    public void ResolveLiteralConstant()
    {
        IJsonEvaluator eval = ServiceProvider.GetRequiredService<IJsonEvaluator>();
        JsonObject json = [];
        IExpressionNode expression = new LiteralExpressionNode("test");
        using (DataContext.Push(json))
        {
            object? rawValue = eval.Resolve(expression, DataContext.Current, out IDataFacade facade);
            Assert.NotNull(rawValue);
            Assert.True(facade.IsTrue(rawValue));
            Assert.Equal("test", rawValue);
        }
    }

    [Fact]
    public void ResolveMember()
    {
        IJsonEvaluator eval = ServiceProvider.GetRequiredService<IJsonEvaluator>();
        JsonObject json = new()
        {
            ["prop"] = "test"
        };
        IExpressionNode expression = new IdentifierExpressionNode(VariableParser.Parse("prop"));
        using (DataContext.Push(json))
        {
            object? rawValue = eval.Resolve(expression, DataContext.Current, out IDataFacade facade);
            Assert.NotNull(rawValue);
            Assert.True(facade.IsTrue(rawValue));
            Assert.Equal("test", rawValue?.ToString());
        }
    }


    [Fact]
    public void ResolveIndex()
    {
        IJsonEvaluator eval = ServiceProvider.GetRequiredService<IJsonEvaluator>();
        JsonObject json = new()
        {
            ["prop"] = new JsonArray { "test", "test2" }
        };
        IExpressionNode expression = new IdentifierExpressionNode(VariableParser.Parse("prop[1]"));
        using (DataContext.Push(json))
        {
            object? rawValue = eval.Resolve(expression, DataContext.Current, out IDataFacade facade);
            Assert.NotNull(rawValue);
            Assert.True(facade.IsTrue(rawValue));
            Assert.Equal("test2", rawValue?.ToString());
        }
    }

    [Fact]
    public void ResolveMemberPath()
    {
        IJsonEvaluator eval = ServiceProvider.GetRequiredService<IJsonEvaluator>();
        JsonObject json = new()
        {
            ["prop"] = new JsonObject()
            {
                ["inner"] = "inner test"
            }
        };
        IExpressionNode expression = new IdentifierExpressionNode(VariableParser.Parse("prop.inner"));
        using (DataContext.Push(json))
        {
            object? rawValue = eval.Resolve(expression, DataContext.Current, out IDataFacade facade);
            Assert.NotNull(rawValue);
            Assert.True(facade.IsTrue(rawValue));
            Assert.Equal("inner test", rawValue?.ToString());
        }
    }
}
