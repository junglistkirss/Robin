using Microsoft.Extensions.DependencyInjection;
using RobinMustache.Abstractions.Nodes;
using System.Collections.Immutable;
using System.Text.Json;

namespace RobinMustache.Specs.Tests;

public class InvertedTests : BaseMustacheTests
{
    public static TheoryData<MustacheTestCase> GetTestsSpec1_4_3()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "specs", "1.4.3", "inverted.json");
        string json = File.ReadAllText(path);
        MustacheTestFile cases = JsonSerializer.Deserialize<MustacheTestFile>(json)!;
        return [.. cases.Tests];
    }

    [Theory]

    [MemberData(nameof(GetTestsSpec1_4_3))]
    public void Should_Add_Correctly(MustacheTestCase @case)
    {
        IStringRenderer renderer = ServiceProvider.GetRequiredService<IStringRenderer>();
        var tokens = Tokenizer.Tokenize(@case.Template.AsSpan());
        ImmutableArray<INode> template = @case.Template.AsSpan().Parse();
        string result = renderer.Render(template, @case.Data);
        Assert.Equal(@case.Expected, result);

    }
}