using Microsoft.Extensions.DependencyInjection;
using RobinMustache.Evaluator.System.Text.Json;
using RobinMustache.Extensions;

namespace RobinMustache.Specs.Tests;

public abstract class BaseMustacheTests
{
    public IServiceProvider ServiceProvider { get; private set; } = default!;

    public BaseMustacheTests()
    {
        ServiceCollection services = [];
        services
            .AddServiceEvaluator()
            .AddJsonAccessors()
            .AddStringRenderer();
        ServiceProvider = services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateOnBuild = true,
            ValidateScopes = true,
        });
    }
}
