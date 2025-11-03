using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Robin.Generators.Accessor
{

    [Generator]
    public class PropertyAccessorGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            // S'inscrit pour recevoir les déclarations de classes avec attribut
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxReceiver is not SyntaxReceiver receiver)
                return;

            // Récupère le symbole de l'attribut
            INamedTypeSymbol attributeSymbol = context.Compilation.GetTypeByMetadataName(typeof(GenerateAccessorAttribute).FullName);
            if (attributeSymbol == null)
                return;

            foreach (ClassDeclarationSyntax classDecl in receiver.CandidateClasses)
            {
                SemanticModel model = context.Compilation.GetSemanticModel(classDecl.SyntaxTree);
                // CAST explicite en INamedTypeSymbol (représente les types nommés : classes, structs, ...)
                if (model.GetDeclaredSymbol(classDecl) is not INamedTypeSymbol namedTypeSymbol)
                    continue;

                // // Vérifie la présence de l'attribut
                // if (!namedTypeSymbol.GetAttributes().Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeSymbol)))
                //     continue;

                string ns = namedTypeSymbol.ContainingNamespace == null || namedTypeSymbol.ContainingNamespace.IsGlobalNamespace
                    ? null
                    : namedTypeSymbol.ContainingNamespace.ToDisplayString();

                string className = namedTypeSymbol.Name;
                string accessorName = $"{className}Accessor";
                string visibility = namedTypeSymbol.DeclaredAccessibility switch
                {
                    Accessibility.Internal => "internal",
                    _ => "public",
                };
                List<IPropertySymbol> properties = [.. namedTypeSymbol
                    .GetMembers()
                    .OfType<IPropertySymbol>()
                    .Where(p => !p.IsStatic && p.DeclaredAccessibility == Accessibility.Public)];

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("using System;");
                if (!string.IsNullOrEmpty(ns))
                {
                    sb.AppendLine($"namespace {ns}");
                    sb.AppendLine("{");
                }

                sb.AppendLineIndented(1 , $"{visibility} static class {accessorName}");
                sb.AppendLineIndented(1, "{");
                sb.AppendLineIndented(2, $"public static object GetPropertyValue(this {className} obj, string propertyName) => propertyName.ToLowerInvariant() switch");
                sb.AppendLineIndented(2, "{");

                if (properties.Count > 0)
                {
                    foreach (IPropertySymbol prop in properties)
                    {
                        // Utilise nameof pour robustesse si le code généré se trouve dans le même namespace
                        sb.AppendLineIndented(3, $"\"{prop.Name.ToLowerInvariant()}\" => obj.{prop.Name},");
                    }
                    sb.AppendLineIndented(3, "_ => null");
                }
                else
                {
                    // Si aucune propriété publique trouvée, on met un cas par défaut
                    sb.AppendLineIndented(3, "_ => throw new ArgumentException($\"Unknown property '{propertyName}'\")");
                }

                sb.AppendLineIndented(2, "};");
                sb.AppendLineIndented(1, "}");

                if (!string.IsNullOrEmpty(ns))
                {
                    sb.AppendLine("}");
                }

                string hintName = $"{accessorName}.g.cs";
                context.AddSource(hintName, sb.ToString());
            }
        }
    }
}