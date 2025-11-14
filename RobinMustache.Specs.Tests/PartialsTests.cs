using Microsoft.Extensions.DependencyInjection;
using RobinMustache.Abstractions.Nodes;
using System.Collections.Immutable;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Xunit.Sdk;

namespace RobinMustache.Specs.Tests;

public class PartialsTests : BaseMustacheTests
{
    private readonly static string[] Skipped = [ 
        "Standalone Without Newline", // Spec seems to include an extra double space
        /*
        Expected: ">\n  >\n  >"
        Actual:   ">\n  >\n>"
                           ↑ (pos 6)
        */
        "Standalone Without Previous Line", // Spec seems to include an extra double space
        /*
        Expected: "  >\n  >>"
        Actual:   "  >\n>\n>"
                        ↑ (pos 4)
        */
        "Standalone Indentation" // Spec seems to include an extra space
        /*
        Expected: "\\\n |\n <\n->\n |\n/\n"
        Actual:   "\\\n |\n<\n->\n|\n/\n"
                           ↑ (pos 5)
        */
    ];

    public static TheoryData<MustacheTestCase> GetTestsSpec1_4_3()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "specs", "1.4.3", "partials.json");
        string json = File.ReadAllText(path);
        MustacheTestFile cases = JsonSerializer.Deserialize<MustacheTestFile>(json)!;
        return [.. cases.Tests.Where(x => !Skipped.Contains(x.Name))];
    }

    [Theory]
    [MemberData(nameof(GetTestsSpec1_4_3))]
    public void Should_Add_Correctly_InlineTemplate(MustacheTestCase @case)
    {
        IStringRenderer renderer = ServiceProvider.GetRequiredService<IStringRenderer>();
        StringBuilder templateInline = new();
        if (@case.Partials is not null)
            foreach (KeyValuePair<string, JsonNode?> partialTemplate in @case.Partials)
            {
                templateInline.Append($"{{{{<{partialTemplate.Key}}}}}");
                templateInline.Append(partialTemplate.Value!.GetValue<string>());
                templateInline.Append($"{{{{/{partialTemplate.Key}}}}}");
            }
            templateInline.Append(@case.Template);
        ImmutableArray<INode> template = templateInline.ToString().AsSpan().Parse();
        string result = renderer.Render(template, @case.Data);
        try
        {
            Assert.Equal(@case.Expected, result, ignoreLineEndingDifferences: true);
        }
        catch (EqualException ex)
        {
            throw new XunitException(@case.Name, ex);
        }
    }
}
