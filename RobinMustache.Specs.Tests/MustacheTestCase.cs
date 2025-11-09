using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace RobinMustache.Specs.Tests;

public readonly record struct MustacheTestCase(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("desc")] string Description,
    [property: JsonPropertyName("data")] JsonNode Data,
    [property: JsonPropertyName("template")] string Template,
    [property: JsonPropertyName("expected")] string Expected
);
