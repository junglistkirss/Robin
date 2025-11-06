using Microsoft.Extensions.DependencyInjection;
using Robin.Contracts.Nodes;
using Robin.Evaluator.System.Text.Json;
using System.Collections.Immutable;
using System.Text.Json;

namespace Robin.MustacheSpecs.Tests;

public class CommentsTests : BaseMustacheTests
{
    public static IEnumerable<object[]> GetTestsSpec1_4_3()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "specs", "1.4.3", "comments.json");
        string json = File.ReadAllText(path);
        var cases = JsonSerializer.Deserialize<MustacheTestFile>(json)!;
        foreach (var test in cases.Tests)
            yield return new MustacheTestCase[]
            {
                test
            };
    }

    [Theory]

    [MemberData(nameof(GetTestsSpec1_4_3))]
    public void Should_Add_Correctly(MustacheTestCase @case)
    {
        IJsonEvaluator eval = ServiceProvider.GetRequiredService<IJsonEvaluator>();
        ImmutableArray<INode> template = @case.Template.AsSpan().Parse();
        string result = eval.RenderString(template, @case.Data);
        if (!@case.Expected.EqualsIgnoringWhitespace(result))
        {
            Assert.Fail($"{@case.Name} : {@case.Description}{Environment.NewLine}Excpected: \"{@case.Expected}\"{Environment.NewLine}Actual: \"{result}\"");
        }
    }
}
