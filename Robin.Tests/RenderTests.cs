using Microsoft.Extensions.DependencyInjection;
using Robin.Abstractions.Extensions;
using Robin.Abstractions.Helpers;
using Robin.Contracts.Nodes;
using Robin.Extensions;
using Robin.Helpers;
using System.Collections;
using System.Collections.Immutable;

namespace Robin.tests;

public class RenderTests
{
    public IServiceProvider ServiceProvider { get; private set; } = default!;

    public RenderTests()
    {
        StringHelpers.AsGlobalHelpers();
        ServiceCollection services = [];
        services
            .AddServiceEvaluator()
            .AddStringRenderer(helperConfig: h =>
            {
                h.TryAddFunction("Length", (object?[] args) =>
                {
                    if (args.Length == 1)
                    {
                        if (args[0] is Array arr) return arr.Length;
                        if (args[0] is IList list) return list.Count;
                        if (args[0] is IDictionary dic) return dic.Count;
                    }
                    return null;
                });
            })
            .AddMemberDelegateAccessor<TestSample>(static (string member, out Delegate value) =>
            {
                value = member switch
                {
                    "Name" => (Func<TestSample, string?>)(x => x.Name),
                    "Age" => (Func<TestSample, int>)(x => x.Age),
                    _ => throw new InvalidDataException($"Member does not exists : {member}"),
                };
                return true;
            })
            .AddMemberDelegateAccessor<ParentTestSample>(static (string member, out Delegate value) =>
            {
                value = member.ToLowerInvariant() switch
                {
                    "alias" => (Func<ParentTestSample, string?>)(x => x.Alias),
                    "nested" => (Func<ParentTestSample, TestSample?>)(x => x.Nested),
                    _ => throw new InvalidDataException($"Member does not exists : {member}"),
                };
                return true;
            });
        ServiceProvider = services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateOnBuild = true,
            ValidateScopes = true,
        });
    }

    [Fact]
    public void Test_Render_SimpleTemplate()
    {
        IStringRenderer renderer = ServiceProvider.GetRequiredService<IStringRenderer>();
        TestSample sample = new() { Name = "Alice", Age = 30 };
        ImmutableArray<INode> template = "Name: {{ Name }}, Age: {{ Age }}".AsSpan().Parse();
        string result = renderer.Render(template, sample);
        Assert.Equal("Name: Alice, Age: 30", result);
    }

    [Fact]
    public void ParentTest_Render_SimpleTemplate()
    {
        IStringRenderer renderer = ServiceProvider.GetRequiredService<IStringRenderer>();
        ParentTestSample parent = new() { Alias = "Bob", Nested = new TestSample { Name = "Alice", Age = 30 } };
        ImmutableArray<INode> template = "Name: {{ Alias }}, Nested: {{ nested.Name }}".AsSpan().Parse();
        string result = renderer.Render(template, parent);
        Assert.Equal("Name: Bob, Nested: Alice", result);
    }

    [Fact]
    public void ParentTest_Render_ImmutableArray()
    {
        IStringRenderer renderer = ServiceProvider.GetRequiredService<IStringRenderer>();

        ImmutableArray<INode> template = "{{#.}}Name: {{ Alias }}, {{# nested }}Nested: {{ Name }}{{/ nested }}{{/.}}".AsSpan().Parse();
        ImmutableArray<ParentTestSample> data = [.. Enumerable.Range(0, 10).Select(i => new ParentTestSample { Alias = "Bob", Nested = new TestSample { Name = "Alice", Age = i } })];
        string result = renderer.Render(template, data);
        Assert.Contains("Name: Bob, Nested: Alice", result);
    }

    [Fact]
    public void ParentTest_Render_Array()
    {
        IStringRenderer renderer = ServiceProvider.GetRequiredService<IStringRenderer>();

        ImmutableArray<INode> template = "{{#.}}Name: {{ Alias }}, {{# nested }}Nested: {{ Name }}{{/ nested }}{{/.}}".AsSpan().Parse();
        ParentTestSample[] data = [.. Enumerable.Range(0, 10).Select(i => new ParentTestSample { Alias = "Bob", Nested = new TestSample { Name = "Alice", Age = i } })];
        string result = renderer.Render(template, data);
        Assert.Contains("Name: Bob, Nested: Alice", result);
    }

    [Fact]
    public void ParentTest_Render_ListOf()
    {
        IStringRenderer renderer = ServiceProvider.GetRequiredService<IStringRenderer>();

        ImmutableArray<INode> template = "{{#.}}Name: {{ Alias }}, {{# nested }}Nested: {{ Name }}{{/ nested }}{{/.}}".AsSpan().Parse();
        List<ParentTestSample> data = [.. Enumerable.Range(0, 10).Select(i => new ParentTestSample { Alias = "Bob", Nested = new TestSample { Name = "Alice", Age = i } })];
        string result = renderer.Render(template, data);
        Assert.Contains("Name: Bob, Nested: Alice", result);
    }

    [Fact]
    public void ParentTest_Render_IEnumerableOf()
    {
        IStringRenderer renderer = ServiceProvider.GetRequiredService<IStringRenderer>();

        ImmutableArray<INode> template = "{{#.}}Name: {{ Alias }}, {{# nested }}Nested: {{ Name }}{{/ nested }}{{/.}}".AsSpan().Parse();
        TestCollection<ParentTestSample> data = new([.. Enumerable.Range(0, 10).Select(i => new ParentTestSample { Alias = "Bob", Nested = new TestSample { Name = "Alice", Age = i } })]);
        string result = renderer.Render(template, data);
        Assert.Contains("Name: Bob, Nested: Alice", result);
    }

    [Fact]
    public void ParentTest_Render_Function()
    {
        IStringRenderer renderer = ServiceProvider.GetRequiredService<IStringRenderer>();

        ImmutableArray<INode> template = "{{{Length(.)}}}".AsSpan().Parse();
        ParentTestSample[] data = [.. Enumerable.Range(0, 10).Select(i => new ParentTestSample { Alias = "Bob", Nested = new TestSample { Name = "Alice", Age = i } })];
        string result = renderer.Render(template, data);
        Assert.Equal("10", result);
    }
    [Fact]
    public void ParentTest_Render_FunctionIgnoreCase()
    {
        IStringRenderer renderer = ServiceProvider.GetRequiredService<IStringRenderer>();

        ImmutableArray<INode> template = "{{{length(.)}}}".AsSpan().Parse();
        ParentTestSample[] data = [.. Enumerable.Range(0, 10).Select(i => new ParentTestSample { Alias = "Bob", Nested = new TestSample { Name = "Alice", Age = i } })];
        string result = renderer.Render(template, data);
        Assert.Equal("10", result);
    }

    [Fact]
    public void ParentTest_Render_NextedFunction()
    {
        IStringRenderer renderer = ServiceProvider.GetRequiredService<IStringRenderer>();

        ImmutableArray<INode> template = "{{{lowercase(trim(.))}}}".AsSpan().Parse();
        string result = renderer.Render(template, "   TEST   ");
        Assert.Equal("test", result);
    }
    [Fact]
    public void ParentTest_Render_NextedFunctionArg()
    {
        IStringRenderer renderer = ServiceProvider.GetRequiredService<IStringRenderer>();

        ImmutableArray<INode> template = "{{{lowercase(trim(. 'c'))}}}".AsSpan().Parse();
        string result = renderer.Render(template, "TESTccc");
        Assert.Equal("test", result);
    }
    private class TestCollection<T>(IEnumerable<T> items) : IEnumerable<T>
    {
        public IEnumerator<T> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
