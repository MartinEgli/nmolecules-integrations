using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NMolecules.Analyzers
{
    public static class IdAnalyzer
    {
        public static void AnalyzeEntityForId(
            SymbolAnalysisContext it,
            Func<INamedTypeSymbol, Diagnostic> onMissingIdentity,
            Func<INamedTypeSymbol, Diagnostic> onMultipleIdentities)
        {
            var classSymbol = (INamedTypeSymbol)it.Symbol;
            var identityCount = CountIdentities(classSymbol);

            if (identityCount == 0)
            {
                it.ReportDiagnostic(onMissingIdentity(classSymbol));
            }
            else if (identityCount > 1)
            {
                it.ReportDiagnostic(onMultipleIdentities(classSymbol));
            }
        }

        private static int CountIdentities(INamedTypeSymbol classSymbol)
        {
            var baseCount = 0;
            var classSymbolBaseType = classSymbol.BaseType;
            if (classSymbolBaseType is not null && classSymbolBaseType.SpecialType != SpecialType.System_Object)
            {
                baseCount = CountIdentities(classSymbolBaseType);
            }

            return CountOwnIdentities(classSymbol) + baseCount;
        }

        private static int CountOwnIdentities(INamedTypeSymbol classSymbol)
        {
            var identities = 0;
            foreach (var member in classSymbol.GetMembers())
            {
                if (member.IsImplicitlyDeclared)
                {
                    continue;
                }

                if (member is not IFieldSymbol and not IPropertySymbol)
                {
                    continue;
                }

                if (member.IsIdentity())
                {
                    identities++;
                }
            }

            return identities;
        }
    }
}
