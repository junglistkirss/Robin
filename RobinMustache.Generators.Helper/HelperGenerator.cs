using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace RobinMustache.Generators.Accessor
{
    [Generator]
    public class HelperGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {


            IncrementalValuesProvider<INamedTypeSymbol> classes = context.SyntaxProvider
                .ForAttributeWithMetadataName(
                    typeof(HelpersAttribute).FullName,
                    predicate: static (node, _) => node is TypeDeclarationSyntax type && type.IsKind(SyntaxKind.PartialKeyword),
                    transform: static (sc, _) =>
                    {
                        if (sc.TargetSymbol is not INamedTypeSymbol namedSymbol)
                            throw new InvalidDataException("TargetSymbol is not a INamedTypeSymbol");
                        return namedSymbol;
                    });
            IncrementalValuesProvider<(IMethodSymbol methodSymbol, AttributeData attributeData)> methods = context.SyntaxProvider.ForAttributeWithMetadataName(
                typeof(GenerateHelperAttribute).FullName,
                    predicate: static (node, _) => node is MethodDeclarationSyntax method && method.IsKind(SyntaxKind.PartialKeyword) && method.IsKind(SyntaxKind.StaticKeyword),
                    transform: static (sc, _) =>
                    {
                        if (sc.TargetSymbol is not IMethodSymbol methodSymbol)
                            throw new InvalidDataException("TargetSymbol is not a INamedTypeSymbol");
                        return (methodSymbol, attributeData: sc.Attributes.Single());
                    });

            var methodsGroupedByClass =
                methods
                    .Select(static (m, _) => (
                        Key: m.methodSymbol.ContainingType,
                        Value: m
                    ));



            IncrementalValuesProvider<(INamedTypeSymbol ContainingType, HelperMethodInfo helperMethodInfo)> methodDeclarations = methods.Select(static (data, _) =>
            {
                return (data.methodSymbol.ContainingType, new HelperMethodInfo
                {
                    HelperName = data.methodSymbol.Name,
                    HelperAccessibility = HelperAccessibility.Public,
                    OutputTypeName = data.methodSymbol.ReturnType
                                    .ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat
                                    .RemoveMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.IncludeNotNullableReferenceTypeModifier)
                                    .WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted)
                                    .WithMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.ExpandNullable)
                                    .RemoveMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier)),
                    Arguments = [..data.methodSymbol.Parameters.Select(x =>
                                    {
                                        return new HelperArgumentInfo
                                        {
                                            Name = x.Name,
                                            LongTypeName  = x.Type
                                                .ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat
                                                .RemoveMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.IncludeNotNullableReferenceTypeModifier)
                                                .WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted)
                                                .WithMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.ExpandNullable)
                                                .RemoveMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier))
                                        };
                                    })]
                });
            });


            IncrementalValuesProvider<(INamedTypeSymbol namedSymbol, HelpersInfo host)> classDeclarations = classes.Select(static (namedSymbol, _) =>
                    {
                        return (namedSymbol, new HelpersInfo
                        {
                            TypeNamespaceName = namedSymbol.ContainingNamespace
                                    .ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat
                                    .WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted)),
                            TypeName = namedSymbol.Name,
                            Accessibility = namedSymbol.DeclaredAccessibility switch
                            {
                                Accessibility.Internal => "internal",
                                _ => "public"
                            }
                        });
                    });
            IncrementalValuesProvider<(HelpersInfo host, IEnumerable<HelperMethodInfo> methods)> groups = classDeclarations.Combine(methodDeclarations.Collect())
                .Select((data, _) =>
                {
                    return (
                        data.Left.host,
                        methods: data.Right
                            .Where(x => SymbolEqualityComparer.Default.Equals(x.ContainingType, data.Left.namedSymbol))
                            .Select(x => x.helperMethodInfo)
                    );
                });

            context.RegisterSourceOutput(groups, (spc, source) =>
            {
                (HelpersInfo host, IEnumerable<HelperMethodInfo> methods) = source;
                StringBuilder sb = new();
                sb.AppendLine("// <auto-generated>");
                sb.AppendLine("#nullable disable");
                sb.AppendLine("using System;");
                //if (source.UseDelegates)
                //    sb.AppendLine("using System.Diagnostics.CodeAnalysis;");
                sb.AppendLine();
                sb.AppendLine($"namespace {host.TypeNamespaceName}");
                sb.AppendLine("{");
                sb.AppendLineIndented(1, $"{host.Accessibility} static partial class {host.TypeName}");
                sb.AppendLineIndented(1, "{");
                //if (source.UseDelegates)
                //    sb.AppendLineIndented(2, $"public static bool GetNamedPropertyDelegate(string propertyName, [NotNull] out Delegate value)");
                //else
                //    sb.AppendLineIndented(2, $"public static bool GetNamedProperty({source.LongTypeName} obj, string propertyName, out object value)");
                //sb.AppendLineIndented(2, "{");
                //sb.AppendLineIndented(3, "switch(propertyName.ToLowerInvariant())");
                //sb.AppendLineIndented(3, "{");

                //if (source.Properties.Length > 0)
                //{
                //    foreach (AccessorPropertyInfo prop in source.Properties)
                //    {
                //        sb.AppendLineIndented(4, $"case \"{prop.Name.ToLowerInvariant()}\":");
                //        if (source.UseDelegates)
                //            sb.AppendLineIndented(5, $"value = (Func<{source.LongTypeName}, {prop.LongTypeName}>)(obj => obj.{prop.Name});");
                //        else
                //            sb.AppendLineIndented(5, $"value = obj.{prop.Name};");
                //        sb.AppendLineIndented(5, "return true;");
                //    }
                //    sb.AppendLineIndented(4, "default:");
                //    if (source.UseDelegates)
                //        sb.AppendLineIndented(5, $"value = (Func<{source.LongTypeName}, object>)(_ => null);");
                //    else
                //        sb.AppendLineIndented(5, $"value = null;");
                //    sb.AppendLineIndented(5, "return false;");
                //}
                //else
                //{
                //    sb.AppendLineIndented(4, "default:");
                //    sb.AppendLineIndented(5, "throw new ArgumentException($\"Source has no properties : '{propertyName}'\");");
                //}

                //sb.AppendLineIndented(3, "}");
                //sb.AppendLineIndented(2, "}");
                //sb.AppendLineIndented(1, "}");

                sb.AppendLine("}");

                string hintName = $"{host.TypeName}.helpers.g.cs";
                spc.AddSource(hintName, SourceText.From(sb.ToString(), Encoding.UTF8));
                //}
            });
        }

    }
}