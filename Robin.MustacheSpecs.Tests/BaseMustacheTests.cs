using Microsoft.Extensions.DependencyInjection;
using Robin.Abstractions.Extensions;
using Robin.Evaluator.System.Text.Json;

namespace Robin.MustacheSpecs.Tests;

public abstract class BaseMustacheTests
{
    public IServiceProvider ServiceProvider { get; private set; } = default!;

    public BaseMustacheTests()
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
}
