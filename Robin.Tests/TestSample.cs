namespace Robin.tests;

internal record TestSample
{
    public string? Name { get; set; }
    public int Age { get; set; }
}
internal record ParentTestSample
{
    public string? Alias { get; set; }
    public TestSample? Nested { get; set; }
}