using Microsoft.Extensions.DependencyInjection;
using Robin.Evaluator.System.Text.Json;
using Robin.Extensions;

namespace Robin.MustacheSpecs.Tests;

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
