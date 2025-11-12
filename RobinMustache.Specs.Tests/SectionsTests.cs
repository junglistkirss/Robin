using Microsoft.Extensions.DependencyInjection;
using RobinMustache.Abstractions.Nodes;
using System.Collections.Immutable;
using System.Text.Json;
using Xunit.Sdk;

namespace RobinMustache.Specs.Tests;

public class SectionsTests : BaseMustacheTests
{
    private readonly static string[] Skipped = ["Standalone Line Endings", "Standalone Without Newline"];

    public static TheoryData<MustacheTestCase> GetTestsSpec1_4_3()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "specs", "1.4.3", "sections.json");
        string json = File.ReadAllText(path);
        MustacheTestFile cases = JsonSerializer.Deserialize<MustacheTestFile>(json)!;
        return [.. cases.Tests.Where(x => !Skipped.Contains(x.Name))];
    }

    [Theory]

    [MemberData(nameof(GetTestsSpec1_4_3))]
    public void Should_Add_Correctly(MustacheTestCase @case)
    {
        IStringRenderer renderer = ServiceProvider.GetRequiredService<IStringRenderer>();
        ImmutableArray<INode> template = @case.Template.AsSpan().Parse();
        string result = renderer.Render(template, @case.Data);
            Assert.Equal(@case.Expected, result);
        try
        {
        }
        catch (EqualException ex)
        {
            throw new XunitException(@case.Name, ex);
        }
    }
}
