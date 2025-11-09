using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;


namespace Robin.Generators.Accessor.Tests;

public class AccessorDelegateGeneratorTests
{
    [Fact]
    public void TestsStruct()
    {
        // Arrange
        string source = @"/*test generators*/
using Robin.Generators.Accessor;

namespace TestNamespace
{
    [GenerateAccessor(UseDelegates = true)]
    public struct Test
    {
        public int Value { get; }
    }
}";
        string code = GenerateTestCode(source);

        Assert.Contains("namespace TestNamespace", code);
        Assert.Contains("public static class TestAccessor", code);
        Assert.Contains("TestNamespace.Test", code);
        Assert.Contains("switch(propertyName.ToLowerInvariant())", code);
        Assert.Contains(@"case ""value""", code);
        Assert.Contains("return true;", code);
        Assert.Contains("default:", code);
        Assert.Contains("value = (Func<TestNamespace.Test, object>)(_ => null);", code);
        Assert.Contains("return false;", code);
    }

    [Fact]
    public void TestsClass()
    {
        // Arrange
        string source = @"/*test generators*/
using Robin.Generators.Accessor;

namespace TestNamespace
{
    [GenerateAccessor(UseDelegates = true)]
    public class Test
    {
        public int Value { get; }
    }
}";
        string code = GenerateTestCode(source);

        Assert.Contains("namespace TestNamespace", code);
        Assert.Contains("public static class TestAccessor", code);
        Assert.Contains("TestNamespace.Test", code);
        Assert.Contains("switch(propertyName.ToLowerInvariant())", code);
        Assert.Contains(@"case ""value""", code);
        Assert.Contains("return true;", code);
        Assert.Contains("default:", code);
        Assert.Contains("value = (Func<TestNamespace.Test, object>)(_ => null);", code);
        Assert.Contains("return false;", code);
    }


    [Fact]
    public void TestsEmpty()
    {
        // Arrange
        string source = @"/*test generators*/
using Robin.Generators.Accessor;

namespace TestNamespace
{
    [GenerateAccessor(UseDelegates = true)]
    public class Test
    {
    }
}";
        string code = GenerateTestCode(source);

        Assert.Contains("namespace TestNamespace", code);
        Assert.Contains("public static class TestAccessor", code);
        Assert.Contains("switch(propertyName.ToLowerInvariant())", code);
        Assert.Contains("default:", code);
        Assert.Contains("throw new ArgumentException", code);
        Assert.Contains("Source has no properties", code);
    }

    [Fact]
    public void TestsInternal()
    {
        // Arrange
        string source = @"/*test generators*/
using Robin.Generators.Accessor;

namespace TestNamespace
{
    [GenerateAccessor(UseDelegates = true)]
    internal class Test
    {
    }
}";
        string code = GenerateTestCode(source);
        Assert.Contains("namespace TestNamespace", code);
        Assert.Contains("internal static class TestAccessor", code);
        Assert.Contains("switch(propertyName.ToLowerInvariant())", code);
        Assert.Contains("default:", code);
        Assert.Contains("throw new ArgumentException", code);
        Assert.Contains("Source has no properties", code);
    }

    [Fact]
    public void TestsAbstract()
    {
        // Arrange
        string source = @"/*test generators*/
using Robin.Generators.Accessor;

namespace TestNamespace
{
    [GenerateAccessor(UseDelegates = true)]
    internal abstract class Test
    {
        public int Value { get; }
    }
}";
        string code = GenerateTestCode(source);
        Assert.Contains("namespace TestNamespace", code);
        Assert.Contains("internal static class TestAccessor", code);
        Assert.Contains("TestNamespace.Test", code);
        Assert.Contains("switch(propertyName.ToLowerInvariant())", code);
        Assert.Contains(@"case ""value""", code);
        Assert.Contains("return true;", code);
        Assert.Contains("default:", code);
        Assert.Contains("value = (Func<TestNamespace.Test, object>)(_ => null);", code);
        Assert.Contains("return false;", code);
    }
    private static string GenerateTestCode(string source)
    {
        CSharpCompilation compilation = CreateCompilation(source);
        AccessorGenerator generator = new();
        System.Collections.Immutable.ImmutableArray<Diagnostic> compilationDiagnostics = compilation.GetDiagnostics();
        Assert.Empty(compilationDiagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));

        // Act
        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            [GeneratorExtensions.AsSourceGenerator(generator)],
            parseOptions: new CSharpParseOptions(LanguageVersion.LatestMajor)
        );
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out Compilation? outputCompilation, out System.Collections.Immutable.ImmutableArray<Diagnostic> diagnostics);
        // Assert
        Assert.Empty(diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
        SyntaxTree result = Assert.Single(driver.GetRunResult().GeneratedTrees);
        string code = result.ToString();
        return code;
    }

    private static CSharpCompilation CreateCompilation(string source)
        => CSharpCompilation.Create(
            "TestNamespace",
            syntaxTrees: [CSharpSyntaxTree.ParseText(source, options: new CSharpParseOptions(LanguageVersion.Latest))],
            references: [
                .. Basic.Reference.Assemblies.NetStandard20.References.All,
                MetadataReference.CreateFromFile(typeof(GenerateAccessorAttribute).Assembly.Location),
            ],
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
}