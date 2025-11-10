using System.Text.Json.Serialization;

namespace RobinMustache.Specs.Tests;

public sealed record MustacheTestFile(
    [property: JsonPropertyName("overview")] string Overview,
    [property: JsonPropertyName("tests")] MustacheTestCase[] Tests
);
