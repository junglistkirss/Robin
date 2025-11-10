using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RobinMustache.Generators.Accessor
{
    internal class SyntaxReceiver : ISyntaxReceiver
    {
        public List<TypeDeclarationSyntax> CandidateClasses { get; } = [];

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is ClassDeclarationSyntax classDecl)
            {
                CandidateClasses.Add(classDecl);
            }
            else if (syntaxNode is StructDeclarationSyntax structDecl)
            {
                CandidateClasses.Add(structDecl);
            }
        }
    }
}