using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NMolecules.Shared.Analyzers
{
    public static class IdAnalyzer
    {
        private const string Identity = "Identity";
        private const string Object = "Object";

        public static void AnalyzeEntityForId(this SymbolAnalysisContext context, Func<INamedTypeSymbol, Diagnostic> onViolation)
        {
            var classSymbol = (INamedTypeSymbol)context.Symbol;
            var hasIdentity = HasIdentity(classSymbol);

            if (!hasIdentity)
            {
                context.ReportDiagnostic(onViolation(classSymbol));
            }
        }

        public static void AnalyzeEntityForId2(SymbolAnalysisContext it, Func<INamedTypeSymbol, Diagnostic> onViolation)
        {
            var classSymbol = (INamedTypeSymbol)it.Symbol;
            var hasIdentity = HasIdentity(classSymbol);

            if (!hasIdentity)
            {
                it.ReportDiagnostic(onViolation(classSymbol));
            }
        }

        private static bool HasIdentity(INamedTypeSymbol classSymbol)
        {
            var hasIdentity = false;
            foreach (var syntaxReference in classSymbol.DeclaringSyntaxReferences)
            {
                if (syntaxReference.GetSyntax() is ClassDeclarationSyntax classDeclarationSyntax)
                {
                    hasIdentity = HasIdentity(classDeclarationSyntax);
                }

                if (hasIdentity)
                {
                    break;
                }
            }

            if (!hasIdentity)
            {
                var classSymbolBaseType = classSymbol.BaseType;
                if (!classSymbolBaseType!.Name.Equals(Object))
                {
                    return HasIdentity(classSymbolBaseType);
                }
            }

            return hasIdentity;
        }

        private static bool HasIdentity(ClassDeclarationSyntax classDeclarationSyntax)
        {
            var hasIdentity = false;
            foreach (var member in classDeclarationSyntax.Members)
            {
                hasIdentity = member switch
                {
                    FieldDeclarationSyntax fieldDeclarationSyntax => HasIdentity(fieldDeclarationSyntax.AttributeLists),
                    PropertyDeclarationSyntax propertyDeclarationSyntax => HasIdentity(propertyDeclarationSyntax.AttributeLists),
                    _ => false
                };

                if (hasIdentity)
                {
                    break;
                }
            }

            return hasIdentity;
        }

        private static bool HasIdentity(SyntaxList<AttributeListSyntax> attributeLists)
        {
            var hasIdentity = false;
            foreach (var attributeList in attributeLists)
            {
                hasIdentity = attributeList.Attributes.Any(it =>
                {
                    var name = it.Name.ToString();
                    var equals = name.Equals(Identity);
                    return equals;
                });
            }

            return hasIdentity;
        }
    }
}