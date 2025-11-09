using Microsoft.Extensions.DependencyInjection;
using RobinMustache.Abstractions.Nodes;
using RobinMustache.MustacheSpecs.Tests;
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
        ImmutableArray<INode> template = @case.Template.AsSpan().Parse();
        string result = renderer.Render(template, @case.Data);
        if (!@case.Expected.EqualsIgnoringWhitespace(result))
        {
            Assert.Fail($"{@case.Name} : {@case.Description}{Environment.NewLine}Excpected: \"{@case.Expected}\"{Environment.NewLine}Actual: \"{result}\"");
        }
    }
}