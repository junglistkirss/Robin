using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace Robin.Generators.Accessor
{
    internal record struct AccessorPropertyInfo
    {
        public string Name { get; internal set; }
        public string LongTypeName { get; internal set; }

    }
    internal record struct AccessorInfo
    {
        public string TypeNamespaceName { get; internal set; }
        public string ShortTypeName { get; internal set; }
        public string LongTypeName { get; internal set; }
        public string Accessibility { get; internal set; }
        public bool UseDelegates { get; set; }
        public AccessorPropertyInfo[] Properties { get; internal set; }
    }


    [Generator]
    public class AccessorGenerator : IIncrementalGenerator
    {

        public static bool TryGetNamedArgument<T>(AttributeData data, string name, out T value)
        {
            value = default!;
            if (data != null && data.NamedArguments.Length > 0 && data.NamedArguments.Any(x => x.Key == name))
            {
                TypedConstant val = data.NamedArguments.Single(x => x.Key == name).Value;
                if (!val.IsNull)
                {
                    if (val.Kind == TypedConstantKind.Array)
                        throw new InvalidOperationException($"Named argument {name} is an array");
                    value = (T)val.Value!;
                    return true;
                }
            }
            return false;
        }
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // Sélectionne toutes les classes et structs
            IncrementalValuesProvider<AccessorInfo> classDeclarations = context.SyntaxProvider
                .ForAttributeWithMetadataName(
                    typeof(GenerateAccessorAttribute).FullName,
                    predicate: static (node, _) => node is TypeDeclarationSyntax,
                    transform: static (sc, _) =>
                    {
                        if (sc.TargetSymbol is not INamedTypeSymbol namedSymbol)
                            throw new InvalidDataException("TargetSymbol is not a INamedTypeSymbol");
                        return sc.Attributes.Select(x =>
                        {
                            return new AccessorInfo
                            {
                                UseDelegates = TryGetNamedArgument(x, nameof(GenerateAccessorAttribute.UseDelegates), out bool value) && value,
                                TypeNamespaceName = namedSymbol.ContainingNamespace
                                    .ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat
                                    .WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted)),
                                ShortTypeName = namedSymbol.Name,
                                LongTypeName = namedSymbol
                                    .ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat
                                    .RemoveMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.IncludeNotNullableReferenceTypeModifier)
                                    .WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted)
                                    .WithMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.ExpandNullable)
                                    .RemoveMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier)),
                                Accessibility = namedSymbol.DeclaredAccessibility switch { Accessibility.Internal => "internal", _ => "public" },
                                Properties = [.. namedSymbol
                                    .GetMembers()
                                    .OfType<IPropertySymbol>()
                                    .Where(p => !p.IsStatic && p.DeclaredAccessibility == Accessibility.Public)
                                    .Select(x => new AccessorPropertyInfo{
                                        Name = x.Name,
                                        LongTypeName = x.Type
                                            .ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat
                                            .RemoveMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.IncludeNotNullableReferenceTypeModifier)
                                            .WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted)
                                            .WithMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.ExpandNullable)
                                            .RemoveMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier)),
                                    })]
                            };
                        }).Single();
                    });

            //// Combine avec le Compilation pour avoir les symboles
            //IncrementalValueProvider<(Compilation Left, System.Collections.Immutable.ImmutableArray<TypeDeclarationSyntax> Right)> compilationAndClasses = context.CompilationProvider.Combine(classDeclarations.Collect());

            context.RegisterSourceOutput(classDeclarations, (spc, source) =>
            {
                //source.
                ////(Compilation compilation, System.Collections.Immutable.ImmutableArray<TypeDeclarationSyntax> classes) = source;

                //INamedTypeSymbol attributeSymbol = compilation.GetTypeByMetadataName(typeof(GenerateAccessorAttribute).FullName);
                //if (attributeSymbol is null) return;

                //foreach (TypeDeclarationSyntax classDecl in classes)
                //{
                //SemanticModel model = compilation.GetSemanticModel(classDecl.SyntaxTree);
                //if (model.GetDeclaredSymbol(classDecl) is not INamedTypeSymbol namedTypeSymbol)
                //    continue;

                //// Vérifie la présence de l'attribut
                //if (!namedTypeSymbol.GetAttributes().Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeSymbol)))
                //    continue;

                string accessorName = $"{source.ShortTypeName}Accessor";

                StringBuilder sb = new();
                sb.AppendLine("// <auto-generated>");
                sb.AppendLine("#nullable disable");
                sb.AppendLine("using System;");
                if (source.UseDelegates)
                    sb.AppendLine("using System.Diagnostics.CodeAnalysis;");
                sb.AppendLine();
                sb.AppendLine($"namespace {source.TypeNamespaceName}");
                sb.AppendLine("{");
                sb.AppendLineIndented(1, $"{source.Accessibility} static class {accessorName}");
                sb.AppendLineIndented(1, "{");
                if (source.UseDelegates)
                    sb.AppendLineIndented(2, $"public static bool GetNamedPropertyDelegate(string propertyName, [NotNull] out Delegate value)");
                else
                    sb.AppendLineIndented(2, $"public static bool GetNamedProperty({source.LongTypeName} obj, string propertyName, out object value)");
                sb.AppendLineIndented(2, "{");
                sb.AppendLineIndented(3, "switch(propertyName.ToLowerInvariant())");
                sb.AppendLineIndented(3, "{");

                if (source.Properties.Length > 0)
                {
                    foreach (AccessorPropertyInfo prop in source.Properties)
                    {
                        sb.AppendLineIndented(4, $"case \"{prop.Name.ToLowerInvariant()}\":");
                        if (source.UseDelegates)
                            sb.AppendLineIndented(5, $"value = (Func<{source.LongTypeName}, {prop.LongTypeName}>)(obj => obj.{prop.Name});");
                        else
                            sb.AppendLineIndented(5, $"value = obj.{prop.Name};");
                        sb.AppendLineIndented(5, "return true;");
                    }
                    sb.AppendLineIndented(4, "default:");
                    if (source.UseDelegates)
                        sb.AppendLineIndented(5, $"value = (Func<{source.LongTypeName}, object>)(_ => null);");
                    else
                        sb.AppendLineIndented(5, $"value = null;");
                    sb.AppendLineIndented(5, "return false;");
                }
                else
                {
                    sb.AppendLineIndented(4, "default:");
                    sb.AppendLineIndented(5, "throw new ArgumentException($\"Source has no properties : '{propertyName}'\");");
                }

                sb.AppendLineIndented(3, "}");
                sb.AppendLineIndented(2, "}");
                sb.AppendLineIndented(1, "}");

                sb.AppendLine("}");

                string hintName = $"{accessorName}.g.cs";
                spc.AddSource(hintName, SourceText.From(sb.ToString(), Encoding.UTF8));
                //}
            });
        }


    }
}