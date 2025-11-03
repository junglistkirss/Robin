using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Robin.Generators.Accessor
{

    [Generator]
    public class PropertyAccessorIncrementalGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // Sélectionne toutes les classes et structs
            var classDeclarations = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (node, _) => node is TypeDeclarationSyntax,
                    transform: static (ctx, _) => (TypeDeclarationSyntax)ctx.Node)
                .Where(static m => m != null);

            // Combine avec le Compilation pour avoir les symboles
            var compilationAndClasses = context.CompilationProvider.Combine(classDeclarations.Collect());

            context.RegisterSourceOutput(compilationAndClasses, (spc, source) =>
            {
                var (compilation, classes) = source;

                INamedTypeSymbol attributeSymbol = compilation.GetTypeByMetadataName(typeof(GenerateAccessorAttribute).FullName);
                if (attributeSymbol is null) return;

                foreach (var classDecl in classes)
                {
                    SemanticModel model = compilation.GetSemanticModel(classDecl.SyntaxTree);
                    if (model.GetDeclaredSymbol(classDecl) is not INamedTypeSymbol namedTypeSymbol)
                        continue;

                    // Vérifie la présence de l'attribut
                    if (!namedTypeSymbol.GetAttributes().Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeSymbol)))
                        continue;

                    string ns = namedTypeSymbol.ContainingNamespace.IsGlobalNamespace ? "" : namedTypeSymbol.ContainingNamespace.ToString();
                    string shortClassName = namedTypeSymbol.Name;
                    string longClassName = namedTypeSymbol.ToDisplayString();
                    string accessorName = $"{shortClassName}Accessor";
                    string visibility = namedTypeSymbol.DeclaredAccessibility switch
                    {
                        Accessibility.Internal => "internal",
                        _ => "public",
                    };

                    IPropertySymbol[] properties = [.. namedTypeSymbol
                        .GetMembers()
                        .OfType<IPropertySymbol>()
                        .Where(p => !p.IsStatic && p.DeclaredAccessibility == Accessibility.Public)];

                    StringBuilder sb = new();
                    sb.AppendLine("using System;");
                    sb.AppendLine("using System.Diagnostics.CodeAnalysis;");
                    sb.AppendLine();

                    if (!string.IsNullOrEmpty(ns))
                    {
                        sb.AppendLine($"namespace {ns}");
                        sb.AppendLine("{");
                    }

                    sb.AppendLineIndented(1, "#nullable disable");
                    sb.AppendLineIndented(1, $"{visibility} static class {accessorName}");
                    sb.AppendLineIndented(1, "{");
                    sb.AppendLineIndented(2, $"public static bool TryGetPropertyValue(this {longClassName} obj, string propertyName, [MaybeNullWhen(true)] out object value)");
                    sb.AppendLineIndented(2, "{");
                    sb.AppendLineIndented(3, "switch(propertyName.ToLowerInvariant())");
                    sb.AppendLineIndented(3, "{");

                    if (properties.Length > 0)
                    {
                        foreach (var prop in properties)
                        {
                            sb.AppendLineIndented(4, $"case \"{prop.Name.ToLowerInvariant()}\":");
                            sb.AppendLineIndented(5, $"value = obj.{prop.Name};");
                            sb.AppendLineIndented(5, "return true;");
                        }
                        sb.AppendLineIndented(4, "default:");
                        sb.AppendLineIndented(5, "value = null;");
                        sb.AppendLineIndented(5, "return false;");
                    }
                    else
                    {
                        sb.AppendLineIndented(3, "default: throw new ArgumentException($\"Unknown property '{propertyName}'\");");
                    }

                    sb.AppendLineIndented(3, "}");
                    sb.AppendLineIndented(2, "}");
                    sb.AppendLineIndented(1, "}");

                    if (!string.IsNullOrEmpty(ns))
                    {
                        sb.AppendLine("}");
                    }

                    string hintName = $"{accessorName}.g.cs";
                    spc.AddSource(hintName, SourceText.From(sb.ToString(), Encoding.UTF8));
                }
            });
        }


    }
}