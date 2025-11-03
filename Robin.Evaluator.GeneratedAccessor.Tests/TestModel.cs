using Robin.Generators.Accessor;

namespace Robin.Evaluator.GeneratedAccessor.Tests;

[GenerateAccessor]
internal class TestModel
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
}
