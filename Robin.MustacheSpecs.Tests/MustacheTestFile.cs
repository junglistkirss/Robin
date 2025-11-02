using System.Text.Json.Serialization;

namespace Robin.MustacheSpecs.Tests;

public sealed record MustacheTestFile(
    [property: JsonPropertyName("overview")] string Overview,
    [property: JsonPropertyName("tests")] IReadOnlyList<MustacheTestCase> Tests
);
